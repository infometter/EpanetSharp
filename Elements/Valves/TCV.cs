using System;

namespace EpanetSharp.Elements.Valves
{
    /// <summary>
    /// Throttle Control Valve (TCV) placeholder.
    /// </summary>
    public sealed class TCV : Valve
    {
        internal TCV(Native.NativeContext ctx, int index0, string id) : base(ctx, index0, id) { }

        /// <summary>
        /// Throttle setting for TCV.
        /// </summary>
        public double ThrottleSetting
        {
            get => Setting;
            set => Setting = value;
        }
    }
}
