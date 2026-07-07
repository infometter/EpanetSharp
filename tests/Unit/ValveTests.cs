using System;
using Xunit;

namespace EpanetSharp.Tests.Unit
{
    public class ValveTests
    {
        [Fact]
        public void ValveFactory_CreatesValves_WhenNativeAvailable()
        {
            if (!EpanetSharp.Core.Project.IsNativeAvailable()) return;

            using var ctx = new EpanetSharp.Native.NativeContext();
            ctx.CreateProject();
            ctx.OpenProject("tests/QuickDemo/rede.inp");

            var links = new EpanetSharp.Collections.LinkCollection(ctx);
            foreach (var l in links)
            {
                if (l is EpanetSharp.Elements.Valves.PRV prv)
                {
                    var s = prv.PressureSetting;
                    break;
                }
            }

            ctx.CloseProject();
        }
    }
}
