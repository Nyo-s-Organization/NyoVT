using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Camera camera;
    public GameObject handler;
    public GameObject settingsFrame;

    public float sensitivity;

    private UpdateModel ModelHandler;

    void Start() {
        ModelHandler = handler.GetComponent<UpdateModel>();
    }

    void Update()
    {
        if (!ModelHandler.model) return;
        if (settingsFrame.active) return;
        if (Input.GetKey(KeyCode.LeftAlt)) {
            if (Input.GetMouseButton(0)) {
                float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;
                float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
                camera.transform.position += new Vector3(mouseX, -mouseY, 0f);
            } else if (Input.GetAxis("Mouse ScrollWheel") != 0f) {
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                camera.orthographicSize += -scroll * 0.1f;
            }
        }
    }
}
