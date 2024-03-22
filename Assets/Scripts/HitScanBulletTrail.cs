using UnityEngine;
using UnityEngine.VFX;

public class HitScanBulletTrail : MonoBehaviour
{
    [SerializeField] private VisualEffect impactEffect;

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

    public void PlayImpactEffect(Vector3 position, Quaternion rotation)
    {
        var impactEffectInstance = Instantiate(impactEffect, position, rotation);
        impactEffectInstance.Play();
        Destroy(impactEffectInstance.gameObject, 1f);
    }
}