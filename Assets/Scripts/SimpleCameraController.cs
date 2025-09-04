using UnityEngine;
using UnityEngine.InputSystem; // Nuevo Input System

public class SimpleCameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float lookSpeed = 0.15f;

    float yaw, pitch;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void Update()
    {
        // Mouse look
        if (Mouse.current != null)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            yaw += delta.x * lookSpeed;
            pitch -= delta.y * lookSpeed;
            pitch = Mathf.Clamp(pitch, -89f, 89f);
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }

        // WASD + Q/E
        Vector3 dir = Vector3.zero;
        if (Keyboard.current != null)
        {
            var kb = Keyboard.current;
            if (kb.wKey.isPressed) dir += Vector3.forward;
            if (kb.sKey.isPressed) dir += Vector3.back;
            if (kb.aKey.isPressed) dir += Vector3.left;
            if (kb.dKey.isPressed) dir += Vector3.right;
            if (kb.eKey.isPressed) dir += Vector3.up;
            if (kb.qKey.isPressed) dir += Vector3.down;
        }

        transform.Translate(dir.normalized * moveSpeed * Time.deltaTime, Space.Self);
    }
}
