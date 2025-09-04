using System.Collections.Generic;

public static class RuleEngine
{
    // Reglas simples (fase 1: sin ML)
    public static List<string> GetTips(MachineKpis r, string machineName)
    {
        var tips = new List<string>();

        // Delivery bajo
        if (r.Delivery < 70f)
            tips.Add("Delivery bajo: revisar surtido de materiales en estaciones A/B.");

        // Quality baja
        if (r.Quality < 80f)
            tips.Add("Quality bajo: inspeccionar calibre, recalibrar herramienta T.");

        // Proceso
        if (r.Process < 70f)
            tips.Add("Proceso con cuellos de botella: verificar parámetros y estandarización.");

        // Mantenimiento
        if (r.Mtto < 75f)
            tips.Add("Mtto bajo: programar mantenimiento preventivo al submódulo crítico.");

        // Training
        if (r.Training < 75f)
            tips.Add("Training bajo: plan de capacitación para operadores nuevos.");

        if (tips.Count == 0)
            tips.Add($"Sin alertas críticas para {machineName}. Mantener estándares.");

        return tips;
    }
}
