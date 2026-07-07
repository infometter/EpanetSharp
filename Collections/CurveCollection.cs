using System;
using System.Collections;
using System.Collections.Generic;
using EpanetSharp.Elements;

namespace EpanetSharp.Collections
{
    /// <summary>
    /// Coleção especializada de <see cref="Curve"/> utilizada pela biblioteca.
    /// Implementa <see cref="IReadOnlyCollection{T}"/> e fornece pesquisas por identificador.
    /// </summary>
    public sealed class CurveCollection : IReadOnlyCollection<Curve>
    {
        private readonly List<Curve> _list = new();
        private readonly Dictionary<string, Curve> _map = new(StringComparer.Ordinal);

        // native-backed
        private readonly Native.NativeContext? _nativeContext;
        private Curve?[]? _nativeCache;
        private Dictionary<string, int>? _idToIndexCache;

        /// <summary>
        /// Inicializa uma nova instância vazia de <see cref="CurveCollection"/>.
        /// </summary>
        public CurveCollection()
        {
        }

        public CurveCollection(Native.NativeContext nativeContext)
        {
            _nativeContext = nativeContext ?? throw new ArgumentNullException(nameof(nativeContext));
        }

        /// <summary>
        /// Número de curvas na coleção.
        /// </summary>
        public int Count
        {
            get
            {
                if (_nativeContext is null) return _list.Count;
                if (!_nativeContext.IsProjectCreated) return 0;
                return _nativeContext.GetCount(Native.NativeConstants.EN_CURVECOUNT);
            }
        }

        /// <summary>
        /// Obtém a curva pelo identificador. Retorna <c>null</c> se não existir.
        /// </summary>
        /// <param name="id">Identificador da curva.</param>
        /// <returns>Instância de <see cref="Curve"/> ou <c>null</c> se não encontrada.</returns>
        public Curve? this[string id]
        {
            get
            {
                if (id is null) throw new ArgumentNullException(nameof(id));
                if (_nativeContext is null)
                {
                    _map.TryGetValue(id, out var curve);
                    return curve;
                }

                if (_idToIndexCache != null && _idToIndexCache.TryGetValue(id, out var idx))
                {
                    return this[idx];
                }

                try
                {
                    int nativeIndex = _nativeContext.GetCurveIndex(id);
                    int idx0 = nativeIndex - 1;
                    EnsureNativeCacheInitialized(Count);
                    return this[idx0];
                }
                catch (EpanetSharp.Exceptions.EpanetException)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Verifica se a coleção contém uma curva com o identificador informado.
        /// </summary>
        /// <param name="id">Identificador da curva.</param>
        /// <returns><c>true</c> se existir; caso contrário, <c>false</c>.</returns>
        public bool Contains(string id)
        {
            if (id is null) throw new ArgumentNullException(nameof(id));
            if (_nativeContext is null) return _map.ContainsKey(id);
            try
            {
                _ = _nativeContext.GetCurveIndex(id);
                return true;
            }
            catch (EpanetSharp.Exceptions.EpanetException)
            {
                return false;
            }
        }

        /// <summary>
        /// Tenta obter a curva com o identificador informado.
        /// </summary>
        /// <param name="id">Identificador da curva.</param>
        /// <param name="curve">Saída com a curva caso encontrada.</param>
        /// <returns><c>true</c> se a curva foi encontrada; caso contrário, <c>false</c>.</returns>
        public bool TryGet(string id, out Curve? curve)
        {
            if (id is null) throw new ArgumentNullException(nameof(id));
            if (_nativeContext is null) return _map.TryGetValue(id, out curve);
            curve = this[id];
            return curve is not null;
        }

        /// <summary>
        /// Adiciona uma curva à coleção. A curva deve possuir <see cref="Curve.Id"/> não-nulo.
        /// </summary>
        /// <param name="curve">Curva a adicionar.</param>
        public void Add(Curve curve)
        {
            if (curve is null) throw new ArgumentNullException(nameof(curve));
            if (curve.Id is null) throw new ArgumentException("Curve.Id must be set.", nameof(curve));
            if (_map.ContainsKey(curve.Id)) throw new ArgumentException("A curve with the same id already exists.", nameof(curve));
            _list.Add(curve);
            _map[curve.Id] = curve;
        }

        private void EnsureNativeCacheInitialized(int count)
        {
            if (_nativeCache is null || _nativeCache.Length != count)
            {
                _nativeCache = new Curve[count];
            }
            if (_idToIndexCache is null)
            {
                _idToIndexCache = new Dictionary<string, int>(StringComparer.Ordinal);
            }
        }

        /// <summary>
        /// Retorna um enumerador que itera sobre a coleção de curvas.
        /// </summary>
        /// <returns>Enumerator de <see cref="Curve"/>.</returns>
        public Curve this[int index]
        {
            get
            {
                if (_nativeContext is null) return _list[index];

                int count = Count;
                if (index < 0 || index >= count) throw new ArgumentOutOfRangeException(nameof(index));
                EnsureNativeCacheInitialized(count);

                var existing = _nativeCache![index];
                if (existing is not null) return existing;

                string id = _nativeContext.GetCurveId(index + 1);
                var curve = new Elements.NativeCurve(_nativeContext, index, id);
                _nativeCache[index] = curve;
                (_idToIndexCache ??= new Dictionary<string, int>(StringComparer.Ordinal))[id] = index;
                return curve;
            }
        }

        public IEnumerator<Curve> GetEnumerator()
        {
            if (_nativeContext is null)
            {
                foreach (var c in _list) yield return c;
                yield break;
            }

            int count = Count;
            for (int i = 0; i < count; i++) yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
