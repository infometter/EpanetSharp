using System;
using System.Collections.Generic;

namespace EpanetSharp.Units
{
    /// <summary>
    /// Converte valores hidráulicos entre unidades de usuário e SI (Sistema Internacional).
    /// Baseado nas fórmulas oficiais do EPANET/WNTR (wntr.epanet.util).
    /// </summary>
    public class UnitConverter
    {
        private readonly FlowUnit _flowUnit;
        private readonly HeadlossFormula _headlossFormula;

        // Fatores de conversão: valor_na_unidade × fator = valor_SI (m³/s)
        private static readonly Dictionary<FlowUnit, double> FlowToSIFactor = new Dictionary<FlowUnit, double>
        {
            { FlowUnit.CFS,  0.028316847 },     // ft³/s  → m³/s
            { FlowUnit.GPM,  6.30902e-5  },     // gal/min → m³/s
            { FlowUnit.MGD,  0.043812636 },     // Mgal/d  → m³/s
            { FlowUnit.IMGD, 0.052616783 },     // Mgal(imp)/d → m³/s
            { FlowUnit.AFD,  0.014276410 },     // acre-ft/d → m³/s
            { FlowUnit.LPS,  0.001        },    // L/s    → m³/s
            { FlowUnit.LPM,  1.6667e-5   },     // L/min  → m³/s
            { FlowUnit.MLD,  0.011574074 },     // ML/d   → m³/s
            { FlowUnit.CMH,  2.7778e-4   },     // m³/h   → m³/s
            { FlowUnit.SI,   1.0          }     // m³/s   (sem conversão)
        };

        // Fator para pressão/carga/elevação: valor × fator = metros
        private static readonly Dictionary<FlowUnit, double> LengthToSIFactor = new Dictionary<FlowUnit, double>
        {
            { FlowUnit.CFS,  0.3048 },
            { FlowUnit.GPM,  0.3048 },
            { FlowUnit.MGD,  0.3048 },
            { FlowUnit.IMGD, 0.3048 },
            { FlowUnit.AFD,  0.3048 },
            { FlowUnit.LPS,  1.0    },
            { FlowUnit.LPM,  1.0    },
            { FlowUnit.MLD,  1.0    },
            { FlowUnit.CMH,  1.0    },
            { FlowUnit.SI,   1.0    }
        };

        /// <summary>
        /// Cria um conversor de unidades para a unidade de vazão e fórmula informadas.
        /// </summary>
        /// <param name="flowUnit">Sistema de unidades de vazão.</param>
        /// <param name="headlossFormula">Fórmula de perda de carga (afeta conversão de rugosidade).</param>
        public UnitConverter(FlowUnit flowUnit, HeadlossFormula headlossFormula)
        {
            _flowUnit        = flowUnit;
            _headlossFormula = headlossFormula;
        }

        /// <summary>
        /// Converte um valor da unidade configurada para SI.
        /// </summary>
        public double ToSI(double value, HydraulicParameter parameter)
        {
            if (double.IsNaN(value) || double.IsInfinity(value)) return value;

            switch (parameter)
            {
                case HydraulicParameter.Flow:
                case HydraulicParameter.Demand:
                    return value * FlowToSIFactor[_flowUnit];

                case HydraulicParameter.Pressure:
                case HydraulicParameter.Head:
                case HydraulicParameter.Elevation:
                case HydraulicParameter.Length:
                    return value * LengthToSIFactor[_flowUnit];

                case HydraulicParameter.Diameter:
                    return DiameterToSI(value);

                case HydraulicParameter.Roughness:
                    return RoughnessToSI(value);

                case HydraulicParameter.Velocity:
                    return VelocityToSI(value);

                case HydraulicParameter.Volume:
                    return VolumeToSI(value);

                case HydraulicParameter.Power:
                    return PowerToSI(value);

                case HydraulicParameter.Quality:
                default:
                    return value; // Sem conversão
            }
        }

