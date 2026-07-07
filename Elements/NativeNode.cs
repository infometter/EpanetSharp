using System;
using EpanetSharp.Native;

namespace EpanetSharp.Elements
{
    /// <summary>
    /// Implementação de nó que delega leitura/escrita de propriedades à API nativa do EPANET.
    /// Propriedades numéricas são lidas via EN_getnodevalue e gravadas via EN_setnodevalue.
    /// O índice internal é zero-based; as chamadas nativas usam 1-based index.
    /// </summary>
    public class NativeNode : Node
    {
        private readonly NativeContext _nativeContext;
        private readonly int _index0; // zero-based index

        // cache simples para propriedades numéricas; null = não carregado
        private double? _elevation;
        private double? _baseDemand;
        private double? _pressure;
        private double? _head;
        private double? _emitter;
        private double? _quality;
        private double? _sourceQuality;
        private double? _pattern; // store pattern as numeric index

        /// <summary>
        /// Cria uma nova instância de <see cref="NativeNode"/> ligada ao contexto nativo.
        /// </summary>
        /// <param name="nativeContext">Contexto nativo que contém o projeto aberto.</param>
        /// <param name="index0">Índice zero-based do nó.</param>
        /// <param name="id">Identificador do nó.</param>
        public NativeNode(NativeContext nativeContext, int index0, string id)
        {
            _nativeContext = nativeContext ?? throw new ArgumentNullException(nameof(nativeContext));
            _index0 = index0;
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Index = index0;
        }

        private int NativeIndex => _index0 + 1; // API nativa espera 1-based index

        /// <summary>
        /// Invalida o cache local de propriedades.
        /// </summary>
        public void InvalidateCache()
        {
            _elevation = null;
            _baseDemand = null;
            _pressure = null;
            _head = null;
            _emitter = null;
            _quality = null;
            _sourceQuality = null;
            _pattern = null;
        }

        public double Elevation
        {
            get => _elevation ??= _nativeContext.GetNodeValue(NativeIndex, NativeConstants.EN_ELEVATION);
            set
            {
                _nativeContext.SetNodeValue(NativeIndex, NativeConstants.EN_ELEVATION, value);
                _elevation = value;
            }
        }

        public double BaseDemand
        {
            get => _baseDemand ??= _nativeContext.GetNodeValue(NativeIndex, NativeConstants.EN_BASEDEMAND);
            set
            {
                _nativeContext.SetNodeValue(NativeIndex, NativeConstants.EN_BASEDEMAND, value);
                _baseDemand = value;
            }
        }

        public double Pressure
        {
            get => _pressure ??= _nativeContext.GetNodeValue(NativeIndex, NativeConstants.EN_PRESSURE);
            set
            {
                _nativeContext.SetNodeValue(NativeIndex, NativeConstants.EN_PRESSURE, value);
                _pressure = value;
            }
        }

        public double Head
        {
            get => _head ??= _nativeContext.GetNodeValue(NativeIndex, NativeConstants.EN_HEAD);
            set
            {
                _nativeContext.SetNodeValue(NativeIndex, NativeConstants.EN_HEAD, value);
                _head = value;
            }
        }

        public double Emitter
        {
            get => _emitter ??= _nativeContext.GetNodeValue(NativeIndex, NativeConstants.EN_EMITTER);
            set
            {
                _nativeContext.SetNodeValue(NativeIndex, NativeConstants.EN_EMITTER, value);
                _emitter = value;
            }
        }

        public double Quality
        {
            get => _quality ??= _nativeContext.GetNodeValue(NativeIndex, NativeConstants.EN_QUALITY);
            set
            {
                _nativeContext.SetNodeValue(NativeIndex, NativeConstants.EN_QUALITY, value);
                _quality = value;
            }
        }

        public double SourceQuality
        {
            get => _sourceQuality ??= _nativeContext.GetNodeValue(NativeIndex, NativeConstants.EN_SOURCE_QUAL);
            set
            {
                _nativeContext.SetNodeValue(NativeIndex, NativeConstants.EN_SOURCE_QUAL, value);
                _sourceQuality = value;
            }
        }

        /// <summary>
        /// Pattern associado ao nó (representado por um índice numérico).
        /// </summary>
        public double Pattern
        {
            get => _pattern ??= _nativeContext.GetNodeValue(NativeIndex, NativeConstants.EN_PATTERN);
            set
            {
                _nativeContext.SetNodeValue(NativeIndex, NativeConstants.EN_PATTERN, value);
                _pattern = value;
            }
        }

        /// <summary>
        /// Comentário associado ao elemento. Atualmente armazenado apenas no lado gerenciado.
        /// Futuramente pode ser sincronizado com a API nativa se funções apropriadas existirem.
        /// </summary>
        public new string? Comment
        {
            get => base.Comment;
            set => base.Comment = value;
        }

        /// <summary>
        /// Tag do elemento. Atualmente armazenada apenas no lado gerenciado.
        /// </summary>
        public new string? Tag
        {
            get => base.Tag;
            set => base.Tag = value;
        }
    }
}
