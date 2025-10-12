using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Camera targetCamera;
    public Toggle uiToggle;

    void Start()
    {
        if (uiToggle != null)
        {
            uiToggle.onValueChanged.AddListener(OnToggleChanged);
            OnToggleChanged(uiToggle.isOn);
        }
    }

    void OnToggleChanged(bool isOn)
    {
        if (isOn)
        {
            targetCamera.backgroundColor = new Color(0f/255f, 255f/255f, 0f/255f, 1f);
        }
        else
        {
            targetCamera.backgroundColor = new Color(125f/255f, 125f/255f, 125f/255f, 0f);
        }
    }
}
