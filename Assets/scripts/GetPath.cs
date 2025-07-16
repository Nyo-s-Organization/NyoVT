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
    public RawImage LoadingCircleImage;

    public bool LoadAsync = true;

    private LoadingCircle loadingCircle;
    private string modelPath = "";
    private int loadModel = -1;

    void Start()
    {
        loadingCircle = LoadingCircleImage.GetComponent<LoadingCircle>();
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
            loadModel = 10;
            modelPath = filePath;
            loadingCircle.load = true;
            loadingCircle.isLoading = true;
        }
    }

    void Update()
    {
        if (loadModel == 0) {
            loadModel = -1;
            if (LoadAsync) {
                StartCoroutine(loader.LoadVRMModelCoroutine(modelPath, (go) => {
                    go.transform.position = Vector3.zero;
                    loadingCircle.isLoading = false;
                }));
            } else {
                GameObject avatar = loader.LoadVRMModel(modelPath);
                if (avatar != null)
                {
                    avatar.transform.position = new Vector3(0, 0, 0);
                }
                loadingCircle.isLoading = false;
            }
        } else if (loadModel > 0) loadModel -= 1;
    }
}
