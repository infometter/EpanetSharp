using System;

namespace EpanetSharp.Exceptions
{
    /// <summary>
    /// Exceção base para erros relacionados à biblioteca EpanetSharp.
    /// Contém informações adicionais retornadas pela camada nativa.
    /// </summary>
    public class EpanetException : Exception
    {
        /// <summary>
        /// Código de erro retornado pela camada nativa (quando aplicável).
        /// </summary>
        public int? ErrorCode { get; }

        /// <summary>
        /// Nome da função nativa que originou o erro (quando aplicável).
        /// </summary>
        public string? Function { get; }

        /// <summary>
        /// Mensagem retornada pela camada nativa (quando aplicável).
        /// </summary>
        public string? NativeMessage { get; }

        /// <summary>
        /// Inicializa uma nova instância de <see cref="EpanetException"/> sem informações adicionais.
        /// </summary>
        public EpanetException()
        {
        }

        /// <summary>
        /// Inicializa uma nova instância de <see cref="EpanetException"/> com a mensagem especificada.
        /// </summary>
        /// <param name="message">A mensagem que descreve o erro.</param>
        public EpanetException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Inicializa uma nova instância de <see cref="EpanetException"/> com a mensagem e a exceção interna.
        /// </summary>
        /// <param name="message">A mensagem que descreve o erro.</param>
        /// <param name="inner">A exceção que causou a atual, se disponível.</param>
        public EpanetException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Inicializa uma nova instância de <see cref="EpanetException"/> contendo informações da camada nativa.
        /// </summary>
        /// <param name="errorCode">Código de erro retornado pela camada nativa.</param>
        /// <param name="function">Nome da função nativa que originou o erro.</param>
        /// <param name="nativeMessage">Mensagem retornada pela camada nativa.</param>
        public EpanetException(int errorCode, string function, string nativeMessage)
            : base(FormatMessage(errorCode, function, nativeMessage))
        {
            ErrorCode = errorCode;
            Function = function;
            NativeMessage = nativeMessage;
        }

        /// <summary>
        /// Inicializa uma nova instância de <see cref="EpanetException"/> contendo informações da camada nativa e uma exceção interna.
        /// </summary>
        /// <param name="errorCode">Código de erro retornado pela camada nativa.</param>
        /// <param name="function">Nome da função nativa que originou o erro.</param>
        /// <param name="nativeMessage">Mensagem retornada pela camada nativa.</param>
        /// <param name="inner">Exceção interna que causou este erro.</param>
        public EpanetException(int errorCode, string function, string nativeMessage, Exception inner)
            : base(FormatMessage(errorCode, function, nativeMessage), inner)
        {
            ErrorCode = errorCode;
            Function = function;
            NativeMessage = nativeMessage;
        }

        private static string FormatMessage(int errorCode, string function, string nativeMessage)
        {
            return $"Native error {errorCode} in {function}: {nativeMessage}";
        }

        /// <summary>
        /// Retorna uma representação textual desta exceção incluindo informações nativas quando disponíveis.
        /// </summary>
        /// <returns>String com detalhes da exceção.</returns>
        public override string ToString()
        {
            var baseText = base.ToString();

            if (ErrorCode is null && Function is null && NativeMessage is null)
            {
                return baseText;
            }

            return $"{baseText}{Environment.NewLine}NativeError: {{ ErrorCode = {ErrorCode}, Function = {Function}, NativeMessage = {NativeMessage} }}";
        }
    }
}
