using System;
using System.Collections;
using System.Collections.Generic;
using EpanetSharp.Elements;

namespace EpanetSharp.Collections
{
    /// <summary>
    /// Coleção especializada de <see cref="Node"/> utilizada pela biblioteca.
    /// Implementa <see cref="IReadOnlyCollection{T}"/> e fornece pesquisas por identificador.
    /// </summary>
    public sealed class NodeCollection : IReadOnlyCollection<Node>
    {
        // compatibilidade: modo em memória (manual) quando NativeContext não é fornecido
        private readonly List<Node> _list = new();
        private readonly Dictionary<string, Node> _map = new(StringComparer.Ordinal);

        // modo nativo: usa NativeContext para consulta lazy
        private readonly Native.NativeContext? _nativeContext;
        private Node?[]? _nativeCache; // indexed by zero-based index
        private Dictionary<string, int>? _idToIndexCache;

        /// <summary>
        /// Inicializa uma nova instância vazia de <see cref="NodeCollection"/> (modo em memória).
        /// </summary>
        public NodeCollection()
        {
        }

        /// <summary>
        /// Inicializa uma nova instância de <see cref="NodeCollection"/> ligada a um <see cref="NativeContext"/>.
        /// Neste modo, os nós são carregados lazy a partir da API nativa.
        /// </summary>
        /// <param name="nativeContext">Contexto nativo associado ao projeto.</param>
        public NodeCollection(Native.NativeContext nativeContext)
        {
            _nativeContext = nativeContext ?? throw new ArgumentNullException(nameof(nativeContext));
        }

        /// <summary>
        /// Número de nós na coleção.
        /// Em modo nativo, consulta EN_getcount quando o projeto estiver criado.
        /// Em modo em memória, retorna o número de itens adicionados manualmente.
        /// </summary>
        public int Count
        {
            get
            {
                if (_nativeContext is null) return _list.Count;
                if (!_nativeContext.IsProjectCreated) return 0;
                return _nativeContext.GetCount(Native.NativeConstants.EN_NODECOUNT);
            }
        }

        /// <summary>
        /// Indexer por índice (zero-based). Em modo nativo, carrega o id do nó via EN_getnodeid quando necessário.
        /// </summary>
        public Node this[int index]
        {
            get
            {
                if (_nativeContext is null)
                {
                    return _list[index];
                }

                int count = Count;
                if (index < 0 || index >= count) throw new ArgumentOutOfRangeException(nameof(index));
                EnsureNativeCacheInitialized(count);

                var existing = _nativeCache![index];
                if (existing is not null) return existing;

                // EN_getnodeid uses 1-based index
                string id = _nativeContext.GetNodeId(index + 1);
                var node = EpanetSharp.Elements.NodeFactory.Create(_nativeContext, index, id);
                node.Project = null;
                node.Network = null;
                _nativeCache[index] = node;
                (_idToIndexCache ??= new Dictionary<string, int>(StringComparer.Ordinal))[id] = index;
                return node;
            }
        }

        /// <summary>
        /// Indexer por identificador. Retorna <c>null</c> se não encontrado.
        /// </summary>
        public Node? this[string id]
        {
            get
            {
                if (id is null) throw new ArgumentNullException(nameof(id));
                if (_nativeContext is null) return _map.TryGetValue(id, out var n) ? n : null;

                if (!_nativeContext.IsProjectCreated) return null;

                if (_idToIndexCache != null && _idToIndexCache.TryGetValue(id, out var idx))
                {
                    return this[idx];
                }

                try
                {
                    int nativeIndex = _nativeContext.GetNodeIndex(id);
                    int idx0 = nativeIndex - 1;
                    EnsureNativeCacheInitialized(Count);
                    var node = this[idx0];
                    return node;
                }
                catch (EpanetSharp.Exceptions.EpanetException)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Verifica se existe um nó com o identificador informado.
        /// </summary>
        public bool Contains(string id)
        {
            if (id is null) throw new ArgumentNullException(nameof(id));
            if (_nativeContext is null) return _map.ContainsKey(id);
            return this[id] is not null;
        }

        /// <summary>
        /// Verifica se a coleção contém a instância de nó informada.
        /// </summary>
        public bool Contains(Node node)
        {
            if (node is null) throw new ArgumentNullException(nameof(node));
            if (node.Id is null) return false;
            if (_nativeContext is null) return _map.TryGetValue(node.Id, out var existing) && ReferenceEquals(existing, node);
            var found = this[node.Id];
            return ReferenceEquals(found, node);
        }

        /// <summary>
        /// Tenta obter o nó com o identificador informado.
        /// </summary>
        public bool TryGet(string id, out Node? node)
        {
            if (id is null) throw new ArgumentNullException(nameof(id));
            node = this[id];
            return node is not null;
        }

        /// <summary>
        /// Adiciona um nó manualmente (modo em memória).
        /// </summary>
        public void Add(Node node)
        {
            if (_nativeContext is not null) throw new InvalidOperationException("Cannot add nodes in native-backed NodeCollection.");
            if (node is null) throw new ArgumentNullException(nameof(node));
            if (node.Id is null) throw new ArgumentException("Node.Id must be set.", nameof(node));
            if (_map.ContainsKey(node.Id)) throw new ArgumentException("A node with the same id already exists.", nameof(node));
            _list.Add(node);
            _map[node.Id] = node;
        }

        private void EnsureNativeCacheInitialized(int count)
        {
            if (_nativeCache is null || _nativeCache.Length != count)
            {
                _nativeCache = new Node[count];
            }
            if (_idToIndexCache is null)
            {
                _idToIndexCache = new Dictionary<string, int>(StringComparer.Ordinal);
            }
        }

        /// <summary>
        /// Retorna um enumerador que itera sobre a coleção de nós.
        /// </summary>
        public IEnumerator<Node> GetEnumerator()
        {
            if (_nativeContext is null)
            {
                foreach (var n in _list)
                {
                    yield return n;
                }
                yield break;
            }

            int count = Count;
            for (int i = 0; i < count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
