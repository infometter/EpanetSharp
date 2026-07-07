using System;

namespace EpanetSharp.Elements.Valves
{
    /// <summary>
    /// General Purpose Valve (GPV) placeholder for vendor-specific valve types.
    /// </summary>
    public sealed class GPV : Valve
    {
        internal GPV(Native.NativeContext ctx, int index0, string id) : base(ctx, index0, id) { }

        /// <summary>
        /// Generic setting for GPV.
        /// </summary>
        public double SettingValue
        {
            get => Setting;
            set => Setting = value;
        }
    }
}
