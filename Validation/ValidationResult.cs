using System.Collections.Generic;
using EpanetSharp.Exceptions;

namespace EpanetSharp.Validation
{
    /// <summary>
    /// Resultado de uma validação de rede hidráulica.
    /// </summary>
    public class ValidationResult
    {
        private readonly List<ValidationError> _errors;

        /// <summary>
        /// Lista completa de itens de validação (erros, avisos e informações).
        /// </summary>
        public IReadOnlyList<ValidationError> Errors => _errors;

        /// <summary>
        /// Retorna true se não há itens com severidade <see cref="ValidationSeverity.Error"/>.
        /// </summary>
        public bool IsValid => ErrorCount == 0;

        /// <summary>Número de erros críticos.</summary>
        public int ErrorCount   { get; private set; }

        /// <summary>Número de avisos.</summary>
        public int WarningCount { get; private set; }

        /// <summary>Número de informações.</summary>
        public int InfoCount    { get; private set; }

        internal ValidationResult(List<ValidationError> errors)
        {
            _errors = errors ?? new List<ValidationError>();
            foreach (var e in _errors)
            {
                if      (e.Severity == ValidationSeverity.Error)   ErrorCount++;
                else if (e.Severity == ValidationSeverity.Warning) WarningCount++;
                else                                               InfoCount++;
            }
        }

        /// <summary>
        /// Lança <see cref="NetworkValidationException"/> caso a rede contenha erros.
        /// </summary>
        public void ThrowIfInvalid()
        {
            if (!IsValid)
                throw new NetworkValidationException(this);
        }
    }
}
