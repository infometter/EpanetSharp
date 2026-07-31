using System;
using EpanetSharp.Native;
using EpanetSharp.Units;

namespace EpanetSharp.Simulation
{
    /// <summary>
    /// Opções de configuração de simulação EPANET, equivalente às opções do WNTR.
    /// Permite configurar a simulação via código sem editar o arquivo INP.
    /// </summary>
    public class SimulationOptions
    {
        // ── Tempo ─────────────────────────────────────────────────────────────

        /// <summary>Duração total da simulação. Zero = período único (steady-state).</summary>
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;

        /// <summary>Passo de tempo hidráulico.</summary>
        public TimeSpan HydraulicTimestep { get; set; } = TimeSpan.FromHours(1);

        /// <summary>Passo de tempo de qualidade da água.</summary>
        public TimeSpan QualityTimestep { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>Passo de tempo de padrões de demanda.</summary>
        public TimeSpan PatternTimestep { get; set; } = TimeSpan.FromHours(1);

        /// <summary>Passo de tempo dos relatórios de saída.</summary>
        public TimeSpan ReportTimestep { get; set; } = TimeSpan.FromHours(1);

        /// <summary>Tempo a partir do qual os resultados são reportados.</summary>
        public TimeSpan ReportStart { get; set; } = TimeSpan.Zero;

        // ── Hidráulica ────────────────────────────────────────────────────────

        /// <summary>Unidades de vazão.</summary>
        public FlowUnit FlowUnits { get; set; } = FlowUnit.SI;

        /// <summary>Fórmula de perda de carga.</summary>
        public HeadlossFormula HeadlossFormula { get; set; } = HeadlossFormula.HazenWilliams;

        /// <summary>Gravidade específica em relação à água (padrão = 1.0).</summary>
        public double SpecificGravity { get; set; } = 1.0;

        /// <summary>Viscosidade cinemática relativa à da água (padrão = 1.0).</summary>
        public double Viscosity { get; set; } = 1.0;

        /// <summary>Número máximo de iterações hidráulicas.</summary>
        public int MaxTrials { get; set; } = 200;

        /// <summary>Precisão de convergência hidráulica.</summary>
        public double Accuracy { get; set; } = 0.001;

        /// <summary>Multiplicador de demanda global.</summary>
        public double DemandMultiplier { get; set; } = 1.0;

        // ── Qualidade da Água ─────────────────────────────────────────────────

        /// <summary>Tipo de análise de qualidade.</summary>
        public QualityAnalysisType QualityType { get; set; } = QualityAnalysisType.None;

        /// <summary>Nome do componente químico (usado se QualityType = Chemical).</summary>
        public string ChemicalName { get; set; } = string.Empty;

        /// <summary>Unidades do componente químico (ex: mg/L).</summary>
        public string ChemicalUnits { get; set; } = "mg/L";

        // ── Pressure Dependent Demand ─────────────────────────────────────────

        /// <summary>Modelo de demanda (DDA ou PDD).</summary>
        public DemandModelType DemandModel { get; set; } = DemandModelType.DDA;

        /// <summary>Pressão mínima para demanda (usado em PDD). Em metros (SI).</summary>
        public double MinimumPressure { get; set; } = 0.0;

        /// <summary>Pressão requerida para demanda total (usado em PDD). Em metros (SI).</summary>
        public double RequiredPressure { get; set; } = 0.1;

        /// <summary>Expoente pressão-demanda (usado em PDD, padrão = 0.5).</summary>
        public double PressureExponent { get; set; } = 0.5;

        // ── Aplicação ─────────────────────────────────────────────────────────

        /// <summary>
        /// Aplica estas opções ao contexto nativo EPANET.
        /// </summary>
        public void ApplyTo(NativeContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            // Duração e timesteps
            context.SetOption(NativeConstants.EN_OPTION_DURATION,  Duration.TotalSeconds);
            context.SetOption(NativeConstants.EN_OPTION_HYDSTEP,   HydraulicTimestep.TotalSeconds);

            // Opções hidráulicas
            context.SetOption(NativeConstants.EN_OPTION_VISCOSITY,   Viscosity);
            context.SetOption(NativeConstants.EN_OPTION_SPECGRAV,    SpecificGravity);
            context.SetOption(NativeConstants.EN_OPTION_ACCURACY,    Accuracy);
            context.SetOption(NativeConstants.EN_OPTION_TRIALS,      MaxTrials);
            context.SetOption(NativeConstants.EN_OPTION_DEMANDMULT,  DemandMultiplier);
        }

        /// <summary>
        /// Lê as opções atuais do contexto nativo e retorna uma instância populada.
        /// </summary>
        public static SimulationOptions FromContext(NativeContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            return new SimulationOptions
            {
                Duration          = TimeSpan.FromSeconds(context.GetOption(NativeConstants.EN_OPTION_DURATION)),
                HydraulicTimestep = TimeSpan.FromSeconds(context.GetOption(NativeConstants.EN_OPTION_HYDSTEP)),
                Viscosity         = context.GetOption(NativeConstants.EN_OPTION_VISCOSITY),
                SpecificGravity   = context.GetOption(NativeConstants.EN_OPTION_SPECGRAV),
                Accuracy          = context.GetOption(NativeConstants.EN_OPTION_ACCURACY),
                MaxTrials         = (int)context.GetOption(NativeConstants.EN_OPTION_TRIALS),
                DemandMultiplier  = context.GetOption(NativeConstants.EN_OPTION_DEMANDMULT)
            };
        }
    }

    /// <summary>Tipo de análise de qualidade da água.</summary>
    public enum QualityAnalysisType
    {
        /// <summary>Sem análise de qualidade.</summary>
        None,
        /// <summary>Análise de componente químico (cloro, fluoreto, etc.).</summary>
        Chemical,
        /// <summary>Análise de idade da água.</summary>
        Age,
        /// <summary>Rastreamento de fonte (trace).</summary>
        Trace
    }

    /// <summary>Modelo de demanda.</summary>
    public enum DemandModelType
    {
        /// <summary>Demand Driven Analysis — demanda fixa, independente da pressão.</summary>
        DDA,
        /// <summary>Pressure Driven Analysis — demanda varia com a pressão disponível.</summary>
        PDD
    }
}
