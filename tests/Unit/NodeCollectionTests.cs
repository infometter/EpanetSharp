using System;
using Xunit;
using EpanetSharp.Core;
using EpanetSharp.Collections;

namespace EpanetSharp.Tests.Unit
{
    public class NodeCollectionTests
    {
        [Fact]
        public void NativeDependentTests_RunOnlyWhenNativeAvailable()
        {
            if (!Project.IsNativeAvailable())
            {
                // Skip test when native library not available
                return;
            }

            using var ctx = new Native.NativeContext();
            ctx.CreateProject();
            ctx.OpenProject("tests/QuickDemo/rede.inp");

            var network = new Network(ctx);
            network.ReloadCounts();

            var nodes = network.Nodes;

            Assert.True(nodes.Count >= 0);

            if (nodes.Count > 0)
            {
                var first = nodes[0];
                Assert.NotNull(first);
                Assert.False(string.IsNullOrEmpty(first.Id));

                var byId = nodes[first.Id];
                Assert.NotNull(byId);
                Assert.Equal(first.Id, byId!.Id);

                Assert.True(nodes.Contains(first.Id));

                Assert.True(nodes.TryGet(first.Id, out var found));
                Assert.Equal(first.Id, found!.Id);

                int seen = 0;
                foreach (var n in nodes)
                {
                    Assert.NotNull(n.Id);
                    seen++;
                }
                Assert.Equal(nodes.Count, seen);
            }

            ctx.CloseProject();
        }
    }
}
