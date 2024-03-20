using UnityEngine;

public class HitScanBulletTrail : MonoBehaviour
{
    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();

        Destroy(gameObject, 2f);
    }

    public void SetPositions(Vector3 start, Vector3 end)
    {
        _lineRenderer.SetPosition(0, start);
        _lineRenderer.SetPosition(1, end);
    }
}