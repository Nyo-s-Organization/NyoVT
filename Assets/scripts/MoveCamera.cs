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
        if (Input.GetKey(KeyCode.LeftAlt)) {
            if (Input.GetMouseButton(0)) {
                float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * 5f;
                float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * 5f;
                camera.transform.position += new Vector3(mouseX, -mouseY, 0f);
            } else if (Input.GetAxis("Mouse ScrollWheel") != 0f) {
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                camera.orthographicSize += -scroll * 0.1f;
            }
        }
    }
}
