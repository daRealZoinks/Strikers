using UnityEngine;

public class Firepoint : MonoBehaviour
{
    private void Update()
    {
        var ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out var hit))
        {
            transform.LookAt(hit.point);
        }
        else
        {
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }
}