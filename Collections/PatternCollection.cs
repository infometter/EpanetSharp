using System;
using System.Collections;
using System.Collections.Generic;
using EpanetSharp.Elements;

namespace EpanetSharp.Collections
{
    /// <summary>
    /// Coleção especializada de <see cref="Pattern"/> utilizada pela biblioteca.
    /// Implementa <see cref="IReadOnlyCollection{T}"/> e fornece pesquisas por identificador/nome.
    /// </summary>
    public sealed class PatternCollection : IReadOnlyCollection<Pattern>
    {
        private readonly List<Pattern> _list = new();
        private readonly Dictionary<string, Pattern> _map = new(StringComparer.Ordinal);

        // native-backed
        private readonly Native.NativeContext? _nativeContext;
        private Pattern?[]? _nativeCache;
        private Dictionary<string, int>? _idToIndexCache;

        /// <summary>
        /// Inicializa uma nova instância vazia de <see cref="PatternCollection"/>.
        /// </summary>
        public PatternCollection()
        {
        }

        public PatternCollection(Native.NativeContext nativeContext)
        {
            _nativeContext = nativeContext ?? throw new ArgumentNullException(nameof(nativeContext));
        }

        /// <summary>
        /// Número de patterns na coleção.
        /// </summary>
        public int Count => _list.Count;

        private int NativeCount
        {
            get
            {
                if (_nativeContext is null) return _list.Count;
                if (!_nativeContext.IsProjectCreated) return 0;
                return _nativeContext.GetCount(Native.NativeConstants.EN_PATTERNCOUNT);
            }
        }

        /// <summary>
        /// Obtém o pattern pelo nome. Retorna <c>null</c> se não existir.
        /// </summary>
        /// <param name="name">Nome do pattern.</param>
        /// <returns>Instância de <see cref="Pattern"/> ou <c>null</c> se não encontrada.</returns>
        public Pattern? this[string id]
        {
            get
            {
                if (id is null) throw new ArgumentNullException(nameof(id));
                if (_nativeContext is null)
                {
                    _map.TryGetValue(id, out var pattern);
                    return pattern;
                }

                if (_idToIndexCache != null && _idToIndexCache.TryGetValue(id, out var idx))
                {
                    return this[idx];
                }

                try
                {
                    int nativeIndex = _nativeContext.GetPatternIndex(id);
                    int idx0 = nativeIndex - 1;
                    EnsureNativeCacheInitialized(NativeCount);
                    return this[idx0];
                }
                catch (EpanetSharp.Exceptions.EpanetException)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Verifica se a coleção contém um pattern com o nome informado.
        /// </summary>
        /// <param name="name">Nome do pattern.</param>
        /// <returns><c>true</c> se existir; caso contrário, <c>false</c>.</returns>
        public bool Contains(string id)
        {
            if (id is null) throw new ArgumentNullException(nameof(id));
            return _map.ContainsKey(id);
        }

        /// <summary>
        /// Tenta obter o pattern com o nome informado.
        /// </summary>
        /// <param name="name">Nome do pattern.</param>
        /// <param name="pattern">Saída com o pattern caso encontrado.</param>
        /// <returns><c>true</c> se o pattern foi encontrado; caso contrário, <c>false</c>.</returns>
        public bool TryGet(string id, out Pattern? pattern)
        {
            if (id is null) throw new ArgumentNullException(nameof(id));
            if (_nativeContext is null) return _map.TryGetValue(id, out pattern);
            pattern = this[id];
            return pattern is not null;
        }

        /// <summary>
        /// Adiciona um pattern à coleção. O pattern deve possuir <see cref="Pattern.Name"/> não-nulo.
        /// </summary>
        /// <param name="pattern">Pattern a adicionar.</param>
        public void Add(Pattern pattern)
        {
            if (pattern is null) throw new ArgumentNullException(nameof(pattern));
            if (pattern.Id is null) throw new ArgumentException("Pattern.Id must be set.", nameof(pattern));
            if (_map.ContainsKey(pattern.Id)) throw new ArgumentException("A pattern with the same id already exists.", nameof(pattern));
            _list.Add(pattern);
            _map[pattern.Id] = pattern;
        }

        private void EnsureNativeCacheInitialized(int count)
        {
            if (_nativeCache is null || _nativeCache.Length != count)
            {
                _nativeCache = new Pattern[count];
            }
            if (_idToIndexCache is null)
            {
                _idToIndexCache = new Dictionary<string, int>(StringComparer.Ordinal);
            }
        }

        public Pattern this[int index]
        {
            get
            {
                if (_nativeContext is null) return _list[index];

                int count = NativeCount;
                if (index < 0 || index >= count) throw new ArgumentOutOfRangeException(nameof(index));
                EnsureNativeCacheInitialized(count);

                var existing = _nativeCache![index];
                if (existing is not null) return existing;

                string id = _nativeContext.GetPatternId(index + 1);
                var pattern = new NativePattern(_nativeContext, index, id);
                pattern.Index = index; // Pattern base has Index setter? If not, ignore
                _nativeCache[index] = pattern;
                (_idToIndexCache ??= new Dictionary<string, int>(StringComparer.Ordinal))[id] = index;
                return pattern;
            }
        }

        /// <summary>
        /// Retorna um enumerador que itera sobre a coleção de patterns.
        /// </summary>
        /// <returns>Enumerator de <see cref="Pattern"/>.</returns>
        public IEnumerator<Pattern> GetEnumerator()
        {
            if (_nativeContext is null)
            {
                foreach (var p in _list) yield return p;
                yield break;
            }

            int count = NativeCount;
            for (int i = 0; i < count; i++) yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
