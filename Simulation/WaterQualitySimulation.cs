using System;

namespace EpanetSharp.Simulation
{
    /// <summary>
    /// Controla a execução da simulação de qualidade (contaminação) via API nativa.
    /// Esta classe expõe os métodos básicos para abertura, inicialização, execução passo-a-passo
    /// e fechamento do módulo de qualidade do EPANET.
    /// </summary>
    public sealed class WaterQualitySimulation : IDisposable
    {
        private readonly Native.NativeContext _ctx;
        private bool _isOpen;
        private bool _disposed;

        /// <summary>
        /// Evento disparado quando um passo de simulação é executado. O argumento é o tamanho do passo (segundos).
        /// </summary>
        public event EventHandler<int>? Progress;

        /// <summary>
        /// Cria uma nova instância de <see cref="WaterQualitySimulation"/> associada ao contexto nativo do projeto.
        /// </summary>
        /// <param name="ctx">Contexto nativo do projeto.</param>
        internal WaterQualitySimulation(Native.NativeContext ctx)
        {
            _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
        }

        /// <summary>
        /// Abre o módulo de qualidade (EN_openQ). Deve ser chamado após o projeto estar aberto.
        /// </summary>
        public void Open()
        {
            EnsureNotDisposed();
            int rc = Native.NativeMethods.EN_openQ(_ctx.ProjectHandle);
            _ctx.CheckError(rc);
            _isOpen = true;
        }

        /// <summary>
        /// Inicializa a simulação de qualidade (EN_initQ).
        /// Opcionalmente especifica nó de rastreamento e tipo de rastreamento.
        /// </summary>
        /// <param name="traceNodeIndex">Índice (1-based) do nó de rastreamento ou 0 para nenhum rastreamento.</param>
        /// <param name="traceType">Tipo de rastreamento (conforme API nativa), por exemplo 0 para None, 1 para Concentration, etc.</param>
        public void Initialize(int traceNodeIndex = 0, int traceType = 0)
        {
            EnsureOpen();
            int rc = Native.NativeMethods.EN_initQ(_ctx.ProjectHandle, traceNodeIndex, traceType);
            _ctx.CheckError(rc);
        }

        /// <summary>
        /// Executa a simulação de qualidade até o fim (EN_runQ).
        /// </summary>
        public void Run()
        {
            EnsureOpen();
            int rc = Native.NativeMethods.EN_runQ(_ctx.ProjectHandle);
            _ctx.CheckError(rc);
        }

        /// <summary>
        /// Avança a simulação de qualidade um passo (EN_nextQ) e retorna o tamanho do passo em segundos.
        /// Retorna -1 quando a simulação foi concluída.
        /// </summary>
        /// <returns>Tamanho do passo (segundos) ou -1 se não houver mais passos.</returns>
        public int Next()
        {
            EnsureOpen();
            int rc = Native.NativeMethods.EN_nextQ(_ctx.ProjectHandle, out int tstep);
            if (rc == 0)
            {
                Progress?.Invoke(this, tstep);
                return tstep;
            }

            _ctx.CheckError(rc);
            return -1; // unreachable
        }

        /// <summary>
        /// Fecha o módulo de qualidade (EN_closeQ).
        /// </summary>
        public void Close()
        {
            if (!_isOpen) return;
            int rc = Native.NativeMethods.EN_closeQ(_ctx.ProjectHandle);
            _ctx.CheckError(rc);
            _isOpen = false;
        }

        private void EnsureOpen()
        {
            EnsureNotDisposed();
            if (!_isOpen) throw new InvalidOperationException("Water quality simulation is not open.");
        }

        private void EnsureNotDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(WaterQualitySimulation));
        }

        /// <summary>
        /// Libera recursos e fecha o módulo se aberto.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            try
            {
                Close();
            }
            catch
            {
                // suprimir exceções durante dispose
            }
            _disposed = true;
        }
    }
}
