using System;

namespace EpanetSharp.Elements.Valves
{
    /// <summary>
    /// Flow Control Valve (FCV).
    /// </summary>
    public sealed class FCV : Valve
    {
        internal FCV(Native.NativeContext ctx, int index0, string id) : base(ctx, index0, id) { }

        /// <summary>
        /// Flow setting for FCV.
        /// </summary>
        public double FlowSetting
        {
            get => Setting;
            set => Setting = value;
        }
    }
}
