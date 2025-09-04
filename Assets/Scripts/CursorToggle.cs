using UnityEngine;
using UnityEngine.InputSystem;

public class CursorToggle : MonoBehaviour
{
    public Key toggleKey = Key.F1;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current[toggleKey].wasPressedThisFrame)
        {
            bool locked = Cursor.lockState == CursorLockMode.Locked;
            Cursor.lockState = locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !locked;
        }
    }
}
