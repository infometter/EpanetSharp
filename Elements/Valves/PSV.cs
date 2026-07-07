using System;

namespace EpanetSharp.Elements.Valves
{
    /// <summary>
    /// Pressure Sustaining Valve (PSV).
    /// </summary>
    public sealed class PSV : Valve
    {
        internal PSV(Native.NativeContext ctx, int index0, string id) : base(ctx, index0, id) { }

        /// <summary>
        /// Pressure setting for PSV.
        /// </summary>
        public double PressureSetting
        {
            get => Setting;
            set => Setting = value;
        }
    }
}
