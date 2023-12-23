using System.Linq;
using UnityEngine;

public class MeleeAttackController : MonoBehaviour
{
    [SerializeField]
    private float attackForce = 1000f;

    [SerializeField]
    private Collider playerCollider;

    private SphereCollider _sphereCollider;

    private readonly Collider[] hitColliders = new Collider[10];

    private void Awake()
    {
        _sphereCollider = GetComponent<SphereCollider>();
    }

    public void Attack()
    {
        Physics.OverlapSphereNonAlloc(transform.position, _sphereCollider.radius, hitColliders);

        var colliders = hitColliders.Where(collider => collider && collider != playerCollider);

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<Rigidbody>(out var rb))
            {
                Vector3 direction = (rb.transform.position - transform.position).normalized;
                rb.AddForce(direction * attackForce, ForceMode.Impulse);
            }
        }
    }
}
