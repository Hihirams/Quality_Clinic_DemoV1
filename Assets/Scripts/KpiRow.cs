using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KpiRow : MonoBehaviour
{
    [Header("Refs")]
    public TextMeshProUGUI nameText;
    public Slider bar;
    public TextMeshProUGUI valueText;
    public Button btnDetail;

    Action _onDetail;

    public void Set(string label, float value, Action onDetailClick)
    {
        if (nameText) nameText.text = label;
        if (bar)
        {
            bar.minValue = 0f;
            bar.maxValue = 100f;
            bar.interactable = false;
            bar.value = Mathf.Clamp(value, 0f, 100f);
        }
        if (valueText) valueText.text = $"{value:0}%";
        _onDetail = onDetailClick;

        if (btnDetail)
        {
            btnDetail.onClick.RemoveAllListeners();
            btnDetail.onClick.AddListener(() => _onDetail?.Invoke());
        }
    }
}
