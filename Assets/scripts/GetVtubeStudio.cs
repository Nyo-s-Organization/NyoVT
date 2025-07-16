using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class GetVtubeStudio : MonoBehaviour
{
    public string iPhoneIP = "192.168.2.29";
    public int iPhonePort = 21412;
    public int listenPort = 50507;
    public bool isRunning = true;

    private UdpClient udpClient;
    private Thread listenThread;
    private Thread sendThread;

    void Start()
    {
        udpClient = new UdpClient(listenPort);

        listenThread = new Thread(ListenForData);
        listenThread.IsBackground = true;
        listenThread.Start();

        sendThread = new Thread(PeriodicSendRequest);
        sendThread.IsBackground = true;
        sendThread.Start();
    }

    void PeriodicSendRequest()
    {
        while (isRunning)
        {
            SendTrackingRequest();
            Thread.Sleep(4500);
        }
    }

    void SendTrackingRequest()
    {
        try
        {
            UdpClient sender = new UdpClient();
            sender.Connect(iPhoneIP, iPhonePort);

            string json = $"{{\"messageType\":\"iOSTrackingDataRequest\",\"time\":5,\"sentBy\":\"NyoVT\",\"ports\":[{listenPort}]}}";

            byte[] data = Encoding.UTF8.GetBytes(json);
            sender.Send(data, data.Length);
            sender.Close();

            Debug.Log($"Sent tracking request to {iPhoneIP}:{iPhonePort}");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error sending tracking request: " + ex.Message);
        }
    }

    void ListenForData()
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

        try
        {
            while (true)
            {
                byte[] data = udpClient.Receive(ref remoteEP);
                string received = Encoding.UTF8.GetString(data);

                Debug.Log($"Received from {remoteEP.Address}:{remoteEP.Port} - {received}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("UDP listening error: " + ex.Message);
        }
    }

    void OnApplicationQuit()
    {
        if (listenThread != null && listenThread.IsAlive)
        {
            listenThread.Abort();
            sendThread.Abort();
        }

        if (udpClient != null)
        {
            udpClient.Close();
        }
    }
}
