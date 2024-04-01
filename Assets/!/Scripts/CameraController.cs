using System.Collections;
using UnityEngine;
using Cinemachine;
using System;

public class CameraController : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    CinemachineComponentBase componentBase;
    float cameraDistance;
    [SerializeField] float sensitivity = 10f;
    [SerializeField] float maxCameraDistance = 30f;
    [SerializeField] float minCameraDistance = 5f;

    private void Update()
    {
        if (componentBase == null)
        {
            componentBase = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
        }
        HandleZoom();
    }

    public void HandleZoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            if (componentBase is CinemachineFramingTransposer)
            {
                cameraDistance = Input.GetAxis("Mouse ScrollWheel") * sensitivity + (componentBase as CinemachineFramingTransposer).m_CameraDistance;
                Debug.Log(cameraDistance);
                cameraDistance = Mathf.Clamp(cameraDistance, minCameraDistance, maxCameraDistance);
                Debug.Log(cameraDistance);
                (componentBase as CinemachineFramingTransposer).m_CameraDistance = cameraDistance;
            }
        }
    }
}
