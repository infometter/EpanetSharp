using System.Collections.Generic;

namespace EpanetSharp.Results
{
    /// <summary>
    /// Resultados de nós (junctions, reservatórios, tanques) para todos os timesteps.
    /// As chaves dos dicionários são os IDs dos nós.
    /// </summary>
    public class NodeResults
    {
        /// <summary>Pressão em cada nó ao longo do tempo (metros em SI).</summary>
        public Dictionary<string, TimeSeries<double>> Pressure { get; }

        /// <summary>Carga piezométrica em cada nó ao longo do tempo (metros em SI).</summary>
        public Dictionary<string, TimeSeries<double>> Head { get; }

        /// <summary>Demanda real em cada nó ao longo do tempo (m³/s em SI).</summary>
        public Dictionary<string, TimeSeries<double>> Demand { get; }

        /// <summary>Qualidade da água em cada nó ao longo do tempo (mg/L ou adimensional).</summary>
        public Dictionary<string, TimeSeries<double>> Quality { get; }

        internal NodeResults()
        {
            Pressure = new Dictionary<string, TimeSeries<double>>();
            Head     = new Dictionary<string, TimeSeries<double>>();
            Demand   = new Dictionary<string, TimeSeries<double>>();
            Quality  = new Dictionary<string, TimeSeries<double>>();
        }

        internal void EnsureNode(string id)
        {
            if (!Pressure.ContainsKey(id)) Pressure[id] = new TimeSeries<double>();
            if (!Head.ContainsKey(id))     Head[id]     = new TimeSeries<double>();
            if (!Demand.ContainsKey(id))   Demand[id]   = new TimeSeries<double>();
            if (!Quality.ContainsKey(id))  Quality[id]  = new TimeSeries<double>();
        }
    }
}
