using System;

namespace EpanetSharp.Elements.Valves
{
    /// <summary>
    /// Pressure Reducing Valve (PRV).
    /// </summary>
    public sealed class PRV : Valve
    {
        internal PRV(Native.NativeContext ctx, int index0, string id) : base(ctx, index0, id) { }

        /// <summary>
        /// Pressure setting for PRV.
        /// </summary>
        public double PressureSetting
        {
            get => Setting;
            set => Setting = value;
        }
    }
}
