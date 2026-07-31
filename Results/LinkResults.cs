using System.Collections.Generic;

namespace EpanetSharp.Results
{
    /// <summary>
    /// Status de um link hidráulico em um dado instante.
    /// </summary>
    public enum LinkStatus
    {
        /// <summary>Link fechado.</summary>
        Closed = 0,
        /// <summary>Link aberto (fluxo normal).</summary>
        Open = 1,
        /// <summary>Link ativo (controle de pressão/fluxo).</summary>
        Active = 2
    }

    /// <summary>
    /// Resultados de links (tubulações, bombas, válvulas) para todos os timesteps.
    /// As chaves dos dicionários são os IDs dos links.
    /// </summary>
    public class LinkResults
    {
        /// <summary>Vazão em cada link ao longo do tempo (m³/s em SI).</summary>
        public Dictionary<string, TimeSeries<double>> Flow { get; }

        /// <summary>Velocidade do escoamento em cada link (m/s em SI).</summary>
        public Dictionary<string, TimeSeries<double>> Velocity { get; }

        /// <summary>Perda de carga em cada link (m/km em SI).</summary>
        public Dictionary<string, TimeSeries<double>> Headloss { get; }

        /// <summary>Status do link (aberto/fechado/ativo) ao longo do tempo.</summary>
        public Dictionary<string, TimeSeries<LinkStatus>> Status { get; }

        /// <summary>Qualidade da água no link ao longo do tempo.</summary>
        public Dictionary<string, TimeSeries<double>> Quality { get; }

        internal LinkResults()
        {
            Flow     = new Dictionary<string, TimeSeries<double>>();
            Velocity = new Dictionary<string, TimeSeries<double>>();
            Headloss = new Dictionary<string, TimeSeries<double>>();
            Status   = new Dictionary<string, TimeSeries<LinkStatus>>();
            Quality  = new Dictionary<string, TimeSeries<double>>();
        }

        internal void EnsureLink(string id)
        {
            if (!Flow.ContainsKey(id))     Flow[id]     = new TimeSeries<double>();
            if (!Velocity.ContainsKey(id)) Velocity[id] = new TimeSeries<double>();
            if (!Headloss.ContainsKey(id)) Headloss[id] = new TimeSeries<double>();
            if (!Status.ContainsKey(id))   Status[id]   = new TimeSeries<LinkStatus>();
            if (!Quality.ContainsKey(id))  Quality[id]  = new TimeSeries<double>();
        }
    }
}
