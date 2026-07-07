using System;

namespace EpanetSharp.Native
{
    /// <summary>
    /// Gerencia o ciclo de vida do handle nativo do projeto EPANET (EN_Project).
    /// Encapsula o ponteiro nativo e expõe operações básicas de criação, destruição e verificação de erros.
    /// Esta classe depende exclusivamente de <see cref="NativeApi"/> para interagir com a camada nativa.
    /// </summary>
    public sealed class NativeContext : IDisposable
    {
        private readonly NativeApi _api = new NativeApi();
        private IntPtr _projectHandle = IntPtr.Zero;
        private bool _disposed;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="NativeContext"/>.
        /// O contexto não cria o projeto automaticamente; é necessário chamar <see cref="CreateProject"/>.
        /// </summary>
        public NativeContext()
        {
        }

        public double GetOption(int option)
        {
            EnsureNotDisposed();
            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");
            return _api.GetOption(_projectHandle, option);
        }

        public void SetOption(int option, double value)
        {
            EnsureNotDisposed();
            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");
            _api.SetOption(_projectHandle, option, value);
        }

        // Expose project handle for simulation wrapper
        internal IntPtr ProjectHandle => _projectHandle;

        /// <summary>
        /// Indica se o projeto nativo foi criado com sucesso neste contexto.
        /// </summary>
        public bool IsProjectCreated => _projectHandle != IntPtr.Zero;

