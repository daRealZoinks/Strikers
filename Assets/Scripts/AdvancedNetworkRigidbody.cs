using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AdvancedNetworkRigidbody : NetworkBehaviour
{
    [SerializeField] private bool teleportEnabled = true;
    [SerializeField] private float teleportIfDistanceGreaterThan = 10.0f;

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

        var localTime = (float)NetworkManager.NetworkTimeSystem.LocalTime;
        var serverTime = (float)NetworkManager.NetworkTimeSystem.ServerTime;
        var timeDifference = localTime - serverTime;

        // Calculate the predicted position and rotation
        var predictedPosition = newValue.Position + newValue.Velocity * timeDifference;
        var predictedRotation = Quaternion.Euler(newValue.AngularVelocity * timeDifference) * newValue.Rotation;

        // Check if the distance between the current position and the predicted position is greater than the threshold
        if (teleportEnabled && Vector3.Distance(_rigidbody.position, predictedPosition) > teleportIfDistanceGreaterThan)
        {
            // Teleport the object to the predicted position
            _rigidbody.position = predictedPosition;
            _rigidbody.rotation = predictedRotation;
        }
        else
        {
            // Smoothly interpolate between the current state and the predicted state
            _rigidbody.position = Vector3.Lerp(_rigidbody.position, predictedPosition, 0.1f);
            _rigidbody.rotation = Quaternion.Lerp(_rigidbody.rotation, predictedRotation, 0.1f);
        }

        _rigidbody.velocity = newValue.Velocity;
        _rigidbody.angularVelocity = newValue.AngularVelocity;
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