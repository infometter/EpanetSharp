using System;

namespace EpanetSharp.Elements
{
    /// <summary>
    /// Implementação de Pattern baseada na API nativa do EPANET.
    /// Fornece acesso aos valores por período via EN_getpatternvalue e EN_setpatternvalue
    /// com cache interno invalidável.
    /// </summary>
    public sealed class NativePattern : Pattern
    {
        private readonly Native.NativeContext _ctx;
        private readonly int _index0; // zero-based
        private readonly int _length;
        private readonly double?[] _cache;

        internal NativePattern(Native.NativeContext ctx, int index0, string id)
        {
            _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
            _index0 = index0;
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Index = index0;
            _length = _ctx.GetPatternLength(index0 + 1);
            _cache = new double?[_length];
        }

        /// <summary>
        /// Comprimento do pattern (número de períodos).
        /// </summary>
        public int Length => _length;

        /// <summary>
        /// Acessa o valor do pattern no período (zero-based).
        /// </summary>
        /// <param name="period">Período zero-based.</param>
        /// <returns>Valor do multiplicador do pattern.</returns>
        public double this[int period]
        {
            get
            {
                if (period < 0 || period >= _length) throw new ArgumentOutOfRangeException(nameof(period));
                if (_cache[period].HasValue) return _cache[period].Value;
                double v = _ctx.GetPatternValue(_index0 + 1, period + 1);
                _cache[period] = v;
                return v;
            }
            set
            {
                if (period < 0 || period >= _length) throw new ArgumentOutOfRangeException(nameof(period));
                _ctx.SetPatternValue(_index0 + 1, period + 1, value);
                _cache[period] = value;
            }
        }

        /// <summary>
        /// Invalida o cache de valores do pattern.
        /// </summary>
        public void InvalidateCache()
        {
            for (int i = 0; i < _cache.Length; i++) _cache[i] = null;
        }
    }
}
