using System;
using Xunit;
using EpanetSharp.Core;

namespace EpanetSharp.Tests.Unit
{
    public class LinkCollectionTests
    {
        [Fact]
        public void Native_LinkCollection_ReturnsPipes_WhenNativeAvailable()
        {
            if (!Project.IsNativeAvailable()) return;

            using var ctx = new Native.NativeContext();
            ctx.CreateProject();
            ctx.OpenProject("tests/QuickDemo/rede.inp");

            var net = new Network(ctx);
            net.ReloadCounts();

            var links = new EpanetSharp.Collections.LinkCollection(ctx);
            // iterate to force creation
            foreach (var l in links)
            {
                if (l is EpanetSharp.Elements.Pipe p)
                {
                    // access properties to ensure native access
                    var _ = p.Length;
                    break;
                }
            }

            ctx.CloseProject();
        }
    }
}
