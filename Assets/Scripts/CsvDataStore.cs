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
	public readonly string Icon; // Texto corto (sin emoji para evitar glifos)
	public StateBadge(Color color, string icon) { Color = color; Icon = icon; }
}

public class CsvDataStore : MonoBehaviour
{
	public string fileName = "summary.csv";
	public Dictionary<string, MachineKpis> byName = new(StringComparer.OrdinalIgnoreCase);

	public static CsvDataStore Instance;

	void Awake()
	{
		if (Instance != null) { Destroy(gameObject); return; }
		Instance = this;
		DontDestroyOnLoad(gameObject);

		var path = Path.Combine(Application.streamingAssetsPath, fileName);
		if (!File.Exists(path)) { Debug.LogError("❌ No existe: " + path); return; }

		string csv = File.ReadAllText(path);
		Parse(csv);
		Debug.Log($"✅ CSV cargado: {byName.Count} máquinas");
	}

	void Parse(string csv)
	{
		byName.Clear();
		var lines = csv.Replace("\r", "").Split('\n');
		if (lines.Length < 2) return;

		// Detectar separador
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
				OverallResults = SafeF(p[1]),
				Delivery = SafeF(p[2]),
				Quality = SafeF(p[3]),
				Parts = SafeF(p[4]),
				Process = SafeF(p[5]),
				Training = SafeF(p[6]),
				Mtto = SafeF(p[7])
			};
			byName[row.MachineName] = row;
		}
	}

	static float SafeF(string s)
	{
		s = s.Replace("%", "").Trim();
		// Invariant para evitar problemas de coma/punto
		if (float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var f))
			return f;
		// Intento alterno con cultura local
		if (float.TryParse(s, out f)) return f;
		return 0f;
	}

	// Umbrales iniciales (pueden moverse a un ScriptableObject después)
	public static StateBadge GetState(float overall)
	{
		if (overall >= 90f) return new StateBadge(Color.green, "OK");
		if (overall >= 80f) return new StateBadge(new Color(0.6f, 1f, 0.6f), "HEALTHY");
		if (overall >= 70f) return new StateBadge(Color.yellow, "WARN");
		return new StateBadge(Color.red, "RISK");
	}
}
