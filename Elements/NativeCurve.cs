using System;
using System.Collections.Generic;

namespace EpanetSharp.Elements
{
    /// <summary>
    /// Implementação concreta de Curve que carrega pontos via API nativa.
    /// </summary>
    public sealed class NativeCurve : Curve
    {
        private readonly Native.NativeContext _ctx;
        private readonly int _index0;
        private List<CurvePoint>? _points;

        internal NativeCurve(Native.NativeContext ctx, int index0, string id)
        {
            _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
            _index0 = index0;
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Index = index0;
        }

        private void EnsureLoaded()
        {
            if (_points != null) return;
            var (xs, ys) = _ctx.GetCurve(_index0 + 1);
            _points = new List<CurvePoint>(xs.Length);
            for (int i = 0; i < xs.Length; i++)
            {
                _points.Add(new CurvePoint(xs[i], ys[i]));
            }
        }

        /// <summary>
        /// Pontos da curva (lazy-loaded).
        /// </summary>
        public IReadOnlyList<CurvePoint> Points
        {
            get
            {
                EnsureLoaded();
                return _points!;
            }
        }

        /// <summary>
        /// Substitui os pontos da curva e atualiza no backend nativo.
        /// </summary>
        public void SetPoints(IList<CurvePoint> points)
        {
            if (points is null) throw new ArgumentNullException(nameof(points));
            var xs = new double[points.Count];
            var ys = new double[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                xs[i] = points[i].X;
                ys[i] = points[i].Y;
            }
            _ctx.SetCurve(_index0 + 1, xs, ys);
            _points = new List<CurvePoint>(points);
        }

        /// <summary>
        /// Invalida o cache de pontos.
        /// </summary>
        public void InvalidateCache()
        {
            _points = null;
        }
    }
}
