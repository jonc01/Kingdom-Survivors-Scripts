using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraGetPlayer : MonoBehaviour
{
    private CinemachineVirtualCamera vCam;
    public GameObject followTarget;
    public Transform targetTransform;

    void Start()
    {
        // vCam = GetComponent<CinemachineVirtualCamera>();

        // if(followTarget == null) followTarget = GameObject.FindGameObjectWithTag("CameraTarget");

        // if(followTarget != null)
        // {
        //     targetTransform = followTarget.transform;

        //     if(vCam.LookAt == null) vCam.LookAt = targetTransform;
        //     if(vCam.Follow == null) vCam.Follow = targetTransform;
        // }
    }

    void Update()
    {
        Setup();
    }

    void Setup()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();

        if(followTarget == null) followTarget = GameObject.FindGameObjectWithTag("CameraTarget");

        if(followTarget != null)
        {
            targetTransform = followTarget.transform;

            if(vCam.LookAt == null) vCam.LookAt = targetTransform;
            if(vCam.Follow == null) vCam.Follow = targetTransform;

            this.enabled = false;
        }
    }
}
