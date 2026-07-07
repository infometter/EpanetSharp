using System;
using Xunit;

namespace EpanetSharp.Tests.Unit
{
    public class PumpTests
    {
        [Fact]
        public void PumpProperties_Accessible_WhenNativeAvailable()
        {
            if (!EpanetSharp.Core.Project.IsNativeAvailable()) return;

            using var ctx = new EpanetSharp.Native.NativeContext();
            ctx.CreateProject();
            ctx.OpenProject("tests/QuickDemo/rede.inp");

            var net = new EpanetSharp.Core.Network(ctx);
            net.ReloadCounts();

            var links = new EpanetSharp.Collections.LinkCollection(ctx);
            foreach (var l in links)
            {
                if (l is EpanetSharp.Elements.Pump p)
                {
                    var _ = p.Power;
                    var __ = p.Speed;
                    break;
                }
            }

            ctx.CloseProject();
        }
    }
}
