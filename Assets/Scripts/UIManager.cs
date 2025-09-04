using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Left Panel")]
    public RectTransform panelLeft;
    public TextMeshProUGUI title;
    public RectTransform kpisGroup;      // <- Content del Scroll
    public KpiRow kpiRowPrefab;

    [Header("Tabs")]
    public Button tabKPIs;
    public Button tabPred;
    public RectTransform predContainer;  // contenedor para la lista de predicciones

    [Header("Detail Panel")]
    public GameObject detailPanel;
    public TextMeshProUGUI detailHeader;
    public RectTransform detailContent;  // content del Scroll detalle
    public TextMeshProUGUI listLinePrefab;
    public Button detailCloseBtn;

    MachineArea _currentArea;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        MachineArea.OnAreaClicked += HandleAreaClicked;
    }
    void OnDisable()
    {
        MachineArea.OnAreaClicked -= HandleAreaClicked;
    }

    void Start()
    {
        // Wiring de pestañas
        if (tabKPIs) tabKPIs.onClick.AddListener(ShowKPIs);
        if (tabPred) tabPred.onClick.AddListener(ShowPred);

        if (detailCloseBtn) detailCloseBtn.onClick.AddListener(() =>
        {
            if (detailPanel) detailPanel.SetActive(false);
        });

        // Carga de prueba para ver el panel al inicio (si quieres quitarlo, comenta estas 2 líneas)
        if (CsvDataStore.Instance != null && CsvDataStore.Instance.byName.Count > 0)
            ShowMachineData("VB-L1"); // pon un nombre existente
    }

    // ==== Callbacks ====
    void HandleAreaClicked(MachineArea area)
    {
        _currentArea = area;
        ShowMachineData(area.machineName);
    }

    // ==== Pestañas ====
    public void ShowKPIs()
    {
        if (!panelLeft) return;
        // Mostrar grupo KPIs y ocultar predicciones
        if (kpisGroup) kpisGroup.gameObject.SetActive(true);
        if (predContainer) predContainer.gameObject.SetActive(false);
    }

    public void ShowPred()
    {
        if (!panelLeft) return;
        // Mostrar predicciones y ocultar KPIs
        if (kpisGroup) kpisGroup.gameObject.SetActive(false);
        if (predContainer) predContainer.gameObject.SetActive(true);
    }

    // ==== Render principal ====
    public void ShowMachineData(string machineName)
    {
        if (!CsvDataStore.Instance) return;

        if (!CsvDataStore.Instance.byName.TryGetValue(machineName, out var row))
            return;

        if (title) title.text = $"Machine: {machineName}";
        ShowKPIs(); // por defecto caemos en la pestaña KPIs

        // Limpiar KPIs anteriores
        foreach (Transform t in kpisGroup) Destroy(t.gameObject);

        // Agregar KPIs (solo los que sabemos que existen ahora mismo)
        AddKpi("Delivery", row.Delivery, () => OpenDetail(machineName, "Delivery"));
        AddKpi("Quality", row.Quality, () => OpenDetail(machineName, "Quality"));
        // TODO: cuando confirmemos nombres exactos, descomentar:
        // AddKpi("Parts", row.Parts, () => OpenDetail(machineName, "Parts"));
        // AddKpi("Process", row.ProcessManufacturing, () => OpenDetail(machineName, "ProcessManufacturing"));
        // AddKpi("Training - DNA", row.TrainingDNA, () => OpenDetail(machineName, "TrainingDNA"));
        AddKpi("Mtto", row.Mtto, () => OpenDetail(machineName, "Mtto"));

        // Predicciones
        BuildPredictions(row);
    }

    void AddKpi(string label, float value, Action onDetail)
    {
        if (!kpiRowPrefab || !kpisGroup) return;
        var rowGO = Instantiate(kpiRowPrefab, kpisGroup);
        rowGO.Set(label, value, onDetail);
        Debug.Log("[UI] KPI agregado: " + label);
    }

    // ==== Detalle ====
    void OpenDetail(string machineName, string kpiName)
    {
        if (!detailPanel || !detailContent || !listLinePrefab) return;

        detailPanel.SetActive(true);
        if (detailHeader) detailHeader.text = $"{machineName} / {kpiName}";

        // limpiar
        foreach (Transform t in detailContent) Destroy(t.gameObject);

        // aquí colocaríamos las líneas leídas de la hoja por máquina
        var placeholders = new List<string> {
            "Subcausa A — 40%",
            "Subcausa B — 35%",
            "Comentario: revisar estándar",
            "Fecha: 2025-02-04"
        };
        foreach (var s in placeholders)
        {
            var line = Instantiate(listLinePrefab, detailContent);
            line.text = "• " + s;
        }
    }

    // ==== Predicciones (reglas) ====
    void BuildPredictions(MachineKpis row)
    {
        if (!predContainer || !listLinePrefab) return;
        foreach (Transform t in predContainer) Destroy(t.gameObject);

        var tips = RuleEngine.GetTips(row, _currentArea);
        foreach (var tip in tips)
        {
            var t = Instantiate(listLinePrefab, predContainer);
            t.text = "• " + tip;
        }
    }
}
