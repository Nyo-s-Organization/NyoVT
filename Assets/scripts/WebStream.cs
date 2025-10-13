using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Threading;
using System.IO;
using System;

public class WebStream : MonoBehaviour
{
    public Camera streamCamera;
    public RenderTexture streamRT;
    public int port = 7829;
    public int frameDelayMS = 16;

    private HttpListener listener;
    private readonly Queue<System.Action> mainThreadQueue = new Queue<System.Action>();
    private byte[] latestFrame;
    private long lastFrameTime = 0;

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
                    string path = context.Request.Url.AbsolutePath;

                    if (path == "/status")
                        HandleStatusRequest(context);
                    else
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
        lastFrameTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        Destroy(tex);

        lock (mainThreadQueue)
        {
            while (mainThreadQueue.Count > 0)
                mainThreadQueue.Dequeue()?.Invoke();
        }
    }

    private void HandleStatusRequest(HttpListenerContext context)
    {
        var response = context.Response;
        response.ContentType = "application/json";
        response.AddHeader("Access-Control-Allow-Origin", "*");
        response.AddHeader("Access-Control-Allow-Headers", "*");
        response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");

        long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        bool alive = (now - lastFrameTime) < 5000;

        string json = $"{{\"alive\":{alive.ToString().ToLower()},\"lastFrame\":{lastFrameTime}}}";
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(json);

        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }

    private void HandleMJPEGStream(HttpListenerContext context)
    {
        var response = context.Response;
        response.ContentType = "multipart/x-mixed-replace; boundary=frame";
        response.AddHeader("Access-Control-Allow-Origin", "*");
        response.AddHeader("Access-Control-Allow-Headers", "*");
        response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
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
