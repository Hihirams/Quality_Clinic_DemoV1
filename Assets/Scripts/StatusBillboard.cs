using TMPro;
using UnityEngine;

[RequireComponent(typeof(Billboard))]
public class StatusBillboard : MonoBehaviour
{
    [Header("Refs")]
    public TextMeshPro text;      // Label (TextMeshPro 3D)
    public MeshRenderer back;     // MeshRenderer del Quad

    [Header("Auto-layout del fondo")]
    public Vector2 padding = new Vector2(1.0f, 0.65f); // ancho/alto extra
    public float backZ = 0f;
    public float textZ = -0.01f;
    public float minWidth = 3.0f;   // evita pill muy angosto
    public float minHeight = 1.4f;  // evita pill muy bajo
    public float inflate = 1.15f;   // multiplica tamaño medido para dar “colchón”
    public float yOffset = 0f;      // subida fija adicional por etiqueta

    [Header("Levitación")]
    public float bobAmplitude = 0.12f;
    public float bobSpeed = 1.1f;

    Vector3 _baseLocalPos;

    void Reset()
    {
        if (!back) back = GetComponent<MeshRenderer>();
        if (!text) text = GetComponentInChildren<TextMeshPro>(true);
    }

    void Awake()
    {
        // base + offset vertical configurable
        _baseLocalPos = transform.localPosition + new Vector3(0f, yOffset, 0f);
        EnsureWiring();
        ApplyLayout();
    }

    void LateUpdate()
    {
        // Vaivén suave
        float y = Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
        transform.localPosition = _baseLocalPos + new Vector3(0f, y, 0f);
    }

    void EnsureWiring()
    {
        if (text)
        {
            if (text.transform.parent != transform)
                text.transform.SetParent(transform, worldPositionStays: false);

            text.transform.localRotation = Quaternion.identity;
            text.transform.localScale = Vector3.one;
            text.alignment = TextAlignmentOptions.Center;

            // ✅ Propiedades correctas en TMP:
            text.enableWordWrapping = false;                 // (NO existe .wordWrapping)
            text.overflowMode = TextOverflowModes.Overflow;  // que no recorte
        }

        if (back && back.transform.parent != transform)
            back.transform.SetParent(transform, worldPositionStays: true);
    }

    void ApplyLayout()
    {
        if (!text || !back) return;

        // Profundidades para evitar z-fighting
        var bp = back.transform.localPosition; bp.z = backZ; back.transform.localPosition = bp;
        var tp = text.transform.localPosition; tp.x = 0f; tp.y = 0f; tp.z = textZ; text.transform.localPosition = tp;

        // Medición real del texto
        text.ForceMeshUpdate(true, true);
        Vector2 size = text.GetRenderedValues(true) * inflate;

        float width = Mathf.Max(minWidth, size.x + padding.x);
        float height = Mathf.Max(minHeight, size.y + padding.y);

        back.transform.localScale = new Vector3(width, height, 1f);
    }

#if UNITY_EDITOR
    // Recalcula en tiempo real cuando cambias valores en el Inspector durante Play
    void OnValidate()
    {
        if (!Application.isPlaying) return;
        EnsureWiring();
        ApplyLayout();
    }
#endif

    public void CaptureBaseFromCurrent()
    {
        _baseLocalPos = transform.localPosition;
    }

    /// <summary>
    /// Rellena el contenido y colorea la tarjeta.
    /// </summary>
    public void Set(string machineName, float overall)
    {
        EnsureWiring();

        var state = CsvDataStore.GetState(overall);

        // Texto en dos líneas (nombre + estado)
        if (text)
        {
            text.text = $"{machineName}\n{overall:0}%  {state.Icon}";

            // Texto SIEMPRE negro (como pediste hace poco).
            // Si quieres contraste automático, cambia a la lógica de luminancia.
            text.color = Color.black;
        }

        if (back)
        {
            // Mantiene la alpha del material (transparencia) y aplica el color del estado
            var a = back.material.color.a;
            back.material.color = new Color(state.Color.r, state.Color.g, state.Color.b, a);
        }

        ApplyLayout();
    }
}
