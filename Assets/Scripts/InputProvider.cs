using UnityEngine;
using UnityEngine.InputSystem;

public class InputProvider : MonoBehaviour
{
    public void OnMove(InputAction.CallbackContext context)
    {
        var value = context.phase switch
        {
            InputActionPhase.Started or InputActionPhase.Performed => context.ReadValue<Vector2>(),
            _ => Vector2.zero,
        };

        Debug.Log($"Move: {value}");
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        var value = context.phase switch
        {
            InputActionPhase.Started or InputActionPhase.Performed => context.ReadValue<Vector2>(),
            _ => Vector2.zero
        };

        Debug.Log($"Look: {value}");
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;

        Debug.Log("Jump");
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
