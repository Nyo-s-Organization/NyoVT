using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRM;

public class UpdateModel : MonoBehaviour
{
    public GameObject model;
    
    private bool loaded = false;
    private float animation_frame = 0f;

    private VRMBlendShapeProxy VRMBlendShapeProxyComponent;
    private Animator animator;
    private Transform head;
    private Transform leftUpperArm;
    private Transform rightUpperArm;

    void Update()
    {
        if (!model) return;
        if (!loaded) {
            loaded = true;
            loadParameters(model);
        }

        VRMBlendShapeProxyComponent.ImmediatelySetValue(BlendShapePreset.A, (Mathf.Sin(animation_frame) + 1f) / 2f);
        head.localRotation = Quaternion.Euler(Mathf.Sin(animation_frame / 2f) * 20f, 0f, 0f);

        animation_frame += 10f * Time.deltaTime;
    }

    void loadParameters(GameObject model) {
        Debug.Log("Loading Model Parameters");
        VRMBlendShapeProxyComponent = model.GetComponent<VRMBlendShapeProxy>();
        animator = model.GetComponent<Animator>();
        head = animator.GetBoneTransform(HumanBodyBones.Head);
        leftUpperArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        rightUpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);

        leftUpperArm.localRotation = Quaternion.Euler(0f, 0f, 75f);
        rightUpperArm.localRotation = Quaternion.Euler(0f, 0f, -75f);
    }
}
