using System;
using System.Collections;
using System.Collections.Generic;
using EpanetSharp.Elements;

namespace EpanetSharp.Collections
{
    /// <summary>
    /// Coleção especializada de <see cref="Link"/> utilizada pela biblioteca.
    /// Implementa <see cref="IReadOnlyCollection{T}"/> e fornece pesquisas por identificador.
    /// </summary>
    public sealed class LinkCollection : IReadOnlyCollection<Link>
    {
        private readonly List<Link> _list = new();
        private readonly Dictionary<string, Link> _map = new(StringComparer.Ordinal);

        // native-backed
        private readonly Native.NativeContext? _nativeContext;
        private Link?[]? _nativeCache;
        private Dictionary<string, int>? _idToIndexCache;

        /// <summary>
        /// Inicializa uma nova instância vazia de <see cref="LinkCollection"/>.
        /// </summary>
        public LinkCollection()
        {
        }

        public LinkCollection(Native.NativeContext nativeContext)
        {
            _nativeContext = nativeContext ?? throw new ArgumentNullException(nameof(nativeContext));
        }

        /// <summary>
        /// Número de enlaces na coleção.
        /// </summary>
        public int Count => _list.Count;

        private int NativeCount
        {
            get
            {
                if (_nativeContext is null) return _list.Count;
                if (!_nativeContext.IsProjectCreated) return 0;
                return _nativeContext.GetCount(Native.NativeConstants.EN_LINKCOUNT);
            }
        }

        /// <summary>
        /// Obtém o enlace pelo identificador. Retorna <c>null</c> se não existir.
        /// </summary>
        /// <param name="id">Identificador do enlace.</param>
        /// <returns>Instância de <see cref="Link"/> ou <c>null</c> se não encontrada.</returns>
        public Link? this[string id]
        {
            get
            {
                if (id is null) throw new ArgumentNullException(nameof(id));
                if (_nativeContext is null)
                {
                    _map.TryGetValue(id, out var link);
                    return link;
                }

                if (_idToIndexCache != null && _idToIndexCache.TryGetValue(id, out var idx))
                {
                    return this[idx];
                }

                try
                {
                    int nativeIndex = _nativeContext.GetLinkIndex(id);
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
        /// Verifica se a coleção contém um enlace com o identificador informado.
        /// </summary>
        /// <param name="id">Identificador do enlace.</param>
        /// <returns><c>true</c> se existir; caso contrário, <c>false</c>.</returns>
        public bool Contains(string id)
        {
            if (id is null) throw new ArgumentNullException(nameof(id));
            return _map.ContainsKey(id);
        }

        /// <summary>
        /// Verifica se a coleção contém o enlace informado.
        /// </summary>
        /// <param name="link">Instância do enlace.</param>
        /// <returns><c>true</c> se existir; caso contrário, <c>false</c>.</returns>
        public bool Contains(Link link)
        {
            if (link is null) throw new ArgumentNullException(nameof(link));
            if (link.Id is null) return false;
            return _map.TryGetValue(link.Id, out var existing) && ReferenceEquals(existing, link);
        }

        /// <summary>
        /// Tenta obter o enlace com o identificador informado.
        /// </summary>
        /// <param name="id">Identificador do enlace.</param>
        /// <param name="link">Saída com o enlace caso encontrado.</param>
        /// <returns><c>true</c> se o enlace foi encontrado; caso contrário, <c>false</c>.</returns>
        public bool TryGet(string id, out Link? link)
        {
            if (id is null) throw new ArgumentNullException(nameof(id));
            if (_nativeContext is null) return _map.TryGetValue(id, out link);
            link = this[id];
            return link is not null;
        }

        /// <summary>
        /// Adiciona um enlace à coleção. O enlace deve possuir <see cref="Link.Id"/> não-nulo.
        /// </summary>
        /// <param name="link">Enlace a adicionar.</param>
        public void Add(Link link)
        {
            if (link is null) throw new ArgumentNullException(nameof(link));
            if (link.Id is null) throw new ArgumentException("Link.Id must be set.", nameof(link));
            if (_map.ContainsKey(link.Id)) throw new ArgumentException("A link with the same id already exists.", nameof(link));
            _list.Add(link);
            _map[link.Id] = link;
        }

        private void EnsureNativeCacheInitialized(int count)
        {
            if (_nativeCache is null || _nativeCache.Length != count)
            {
                _nativeCache = new Link[count];
            }
            if (_idToIndexCache is null)
            {
                _idToIndexCache = new Dictionary<string, int>(StringComparer.Ordinal);
            }
        }

        // native indexer by zero-based index
        public Link this[int index]
        {
            get
            {
                if (_nativeContext is null) return _list[index];

                int count = NativeCount;
                if (index < 0 || index >= count) throw new ArgumentOutOfRangeException(nameof(index));
                EnsureNativeCacheInitialized(count);

                var existing = _nativeCache![index];
                if (existing is not null) return existing;

                // EN_getlinkid expects 1-based index
                string id = _nativeContext.GetLinkId(index + 1);
                var link = EpanetSharp.Elements.LinkFactory.Create(_nativeContext, index, id);
                link.Project = null;
                _nativeCache[index] = link;
                (_idToIndexCache ??= new Dictionary<string, int>(StringComparer.Ordinal))[id] = index;
                return link;
            }
        }

        /// <summary>
        /// Retorna um enumerador que itera sobre a coleção de enlaces.
        /// </summary>
        /// <returns>Enumerator de <see cref="Link"/>.</returns>
        public IEnumerator<Link> GetEnumerator()
        {
            if (_nativeContext is null)
            {
                foreach (var l in _list)
                {
                    yield return l;
                }
                yield break;
            }

            int count = NativeCount;
            for (int i = 0; i < count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
