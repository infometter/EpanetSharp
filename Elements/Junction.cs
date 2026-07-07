using System;

namespace EpanetSharp.Elements
{
    /// <summary>
    /// Representa um junction (junção) da rede.
    /// Fornece propriedades relacionadas à demanda e consumo do nó.
    /// </summary>
    public sealed class Junction : NativeNode
    {
        /// <summary>
        /// Cria uma instância de <see cref="Junction"/> baseada em um nó nativo.
        /// </summary>
        internal Junction(Native.NativeContext ctx, int index0, string id)
            : base(ctx, index0, id)
        {
        }

        /// <summary>
        /// Demanda base do nó (Base demand), em unidades do projeto.
        /// </summary>
        public double Demand
        {
            get => BaseDemand;
            set => BaseDemand = value;
        }
    }
}
