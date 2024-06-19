using UnityEngine;
using UnityEngine.InputSystem;

public class InputProvider : MonoBehaviour
{
    [SerializeField]
    private MeleeAttackController meleeAttackController;
    [SerializeField]
    private GunManager gunManager;

    private CharacterMovementController _characterMovementController;

    private void Awake()
    {
        _characterMovementController = GetComponent<CharacterMovementController>();
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

        _characterMovementController.LookInput = value;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;

        _characterMovementController.Jump();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;

        gunManager.Shoot();
    }

    public void OnMelee(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;

        meleeAttackController.ExecuteAttack();
    }
}
