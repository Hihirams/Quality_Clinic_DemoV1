using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KpiRow : MonoBehaviour
{
    public TMP_Text txtName;
    public Slider bar;
    public TMP_Text txtValue;
    public Button btnDetail;

    public void Set(string label, float value, Action onDetail)
    {
        if (txtName) txtName.text = label;
        if (bar)
        {
            bar.minValue = 0; bar.maxValue = 100; bar.interactable = false;
            bar.value = Mathf.Clamp(value, 0f, 100f);
        }
        if (txtValue) txtValue.text = $"{value:0}%";

        if (btnDetail)
        {
            btnDetail.onClick.RemoveAllListeners();
            if (onDetail != null) btnDetail.onClick.AddListener(() => onDetail());
        }
    }
}