        /// <summary>
        /// Converte um valor de SI para a unidade configurada.
        /// </summary>
        public double FromSI(double value, HydraulicParameter parameter)
        {
            if (double.IsNaN(value) || double.IsInfinity(value)) return value;

            switch (parameter)
            {
                case HydraulicParameter.Flow:
                case HydraulicParameter.Demand:
                    return value / FlowToSIFactor[_flowUnit];

                case HydraulicParameter.Pressure:
                case HydraulicParameter.Head:
                case HydraulicParameter.Elevation:
                case HydraulicParameter.Length:
                    return value / LengthToSIFactor[_flowUnit];

                case HydraulicParameter.Diameter:
                    return DiameterFromSI(value);

                case HydraulicParameter.Roughness:
                    return RoughnessFromSI(value);

                case HydraulicParameter.Velocity:
                    return VelocityFromSI(value);

                case HydraulicParameter.Volume:
                    return VolumeFromSI(value);

                case HydraulicParameter.Power:
                    return PowerFromSI(value);

                case HydraulicParameter.Quality:
                default:
                    return value;
            }
        }

        // ── Conversões específicas ─────────────────────────────────────────────

        private double DiameterToSI(double value)
        {
            // Métrico: mm → m     Imperial: polegadas → m
            return _flowUnit.IsMetric() ? value / 1000.0 : value * 0.0254;
        }

        private double DiameterFromSI(double value)
        {
            return _flowUnit.IsMetric() ? value * 1000.0 : value / 0.0254;
        }

        private double VelocityToSI(double value)
        {
            // Métrico: m/s (sem conversão)   Imperial: ft/s → m/s
            return _flowUnit.IsMetric() ? value : value * 0.3048;
        }

        private double VelocityFromSI(double value)
        {
            return _flowUnit.IsMetric() ? value : value / 0.3048;
        }

        private double VolumeToSI(double value)
        {
            // Métrico: m³ (sem conversão)   Imperial: ft³ → m³
            return _flowUnit.IsMetric() ? value : value * 0.028316847;
        }

        private double VolumeFromSI(double value)
        {
            return _flowUnit.IsMetric() ? value : value / 0.028316847;
        }

        private double PowerToSI(double value)
        {
            // Métrico: kW → W    Imperial: hp → W
            return _flowUnit.IsMetric() ? value * 1000.0 : value * 745.699872;
        }

        private double PowerFromSI(double value)
        {
            return _flowUnit.IsMetric() ? value / 1000.0 : value / 745.699872;
        }

        /// <summary>
        /// Conversão de rugosidade — depende da fórmula de perda de carga.
        /// </summary>
        private double RoughnessToSI(double value)
        {
            switch (_headlossFormula)
            {
                case HeadlossFormula.HazenWilliams:
                    // H-W é adimensional — sem conversão
                    return value;

                case HeadlossFormula.DarcyWeisbach:
                    // Métrico: mm → m    Imperial: millinch (mil) → m
                    return _flowUnit.IsMetric() ? value / 1000.0 : value * 0.0254 / 1000.0;

                case HeadlossFormula.ChezyManning:
                    // Métrico: s/m^(1/3) (sem conversão)   Imperial: fator 1.486
                    return _flowUnit.IsMetric() ? value : value / 1.486;

                default:
                    return value;
            }
        }

        private double RoughnessFromSI(double value)
        {
            switch (_headlossFormula)
            {
                case HeadlossFormula.HazenWilliams:
                    return value;

                case HeadlossFormula.DarcyWeisbach:
                    return _flowUnit.IsMetric() ? value * 1000.0 : value / 0.0254 * 1000.0;

                case HeadlossFormula.ChezyManning:
                    return _flowUnit.IsMetric() ? value : value * 1.486;

                default:
                    return value;
            }
        }

        /// <summary>
        /// Retorna a unidade de vazão configurada.
        /// </summary>
        public FlowUnit FlowUnit => _flowUnit;

        /// <summary>
        /// Retorna a fórmula de perda de carga configurada.
        /// </summary>
        public HeadlossFormula HeadlossFormula => _headlossFormula;
    }
}
