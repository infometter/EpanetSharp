using System;

namespace EpanetSharp.Utilities
{
    /// <summary>
    /// Fornece validações de argumentos simples utilizadas pela biblioteca.
    /// </summary>
    internal static class Guard
    {
        /// <summary>
        /// Verifica se o valor informado é nulo e lança <see cref="ArgumentNullException"/> quando aplicável.
        /// </summary>
        /// <param name="value">Valor a ser verificado.</param>
        /// <param name="paramName">Nome do parâmetro avaliado.</param>
        internal static void NotNull(object? value, string paramName)
        {
            if (value is null)
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}
