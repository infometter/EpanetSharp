namespace EpanetSharp.Units
{
    /// <summary>
    /// Unidades de vazão suportadas pelo EPANET.
    /// Corresponde à enumeração FlowUnit do WNTR.
    /// </summary>
    public enum FlowUnit
    {
        /// <summary>Litros por segundo (métrico)</summary>
        LPS,
        /// <summary>Litros por minuto (métrico)</summary>
        LPM,
        /// <summary>Megalitros por dia (métrico)</summary>
        MLD,
        /// <summary>Metros cúbicos por hora (métrico)</summary>
        CMH,
        /// <summary>Pés cúbicos por segundo (imperial)</summary>
        CFS,
        /// <summary>Galões por minuto (imperial)</summary>
        GPM,
        /// <summary>Milhões de galões por dia (imperial)</summary>
        MGD,
        /// <summary>Milhões de galões imperiais por dia (imperial)</summary>
        IMGD,
        /// <summary>Acre-pés por dia (imperial)</summary>
        AFD,
        /// <summary>Sistema Internacional — m³/s (SI)</summary>
        SI
    }

    /// <summary>
    /// Métodos de extensão para <see cref="FlowUnit"/>.
    /// </summary>
    public static class FlowUnitExtensions
    {
        /// <summary>
        /// Retorna o nome amigável da unidade de vazão.
        /// </summary>
        public static string GetFriendlyName(this FlowUnit unit)
        {
            switch (unit)
            {
                case FlowUnit.LPS:  return "Litros por Segundo";
                case FlowUnit.LPM:  return "Litros por Minuto";
                case FlowUnit.MLD:  return "Megalitros por Dia";
                case FlowUnit.CMH:  return "Metros Cúbicos por Hora";
                case FlowUnit.CFS:  return "Pés Cúbicos por Segundo";
                case FlowUnit.GPM:  return "Galões por Minuto";
                case FlowUnit.MGD:  return "Milhões de Galões por Dia";
                case FlowUnit.IMGD: return "Milhões de Galões Imperiais por Dia";
                case FlowUnit.AFD:  return "Acre-Pés por Dia";
                case FlowUnit.SI:   return "Sistema Internacional (m³/s)";
                default:            return unit.ToString();
            }
        }

        /// <summary>
        /// Retorna true se a unidade pertence ao sistema métrico.
        /// </summary>
        public static bool IsMetric(this FlowUnit unit)
        {
            return unit == FlowUnit.LPS ||
                   unit == FlowUnit.LPM ||
                   unit == FlowUnit.MLD ||
                   unit == FlowUnit.CMH ||
                   unit == FlowUnit.SI;
        }

        /// <summary>
        /// Converte a string do arquivo INP para o enum <see cref="FlowUnit"/>.
        /// </summary>
        public static bool TryParse(string value, out FlowUnit result)
        {
            if (value == null) { result = FlowUnit.SI; return false; }
            switch (value.Trim().ToUpperInvariant())
            {
                case "LPS":  result = FlowUnit.LPS;  return true;
                case "LPM":  result = FlowUnit.LPM;  return true;
                case "MLD":  result = FlowUnit.MLD;  return true;
                case "CMH":  result = FlowUnit.CMH;  return true;
                case "CFS":  result = FlowUnit.CFS;  return true;
                case "GPM":  result = FlowUnit.GPM;  return true;
                case "MGD":  result = FlowUnit.MGD;  return true;
                case "IMGD": result = FlowUnit.IMGD; return true;
                case "AFD":  result = FlowUnit.AFD;  return true;
                case "SI":   result = FlowUnit.SI;   return true;
                default:     result = FlowUnit.SI;   return false;
            }
        }
    }
}
