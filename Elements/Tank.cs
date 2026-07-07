using System;

namespace EpanetSharp.Elements
{
    /// <summary>
    /// Representa um tanque da rede.
    /// Exponha propriedades relacionadas ao nível, volume e geometria do tanque.
    /// </summary>
    public sealed class Tank : NativeNode
    {
        internal Tank(Native.NativeContext ctx, int index0, string id)
            : base(ctx, index0, id)
        {
        }

        /// <summary>
        /// Nível atual do tanque (head).
        /// </summary>
        public double Level
        {
            get => Head;
            set => Head = value;
        }

        /// <summary>
        /// Volume atual do tanque. (Valor depende da configuração do projeto e curvas associadas.)
        /// </summary>
        public double Volume
        {
            get => 0.0; // placeholder: para cálculo real é necessário consultar curvas/geom.
        }
    }
}
