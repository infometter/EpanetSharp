namespace EpanetSharp.Validation
{
    /// <summary>
    /// Representa um único item de validação (erro, aviso ou informação).
    /// </summary>
    public class ValidationError
    {
        /// <summary>Severidade do item.</summary>
        public ValidationSeverity Severity { get; }

        /// <summary>Mensagem descritiva.</summary>
        public string Message { get; }

        /// <summary>ID do elemento envolvido (nó ou link), se aplicável.</summary>
        public string ElementId { get; }

        /// <summary>
        /// Cria um novo item de validação.
        /// </summary>
        public ValidationError(ValidationSeverity severity, string message, string elementId = null)
        {
            Severity  = severity;
            Message   = message;
            ElementId = elementId;
        }

        /// <inheritdoc/>
        public override string ToString() =>
            ElementId != null
                ? string.Format("[{0}] {1}: {2}", Severity, ElementId, Message)
                : string.Format("[{0}] {1}", Severity, Message);
    }
}
