using Cinemachine;
using UnityEngine;

public class FirePoint : MonoBehaviour
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var thisTransform = transform;
        var position = thisTransform.position;

        if (!cinemachineVirtualCamera) return;
        var cinemachineVirtualCameraTransform = cinemachineVirtualCamera.transform;
        var ray = new Ray(cinemachineVirtualCameraTransform.position, cinemachineVirtualCameraTransform.forward);
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask))
        {
            Gizmos.DrawLine(position, hit.point);
        }
    }
}