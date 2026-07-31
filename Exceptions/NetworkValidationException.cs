using System;
using EpanetSharp.Validation;

namespace EpanetSharp.Exceptions
{
    /// <summary>
    /// Exceção lançada quando a validação de uma rede hidráulica detecta erros críticos.
    /// </summary>
    public class NetworkValidationException : EpanetException
    {
        /// <summary>
        /// Resultado completo da validação que originou esta exceção.
        /// </summary>
        public ValidationResult ValidationResult { get; }

        /// <summary>
        /// Cria uma nova exceção de validação com a mensagem informada.
        /// </summary>
        public NetworkValidationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Cria uma nova exceção de validação a partir de um <see cref="ValidationResult"/>.
        /// </summary>
        public NetworkValidationException(ValidationResult result)
            : base(BuildMessage(result))
        {
            ValidationResult = result;
        }

        private static string BuildMessage(ValidationResult result)
        {
            if (result == null) return "Network validation failed.";
            var sb = new System.Text.StringBuilder("Network validation failed with ");
            sb.Append(result.ErrorCount);
            sb.Append(" error(s)");
            if (result.WarningCount > 0)
            {
                sb.Append(" and ");
                sb.Append(result.WarningCount);
                sb.Append(" warning(s)");
            }
            sb.Append(":");
            foreach (var err in result.Errors)
                sb.Append("\n  [").Append(err.Severity).Append("] ").Append(err.Message);
            return sb.ToString();
        }
    }
}
