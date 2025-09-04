using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

[Serializable]
public class MachineKpis
{
    public string MachineName;
    public float OverallResults;
    public float Delivery, Quality, Parts, Process, Training, Mtto;
}

public readonly struct StateBadge
{
    public readonly Color Color;
    public readonly string Icon; // texto corto (sin emoji)
    public StateBadge(Color color, string icon) { Color = color; Icon = icon; }
}

public class CsvDataStore : MonoBehaviour
{
    [Header("Archivo en StreamingAssets")]
    public string fileName = "summary.csv";

    // Búsqueda exacta y por alias normalizado
    public Dictionary<string, MachineKpis> byName = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, MachineKpis> byAlias = new(StringComparer.OrdinalIgnoreCase);

    public static CsvDataStore Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        var path = Path.Combine(Application.streamingAssetsPath, fileName);
        if (!File.Exists(path))
        {
            Debug.LogError("❌ No existe CSV: " + path);
            return;
        }

        try
        {
            var csv = File.ReadAllText(path);
            Parse(csv);
            Debug.Log($"✅ CSV cargado: {byName.Count} máquinas");
        }
        catch (Exception e)
        {
            Debug.LogError("❌ Error leyendo CSV: " + e.Message);
        }
    }

    void Parse(string csv)
    {
        byName.Clear();
        byAlias.Clear();

        var lines = csv.Replace("\r", "").Split('\n');
        if (lines.Length < 2) return;

        // Detecta separador ; o ,
        char d = lines[0].Contains(';') ? ';' : ',';

        for (int i = 1; i < lines.Length; i++)
        {
            var ln = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(ln)) continue;

            var p = ln.Split(d);
            if (p.Length < 8) continue;

            var row = new MachineKpis
            {
                MachineName = p[0].Trim(),
                OverallResults = F(p[1]),
                Delivery = F(p[2]),
                Quality = F(p[3]),
                Parts = F(p[4]),
                Process = F(p[5]),
                Training = F(p[6]),
                Mtto = F(p[7]),
            };

            byName[row.MachineName] = row;
            byAlias[Normalize(row.MachineName)] = row; // alias sin espacios/guiones etc.
        }

        static float F(string s)
        {
            s = s.Replace("%", "").Trim();
            if (float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var f)) return f;
            if (float.TryParse(s, out f)) return f;
            return 0f;
        }
    }

    // Normaliza: quita espacios/guiones/_ y pone minúsculas
    public static string Normalize(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        s = s.Trim().ToLowerInvariant();
        s = s.Replace(" ", "").Replace("-", "").Replace("_", "");
        return s;
    }

    // Búsqueda robusta para UIManager/SelectionManager
    public bool TryGetRow(string machineName, out MachineKpis row)
    {
        row = default;
        if (string.IsNullOrWhiteSpace(machineName)) return false;

        // 1) intento exacto
        if (byName.TryGetValue(machineName, out row)) return true;

        // 2) intento por alias
        var key = Normalize(machineName);
        return byAlias.TryGetValue(key, out row);
    }

    // Umbrales (ajústalos)
    public static StateBadge GetState(float overall)
    {
        if (overall >= 85f) return new StateBadge(new Color(0.20f, 0.80f, 0.35f), "OK");   // verde
        if (overall >= 70f) return new StateBadge(new Color(0.60f, 1.00f, 0.60f), "UP");   // verde claro
        if (overall >= 50f) return new StateBadge(new Color(1.00f, 0.92f, 0.25f), "WARN"); // amarillo
        return new StateBadge(new Color(0.90f, 0.20f, 0.20f), "RISK");                      // rojo
    }
}
