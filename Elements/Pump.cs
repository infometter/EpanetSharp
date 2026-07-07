using System;

namespace EpanetSharp.Elements
{
    /// <summary>
    /// Representa uma bomba na rede.
    /// Exponha propriedades relacionadas à potência, velocidade, energia, padrão e curva.
    /// </summary>
    public sealed class Pump : Link
    {
        private readonly Native.NativeContext _ctx;
        private readonly int _index0;

        internal Pump(Native.NativeContext ctx, int index0, string id)
        {
            _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
            _index0 = index0;
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Index = index0;
        }

        private int NativeIndex => _index0 + 1;

        /// <summary>
        /// Potência da bomba (quando disponível na API nativa).
        /// </summary>
        public double Power
        {
            get => TryGetLinkParam(Native.NativeConstants.EN_LINK_POWER);
            set => TrySetLinkParam(Native.NativeConstants.EN_LINK_POWER, value);
        }

        /// <summary>
        /// Velocidade operacional da bomba.
        /// </summary>
        public double Speed
        {
            get => TryGetLinkParam(Native.NativeConstants.EN_LINK_SPEED);
            set => TrySetLinkParam(Native.NativeConstants.EN_LINK_SPEED, value);
        }

        /// <summary>
        /// Energia consumida pela bomba (relatório), quando suportado.
        /// </summary>
        public double Energy
        {
            get => TryGetLinkParam(Native.NativeConstants.EN_LINK_ENERGY);
        }

        /// <summary>
        /// Identificador do padrão de operação associado à bomba.
        /// </summary>
        public string? Pattern
        {
            get
            {
                try
                {
                    // pattern may be represented as numeric index in native API
                    double p = _ctx.GetLinkValue(NativeIndex, Native.NativeConstants.EN_LINK_PATTERN);
                    return p.ToString();
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                if (value is null) throw new ArgumentNullException(nameof(value));
                if (double.TryParse(value, out var v))
                {
                    _ctx.SetLinkValue(NativeIndex, Native.NativeConstants.EN_LINK_PATTERN, v);
                }
                else
                {
                    throw new ArgumentException("Pattern must be numeric pattern index as string.", nameof(value));
                }
            }
        }

        /// <summary>
        /// Nome/índice da curva associada à bomba (quando disponível).
        /// </summary>
        public string? Curve
        {
            get
            {
                try
                {
                    double c = _ctx.GetLinkValue(NativeIndex, Native.NativeConstants.EN_LINK_CURVE);
                    return c.ToString();
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                if (value is null) throw new ArgumentNullException(nameof(value));
                if (double.TryParse(value, out var v))
                {
                    _ctx.SetLinkValue(NativeIndex, Native.NativeConstants.EN_LINK_CURVE, v);
                }
                else
                {
                    throw new ArgumentException("Curve must be numeric curve index as string.", nameof(value));
                }
            }
        }

        private double TryGetLinkParam(int code)
        {
            try
            {
                return _ctx.GetLinkValue(NativeIndex, code);
            }
            catch
            {
                return double.NaN;
            }
        }

        private void TrySetLinkParam(int code, double value)
        {
            try
            {
                _ctx.SetLinkValue(NativeIndex, code, value);
            }
            catch
            {
                // ignore if not supported
            }
        }
    }
}
