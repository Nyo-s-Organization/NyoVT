using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GetPath : MonoBehaviour
{
    public Button submitButton;
    public LoadModel loader;

    private string modelPath = "";
    private bool loadModel = false;

    void Start()
    {
        submitButton.onClick.AddListener(ShowFileBrowser);
    }

    void ShowFileBrowser()
    {
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    IEnumerator ShowLoadDialogCoroutine()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("VRM Files", ".vrm"));
        FileBrowser.SetDefaultFilter(".vrm");
        FileBrowser.ShowHiddenFiles = false;

        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null, "Select VRM Model", "Submit");

        if (FileBrowser.Success)
        {
            string filePath = FileBrowser.Result[0];
            Debug.Log("Selected file: " + filePath);
            loadModel = true;
            modelPath = filePath;
        }
    }

    void Update()
    {
        if (loadModel)
        {
            loadModel = false;
            GameObject avatar = loader.LoadVRMModel(modelPath);
            if (avatar != null)
            {
                avatar.transform.position = new Vector3(0, 0, 0);
            }
        }
    }
}
