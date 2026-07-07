using System;

namespace EpanetSharp.Elements
{
    internal static class NodeFactory
    {
        // assumed native type codes (adjust if EPANET uses different values)
        private const int TYPE_JUNCTION = 0;
        private const int TYPE_RESERVOIR = 1;
        private const int TYPE_TANK = 2;

        public static Node Create(Native.NativeContext ctx, int index0, string id)
        {
            if (ctx is null) throw new ArgumentNullException(nameof(ctx));

            int nativeIndex = index0 + 1;
            int type = ctx.GetNodeType(nativeIndex);

            return type switch
            {
                TYPE_JUNCTION => new Junction(ctx, index0, id),
                TYPE_RESERVOIR => new Reservoir(ctx, index0, id),
                TYPE_TANK => new Tank(ctx, index0, id),
                _ => new NativeNode(ctx, index0, id),
            };
        }
    }
}
