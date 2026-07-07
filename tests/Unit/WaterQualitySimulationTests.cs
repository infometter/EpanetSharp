using System;
using Xunit;
using EpanetSharp.Core;

namespace EpanetSharp.Tests
{
    public class WaterQualitySimulationTests
    {
        [Fact]
        public void QualitySimulation_OpenInitializeClose_WhenNativeUnavailable_Skipped()
        {
            if (!Project.IsNativeAvailable()) return;

            using var proj = new Project();
            proj.Open();
            var sim = proj.QualitySimulation;
            sim.Open();
            sim.Initialize();
            // Attempt run/next should not throw (depends on network state)
            sim.Close();
        }
    }
}
