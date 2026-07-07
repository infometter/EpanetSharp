namespace EpanetSharp.Elements
{
    /// <summary>
    /// Representa um ponto (X,Y) em uma curva.
    /// </summary>
    public sealed class CurvePoint
    {
        public CurvePoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Coordenada X do ponto.
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Coordenada Y do ponto.
        /// </summary>
        public double Y { get; }
    }
}
