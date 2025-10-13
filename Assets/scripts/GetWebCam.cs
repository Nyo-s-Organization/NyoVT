using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetWebCam : MonoBehaviour
{
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log("Camera " + i + ": " + devices[i].name);
        }

        if (devices.Length > 0)
        {
            WebCamTexture webcamTexture = new WebCamTexture(devices[0].name);
            Renderer renderer = GetComponent<Renderer>();
            renderer.material.mainTexture = webcamTexture;
            webcamTexture.Play();
        }
    }
}
