using System;
using System.IO;
using Xunit;
using EpanetSharp.Native;

namespace EpanetSharp.Tests.Integration
{
    public class NativeDiagnosticTests
    {
        private static string TestInpPath => Path.Combine("..", "TestNetworks", "Net1.inp");

        [Fact]
        public void Native_GetCount_ReturnsExpected_LinkCount()
        {
            var full = Path.GetFullPath(TestInpPath);
            Assert.True(File.Exists(full), $"INP not found: {full}");

            var ctx = new NativeContext();
            ctx.OpenProject(full);

            try
            {
                // Use literal count codes: EN_NODECOUNT=0, EN_LINKCOUNT=2
                int nodes = ctx.GetCount(0);
                int links = ctx.GetCount(2);

                // Expect 4 nodes (3 junctions + 1 reservoir) and 3 links (3 pipes)
                Assert.Equal(4, nodes);
                Assert.Equal(3, links);
            }
            finally
            {
                ctx.CloseProject();
                ctx.DestroyProject();
            }
        }
    }
}
