using System;

namespace EpanetSharp.Elements
{
    /// <summary>
    /// Representa as opções de simulação/configuração da rede EPANET.
    /// Implementação inicial com propriedades mínimas; será expandida conforme necessário.
    /// </summary>
    public sealed class Options
    {
        private readonly Native.NativeContext? _nativeContext;

        internal Options()
        {
        }

        internal Options(Native.NativeContext nativeContext)
        {
            _nativeContext = nativeContext ?? throw new ArgumentNullException(nameof(nativeContext));
        }

        /// <summary>
        /// Unidades de saída (EN_getoption/EN_setoption - UNITS).
        /// Valor numérico conforme a API nativa.
        /// </summary>
        public double Units
        {
            get => _nativeContext is null ? 0.0 : _nativeContext.GetOption(Native.NativeConstants.EN_OPTION_UNITS);
            set { if (_nativeContext is null) throw new InvalidOperationException("Native context not available."); _nativeContext.SetOption(Native.NativeConstants.EN_OPTION_UNITS, value); }
        }

        /// <summary>
        /// Fórmula de perda de carga (HeadLossFormula) (EN_HEADLOSS).
        /// </summary>
        public double HeadLossFormula
        {
            get => _nativeContext is null ? 0.0 : _nativeContext.GetOption(Native.NativeConstants.EN_OPTION_HEADLOSS);
            set { if (_nativeContext is null) throw new InvalidOperationException("Native context not available."); _nativeContext.SetOption(Native.NativeConstants.EN_OPTION_HEADLOSS, value); }
        }

        /// <summary>
        /// Multiplicador de demanda (EN_DEMANDMULT).
        /// </summary>
        public double DemandMultiplier
        {
            get => _nativeContext is null ? 1.0 : _nativeContext.GetOption(Native.NativeConstants.EN_OPTION_DEMANDMULT);
            set { if (_nativeContext is null) throw new InvalidOperationException("Native context not available."); _nativeContext.SetOption(Native.NativeConstants.EN_OPTION_DEMANDMULT, value); }
        }

        /// <summary>
        /// Viscosidade (EN_VISCOSITY).
        /// </summary>
        public double Viscosity
        {
            get => _nativeContext is null ? 0.0 : _nativeContext.GetOption(Native.NativeConstants.EN_OPTION_VISCOSITY);
            set { if (_nativeContext is null) throw new InvalidOperationException("Native context not available."); _nativeContext.SetOption(Native.NativeConstants.EN_OPTION_VISCOSITY, value); }
        }

        /// <summary>
        /// Gravidade específica (EN_SPECGRAV).
        /// </summary>
        public double SpecificGravity
        {
            get => _nativeContext is null ? 0.0 : _nativeContext.GetOption(Native.NativeConstants.EN_OPTION_SPECGRAV);
            set { if (_nativeContext is null) throw new InvalidOperationException("Native context not available."); _nativeContext.SetOption(Native.NativeConstants.EN_OPTION_SPECGRAV, value); }
        }

        /// <summary>
        /// Precisão da solução (EN_ACCURACY).
        /// </summary>
        public double Accuracy
        {
            get => _nativeContext is null ? 0.0 : _nativeContext.GetOption(Native.NativeConstants.EN_OPTION_ACCURACY);
            set { if (_nativeContext is null) throw new InvalidOperationException("Native context not available."); _nativeContext.SetOption(Native.NativeConstants.EN_OPTION_ACCURACY, value); }
        }

        /// <summary>
        /// Número máximo de tentativas (EN_TRIALS).
        /// </summary>
        public double Trials
        {
            get => _nativeContext is null ? 0.0 : _nativeContext.GetOption(Native.NativeConstants.EN_OPTION_TRIALS);
            set { if (_nativeContext is null) throw new InvalidOperationException("Native context not available."); _nativeContext.SetOption(Native.NativeConstants.EN_OPTION_TRIALS, value); }
        }

        /// <summary>
        /// Pattern associado ao cálculo (EN_PATTERN) - valor numérico representando índice/nome conforme API nativa.
        /// </summary>
        public double Pattern
        {
            get => _nativeContext is null ? 0.0 : _nativeContext.GetOption(Native.NativeConstants.EN_OPTION_PATTERN);
            set { if (_nativeContext is null) throw new InvalidOperationException("Native context not available."); _nativeContext.SetOption(Native.NativeConstants.EN_OPTION_PATTERN, value); }
        }

        /// <summary>
        /// Duração da simulação (EN_DURATION) em segundos.
        /// </summary>
        public double Duration
        {
            get => _nativeContext is null ? 0.0 : _nativeContext.GetOption(Native.NativeConstants.EN_OPTION_DURATION);
            set { if (_nativeContext is null) throw new InvalidOperationException("Native context not available."); _nativeContext.SetOption(Native.NativeConstants.EN_OPTION_DURATION, value); }
        }

        /// <summary>
        /// Time step de integração (EN_HYDSTEP) em segundos.
        /// </summary>
        public double TimeStep
        {
            get => _nativeContext is null ? 0.0 : _nativeContext.GetOption(Native.NativeConstants.EN_OPTION_HYDSTEP);
            set { if (_nativeContext is null) throw new InvalidOperationException("Native context not available."); _nativeContext.SetOption(Native.NativeConstants.EN_OPTION_HYDSTEP, value); }
        }
    }
}
