using System;

namespace EpanetSharp.Elements
{
    /// <summary>
    /// Representa um reservatório da rede.
    /// Fornece propriedades relacionadas à carga hidráulica (head) do reservatório.
    /// </summary>
    public sealed class Reservoir : NativeNode
    {
        internal Reservoir(Native.NativeContext ctx, int index0, string id)
            : base(ctx, index0, id)
        {
        }

        /// <summary>
        /// Nível de carga (head) do reservatório.
        /// </summary>
        public double ReservoirHead
        {
            get => Head;
            set => Head = value;
        }
    }
}
