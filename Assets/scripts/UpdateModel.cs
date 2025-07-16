using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateModel : MonoBehaviour
{
    public GameObject model;

    void Update()
    {
        if (!model) return;
        Debug.Log("Model has been loaded");
    }
}
