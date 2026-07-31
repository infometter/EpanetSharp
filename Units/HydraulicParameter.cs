namespace EpanetSharp.Units
{
    /// <summary>
    /// Parâmetros hidráulicos que podem ser convertidos entre sistemas de unidades.
    /// Usado em conjunto com <see cref="UnitConverter"/>.
    /// </summary>
    public enum HydraulicParameter
    {
        /// <summary>Vazão (m³/s em SI)</summary>
        Flow,
        /// <summary>Pressão / Carga (m em SI)</summary>
        Pressure,
        /// <summary>Carga piezométrica (m em SI)</summary>
        Head,
        /// <summary>Diâmetro (m em SI)</summary>
        Diameter,
        /// <summary>Comprimento (m em SI)</summary>
        Length,
        /// <summary>Coeficiente de rugosidade (depende da fórmula)</summary>
        Roughness,
        /// <summary>Elevação (m em SI)</summary>
        Elevation,
        /// <summary>Velocidade (m/s em SI)</summary>
        Velocity,
        /// <summary>Volume de tanques (m³ em SI)</summary>
        Volume,
        /// <summary>Demanda (m³/s em SI — mesma conversão que Flow)</summary>
        Demand,
        /// <summary>Potência de bombas (W em SI)</summary>
        Power,
        /// <summary>Qualidade da água — adimensional, sem conversão</summary>
        Quality
    }
}
