using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public MachineArea current;

    void OnEnable()
    {
        MachineArea.OnAreaClicked += HandleClick;
    }

    void OnDisable()
    {
        MachineArea.OnAreaClicked -= HandleClick;
    }

    void HandleClick(MachineArea clicked)
    {
        // Apaga la selección anterior si es diferente
        if (current != null && current != clicked)
            current.SetSelected(false);

        // Si clickeas la misma, la deselecciona (toggle)
        if (current == clicked)
        {
            current.SetSelected(false);
            current = null;
            return;
        }

        // Nueva selección
        current = clicked;
        current.SetSelected(true);

        // Notifica al panel lateral
        if (UIManager.Instance != null)
            UIManager.Instance.ShowMachineData(clicked.machineName);
        else
            Debug.LogWarning("UIManager.Instance es null (¿tienes el componente UIManager en _UIManager en la escena?).");
    }
}
