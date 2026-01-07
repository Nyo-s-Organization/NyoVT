using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

public class CustomMovementAPI : MonoBehaviour
{
    public Vector3 receivedPosition;
    public Vector3 receivedRotation;
    public Vector3 eyeLeft;
    public Vector3 eyeRight;
    public List<BlendShape> blendShapes = new List<BlendShape>();
    public Boolean InUse = false;

    private HttpListener listener;
    private CancellationTokenSource cancelSource;

    void Start()
    {
        cancelSource = new CancellationTokenSource();
        StartWebSocketServer();
    }

    void OnDestroy()
    {
        cancelSource.Cancel();
        listener?.Stop();
    }

    void OnApplicationQuit()
    {
        listener?.Stop();
    }

    async void StartWebSocketServer()
    {
        listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:7830/");
        listener.Start();

        Debug.Log("Custom API server started on ws://localhost:7830/");

        while (!cancelSource.Token.IsCancellationRequested)
        {
            var context = await listener.GetContextAsync();

            InUse = true;

            if (context.Request.IsWebSocketRequest)
            {
                Debug.Log("This is a websocket request");
                var wsContext = await context.AcceptWebSocketAsync(null);
                _ = HandleClient(wsContext.WebSocket);
            }
            else
            {
                InUse = true;
                if (context.Request.HttpMethod == "POST")
                {
                    using var reader = new StreamReader(
                        context.Request.InputStream,
                        context.Request.ContentEncoding
                    );

                    string body = await reader.ReadToEndAsync();

                    ParseMessage(body);

                    context.Response.StatusCode = 200;
                    try
                    {
                        byte[] response = Encoding.UTF8.GetBytes("OK");
                        context.Response.OutputStream.Write(response, 0, response.Length);
                    }
                    catch (IOException) {}
                    finally
                    {
                        context.Response.Close();
                    }
                }
                else
                {
                    context.Response.StatusCode = 405;
                }

                context.Response.Close();
            }
        }
    }

    async Task HandleClient(WebSocket socket)
    {
        var buffer = new byte[1024];

        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(
                new ArraySegment<byte>(buffer),
                CancellationToken.None
            );

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await socket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Closing",
                    CancellationToken.None
                );
                return;
            }

            string json = Encoding.UTF8.GetString(buffer, 0, result.Count);
            ParseMessage(json);
        }
    }

    void ParseMessage(string json)
    {
        try
        {
            var data = JsonUtility.FromJson<Wrapper>(json);

            if (data.position != null && data.position.Length == 3)
            {
                receivedPosition = new Vector3(
                    data.position[0],
                    data.position[1],
                    data.position[2]
                );
            }

            if (data.rotation != null && data.rotation.Length == 3)
            {
                receivedRotation = new Vector3(
                    data.rotation[0],
                    data.rotation[1],
                    data.rotation[2]
                );
            }

            if (data.blendshape != null)
            {
                string key = data.blendshape.k;
                float value = data.blendshape.v;

                var existing = blendShapes.Find(b => b.k == key);
                if (existing != null)
                    existing.v = value;
                else
                    blendShapes.Add(new BlendShape { k = key, v = value });
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Invalid message: " + e.Message);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public float[] position;
        public float[] rotation;
        public BlendShape blendshape;
    }
}
