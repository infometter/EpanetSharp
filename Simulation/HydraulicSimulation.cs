using System;
using System.Threading;

namespace EpanetSharp.Simulation
{
    /// <summary>
    /// Controla a execução da simulação hidráulica via API nativa (EN_openH, EN_initH, EN_runH, EN_nextH, EN_closeH).
    /// </summary>
    public sealed class HydraulicSimulation : IDisposable
    {
        private readonly Native.NativeContext _ctx;
        private bool _isOpen;
        private bool _disposed;

        public event EventHandler<int>? Progress; // reporta tstep em segundos ou outro indicador

        internal HydraulicSimulation(Native.NativeContext ctx)
        {
            _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
        }

        /// <summary>
        /// Abre (prepara) o motor de simulação hidráulica.
        /// </summary>
        public void Open()
        {
            EnsureNotDisposed();
            int rc = Native.NativeMethods.EN_openH(_ctx.ProjectHandle);
            _ctx.CheckError(rc);
            _isOpen = true;
        }

        /// <summary>
        /// Inicializa a simulação após a abertura.
        /// </summary>
        public void Initialize()
        {
            EnsureOpen();
            int rc = Native.NativeMethods.EN_initH(_ctx.ProjectHandle);
            _ctx.CheckError(rc);
        }

        /// <summary>
        /// Executa a simulação completa até o fim (EN_runH).
        /// </summary>
        public void Run()
        {
            EnsureOpen();
            int rc = Native.NativeMethods.EN_runH(_ctx.ProjectHandle);
            _ctx.CheckError(rc);
        }

        /// <summary>
        /// Avança a simulação para o próximo passo e retorna o tamanho do passo em segundos.
        /// Retorna -1 quando não há mais passos.
        /// </summary>
        public int Next()
        {
            EnsureOpen();
            int rc = Native.NativeMethods.EN_nextH(_ctx.ProjectHandle, out int tstep);
            if (rc == 0)
            {
                Progress?.Invoke(this, tstep);
                return tstep;
            }

            // se rc <> 0, lançar exceção via CheckError
            _ctx.CheckError(rc);
            return -1; // unreachable
        }

        /// <summary>
        /// Fecha o motor de simulação hidráulica.
        /// </summary>
        public void Close()
        {
            if (!_isOpen) return;
            int rc = Native.NativeMethods.EN_closeH(_ctx.ProjectHandle);
            _ctx.CheckError(rc);
            _isOpen = false;
        }

        private void EnsureOpen()
        {
            EnsureNotDisposed();
            if (!_isOpen) throw new InvalidOperationException("Hydraulic simulation is not open.");
        }

        private void EnsureNotDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(HydraulicSimulation));
        }

        public void Dispose()
        {
            if (_disposed) return;
            try
            {
                Close();
            }
            catch
            {
                // suprimir
            }
            _disposed = true;
        }
    }
}
