using UnityEngine;
using UnityEngine.InputSystem;

public class InputProvider : MonoBehaviour
{
    [SerializeField]
    private CharacterCameraController characterCameraController;
    private CharacterMovementController _characterMovementController;
    private CharacterWallRunController _characterWallRunController;

    private void Awake()
    {
        _characterMovementController = GetComponent<CharacterMovementController>();
        _characterWallRunController = GetComponent<CharacterWallRunController>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        var value = context.phase switch
        {
            InputActionPhase.Started or InputActionPhase.Performed => context.ReadValue<Vector2>(),
            _ => Vector2.zero,
        };

        _characterMovementController.MovementInput = value;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        var value = context.phase switch
        {
            InputActionPhase.Started or InputActionPhase.Performed => context.ReadValue<Vector2>(),
            _ => Vector2.zero
        };

        characterCameraController.LookInput = value;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;

        _characterMovementController.Jump();
        _characterWallRunController.WallJump();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;

        Debug.Log("Fire");
    }

    public void OnMelee(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;

        Debug.Log("Melee");
    }
}
