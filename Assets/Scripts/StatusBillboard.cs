using TMPro;
using UnityEngine;

[RequireComponent(typeof(Billboard))]
public class StatusBillboard : MonoBehaviour
{
    public TextMeshPro text;
    public MeshRenderer back;

    [Header("Auto-layout del fondo")]
    public Vector2 padding = new Vector2(1.0f, 0.65f);
    public float backZ = 0f;
    public float textZ = -0.01f;
    public float minWidth = 3.0f;
    public float minHeight = 1.4f;

    [Header("Ajuste fino")]
    public float inflate = 1.15f;

    [Header("Altura adicional (por etiqueta)")]
    public float extraHeight = 1.0f;

    [Header("Offset manual")]
    public float yOffset = 0f;   // ← mueve toda la etiqueta hacia arriba/abajo

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
        _baseLocalPos = transform.localPosition + new Vector3(0f, yOffset, 0f); // ← usa yOffset
        EnsureWiring();
        ApplyLayout();
    }

    void LateUpdate()
    {
        float y = Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
        transform.localPosition = _baseLocalPos + new Vector3(0f, y, 0f);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (Application.isPlaying) ApplyLayout();
    }
#endif

    [ContextMenu("Capture Base From Current")]
    public void CaptureBaseFromCurrent()
    {
        _baseLocalPos = transform.localPosition; // fija la base actual
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
            text.enableWordWrapping = false;
            text.overflowMode = TextOverflowModes.Overflow;
        }
        if (back && back.transform.parent != transform)
            back.transform.SetParent(transform, worldPositionStays: true);
    }

    void ApplyLayout()
    {
        if (!text || !back) return;

        var bp = back.transform.localPosition; bp.z = backZ; back.transform.localPosition = bp;
        var tp = text.transform.localPosition; tp.x = 0f; tp.y = 0f; tp.z = textZ; text.transform.localPosition = tp;

        text.ForceMeshUpdate(true, true);
        Vector2 size = text.GetRenderedValues(true) * inflate;

        float width = Mathf.Max(minWidth, size.x + padding.x);
        float height = Mathf.Max(minHeight, size.y + padding.y);

        back.transform.localScale = new Vector3(width, height, 1f);
    }

    public void Set(string machineName, float overall)
    {
        EnsureWiring();

        var state = CsvDataStore.GetState(overall);

        if (text)
        {
            text.text = $"{machineName}\n{overall:0}%  {state.Icon}";
            float L = 0.2126f * state.Color.r + 0.7152f * state.Color.g + 0.0722f * state.Color.b;
            text.color = (L > 0.55f) ? new Color(0.12f, 0.12f, 0.12f, 1f) : Color.white;
        }

        if (back)
        {
            var c = back.material.color;
            back.material.color = new Color(state.Color.r, state.Color.g, state.Color.b, c.a);
        }

        ApplyLayout();
    }
}
