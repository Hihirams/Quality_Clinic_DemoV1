using UnityEngine;

public class SelectionManager : MonoBehaviour
{
	public MachineArea current;

	void OnEnable() => MachineArea.OnAreaClicked += HandleClick;
	void OnDisable() => MachineArea.OnAreaClicked -= HandleClick;

	void HandleClick(MachineArea clicked)
	{
		if (current != null && current != clicked)
			current.SetSelected(false);

		if (current == clicked)
		{
			current.SetSelected(false);
			current = null;
			return;
		}

		current = clicked;
		current.SetSelected(true);

		// (Cuando tengas UIManager) -> UIManager.Instance.ShowMachineData(current.machineName);
	}
}
