using System;
using EpanetSharp.Exceptions;

namespace EpanetSharp.Utilities
{
    /// <summary>
    /// Auxilia na criação e lançamento consistente de exceções.
    /// </summary>
    internal static class ThrowHelper
    {
        /// <summary>
        /// Lança uma <see cref="ArgumentNullException"/> para o nome do parâmetro especificado.
        /// </summary>
        /// <param name="paramName">Nome do parâmetro que é nulo.</param>
        internal static void ThrowArgumentNull(string paramName)
        {
            throw new ArgumentNullException(paramName);
        }

        /// <summary>
        /// Lança uma <see cref="EpanetException"/> com a mensagem informada.
        /// </summary>
        /// <param name="message">Mensagem da exceção.</param>
        internal static void ThrowEpanet(string message)
        {
            throw new EpanetException(message);
        }
    }
}
