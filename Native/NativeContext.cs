using System;
using System.Diagnostics;

namespace EpanetSharp.Native
{
    /// <summary>
    /// Gerencia o ciclo de vida do handle nativo do projeto EPANET (EN_Project).
    /// Encapsula o ponteiro nativo e expõe operações básicas de criação, destruição e verificação de erros.
    /// </summary>
    public sealed class NativeContext : IDisposable
    {
        private IntPtr _projectHandle = IntPtr.Zero;
        private bool _disposed;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="NativeContext"/>.
        /// O contexto não cria o projeto automaticamente; é necessário chamar <see cref="CreateProject"/>.
        /// </summary>
        public NativeContext()
        {
        }

        /// <summary>
        /// Indica se o projeto nativo foi criado com sucesso neste contexto.
        /// </summary>
        public bool IsProjectCreated => _projectHandle != IntPtr.Zero;

        /// <summary>
        /// Cria o projeto nativo e guarda o handle internamente.
        /// Lança <see cref="Exceptions.EpanetException"/> caso ocorra erro durante a criação.
        /// </summary>
        public void CreateProject()
        {
            EnsureNotDisposed();

            if (IsProjectCreated)
            {
                return;
            }

            int code = NativeMethods.CreateProject(out IntPtr handle);
            NativeMethods.CheckError(code);

            _projectHandle = handle;
            Debug.Assert(_projectHandle != IntPtr.Zero);
        }

        /// <summary>
        /// Destroi o projeto nativo associado a este contexto.
        /// Se o projeto não estiver criado, o método não faz nada.
        /// </summary>
        public void DestroyProject()
        {
            EnsureNotDisposed();

            if (!IsProjectCreated)
            {
                return;
            }

            int code = NativeMethods.DestroyProject(_projectHandle);
            // Sempre limpar o handle local mesmo que ocorra erro na lib nativa.
            _projectHandle = IntPtr.Zero;
            NativeMethods.CheckError(code);
        }

        /// <summary>
        /// Verifica um código de retorno nativo e lança uma exceção caso não seja sucesso.
        /// Este método delega a verificação para <see cref="NativeMethods.CheckError(int)"/>.
        /// </summary>
        /// <param name="code">Código retornado por uma operação nativa.</param>
        public void CheckError(int code)
        {
            EnsureNotDisposed();
            NativeMethods.CheckError(code);
        }

        /// <summary>
        /// Libera os recursos utilizados pelo objeto <see cref="NativeContext"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Implementação do padrão Dispose.
        /// </summary>
        /// <param name="disposing">Indica se a chamada foi feita explicitamente via <see cref="Dispose"/>.</param>
        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            // Sempre tente destruir o projeto nativo.
            try
            {
                DestroyProject();
            }
            catch
            {
                // Suprimir exceções no finalizador / dispose para evitar lançar durante coleta.
            }

            _disposed = true;
        }

        /// <summary>
        /// Garante que o objeto não foi descartado.
        /// </summary>
        private void EnsureNotDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(NativeContext));
            }
        }

        /// <summary>
        /// Finalizador que garante liberação de recursos não gerenciados caso <see cref="Dispose"/> não tenha sido chamado.
        /// </summary>
        ~NativeContext()
        {
            Dispose(disposing: false);
        }
    }
}

