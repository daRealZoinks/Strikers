using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class MeleeAttackController : NetworkBehaviour
{
    [SerializeField] private float attackForce = 1000f;
    [SerializeField] private float attackCooldown = 1f;

    [SerializeField] private Collider playerCollider;
    [SerializeField] private Transform cameraTransform;

    private SphereCollider _sphereCollider;
    private float _nextAttackTime;

    private bool CanAttack => _nextAttackTime >= attackCooldown;

    private void Awake()
    {
        _sphereCollider = GetComponent<SphereCollider>();
    }

    private void Update()
    {
        if (IsOwner && !CanAttack)
        {
            _nextAttackTime += Time.deltaTime;
        }
    }

    public void ExecuteAttack()
    {
        if (IsOwner && CanAttack)
        {
            AttackServerRpc(transform.position);
            Attack(transform.position);
            _nextAttackTime = 0;
        }
    }

    [ServerRpc]
    private void AttackServerRpc(Vector3 transformPosition)
    {
        AttackClientRpc(transformPosition);
    }

    [ClientRpc]
    private void AttackClientRpc(Vector3 transformPosition)
    {
        if (!IsOwner)
        {
            Attack(transformPosition);
        }
    }

    private void Attack(Vector3 position)
    {
        var colliders = Physics.OverlapSphere(position, _sphereCollider.radius);

        var filteredColliders = colliders.Where(c => c && c != playerCollider);

        foreach (var filteredCollider in filteredColliders)
        {
            if (!filteredCollider.TryGetComponent<Rigidbody>(out var rb)) continue;

            var direction = cameraTransform.forward.normalized * attackForce;

            // compensate for the current velocity relative to the camera
            // if the object is moving towards the camera, add force in the opposite direction
            // if the object is moving away from the camera, don't compensate

            var velocity = rb.velocity;
            var dot = Vector3.Dot(velocity.normalized, cameraTransform.forward);

            if (dot < 0)
            {
                direction -= velocity;
            }

            rb.velocity = Vector3.zero;

            rb.AddForce(direction, ForceMode.Impulse);
        }
    }
}