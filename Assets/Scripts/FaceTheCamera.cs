using UnityEngine;

public class FaceTheCamera : MonoBehaviour
{
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        transform.forward = _camera.transform.forward;
    }
}
