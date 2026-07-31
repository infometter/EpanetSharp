namespace EpanetSharp.Units
{
    /// <summary>
    /// Fórmulas de perda de carga suportadas pelo EPANET.
    /// Corresponde à enumeração HeadlossFormula do WNTR.
    /// </summary>
    public enum HeadlossFormula
    {
        /// <summary>Hazen-Williams (H-W) — padrão do EPANET</summary>
        HazenWilliams,
        /// <summary>Darcy-Weisbach (D-W)</summary>
        DarcyWeisbach,
        /// <summary>Chezy-Manning (C-M)</summary>
        ChezyManning
    }

    /// <summary>
    /// Métodos de extensão para <see cref="HeadlossFormula"/>.
    /// </summary>
    public static class HeadlossFormulaExtensions
    {
        /// <summary>
        /// Retorna o código de string usado no arquivo INP (H-W, D-W, C-M).
        /// </summary>
        public static string ToInpCode(this HeadlossFormula formula)
        {
            switch (formula)
            {
                case HeadlossFormula.HazenWilliams: return "H-W";
                case HeadlossFormula.DarcyWeisbach:  return "D-W";
                case HeadlossFormula.ChezyManning:   return "C-M";
                default:                             return "H-W";
            }
        }

        /// <summary>
        /// Retorna o nome amigável da fórmula.
        /// </summary>
        public static string GetFriendlyName(this HeadlossFormula formula)
        {
            switch (formula)
            {
                case HeadlossFormula.HazenWilliams: return "Hazen-Williams";
                case HeadlossFormula.DarcyWeisbach:  return "Darcy-Weisbach";
                case HeadlossFormula.ChezyManning:   return "Chezy-Manning";
                default:                             return formula.ToString();
            }
        }

        /// <summary>
        /// Converte código INP para o enum.
        /// </summary>
        public static bool TryParse(string value, out HeadlossFormula result)
        {
            if (value == null) { result = HeadlossFormula.HazenWilliams; return false; }
            switch (value.Trim().ToUpperInvariant())
            {
                case "H-W": case "HW": case "HAZEN-WILLIAMS":
                    result = HeadlossFormula.HazenWilliams; return true;
                case "D-W": case "DW": case "DARCY-WEISBACH":
                    result = HeadlossFormula.DarcyWeisbach;  return true;
                case "C-M": case "CM": case "CHEZY-MANNING":
                    result = HeadlossFormula.ChezyManning;   return true;
                default:
                    result = HeadlossFormula.HazenWilliams; return false;
            }
        }
    }
}
