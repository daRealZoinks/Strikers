using Cinemachine;
using UnityEngine;

public class CameraOrbiting : MonoBehaviour
{
    [SerializeField] private float speed = 0.03f;

    private CinemachineVirtualCamera _virtualCamera;

    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        var orbitalTransposer = _virtualCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();

        orbitalTransposer.m_XAxis.m_InputAxisValue = speed;
    }
}