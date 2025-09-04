using System.Linq;
using UnityEngine;

public class MachineArea : MonoBehaviour
{
    [Header("Identidad (igual en CSV)")]
    public string machineName = "VB-L1";

    [Header("Renderers a tintar (si vacío, usa hijos)")]
    public Renderer[] toTint;

    [Header("Tono de selección (se calcula desde el estado)")]
    public Color _stateColor = Color.gray;
    Color _baseTint, _selectedTint;

    Color[] _originalColors;

    public static System.Action<MachineArea> OnAreaClicked;

    void Awake()
    {
        if (toTint == null || toTint.Length == 0)
            toTint = GetComponentsInChildren<Renderer>();

        _originalColors = toTint.Select(r => r.material.color).ToArray();
        TintTo(Color.gray);
    }

    public void ApplyStateColor(Color c)
    {
        _stateColor = c;
        _baseTint = Color.Lerp(Color.gray, _stateColor, 0.20f);
        _selectedTint = Color.Lerp(Color.gray, _stateColor, 0.65f);
        TintTo(_baseTint);
    }

    void TintTo(Color c)
    {
        for (int i = 0; i < toTint.Length; i++)
            toTint[i].material.color = c;
    }

    public void SetSelected(bool on) => TintTo(on ? _selectedTint : _baseTint);

    void OnMouseDown()
    {
        Debug.Log($"👉 Click en máquina: {machineName}");
        OnAreaClicked?.Invoke(this);
    }

    public void ResetTint()
    {
        for (int i = 0; i < toTint.Length; i++)
            toTint[i].material.color = _originalColors[i];
    }
}
