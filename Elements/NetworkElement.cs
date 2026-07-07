using System;
using EpanetSharp.Core;

namespace EpanetSharp.Elements
{
    /// <summary>
    /// Representa um elemento genérico da rede (base para nós, enlaces, patterns e curvas).
    /// Contém propriedades comuns como identificador, índice, tag e comentário, além de referências
    /// ao <see cref="Project"/> e à <see cref="Core.Network"/> associadas.
    /// </summary>
    public abstract class NetworkElement
    {
        /// <summary>
        /// Identificador textual do elemento (por exemplo, nome usado no arquivo de entrada).
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Índice numérico do elemento (por exemplo, posição na lista do EPANET).
        /// Pode ser zero ou negativo quando não inicializado.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Tag arbitrária associada ao elemento, utilizada para categorização pelo usuário.
        /// </summary>
        public string? Tag { get; set; }

        /// 
        /// <summary>
        /// Comentário ou descrição livre do elemento.
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// Referência ao projeto que contém este elemento. Pode ser nula quando o elemento
        /// não estiver associado a um projeto específico.
        /// </summary>
        public Project? Project { get; internal set; }

        /// <summary>
        /// Referência à rede que contém este elemento. Pode ser nula quando o elemento
        /// não estiver adicionado a uma rede.
        /// </summary>
        public Core.Network? Network { get; internal set; }
    }
}
