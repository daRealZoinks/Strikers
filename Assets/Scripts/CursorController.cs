using UnityEngine;

public class CursorController : MonoBehaviour
{
    private bool _isCursorLocked;

    public bool IsCursorLocked
    {
        get { return _isCursorLocked; }
        set
        {
            _isCursorLocked = value;
            UpdateCursorState();
        }
    }

    private void UpdateCursorState()
    {
        if (_isCursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}