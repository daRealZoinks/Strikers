using Unity.Netcode;
using UnityEngine;

public class AdvancedNetworkRigidbody : NetworkBehaviour
{
    [SerializeField] private bool teleportEnabled;
    [SerializeField] private float teleportIfDistanceGreaterThan = 3.0f;

    private Rigidbody _rigidbody;

    private float _distance;
    private float _angle;

    private Vector3 _networkPosition;
    private Quaternion _networkRotation;

    private readonly NetworkVariable<PhysicsSnapshot> _physicsSnapshot =
        new(writePerm: NetworkVariableWritePermission.Owner);

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _networkPosition = new Vector3();
        _networkRotation = new Quaternion();

        _physicsSnapshot.OnValueChanged += OnPhysicsSnapshotChanged;
    }

    private void OnPhysicsSnapshotChanged(PhysicsSnapshot previousValue, PhysicsSnapshot newValue)
    {
        if (IsOwner) return;

        _networkPosition = newValue.Position;

        if (teleportEnabled)
        {
            if (Vector3.Distance(_rigidbody.position, _networkPosition) > teleportIfDistanceGreaterThan)
            {
                _rigidbody.position = _networkPosition;
            }
        }

        _networkRotation = newValue.Rotation;

        _rigidbody.rotation = _networkRotation;

        _rigidbody.velocity = _physicsSnapshot.Value.Velocity;
        _distance = Vector3.Distance(_rigidbody.position, _networkPosition);

        _rigidbody.angularVelocity = _physicsSnapshot.Value.AngularVelocity;
        _angle = Quaternion.Angle(_rigidbody.rotation, _networkRotation);
    }

    private void FixedUpdate()
    {
        if (!NetworkManager.IsListening) return;

        if (IsOwner)
        {
            _physicsSnapshot.Value = new PhysicsSnapshot
            {
                Position = _rigidbody.position,
                Rotation = _rigidbody.rotation,
                Velocity = _rigidbody.velocity,
                AngularVelocity = _rigidbody.angularVelocity
            };
        }
        else
        {
            _rigidbody.position = Vector3.MoveTowards(_rigidbody.position, _networkPosition, _distance);
            _rigidbody.rotation = Quaternion.RotateTowards(_rigidbody.rotation, _networkRotation, _angle);
        }
    }

    private struct PhysicsSnapshot : INetworkSerializable
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;
        public Vector3 AngularVelocity;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Position);
            serializer.SerializeValue(ref Rotation);
            serializer.SerializeValue(ref Velocity);
            serializer.SerializeValue(ref AngularVelocity);
        }
    }
}