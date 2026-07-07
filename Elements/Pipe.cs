using System;

namespace EpanetSharp.Elements
{
    /// <summary>
    /// Representa uma tubulação (pipe) da rede.
    /// Fornece propriedades físicas e de estado que refletem diretamente os valores na API nativa.
    /// </summary>
    public sealed class Pipe : Link
    {
        private readonly Native.NativeContext _ctx;
        private readonly int _index0; // zero-based

        /// <summary>
        /// Cria uma instância de <see cref="Pipe"/> ligada ao contexto nativo.
        /// </summary>
        internal Pipe(Native.NativeContext ctx, int index0, string id)
        {
            _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
            _index0 = index0;
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Index = index0;
        }

        private int NativeIndex => _index0 + 1;

        /// <summary>
        /// Comprimento da tubulação.
        /// </summary>
        public double Length
        {
            get => _ctx.GetLinkValue(NativeIndex, Native.NativeConstants.EN_LINK_LENGTH);
            set => _ctx.SetLinkValue(NativeIndex, Native.NativeConstants.EN_LINK_LENGTH, value);
        }

        /// <summary>
        /// Diâmetro interno da tubulação.
        /// </summary>
        public double Diameter
        {
            get => _ctx.GetLinkValue(NativeIndex, Native.NativeConstants.EN_LINK_DIAMETER);
            set => _ctx.SetLinkValue(NativeIndex, Native.NativeConstants.EN_LINK_DIAMETER, value);
        }

        /// <summary>
        /// Rugosidade da tubulação (coeficiente de Manning/Colebrook dependendo do modelo).
        /// </summary>
        public double Roughness
        {
            get => _ctx.GetLinkValue(NativeIndex, Native.NativeConstants.EN_LINK_ROUGHNESS);
            set => _ctx.SetLinkValue(NativeIndex, Native.NativeConstants.EN_LINK_ROUGHNESS, value);
        }

        /// <summary>
        /// Perda menor associada ao componente.
        /// </summary>
        public double MinorLoss
        {
            get => _ctx.GetLinkValue(NativeIndex, Native.NativeConstants.EN_LINK_MINORLOSS);
            set => _ctx.SetLinkValue(NativeIndex, Native.NativeConstants.EN_LINK_MINORLOSS, value);
        }

        /// <summary>
        /// Status operacional da tubulação (aberta/fechada etc.).
        /// </summary>
        public double Status
        {
            get => _ctx.GetLinkValue(NativeIndex, Native.NativeConstants.EN_LINK_STATUS);
            set => _ctx.SetLinkValue(NativeIndex, Native.NativeConstants.EN_LINK_STATUS, value);
        }

        /// <summary>
        /// Vazão atual na tubulação.
        /// </summary>
        public double Flow
        {
            get => _ctx.GetLinkValue(NativeIndex, Native.NativeConstants.EN_LINK_FLOW);
            set => _ctx.SetLinkValue(NativeIndex, Native.NativeConstants.EN_LINK_FLOW, value);
        }

        /// <summary>
        /// Velocidade do fluido na tubulação.
        /// </summary>
        public double Velocity
        {
            get => _ctx.GetLinkValue(NativeIndex, Native.NativeConstants.EN_LINK_VELOCITY);
        }

        /// <summary>
        /// Perda de carga ao longo da tubulação.
        /// </summary>
        public double HeadLoss
        {
            get => _ctx.GetLinkValue(NativeIndex, Native.NativeConstants.EN_LINK_HEADLOSS);
        }
    }
}
