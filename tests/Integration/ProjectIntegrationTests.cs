using System;
using System.IO;
using Xunit;
using EpanetSharp.Core;

namespace EpanetSharp.Tests.Integration
{
    public class ProjectIntegrationTests
    {
        private static string TestInpPath => Path.Combine("..", "TestNetworks", "Net1.inp");

        [Fact]
        public void Open_Close_ReadVersion_And_Counts()
        {
            var full = Path.GetFullPath(TestInpPath);
            Assert.True(File.Exists(full), $"INP not found: {full}");

            using var proj = Project.Open(full);

            // Version should be non-empty
            Assert.False(string.IsNullOrWhiteSpace(proj.Version));

            // Ensure native available
            if (!Project.IsNativeAvailable())
            {
                // Skip logically when native not available in environment
                return;
            }

            // Network counts should be > 0 for this sample
            Assert.True(proj.Network.NodeCount > 0, "NodeCount should be > 0");
            Assert.True(proj.Network.LinkCount > 0, "LinkCount should be > 0");

            proj.Close();
        }
    }
}
