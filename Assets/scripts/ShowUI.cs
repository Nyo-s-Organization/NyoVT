using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowUI : MonoBehaviour
{
    public GameObject buttonUI;

    void Update()
    {
        if (!Application.isFocused) {
            buttonUI.SetActive(false);
        } else {
            buttonUI.SetActive(true);
        }
    }
}
