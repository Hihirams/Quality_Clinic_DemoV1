using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panel izquierdo")]
    public RectTransform panelLeft;
    public TMP_Text title;
    public RectTransform kpisGroup;
    public KpiRow kpiRowPrefab;

    [Header("Tabs")]
    public Button tabKPIs;
    public Button tabPred;
    public RectTransform predContainer; // contenedor vertical para predicciones (Text TMP por línea)

    [Header("Panel de detalle")]
    public GameObject detailPanel;
    public TMP_Text detailHeader;
    public RectTransform detailContent; // Vertical Layout
    public TMP_Text listLinePrefab;     // pequeño Text (TMP) para listas
    public Button detailCloseBtn;

    string currentMachine = null;
    readonly List<GameObject> spawned = new();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (tabKPIs) tabKPIs.onClick.AddListener(ShowKPIs);
        if (tabPred) tabPred.onClick.AddListener(ShowPredictions);
        if (detailCloseBtn) detailCloseBtn.onClick.AddListener(CloseDetail); // ← usa el método público

        if (CsvDataStore.Instance != null && CsvDataStore.Instance.byName.Count > 0)
        {
            ShowMachineData(CsvDataStore.Instance.byName.Keys.First());
        }
    }


    public void ShowNone()
    {
        currentMachine = null;
        title.text = "Machine: —";
        ClearGroup(kpisGroup);
        ClearGroup(predContainer);
        if (detailPanel) detailPanel.SetActive(false);
    }

    public void ShowMachineData(string machineName)
    {
        if (CsvDataStore.Instance == null) return;

        if (!CsvDataStore.Instance.TryGetRow(machineName, out var row))
        {
            Debug.LogWarning($"⚠️ UI: no hay datos para '{machineName}' (revisa nombre o CSV)");
            return;
        }

        currentMachine = row.MachineName; // guarda el nombre como está en CSV
        title.text = $"Machine: {currentMachine}  |  Overall {row.OverallResults:0}%";

        BuildKPIs(row);
        BuildPredictions(row);
        ShowKPIs();  // asegura que la pestaña visible sea KPIs
        Debug.Log($"[UI] ShowMachineData: {currentMachine}");
    }


    // -------- KPIs ----------
    void BuildKPIs(MachineKpis row)
    {
        ClearGroup(kpisGroup);

        SpawnKpi("Delivery", row.Delivery);
        SpawnKpi("Quality", row.Quality);
        SpawnKpi("Parts", row.Parts);
        SpawnKpi("Process", row.Process);
        SpawnKpi("Training", row.Training);
        SpawnKpi("Mtto", row.Mtto);

        void SpawnKpi(string label, float value)
        {
            var item = Instantiate(kpiRowPrefab, kpisGroup);
            item.Set(label, value, () => ShowDetail(label, value));
        }

        // 🔧 muy importante para que aparezcan en el Scroll
        LayoutRebuilder.ForceRebuildLayoutImmediate(kpisGroup);
    }


    // -------- Predicciones ----------
    void BuildPredictions(MachineKpis row)
    {
        ClearGroup(predContainer);
        var tips = RuleEngine.GetTips(row, currentMachine);
        foreach (var tip in tips)
        {
            var line = Instantiate(listLinePrefab, predContainer);
            line.text = "• " + tip;
            spawned.Add(line.gameObject);
        }
    }

    // -------- Detalle ----------
    void ShowDetail(string kpiName, float value)
    {
        if (!detailPanel) return;
        detailPanel.SetActive(true);
        detailHeader.text = $"{currentMachine} — {kpiName}";

        ClearGroup(detailContent);

        // De momento: ejemplos simples
        AddDetail($"Valor actual: {value:0}%");
        AddDetail("Subcausas/histórico: por integrar desde hojas por máquina.");
        AddDetail("Acciones sugeridas: ver pestaña Predicciones.");

        void AddDetail(string s)
        {
            var t = Instantiate(listLinePrefab, detailContent);
            t.text = "– " + s;
        }
    }

    // -------- Tabs ----------
    public void ShowKPIs()
    {
        if (kpisGroup) kpisGroup.gameObject.SetActive(true);
        if (predContainer) predContainer.gameObject.SetActive(false);
    }

    public void ShowPredictions()
    {
        if (kpisGroup) kpisGroup.gameObject.SetActive(false);
        if (predContainer) predContainer.gameObject.SetActive(true);
    }

    // -------- Helpers ----------
    void ClearGroup(RectTransform group)
    {
        if (!group) return;
        for (int i = group.childCount - 1; i >= 0; i--)
            Destroy(group.GetChild(i).gameObject);
    }

    public void CloseDetail()
    {
        if (detailPanel) detailPanel.SetActive(false);
    }

}
