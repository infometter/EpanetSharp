using System;

namespace EpanetSharp.Elements
{
    /// <summary>
    /// Representa uma válvula genérica na rede. Subclasses especializadas devem expor
    /// propriedades específicas ao tipo de válvula.
    /// </summary>
    public abstract class Valve : Link
    {
        protected readonly Native.NativeContext _ctx;
        protected readonly int _index0;

        internal Valve(Native.NativeContext ctx, int index0, string id)
        {
            _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
            _index0 = index0;
            Id = id;
            Index = index0;
        }

        private int NativeIndex => _index0 + 1;

        /// <summary>
        /// Configuração/definição da válvula (quando suportado pela API nativa).
        /// </summary>
        public virtual double Setting
        {
            get => _ctx.GetLinkValue(NativeIndex, Native.NativeConstants.EN_LINK_SETTING);
            set => _ctx.SetLinkValue(NativeIndex, Native.NativeConstants.EN_LINK_SETTING, value);
        }
    }
}
