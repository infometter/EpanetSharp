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
        private Simulation.HydraulicSimulation? _simulation;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Project"/>.
        /// </summary>
        public Project()
        {
            _nativeContext = new NativeContext();
            Network = new Network(_nativeContext);
        }

        /// <summary>
        /// Abre um projeto a partir do arquivo INP especificado e retorna a instância <see cref="Project"/> inicializada.
        /// Este método cria uma nova instância de <see cref="Project"/>, invoca <see cref="Open()"/> para preparar
        /// o contexto nativo e valida que o arquivo de entrada existe. A leitura do conteúdo do arquivo não é
        /// realizada por enquanto; este método apenas prepara a instância para uso.
        /// </summary>
        /// <param name="inpFile">Caminho para o arquivo de entrada (.inp) do EPANET.</param>
        /// <returns>Instância de <see cref="Project"/> pronta para uso.</returns>
        /// <exception cref="ArgumentNullException">Se <paramref name="inpFile"/> for nulo.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Se o arquivo informado não existir.</exception>
        public static Project Open(string inpFile)
        {
            if (inpFile is null) throw new ArgumentNullException(nameof(inpFile));

            if (!System.IO.File.Exists(inpFile))
            {
                throw new System.IO.FileNotFoundException("Input file not found.", inpFile);
            }

            var project = new Project();
            project.OpenFile(inpFile);
            return project;
        }

        /// <summary>
        /// Abre (cria) o projeto nativo e carrega o arquivo INP especificado.
        /// </summary>
        /// <param name="inpFile">Caminho para o arquivo INP a ser aberto.</param>
        public void OpenFile(string inpFile)
        {
            EnsureNotDisposed();

            if (inpFile is null) throw new ArgumentNullException(nameof(inpFile));

            // Cria o projeto nativo se necessário e abre o arquivo INP.
            _nativeContext.OpenProject(inpFile);
            InputFilePath = inpFile;

            // Após abrir o INP via API nativa, recarrega os contadores da rede para refletir
            // o conteúdo do arquivo (nós, enlaces, etc.).
            try
            {
                Network.ReloadCounts();
            }
            catch
            {
                // Não propagar erros de reload aqui; a chamada nativa já pode ter sido concluída.
            }
        }

        /// <summary>
        /// Representa a rede hidráulica associada a este projeto.
        /// A propriedade é inicializada com uma instância vazia e pode ser populada
        /// quando os dados do projeto forem lidos.
        /// </summary>
        public Network Network { get; }

        /// <summary>
        /// Expose the native context for advanced operations (used by Reporting/Report).
        /// </summary>
        public Native.NativeContext NativeContext => _nativeContext;

        /// <summary>
        /// Acesso ao módulo de simulação hidráulica associado a este projeto.
        /// </summary>
        public Simulation.HydraulicSimulation Simulation
        {
            get
            {
                if (_simulation is null)
                {
                    _simulation = new Simulation.HydraulicSimulation(_nativeContext);
                }
                return _simulation;
            }
        }

        private Simulation.WaterQualitySimulation? _qualitySimulation;

        /// <summary>
        /// Acesso ao módulo de simulação de qualidade associado a este projeto.
        /// </summary>
        public Simulation.WaterQualitySimulation QualitySimulation
        {
            get
            {
                if (_qualitySimulation is null)
                {
                    _qualitySimulation = new Simulation.WaterQualitySimulation(_nativeContext);
                }
                return _qualitySimulation;
            }
        }

        /// <summary>
        /// Verifica se a biblioteca nativa EPANET está disponível para chamadas P/Invoke.
        /// Retorna <c>true</c> se a DLL nativa responder às chamadas previstas; caso contrário, <c>false</c>.
        /// </summary>
        public static bool IsNativeAvailable()
        {
            try
            {
                var api = new Native.NativeApi();
                return api.IsAvailable();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Caminho do arquivo INP associado a este projeto quando aberto via <see cref="Open(string)"/>.
        /// Pode ser nulo caso o projeto tenha sido criado sem associação a um arquivo.
        /// </summary>
        public string? InputFilePath { get; private set; }

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

                // Em futuras implementações, solicitar versão à camada nativa.
                return "EpanetSharp-0.1.0";
            }
        }

        /// <summary>
        /// Abre (cria) o projeto nativo.
        /// </summary>
        public void Open()
        {
            EnsureNotDisposed();

            if (!_nativeContext.IsProjectCreated)
            {
                _nativeContext.CreateProject();
            }
        }



        /// <summary>
        /// Fecha (destrói) o projeto nativo.
        /// </summary>
        public void Close()
        {
            EnsureNotDisposed();

            if (_nativeContext.IsProjectCreated)
            {
                _nativeContext.DestroyProject();
            }
        }

        /// <summary>
        /// Executa a simulação do projeto.
        /// Implementação atual apenas valida estado e delega verificação de erros à camada nativa.
        /// </summary>
        public void Run()
        {
            EnsureNotDisposed();

            if (!_nativeContext.IsProjectCreated)
            {
                throw new InvalidOperationException("Projeto não está aberto.");
            }

            // Em futura implementação, executar chamadas nativas que retornam um código.
            // Simular operação bem-sucedida passando código 0 para CheckError.
            _nativeContext.CheckError(0);
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
                // Dispose explícito: permitir que exceções fluam para o chamador.
                _nativeContext?.Dispose();
            }
            else
            {
                // Finalizador: suprimir exceções para não abortar o GC.
                try
                {
                    _nativeContext?.Dispose();
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
                throw new ObjectDisposedException(nameof(Project));
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
