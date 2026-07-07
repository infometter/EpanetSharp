using System;

namespace EpanetSharp.Elements.Valves
{
    /// <summary>
    /// Pressure Breaker Valve (PBV) placeholder.
    /// </summary>
    public sealed class PBV : Valve
    {
        internal PBV(Native.NativeContext ctx, int index0, string id) : base(ctx, index0, id) { }

        /// <summary>
        /// Pressure setting for PBV.
        /// </summary>
        public double PressureSetting
        {
            get => Setting;
            set => Setting = value;
        }
    }
}
