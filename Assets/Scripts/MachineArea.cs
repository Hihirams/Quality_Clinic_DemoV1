using System;
using System.Linq;
using UnityEngine;

public class MachineArea : MonoBehaviour
{
    [Header("Identidad (igual que en CSV)")]
    public string machineName = "VB-L1";      // Debe coincidir con 'MachineName' del CSV

    [Header("Renderers a tintar (si vacío, usa los hijos)")]
    public Renderer[] toTint;

    // Evento de clic (lo escucha SelectionManager)
    public static Action<MachineArea> OnAreaClicked;

    // Copia de colores originales (por si luego la necesitas)
    Color[] _originalColors;

    // Estado (base vs. seleccionado)
    Color _stateColor = Color.gray;
    Color _baseTint, _selectedTint;

    void Awake()
    {
        if (toTint == null || toTint.Length == 0)
            toTint = GetComponentsInChildren<Renderer>();

        _originalColors = toTint.Select(r => r.material.color).ToArray();
    }

    // Define el color de estado (según Overall) y pinta el tono base
    public void ApplyStateColor(Color c)
    {
        _stateColor = c;
        _baseTint = Color.Lerp(Color.gray, _stateColor, 0.20f); // base suave
        _selectedTint = Color.Lerp(Color.gray, _stateColor, 0.65f); // seleccionado fuerte
        TintTo(_baseTint);
    }

    void TintTo(Color c)
    {
        for (int i = 0; i < toTint.Length; i++)
            toTint[i].material.color = c;
    }

    public void SetSelected(bool on) => TintTo(on ? _selectedTint : _baseTint);

    // Compat (si otros scripts llaman ClearTint/Tint)
    public void ClearTint() => SetSelected(false);
    public void Tint(bool on) => SetSelected(on);

    void OnMouseDown()
    {
        if (CursorToggle.IsOverUI()) return;  // ← no selecciones si clic fue sobre UI
        Debug.Log($"👉 Click en máquina: {machineName}");
        OnAreaClicked?.Invoke(this);
    }

}
