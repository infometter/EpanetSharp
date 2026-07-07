using System;
using EpanetSharp.Native;
using EpanetSharp.Exceptions;

namespace EpanetSharp.Core
{
    /// <summary>
    /// Representa um projeto EPANET em memória e provê operações básicas de ciclo de vida.
    /// Esta classe utiliza <see cref="NativeContext"/> para interagir com a camada nativa.
    /// </summary>
    public class Project : IDisposable
    {
        private readonly NativeContext _nativeContext;
        private bool _disposed;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Project"/>.
        /// </summary>
        public Project()
        {
            _nativeContext = new NativeContext();
        }

        /// <summary>
        /// Indica se o projeto está aberto (projeto nativo criado).
        /// </summary>
        public bool IsOpen => _nativeContext.IsProjectCreated;

        /// <summary>
        /// Obtém a versão do projeto/biblioteca.
        /// Implementação atual retorna uma versão placeholder até integração com a camada nativa.
        /// </summary>
        public string Version
        {
            get
            {
                EnsureNotDisposed();
                try
                {
                    // Em futuras implementações, solicitar versão à camada nativa.
                    return "EpanetSharp-0.1.0";
                }
                catch (EpanetException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new EpanetException("Falha ao obter versão.", ex);
                }
            }
        }

        /// <summary>
        /// Abre (cria) o projeto nativo.
        /// </summary>
        public void Open()
        {
            EnsureNotDisposed();
            try
            {
                if (!_nativeContext.IsProjectCreated)
                {
                    _nativeContext.CreateProject();
                }
            }
            catch (EpanetException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new EpanetException("Falha ao abrir o projeto.", ex);
            }
        }

        /// <summary>
        /// Fecha (destrói) o projeto nativo.
        /// </summary>
        public void Close()
        {
            EnsureNotDisposed();
            try
            {
                if (_nativeContext.IsProjectCreated)
                {
                    _nativeContext.DestroyProject();
                }
            }
            catch (EpanetException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new EpanetException("Falha ao fechar o projeto.", ex);
            }
        }

        /// <summary>
        /// Executa a simulação do projeto.
        /// Implementação atual apenas valida estado e delega verificação de erros à camada nativa.
        /// </summary>
        public void Run()
        {
            EnsureNotDisposed();
            try
            {
                if (!_nativeContext.IsProjectCreated)
                {
                    throw new EpanetException("Projeto não está aberto.");
                }

                // Em futura implementação, executar chamadas nativas que retornam um código.
                // Simular operação bem-sucedida passando código 0 para CheckError.
                _nativeContext.CheckError(0);
            }
            catch (EpanetException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new EpanetException("Falha ao executar Run().", ex);
            }
        }

        /// <summary>
        /// Libera os recursos utilizados pelo objeto <see cref="Project"/>.
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
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                try
                {
                    _nativeContext?.Dispose();
                }
                catch (EpanetException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new EpanetException("Falha ao liberar recursos do projeto.", ex);
                }
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
                throw new EpanetException("Project já descartado.");
            }
        }

        /// <summary>
        /// Finalizador que garante liberação de recursos não gerenciados caso <see cref="Dispose"/> não tenha sido chamado.
        /// </summary>
        ~Project()
        {
            Dispose(disposing: false);
        }
    }
}
