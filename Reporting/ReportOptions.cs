using System;

namespace EpanetSharp.Reporting
{
    /// <summary>
    /// Opções para geração de relatórios.
    /// </summary>
    public sealed class ReportOptions
    {
        /// <summary>
        /// Indica se o sumário deve incluir energia (bom para bombas).
        /// </summary>
        public bool IncludeEnergy { get; set; } = true;

        /// <summary>
        /// Indica se o relatório deve incluir dados por nó.
        /// </summary>
        public bool IncludeNodes { get; set; } = true;

        /// <summary>
        /// Indica se o relatório deve incluir dados por enlace.
        /// </summary>
        public bool IncludeLinks { get; set; } = true;

        /// <summary>
        /// Formato textual (plain, csv, json). Atualmente apenas plain e csv são suportados.
        /// </summary>
        public string Format { get; set; } = "plain";
    }
}
