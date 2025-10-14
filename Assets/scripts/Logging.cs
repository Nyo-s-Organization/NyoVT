using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Logging : MonoBehaviour
{
    public TMP_Text text;
    public string output = "";
    public string stack = "";

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        output += logString;
        stack = stackTrace;
        text.text = output;
    }
}
