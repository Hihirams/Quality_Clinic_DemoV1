using UnityEngine;
using UnityEngine.EventSystems;

public class CursorToggle : MonoBehaviour
{
    public KeyCode toggleKey = KeyCode.Escape;
    public bool lockAtStart = false;

    void Start() { SetLock(lockAtStart); }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            SetLock(Cursor.lockState != CursorLockMode.Locked);
    }

    public static bool IsOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    public static void SetLock(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
