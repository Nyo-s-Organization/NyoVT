using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRM;

public class UpdateModel : MonoBehaviour
{
    public GameObject model;

    // input type is the kind of tracking input
    // when it's 0 it corresponds to vseeface
    public int inputType;

    public GetVtubeStudio getVtubeStudio;

    public Button resetButton;

    public GameObject settingsFrame;
    
    private bool loaded = false;
    private float animation_frame = 0f;

    private VRMBlendShapeProxy VRMBlendShapeProxyComponent;
    private Animator animator;
    private Transform head;

    private Transform leftEye;
    private Transform rightEye;

    private bool hasBlinkL;
    private bool hasBlinkR;

    private Transform leftUpperArm;
    private Transform rightUpperArm;

    private Vector3 callibrationPosition;
    private Vector3 callibrationRotation;

    void Start() {
        resetButton.onClick.AddListener(ResetCallibration);
    }

    void ResetCallibration() {
        callibrationPosition = getVtubeStudio.trackingPosition;
        callibrationRotation = getVtubeStudio.trackingRotation;
    }

    void Update()
    {
        if (!model) return;
        if (!loaded) {
            loaded = true;
            loadParameters(model);
            getVtubeStudio.StartVtubeStudio();
        }

        VRMBlendShapeProxyComponent.ImmediatelySetValue(BlendShapePreset.A, (Mathf.Sin(animation_frame) + 1f) / 2f);

        if (inputType == 0) {
            if (settingsFrame.active) {
                model.transform.position = new Vector3(
                    0.2f,
                    getVtubeStudio.trackingPosition.y - callibrationPosition.y,
                    getVtubeStudio.trackingPosition.z - callibrationPosition.z
                );
            } else {
                model.transform.position = new Vector3(
                    getVtubeStudio.trackingPosition.x - callibrationPosition.x,
                    getVtubeStudio.trackingPosition.y - callibrationPosition.y,
                    getVtubeStudio.trackingPosition.z - callibrationPosition.z
                );
            }

            head.localRotation = Quaternion.Euler(
                getVtubeStudio.trackingRotation.y - callibrationRotation.y,
                getVtubeStudio.trackingRotation.x - callibrationRotation.x,
                getVtubeStudio.trackingRotation.z - callibrationRotation.z
            );
            leftEye.localRotation = Quaternion.Euler(getVtubeStudio.eyeLeft.x, getVtubeStudio.eyeLeft.y, getVtubeStudio.eyeLeft.z);
            rightEye.localRotation = Quaternion.Euler(getVtubeStudio.eyeRight.x, getVtubeStudio.eyeRight.y, getVtubeStudio.eyeRight.z);

            foreach (BlendShape shape in getVtubeStudio.blendShapes)
            {
                switch (shape.k) {
                    case "eyeBlink_L":
                        if (hasBlinkL) {
                            VRMBlendShapeProxyComponent.ImmediatelySetValue(BlendShapePreset.Blink_L, shape.v * 2f);
                        }
                    break;
                    case "eyeBlink_R":
                        if (hasBlinkR) {
                            VRMBlendShapeProxyComponent.ImmediatelySetValue(BlendShapePreset.Blink_R, shape.v * 2f);
                        }
                    break;
                }
            }
        }

        animation_frame += 10f * Time.deltaTime;
    }

    void loadParameters(GameObject model) {
        Debug.Log("Loading Model Parameters");
        VRMBlendShapeProxyComponent = model.GetComponent<VRMBlendShapeProxy>();
        animator = model.GetComponent<Animator>();
        head = animator.GetBoneTransform(HumanBodyBones.Head);

        leftEye = animator.GetBoneTransform(HumanBodyBones.LeftEye);
        rightEye = animator.GetBoneTransform(HumanBodyBones.RightEye);

        hasBlinkL = VRMBlendShapeProxyComponent.GetValue(BlendShapePreset.Blink_L) >= 0f;
        hasBlinkR = VRMBlendShapeProxyComponent.GetValue(BlendShapePreset.Blink_R) >= 0f;

        leftUpperArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        rightUpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);

        leftUpperArm.localRotation = Quaternion.Euler(0f, 0f, 75f);
        rightUpperArm.localRotation = Quaternion.Euler(0f, 0f, -75f);
    }
}
