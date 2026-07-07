using System;
using Xunit;
using EpanetSharp.Core;

namespace EpanetSharp.Tests
{
    public class HydraulicSimulationTests
    {
        [Fact]
        public void Simulation_OpenInitializeClose_WhenNativeUnavailable_Skipped()
        {
            if (!Project.IsNativeAvailable()) return;

            using var proj = new Project();
            proj.Open();
            var sim = proj.Simulation;
            sim.Open();
            sim.Initialize();
            // Attempt run/next should not throw (depends on network state)
            sim.Close();
        }
    }
}
