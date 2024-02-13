using System;
using Cinemachine;
using UnityEngine;

public class Firepoint : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    [SerializeField] private LayerMask layerMask;

    private void Update()
    {
        if (!cinemachineVirtualCamera) return;

        var cinemachineVirtualCameraTransform = cinemachineVirtualCamera.transform;
        var ray = new Ray(cinemachineVirtualCameraTransform.position, cinemachineVirtualCameraTransform.forward);

        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask)) return;

        transform.LookAt(hit.point);
    }
}