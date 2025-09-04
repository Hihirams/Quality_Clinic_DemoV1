using UnityEngine;

public class AreaStatusSpawner : MonoBehaviour
{
    public StatusBillboard billboardPrefab;

    [SerializeField] float upFactor = 0.5f; // altura relativa sobre el techo

    void Start()
    {
        if (CsvDataStore.Instance == null) { Debug.LogError("❌ Falta CsvDataStore"); return; }
        if (!billboardPrefab) { Debug.LogError("❌ Asigna billboardPrefab en el Inspector"); return; }

        var areas = FindObjectsOfType<MachineArea>(includeInactive: false);
        foreach (var a in areas)
        {
            // 1) Buscar la fila del CSV (acepta alias normalizados)
            if (!CsvDataStore.Instance.TryGetRow(a.machineName, out var row))
            {
                Debug.LogWarning($"⚠️ Spawner: no hay fila para '{a.machineName}'");
                continue; // <- muy importante
            }

            // 2) Aplicar color base/selección del área según Overall
            var state = CsvDataStore.GetState(row.OverallResults);
            a.ApplyStateColor(state.Color);

            // 3) Calcular posición: centro del bounds + altura
            Bounds b = GetWorldBounds(a);
            float up = Mathf.Max(0.4f, b.size.y * upFactor);
            Vector3 worldPos = new Vector3(b.center.x, b.max.y + up, b.center.z);

            // 4) Instanciar el billboard como hijo del área y setear datos
            var bb = Instantiate(billboardPrefab, worldPos, Quaternion.identity, a.transform);
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
