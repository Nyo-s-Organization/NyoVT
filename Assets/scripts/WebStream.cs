using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Threading;
using System.IO;

public class WebStream : MonoBehaviour
{
    public Camera streamCamera;
    public RenderTexture streamRT;
    public int port = 7829;
    public int frameDelayMS = 16;

    private HttpListener listener;
    private readonly Queue<System.Action> mainThreadQueue = new Queue<System.Action>();
    private byte[] latestFrame;

    void Start()
    {
        listener = new HttpListener();
        listener.Prefixes.Add($"http://localhost:{port}/");
        listener.Start();
        Debug.Log($"[WebStream] Listening on http://localhost:{port}/");

        ThreadPool.QueueUserWorkItem(o =>
        {
            while (listener.IsListening)
            {
                try
                {
                    var context = listener.GetContext();
                    Debug.Log("[WebStream] Got request");

                    ThreadPool.QueueUserWorkItem(_ => HandleMJPEGStream(context));
                }
                catch { break; }
            }
        });
    }

    void Update()
    {
        RenderTexture.active = streamRT;
        Texture2D tex = new Texture2D(streamRT.width, streamRT.height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, streamRT.width, streamRT.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        latestFrame = tex.EncodeToJPG();
        Destroy(tex);

        lock (mainThreadQueue)
        {
            while (mainThreadQueue.Count > 0)
                mainThreadQueue.Dequeue()?.Invoke();
        }
    }

    private void HandleMJPEGStream(HttpListenerContext context)
    {
        var response = context.Response;
        response.ContentType = "multipart/x-mixed-replace; boundary=frame";
        Stream output = response.OutputStream;

        try
        {
            while (listener.IsListening && output.CanWrite)
            {
                if (latestFrame != null)
                {
                    string header = $"--frame\r\nContent-Type: image/jpeg\r\nContent-Length: {latestFrame.Length}\r\n\r\n";
                    byte[] headerBytes = System.Text.Encoding.ASCII.GetBytes(header);

                    output.Write(headerBytes, 0, headerBytes.Length);
                    output.Write(latestFrame, 0, latestFrame.Length);
                    output.Write(System.Text.Encoding.ASCII.GetBytes("\r\n"), 0, 2);
                    output.Flush();
                }

                Thread.Sleep(frameDelayMS);
            }
        }
        catch { }
        finally
        {
            try { response.OutputStream.Close(); } catch { }
        }
    }

    void OnApplicationQuit()
    {
        listener?.Stop();
    }
}
