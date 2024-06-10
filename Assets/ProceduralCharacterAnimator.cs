using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ProceduralCharacterAnimator : MonoBehaviour
{
    [SerializeField] private CharacterMovementController characterMovementController;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Transform head;
    [SerializeField] private List<Transform> bones;
    [SerializeField] private float maxAngle = 20f;
    [SerializeField] private float smoothTime = 0.5f;

    private void Update()
    {
        AnimateBody();

        AnimateHead();
    }

    private void AnimateBody()
    {
        if (!characterMovementController)
            return;

        var direction = characterMovementController.Rigidbody.velocity;

        direction = transform.InverseTransformDirection(direction);

        if (direction.magnitude > characterMovementController.MaxSpeed)
        {
            direction = direction.normalized * characterMovementController.MaxSpeed;
        }

        direction /= characterMovementController.MaxSpeed;

        var xAngle = Quaternion.Euler(-direction.x * maxAngle, 0, 0);
        var zAngle = Quaternion.Euler(0, 0, direction.z * maxAngle);

        var targetRotation = xAngle * zAngle;

        foreach (var bone in bones)
        {
            bone.localRotation = Quaternion.Lerp(bone.localRotation, targetRotation, smoothTime);
        }
    }

    private void AnimateHead()
    {
        if (!virtualCamera)
            return;

        var rotationEulerAngles = virtualCamera.transform.rotation.eulerAngles;

        Debug.Log($"{rotationEulerAngles.x}");

        var targetRotation = Quaternion.Euler(rotationEulerAngles.x, 0, 0);

        targetRotation.eulerAngles -= new Vector3(89.98f, 0, 0);

        head.rotation = Quaternion.Lerp(head.localRotation, targetRotation, smoothTime);
    }
}