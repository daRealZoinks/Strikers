using System.Linq;
using UnityEngine;

public class MeleeAttackController : MonoBehaviour
{
    [SerializeField]
    private float attackForce = 1000f;

    [SerializeField]
    private Collider playerCollider;

    [SerializeField]
    private Transform cameraTransform;

    private SphereCollider _sphereCollider;

    private readonly Collider[] hitColliders = new Collider[10];

    private void Awake()
    {
        _sphereCollider = GetComponent<SphereCollider>();
    }

    public void Attack()
    {
        Physics.OverlapSphereNonAlloc(transform.position, _sphereCollider.radius, hitColliders);

        var filteredColliders = hitColliders.Where(c => c && c != playerCollider);

        foreach (var filteredCollider in filteredColliders)
        {
            if (!filteredCollider.TryGetComponent<Rigidbody>(out var rb)) continue;

            var direction = (rb.transform.position - cameraTransform.position).normalized;
            rb.AddForce(direction * attackForce, ForceMode.Impulse);
        }
    }
}