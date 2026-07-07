using System;

namespace EpanetSharp.Elements
{
    /// <summary>
    /// Representa um controle da rede. O controle é exposto como uma string textual
    /// (definição conforme arquivo INP) e possui um identificador/índice quando gerenciado
    /// via API nativa.
    /// </summary>
    public sealed class Control : NetworkElement
    {
        internal Control(string id, int index0)
        {
            Id = id;
            Index = index0;
        }

        /// <summary>
        /// Texto de definição do controle (por exemplo, conforme linha do arquivo INP).
        /// </summary>
        public string? Definition { get; internal set; }
    }
}
