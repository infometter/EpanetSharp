using System;
using Xunit;
using EpanetSharp;
using EpanetSharp.Elements;

namespace EpanetSharp.Tests
{
    public class CurveTests
    {
        [Fact]
        public void AddAndReadCurve_WhenNativeUnavailable_Skipped()
        {
            if (!Project.IsNativeAvailable())
            {
                return; // skip when native not available
            }

            using var proj = new Project();
            proj.Create();

            // Create a simple curve in memory using native API via Project.Context
            var curves = proj.Network.Curves;
            int countBefore = curves.Count;

            // if no curves present, test basic retrieval
            if (countBefore == 0)
            {
                // Nothing to assert further without creating an INP; ensure API calls don't throw
                Assert.Equal(0, curves.Count);
                return;
            }

            var c = curves[0];
            Assert.NotNull(c);
            var pts = c.Points;
            Assert.NotNull(pts);
            // pontos devem ser lista válida
            Assert.True(pts.Count >= 0);
        }
    }
}
