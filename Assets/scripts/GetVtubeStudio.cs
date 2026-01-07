using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[Serializable]
public class Vector3Data
{
    public float x;
    public float y;
    public float z;
}

[Serializable]
public class BlendShape
{
    public string k;  // key
    public float v;   // value
}

[Serializable]
public class TrackingData
{
    public long Timestamp;
    public int Hotkey;
    public bool FaceFound;
    public Vector3Data Position;
    public Vector3Data Rotation;
    public Vector3Data EyeLeft;
    public Vector3Data EyeRight;
    public List<BlendShape> BlendShapes;
}

public class GetVtubeStudio : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField ipInputField; // assign in Inspector

    [Header("Settings")]
    public string iPhoneIP;
    public int iPhonePort = 21412;
    public int listenPort = 50507;
    public bool isRunning = true;

    public Vector3 trackingPosition;
    public Vector3 trackingRotation;
    public Vector3 eyeLeft;
    public Vector3 eyeRight;
    public List<BlendShape> blendShapes;

    private UdpClient udpClient;
    private Thread listenThread;
    private Thread sendThread;

    private void Awake()
    {
        // Load saved IP or default
        iPhoneIP = PlayerPrefs.GetString("iPhoneIP", "192.168.178.1");

        // Set the TMP InputField text
        if (ipInputField != null)
        {
            ipInputField.text = iPhoneIP;
            ipInputField.onEndEdit.AddListener(OnIPChanged);
        }
    }

    private void OnIPChanged(string newIP)
    {
        iPhoneIP = newIP;
        PlayerPrefs.SetString("iPhoneIP", iPhoneIP);
        PlayerPrefs.Save();
        Debug.Log($"Saved new iPhone IP: {iPhoneIP}");
    }

    public void StartVtubeStudio()
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
                TrackingData trackingData = JsonUtility.FromJson<TrackingData>(received);

                trackingPosition.x = trackingData.Position.x / 25f;
                trackingPosition.y = trackingData.Position.y / 25f;
                trackingPosition.z = -trackingData.Position.z / 25f;
                trackingRotation.x = -trackingData.Rotation.x;
                trackingRotation.y = trackingData.Rotation.y;
                trackingRotation.z = -trackingData.Rotation.z;

                eyeLeft.x = trackingData.EyeLeft.x;
                eyeLeft.y = -trackingData.EyeLeft.y;
                eyeLeft.z = trackingData.EyeLeft.z;
                eyeRight.x = trackingData.EyeRight.x;
                eyeRight.y = -trackingData.EyeRight.y;
                eyeRight.z = trackingData.EyeRight.z;

                blendShapes = trackingData.BlendShapes;

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
            listenThread.Abort();
        if (sendThread != null && sendThread.IsAlive)
            sendThread.Abort();

        if (udpClient != null)
            udpClient.Close();
    }
}
