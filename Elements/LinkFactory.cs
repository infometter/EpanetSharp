using System;

namespace EpanetSharp.Elements
{
    internal static class LinkFactory
    {
        private const int TYPE_PIPE = 0; // assumed mapping; adjust if different
        private const int TYPE_PUMP = 1; // assumed mapping
        private const int TYPE_PRV = 2;
        private const int TYPE_PSV = 3;
        private const int TYPE_PBV = 4;
        private const int TYPE_FCV = 5;
        private const int TYPE_TCV = 6;
        private const int TYPE_GPV = 7;

        public static Link Create(Native.NativeContext ctx, int index0, string id)
        {
            if (ctx is null) throw new ArgumentNullException(nameof(ctx));

            int nativeIndex = index0 + 1;
            int type = ctx.GetLinkType(nativeIndex);

            return type switch
            {
                TYPE_PIPE => new Pipe(ctx, index0, id),
                TYPE_PUMP => new Pump(ctx, index0, id),
                TYPE_PRV => new Valves.PRV(ctx, index0, id),
                TYPE_PSV => new Valves.PSV(ctx, index0, id),
                TYPE_PBV => new Valves.PBV(ctx, index0, id),
                TYPE_FCV => new Valves.FCV(ctx, index0, id),
                TYPE_TCV => new Valves.TCV(ctx, index0, id),
                TYPE_GPV => new Valves.GPV(ctx, index0, id),
                _ => new LinkPlaceholder(id) // fallback generic Link wrapper
            };
        }

        // simple placeholder for non-implemented link types
        private sealed class LinkPlaceholder : Link
        {
            public LinkPlaceholder(string id)
            {
                Id = id;
            }
        }
    }
}
