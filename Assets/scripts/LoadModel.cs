using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRM;
using UniGLTF;

public class LoadModel : MonoBehaviour
{
    private VRMImporterContext loadedContext;
    private RuntimeGltfInstance loadedInstance;

    public GameObject LoadVRMModel(string path) {
        if (!path.ToLower().EndsWith(".vrm")){
            Debug.LogWarning("This is not a VRM file");
            return null;
        }

        using var glbData = new GlbFileParser(path).Parse();
        var vrmData = new VRMData(glbData);
        var materialGenerator = new BuiltInVrmMaterialDescriptorGenerator(vrmData.VrmExtension);
        
        loadedContext = new VRMImporterContext(vrmData, materialGenerator: materialGenerator);
        loadedInstance = loadedContext.Load();
        loadedInstance.ShowMeshes();
        loadedInstance.EnableUpdateWhenOffscreen();

        Debug.Log("VRM model imported: " + loadedInstance.Root.name);
        return loadedInstance.Root;
    }

    public IEnumerator LoadVRMModelCoroutine(string path, System.Action<GameObject> onComplete)
    {
        if (!path.ToLower().EndsWith(".vrm"))
        {
            Debug.LogWarning("This is not a VRM file");
            yield break;
        }

        yield return null;

        using var glbData = new GlbFileParser(path).Parse();
        yield return null;

        var vrmData = new VRMData(glbData);
        var materialGenerator = new BuiltInVrmMaterialDescriptorGenerator(vrmData.VrmExtension);
        loadedContext = new VRMImporterContext(vrmData, materialGenerator: materialGenerator);
        yield return null;

        loadedInstance = loadedContext.Load();
        yield return null;

        loadedInstance.ShowMeshes();
        loadedInstance.EnableUpdateWhenOffscreen();

        Debug.Log("VRM model imported: " + loadedInstance.Root.name);
        onComplete?.Invoke(loadedInstance.Root);
    }
}
