using System.Collections.Generic;

public static class RuleEngine
{
    const float LOW_DELIVERY = 70f;
    const float LOW_QUALITY = 80f;
    const float LOW_MTTO = 60f;

    public static List<string> GetTips(MachineKpis row, string machineName)
    {
        var tips = new List<string>();

        if (row.Delivery < LOW_DELIVERY)
            tips.Add("Delivery bajo: revisar surtido de materiales (estaciones A/B) y cuellos de cambio de modelo.");

        if (row.Quality < LOW_QUALITY)
            tips.Add("Quality bajo: inspeccionar calibre y recalibrar herramienta crítica.");

        if (row.Mtto < LOW_MTTO && row.Process < 80f)
            tips.Add("Mtto bajo + proceso inestable: agenda mantenimiento preventivo en el submódulo con más paros.");

        if (!string.IsNullOrEmpty(machineName) && machineName.ToUpper().Contains("VB"))
            tips.Add("VB: validar alineación/torque en estaciones intermedias.");

        if (tips.Count == 0) tips.Add("Sin alertas significativas. Mantener rutina estándar.");
        return tips;
    }
}
