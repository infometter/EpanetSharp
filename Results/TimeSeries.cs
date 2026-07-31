using System;
using System.Collections.Generic;

namespace EpanetSharp.Results
{
    /// <summary>
    /// Série temporal de valores do tipo <typeparamref name="T"/> associados a timestamps.
    /// Permite acesso sequencial e análise estatística dos resultados de simulação.
    /// </summary>
    /// <typeparam name="T">Tipo do valor armazenado (double, LinkStatus, etc.).</typeparam>
    public class TimeSeries<T>
    {
        private readonly List<long>  _timestamps; // segundos desde o início
        private readonly List<T>     _values;

        internal TimeSeries()
        {
            _timestamps = new List<long>();
            _values     = new List<T>();
        }

        internal TimeSeries(int capacity)
        {
            _timestamps = new List<long>(capacity);
            _values     = new List<T>(capacity);
        }

        /// <summary>Timestamps em segundos a partir do início da simulação.</summary>
        public IReadOnlyList<long> Timestamps => _timestamps;

        /// <summary>Valores na série temporal.</summary>
        public IReadOnlyList<T> Values => _values;

        /// <summary>Número de pontos na série.</summary>
        public int Count => _values.Count;

        internal void Add(long timestampSeconds, T value)
        {
            _timestamps.Add(timestampSeconds);
            _values.Add(value);
        }

        /// <summary>
        /// Retorna o valor no índice especificado.
        /// </summary>
        public T GetValueAt(int index)
        {
            if (index < 0 || index >= _values.Count)
                throw new ArgumentOutOfRangeException("index");
            return _values[index];
        }

        /// <summary>
        /// Retorna o último valor da série (instante final da simulação).
        /// </summary>
        public T LastValue()
        {
            if (_values.Count == 0) throw new InvalidOperationException("Série temporal vazia.");
            return _values[_values.Count - 1];
        }

        /// <summary>
        /// Retorna o primeiro valor da série (instante inicial da simulação).
        /// </summary>
        public T FirstValue()
        {
            if (_values.Count == 0) throw new InvalidOperationException("Série temporal vazia.");
            return _values[0];
        }
    }

    /// <summary>
    /// Extensões de análise estatística para <see cref="TimeSeries{T}"/> de double.
    /// </summary>
    public static class TimeSeriesDoubleExtensions
    {
        /// <summary>Valor mínimo da série.</summary>
        public static double Min(this TimeSeries<double> series)
        {
            if (series.Count == 0) throw new InvalidOperationException("Série temporal vazia.");
            double min = double.MaxValue;
            for (int i = 0; i < series.Count; i++)
                if (series.Values[i] < min) min = series.Values[i];
            return min;
        }

        /// <summary>Valor máximo da série.</summary>
        public static double Max(this TimeSeries<double> series)
        {
            if (series.Count == 0) throw new InvalidOperationException("Série temporal vazia.");
            double max = double.MinValue;
            for (int i = 0; i < series.Count; i++)
                if (series.Values[i] > max) max = series.Values[i];
            return max;
        }

        /// <summary>Média aritmética da série.</summary>
        public static double Average(this TimeSeries<double> series)
        {
            if (series.Count == 0) throw new InvalidOperationException("Série temporal vazia.");
            double sum = 0;
            for (int i = 0; i < series.Count; i++) sum += series.Values[i];
            return sum / series.Count;
        }

        /// <summary>
        /// Interpola linearmente o valor em um timestamp (segundos) entre dois pontos da série.
        /// Retorna o valor mais próximo se o timestamp estiver fora dos limites.
        /// </summary>
        public static double InterpolateAt(this TimeSeries<double> series, long timestampSeconds)
        {
            if (series.Count == 0) throw new InvalidOperationException("Série temporal vazia.");
            if (series.Count == 1) return series.Values[0];

            // Busca binária pelo timestamp
            var timestamps = series.Timestamps;
            int lo = 0, hi = timestamps.Count - 1;

            if (timestampSeconds <= timestamps[lo]) return series.Values[lo];
            if (timestampSeconds >= timestamps[hi]) return series.Values[hi];

            while (lo < hi - 1)
            {
                int mid = (lo + hi) / 2;
                if (timestamps[mid] <= timestampSeconds) lo = mid;
                else hi = mid;
            }

            double t1 = timestamps[lo], t2 = timestamps[hi];
            double v1 = series.Values[lo],  v2 = series.Values[hi];
            double fraction = (timestampSeconds - t1) / (t2 - t1);
            return v1 + fraction * (v2 - v1);
        }
    }
}
