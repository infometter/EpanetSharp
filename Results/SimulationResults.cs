using System;
using System.Collections.Generic;

namespace EpanetSharp.Results
{
    /// <summary>
    /// Contém todos os resultados de uma simulação EPANET,
    /// organizados por tipo de elemento e parâmetro.
    /// Equivalente ao SimulationResults do WNTR.
    /// </summary>
    public class SimulationResults
    {
        /// <summary>Resultados dos nós (pressure, head, demand, quality).</summary>
        public NodeResults Nodes { get; }

        /// <summary>Resultados dos links (flow, velocity, headloss, status, quality).</summary>
        public LinkResults Links { get; }

        /// <summary>
        /// Timestamps da simulação, em segundos a partir do início.
        /// </summary>
        public IReadOnlyList<long> Timestamps { get; }

        /// <summary>Duração total da simulação em segundos.</summary>
        public long DurationSeconds { get; }

        /// <summary>Número de timesteps coletados.</summary>
        public int TimestepCount => Timestamps.Count;

        /// <summary>
        /// Retorna true se a simulação tem mais de um timestep (extended period).
        /// </summary>
        public bool IsExtendedPeriod => TimestepCount > 1;

        internal SimulationResults(NodeResults nodes, LinkResults links,
            List<long> timestamps, long durationSeconds)
        {
            Nodes           = nodes;
            Links           = links;
            Timestamps      = timestamps;
            DurationSeconds = durationSeconds;
        }

        /// <summary>
        /// Obtém a pressão em um nó em um timestamp específico (segundos), com interpolação.
        /// </summary>
        public double GetPressureAt(string nodeId, long timestampSeconds)
        {
            if (!Nodes.Pressure.TryGetValue(nodeId, out var series))
                throw new ArgumentException(string.Format("Nó '{0}' não encontrado nos resultados.", nodeId));
            return series.InterpolateAt(timestampSeconds);
        }

        /// <summary>
        /// Obtém a vazão em um link em um timestamp específico (segundos), com interpolação.
        /// </summary>
        public double GetFlowAt(string linkId, long timestampSeconds)
        {
            if (!Links.Flow.TryGetValue(linkId, out var series))
                throw new ArgumentException(string.Format("Link '{0}' não encontrado nos resultados.", linkId));
            return series.InterpolateAt(timestampSeconds);
        }
    }
}
