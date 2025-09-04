using UnityEngine;
using UnityEngine.InputSystem; // Requiere Input System activo

/// Flycam suave con Input System.
/// Controles:
///  - WASD: mover
///  - Espacio / E: subir
///  - Ctrl / Q: bajar
///  - Shift: sprint
///  - Rueda: ajustar velocidad base
///  - ESC: alterna lock/unlock del cursor
///  - C: alterna "mantener RMB para mirar" vs "mirar siempre"
///  - F10: forzar lock (por si el cursor quedó suelto)
///  - F11: forzar unlock
public class SimpleCameraController : MonoBehaviour
{
    [Header("Movimiento")]
    public float baseSpeed = 6f;
    public float sprintMultiplier = 2.5f;
    public float acceleration = 14f;
    public float deceleration = 16f;
    public float maxSpeed = 25f;
    public float verticalBoost = 1.0f;

    [Header("Mira (mouse)")]
    public float lookSensitivity = 0.12f;
    public bool holdRightToLook = false; // ← mira SIEMPRE por defecto
    public bool invertY = false;
    public float pitchLimit = 85f;

    [Header("Comodidad")]
    public bool lockCursorOnPlay = true;
    public float wheelSpeedStep = 0.75f;
    public float minBaseSpeed = 1.0f;
    public float maxBaseSpeed = 20.0f;

    Vector3 _velocity;
    float _yaw, _pitch;
    bool _cursorLocked;

    void Start()
    {
        var e = transform.eulerAngles;
        _yaw = e.y;
        _pitch = e.x;

        if (lockCursorOnPlay) LockCursor(true);
    }

    void Update()
    {
        // Si la ventana pierde foco, evita lecturas raras
        if (!Application.isFocused) return;

        var kb = Keyboard.current;
        var mouse = Mouse.current;
        if (kb == null || mouse == null) return;

        // Toggle modo mirar (RMB requerido o no)
        if (kb.cKey.wasPressedThisFrame)
            holdRightToLook = !holdRightToLook;

        // Forzar lock/unlock
        if (kb.f10Key.wasPressedThisFrame) LockCursor(true);
        if (kb.f11Key.wasPressedThisFrame) LockCursor(false);

        // ESC alterna lock/unlock
        if (kb.escapeKey.wasPressedThisFrame) LockCursor(!_cursorLocked);

        // Ajustar velocidad con la rueda
        float wheel = mouse.scroll.ReadValue().y;
        if (Mathf.Abs(wheel) > 0.01f)
            baseSpeed = Mathf.Clamp(baseSpeed + Mathf.Sign(wheel) * wheelSpeedStep, minBaseSpeed, maxBaseSpeed);

        // Mouse look
        bool allowLook = _cursorLocked && (!holdRightToLook || mouse.rightButton.isPressed);
        if (allowLook)
        {
            Vector2 d = mouse.delta.ReadValue();
            float sx = d.x * 180f * lookSensitivity * Time.deltaTime;
            float sy = d.y * 180f * lookSensitivity * Time.deltaTime;

            _yaw += sx;
            _pitch += (invertY ? sy : -sy);
            _pitch = Mathf.Clamp(_pitch, -pitchLimit, pitchLimit);

            transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        }

        // Dirección deseada (WASD + Space/Ctrl + E/Q)
        Vector3 wish = Vector3.zero;
        if (kb.wKey.isPressed) wish += Vector3.forward;
        if (kb.sKey.isPressed) wish += Vector3.back;
        if (kb.aKey.isPressed) wish += Vector3.left;
        if (kb.dKey.isPressed) wish += Vector3.right;

        float up = 0f;
        if (kb.spaceKey.isPressed || kb.eKey.isPressed) up += 1f;
        if (kb.leftCtrlKey.isPressed || kb.qKey.isPressed) up -= 1f;
        wish += Vector3.up * up * verticalBoost;

        // A la orientación de la cámara
        wish = transform.TransformDirection(wish.normalized);

        // Velocidad objetivo (con sprint)
        float targetSpeed = baseSpeed * (kb.leftShiftKey.isPressed ? sprintMultiplier : 1f);
        targetSpeed = Mathf.Min(targetSpeed, maxSpeed);
        Vector3 targetVel = wish * targetSpeed;

        // Aceleración/frenado suaves
        if (wish.sqrMagnitude > 0.0001f)
            _velocity = Vector3.MoveTowards(_velocity, targetVel, acceleration * Time.deltaTime);
        else
            _velocity = Vector3.MoveTowards(_velocity, Vector3.zero, deceleration * Time.deltaTime);

        // Aplicar movimiento
        transform.position += _velocity * Time.deltaTime;
    }

    void LockCursor(bool locked)
    {
        _cursorLocked = locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
