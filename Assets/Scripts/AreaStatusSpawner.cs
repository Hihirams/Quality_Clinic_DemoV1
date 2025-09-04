using UnityEngine;

public class AreaStatusSpawner : MonoBehaviour
{
    [SerializeField] float upFactor = 0.30f;   // 0.25–0.35 recomendado
    public StatusBillboard billboardPrefab;

    void Start()
    {
        if (CsvDataStore.Instance == null) { Debug.LogError("❌ Falta CsvDataStore en escena"); return; }
        if (!billboardPrefab) { Debug.LogError("❌ Asigna billboardPrefab"); return; }

        var areas = FindObjectsOfType<MachineArea>(includeInactive: false);

        foreach (var a in areas)
        {
            if (!CsvDataStore.Instance.byName.TryGetValue(a.machineName, out var row))
            {
                Debug.LogWarning($"⚠️ No hay fila en CSV para '{a.machineName}'");
                continue;
            }

            var state = CsvDataStore.GetState(row.OverallResults);
            a.ApplyStateColor(state.Color);

            Bounds b = GetWorldBounds(a);
            float up = Mathf.Max(0.4f, b.size.y * upFactor);
            Vector3 worldPos = new Vector3(b.center.x, b.max.y + up, b.center.z);

            var bb = Instantiate(billboardPrefab, worldPos, Quaternion.identity, a.transform);

            // levantar por-prefab
            bb.transform.position += Vector3.up * bb.extraHeight;

            bb.Set(a.machineName, row.OverallResults);
        }
    }

    static Bounds GetWorldBounds(MachineArea a)
    {
        var rends = a.GetComponentsInChildren<Renderer>();
        if (rends.Length > 0)
        {
            Bounds b = rends[0].bounds;
            for (int i = 1; i < rends.Length; i++) b.Encapsulate(rends[i].bounds);
            return b;
        }
        var cols = a.GetComponentsInChildren<Collider>();
        if (cols.Length > 0)
        {
            Bounds b = cols[0].bounds;
            for (int i = 1; i < cols.Length; i++) b.Encapsulate(cols[i].bounds);
            return b;
        }
        return new Bounds(a.transform.position, Vector3.one);
    }
}
