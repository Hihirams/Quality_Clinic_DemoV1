using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class PerMachineCsvLoader
{
    public static List<string> Load(string machineName, string kpiFilter)
    {
        List<string> result = new();
        string path = Path.Combine(Application.streamingAssetsPath, machineName + ".csv");

        if (!File.Exists(path))
        {
            result.Add($"(No se encontró {machineName}.csv en StreamingAssets)");
            return result;
        }

        var lines = File.ReadAllLines(path);
        if (lines.Length <= 1) { result.Add("(Sin datos)"); return result; }

        char d = lines[0].Contains(";") ? ';' : ',';
        var headers = lines[0].Split(d);

        int iKpi = Find(headers, "KPI");
        int iSub = Find(headers, "Subcausa");
        int iVal = Find(headers, "Valor");
        int iCom = Find(headers, "Comentario");
        int iFec = Find(headers, "Fecha");

        for (int i = 1; i < lines.Length; i++)
        {
            var ln = lines[i].Trim();
            if (string.IsNullOrEmpty(ln)) continue;

            var p = ln.Split(d);
            if (iKpi < 0 || iKpi >= p.Length) continue;

            string kpi = p[iKpi].Trim();
            if (!string.Equals(kpi, kpiFilter, System.StringComparison.OrdinalIgnoreCase))
                continue;

            string sub = Safe(p, iSub);
            string val = Safe(p, iVal);
            string com = Safe(p, iCom);
            string fec = Safe(p, iFec);

            result.Add($"{kpi} | {sub} | {val} | {com} | {fec}");
        }

        if (result.Count == 0) result.Add($"(Sin filas para KPI = {kpiFilter})");
        return result;
    }

    static int Find(string[] a, string name)
    {
        for (int i = 0; i < a.Length; i++)
            if (string.Equals(a[i].Trim(), name, System.StringComparison.OrdinalIgnoreCase)) return i;
        return -1;
    }

    static string Safe(string[] p, int idx) => (idx >= 0 && idx < p.Length) ? p[idx].Trim() : "-";
}
