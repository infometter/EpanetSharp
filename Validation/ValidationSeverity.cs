namespace EpanetSharp.Validation
{
    /// <summary>
    /// Severidade de um item de validação.
    /// </summary>
    public enum ValidationSeverity
    {
        /// <summary>Informação apenas — não impede a simulação.</summary>
        Info,
        /// <summary>Aviso — a simulação pode prosseguir mas há algo suspeito.</summary>
        Warning,
        /// <summary>Erro crítico — a simulação não deve ser executada.</summary>
        Error
    }
}
