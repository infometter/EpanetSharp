using System;
using System.Collections.Generic;
using EpanetSharp.Elements;
using EpanetSharp.Collections;
using EpanetSharp.Native;

namespace EpanetSharp.Core
{
    /// <summary>
    /// Representa a rede hidráulica associada a um <see cref="Project"/>.
    /// A rede contém coleções de nós, enlaces, padrões, curvas e um conjunto de opções.
    /// Esta classe carrega contadores básicos da camada nativa quando o projeto está disponível.
    /// </summary>
    public sealed class Network
    {
        private readonly NodeCollection _nodes = new();
        private readonly LinkCollection _links = new();
        private readonly PatternCollection _patterns = new();
        private readonly CurveCollection _curves = new();
        private readonly NativeContext _nativeContext;
        private readonly ControlCollection _controls;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="Network"/> com coleções vazias e um contexto nativo.
        /// </summary>
        /// <param name="nativeContext">Contexto nativo associado ao projeto.</param>
        public Network(NativeContext nativeContext)
        {
            _nativeContext = nativeContext ?? throw new System.ArgumentNullException(nameof(nativeContext));
            Options = _nativeContext is null ? new Options() : new Options(_nativeContext);
            // initialize controls collection native-backed
            _controls = _nativeContext is null ? new ControlCollection() : new ControlCollection(_nativeContext);

            // Carrega automaticamente os contadores se o projeto nativo já estiver criado.
            if (_nativeContext.IsProjectCreated)
            {
                ReloadCounts();
            }
        }

        /// <summary>
        /// Coleção de nós (junctions, reservoirs, tanks) da rede.
        /// Inicialmente vazia.
        /// </summary>
        public NodeCollection Nodes => _nodes;

        /// <summary>
        /// Coleção de enlaces (pipes, pumps, valves) da rede.
        /// Inicialmente vazia.
        /// </summary>
        public LinkCollection Links => _links;

        /// <summary>
        /// Coleção de patterns de demanda utilizados na simulação.
        /// Inicialmente vazia.
        /// </summary>
        public PatternCollection Patterns => _patterns;

        /// <summary>
        /// Coleção de curvas (por exemplo curvas de bomba) associadas à rede.
        /// Inicialmente vazia.
        /// </summary>
        public CurveCollection Curves => _curves;

        /// <summary>
        /// Coleção de controles da rede.
        /// </summary>
        public ControlCollection Controls => _controls;

        /// <summary>
        /// Opções de simulação/configuração da rede.
        /// Instância inicializada vazia; os valores serão populados quando o projeto for lido.
        /// </summary>
        public Options Options { get; }

        /// <summary>
        /// Número de nós na rede conforme retornado por EN_getcount.
        /// </summary>
        public int NodeCount { get; private set; }

        /// <summary>
        /// Número de enlaces na rede conforme retornado por EN_getcount.
        /// </summary>
        public int LinkCount { get; private set; }

        /// <summary>
        /// Número de reservatórios na rede conforme retornado por EN_getcount.
        /// </summary>
        public int ReservoirCount { get; private set; }

        /// <summary>
        /// Número de tanques na rede conforme retornado por EN_getcount.
        /// </summary>
        public int TankCount { get; private set; }

        /// <summary>
        /// Número de patterns na rede conforme retornado por EN_getcount.
        /// </summary>
        public int PatternCount { get; private set; }

        /// <summary>
        /// Número de curvas na rede conforme retornado por EN_getcount.
        /// </summary>
        public int CurveCount { get; private set; }

        /// <summary>
        /// Número de controles na rede conforme retornado por EN_getcount.
        /// </summary>
        public int ControlCount { get; private set; }

        /// <summary>
        /// Recarrega os contadores consultando a camada nativa via EN_getcount.
        /// NOTA: EN_TANKCOUNT retorna a soma de tanks + reservoirs conforme a API do EPANET.
        /// Para obter contagens separadas seria necessário iterar pelos nós usando EN_getnodetype.
        /// </summary>
        public void ReloadCounts()
        {
            if (!_nativeContext.IsProjectCreated)
            {
                throw new System.InvalidOperationException("Native project is not created.");
            }

            NodeCount = _nativeContext.GetCount(NativeConstants.EN_NODECOUNT);
            LinkCount = _nativeContext.GetCount(NativeConstants.EN_LINKCOUNT);
            TankCount = _nativeContext.GetCount(NativeConstants.EN_TANKCOUNT);  // tanks + reservoirs
            // ReservoirCount não tem equivalente direto em EN_getcount; EN_TANKCOUNT inclui ambos.
            // Para obter apenas reservatórios, seria necessário iterar pelos nós e verificar EN_getnodetype.
            ReservoirCount = 0;  // TODO: implementar contagem via iteração se necessário
            PatternCount = _nativeContext.GetCount(NativeConstants.EN_PATTERNCOUNT);
            CurveCount = _nativeContext.GetCount(NativeConstants.EN_CURVECOUNT);
            ControlCount = _nativeContext.GetCount(NativeConstants.EN_CONTROLCOUNT);
        }
    }
}
