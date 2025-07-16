using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Camera camera;
    public GameObject handler;

    private UpdateModel ModelHandler;

    void Start() {
        ModelHandler = handler.GetComponent<UpdateModel>();
    }

    void Update()
    {
        if (!ModelHandler.model) return;
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButton(0)) {
            float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * 5f;
            float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * 5f;
            camera.transform.position += new Vector3(mouseX, -mouseY, 0f);
        }
    }
}
