using System;

namespace EpanetSharp.Elements
{
    /// <summary>
    /// Implementação simples de Node utilizada internamente para carregamento lazy.
    /// </summary>
    public sealed class BasicNode : Node
    {
        public BasicNode(string id, int index)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Index = index;
        }
    }
}