        /// <summary>
        /// Cria o projeto nativo e guarda o handle internamente.
        /// Lança <see cref="EpanetException"/> caso ocorra erro durante a criação.
        /// </summary>
        public void CreateProject()
        {
            EnsureNotDisposed();

            if (IsProjectCreated)
            {
                return;
            }

            _projectHandle = _api.CreateProject();
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

            try
            {
                _api.DeleteProject(_projectHandle);
            }
            finally
            {
                // Sempre limpar o handle local mesmo que ocorra erro na lib nativa.
                _projectHandle = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Abre um arquivo INP no projeto nativo. O projeto é criado se necessário.
        /// </summary>
        /// <param name="inpFile">Caminho para o arquivo INP.</param>
        public void OpenProject(string inpFile)
        {
            EnsureNotDisposed();

            if (inpFile is null) throw new ArgumentNullException(nameof(inpFile));

            if (!IsProjectCreated)
            {
                _projectHandle = _api.CreateProject();
            }

            _api.Open(_projectHandle, inpFile);
        }

        /// <summary>
        /// Fecha o projeto nativo (EN_close) e o deleta.
        /// </summary>
        public void CloseProject()
        {
            EnsureNotDisposed();

            if (!IsProjectCreated) return;

            try
            {
                _api.Close(_projectHandle);
            }
            finally
            {
                // Em qualquer caso, delete o projeto para liberar recursos.
                _api.DeleteProject(_projectHandle);
                _projectHandle = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Verifica um código de retorno nativo e lança uma exceção caso não seja sucesso.
        /// </summary>
        /// <param name="code">Código retornado por uma operação nativa.</param>
        public void CheckError(int code)
        {
            EnsureNotDisposed();
            _api.CheckError(code);
        }

        /// <summary>
        /// Obtém um contador do projeto nativo (EN_getcount) para o código informado.
        /// </summary>
        /// <param name="countCode">Código de contador conforme <see cref="NativeConstants"/>.</param>
        /// <returns>Valor do contador.</returns>
        public int GetCount(int countCode)
        {
            EnsureNotDisposed();

            if (!IsProjectCreated)
            {
                throw new InvalidOperationException("Native project is not created.");
            }

            return _api.GetCount(_projectHandle, countCode);
        }
        public int GetControlIndex(string id)
        {
            EnsureNotDisposed();
            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");
            return _api.GetControlIndex(_projectHandle, id);
        }

        public string GetControlId(int index)
        {
            EnsureNotDisposed();
            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");
            return _api.GetControlId(_projectHandle, index);
        }

        public string GetControlDefinition(int index)
        {
            EnsureNotDisposed();
            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");
            return _api.GetControl(_projectHandle, index);
        }

        public void AddControl(string controlText)
        {
            EnsureNotDisposed();
            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");
            _api.AddControl(_projectHandle, controlText);
        }

        public void DeleteControl(int index)
        {
            EnsureNotDisposed();
            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");
            _api.DeleteControl(_projectHandle, index);
        }

        /// <summary>
        /// Obtém o ID do nó no índice especificado, delegando para <see cref="NativeApi"/>.
        /// </summary>
        public string GetNodeId(int index)
        {
            EnsureNotDisposed();

            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");

            return _api.GetNodeId(_projectHandle, index);
        }

        /// <summary>
        /// Obtém o índice de um nó dado seu ID, delegando para <see cref="NativeApi"/>.
        /// </summary>
        public int GetNodeIndex(string id)
        {
            EnsureNotDisposed();

            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");

            return _api.GetNodeIndex(_projectHandle, id);
        }

        /// <summary>
        /// Obtém o tipo de um nó (código retornado pela API nativa).
        /// </summary>
        public int GetNodeType(int index)
        {
            EnsureNotDisposed();

            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");

            return _api.GetNodeType(_projectHandle, index);
        }

        /// <summary>
        /// Obtém um valor numérico do nó no índice (1-based para a API nativa).
        /// </summary>
        public double GetNodeValue(int index, int paramCode)
        {
            EnsureNotDisposed();

            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");

            return _api.GetNodeValue(_projectHandle, index, paramCode);
        }

        /// <summary>
        /// Define um valor numérico do nó no índice (1-based para a API nativa).
        /// </summary>
        public void SetNodeValue(int index, int paramCode, double value)
        {
            EnsureNotDisposed();

            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");

            _api.SetNodeValue(_projectHandle, index, paramCode, value);
        }

        public string GetNodeComment(int index)
        {
            EnsureNotDisposed();
            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");
            return _api.GetNodeComment(_projectHandle, index);
        }

        public void SetNodeComment(int index, string comment)
        {
            EnsureNotDisposed();
            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");
            _api.SetNodeComment(_projectHandle, index, comment);
        }

        public string GetNodeTag(int index)
        {
            EnsureNotDisposed();
            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");
            return _api.GetNodeTag(_projectHandle, index);
        }

        public void SetNodeTag(int index, string tag)
        {
            EnsureNotDisposed();
            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");
            _api.SetNodeTag(_projectHandle, index, tag);
        }

        public string GetLinkId(int index)
        {
            EnsureNotDisposed();
            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");
            return _api.GetLinkId(_projectHandle, index);
        }

        public int GetLinkIndex(string id)
        {
            EnsureNotDisposed();
            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");
            return _api.GetLinkIndex(_projectHandle, id);
        }

        public int GetLinkType(int index)
        {
            EnsureNotDisposed();
            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");
            return _api.GetLinkType(_projectHandle, index);
        }

        public double GetLinkValue(int index, int paramCode)
        {
            EnsureNotDisposed();
            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");
            return _api.GetLinkValue(_projectHandle, index, paramCode);
        }

        public void SetLinkValue(int index, int paramCode, double value)
        {
            EnsureNotDisposed();
            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");
            _api.SetLinkValue(_projectHandle, index, paramCode, value);
        }

        public int GetPatternIndex(string id)
        {
            EnsureNotDisposed();
            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");
            return _api.GetPatternIndex(_projectHandle, id);
        }

        public string GetPatternId(int index)
        {
            EnsureNotDisposed();
            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");
            return _api.GetPatternId(_projectHandle, index);
        }

        public int GetPatternLength(int index)
        {
            EnsureNotDisposed();
            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");
            return _api.GetPatternLength(_projectHandle, index);
        }

        public double GetPatternValue(int index, int period)
        {
            EnsureNotDisposed();
            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");
            return _api.GetPatternValue(_projectHandle, index, period);
        }

        public void SetPatternValue(int index, int period, double value)
        {
            EnsureNotDisposed();
            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");
            _api.SetPatternValue(_projectHandle, index, period, value);
        }

        public int GetCurveIndex(string id)
        {
            EnsureNotDisposed();
            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");
            return _api.GetCurveIndex(_projectHandle, id);
        }

        public string GetCurveId(int index)
        {
            EnsureNotDisposed();
            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");
            return _api.GetCurveId(_projectHandle, index);
        }

        public (double[] x, double[] y) GetCurve(int index)
        {
            EnsureNotDisposed();
            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");
            return _api.GetCurve(_projectHandle, index);
        }

        public void SetCurve(int index, double[] x, double[] y)
        {
            EnsureNotDisposed();
            if (!IsProjectCreated) throw new InvalidOperationException("Native project is not created.");
            _api.SetCurve(_projectHandle, index, x, y);
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
        /// Quando <paramref name="disposing"/> é <c>true</c>, o método é chamado a partir de Dispose() explícito
        /// e quaisquer exceções devem ser propagadas. Quando <paramref name="disposing"/> é <c>false</c>,
        /// o método é chamado a partir do finalizador e exceções devem ser suprimidas.
        /// </summary>
        /// <param name="disposing">Indica se a chamada foi feita explicitamente via <see cref="Dispose"/>.</param>
        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // Dispose explícito: permitir que exceções fluam para o chamador.
                DestroyProject();
            }
            else
            {
                // Finalizador: suprimir quaisquer exceções para não abortar a coleta.
                try
                {
                    DestroyProject();
                }
                catch
                {
                    // suprimir
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

