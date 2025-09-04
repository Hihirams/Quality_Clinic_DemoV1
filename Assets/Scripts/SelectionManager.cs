using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public MachineArea current;

    void OnEnable() { MachineArea.OnAreaClicked += HandleClick; }
    void OnDisable() { MachineArea.OnAreaClicked -= HandleClick; }

    void HandleClick(MachineArea clicked)
    {
        if (current != null && current != clicked)
            current.SetSelected(false);

        if (current == clicked)
        {
            current.SetSelected(false);
            current = null;
            // También puedes limpiar la UI si quieres:
            // UIManager.Instance?.ShowNone();
            return;
        }

        current = clicked;
        current.SetSelected(true);

        // 👉 Carga datos en UI SIEMPRE que hagas click
        UIManager.Instance?.ShowMachineData(clicked.machineName);
    }
}
