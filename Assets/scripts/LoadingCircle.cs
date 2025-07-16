using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCircle : MonoBehaviour
{
    public RawImage LoadingCircleImage;
    public bool isLoading = false;
    public bool load = false;

    void Update()
    {
        if (load) {
            load = false;
            LoadingCircleImage.enabled = true;
        }
        if (!isLoading) {
            LoadingCircleImage.enabled = false;
        } else {
            LoadingCircleImage.transform.Rotate(0f, 0f, 10f);
        }
    }
}
