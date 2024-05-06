using UnityEngine;
using UnityEngine.VFX;

public class HitScanBullet : MonoBehaviour
{
    [SerializeField] private VisualEffect impactEffect;
    [SerializeField] private AudioSource hitSound;

    [SerializeField] private float timeToDestroy = 2f;

    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();

        Destroy(gameObject, timeToDestroy);
    }

    private void Update()
    {
        var colorGradient = _lineRenderer.colorGradient;
        var alphaKeys = colorGradient.alphaKeys;

        var alphaKey1 = alphaKeys[1];
        alphaKey1.alpha -= Time.deltaTime / timeToDestroy;
        alphaKeys[1] = alphaKey1;

        var alphaKey2 = alphaKeys[2];
        alphaKey2.alpha -= Time.deltaTime / timeToDestroy;
        alphaKeys[2] = alphaKey2;

        colorGradient.alphaKeys = alphaKeys;

        _lineRenderer.colorGradient = colorGradient;
    }

    public void SetPositions(Vector3 start, Vector3 middle, Vector3 end)
    {
        _lineRenderer.SetPosition(0, start);
        _lineRenderer.SetPosition(1, middle);
        _lineRenderer.SetPosition(2, end);
    }

    public void PlayImpactSequence(Vector3 position, Quaternion rotation)
    {
        var impactEffectInstance = Instantiate(impactEffect, position, rotation);
        impactEffectInstance.Play();
        Destroy(impactEffectInstance.gameObject, 1f);
        AudioSource.PlayClipAtPoint(hitSound.clip, position);
    }
}