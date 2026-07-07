using System;
using Xunit;
using EpanetSharp.Core;

namespace EpanetSharp.Tests
{
    public class OptionsTests
    {
        [Fact]
        public void Options_Getters_DoNotThrow_WhenNativeUnavailable()
        {
            using var proj = new Project();
            // Project.Options exist even without native
            var opts = proj.Network.Options;
            Assert.NotNull(opts);

            // Access properties; when native not available should return defaults and not throw
            _ = opts.Units;
            _ = opts.HeadLossFormula;
            _ = opts.DemandMultiplier;
            _ = opts.Viscosity;
            _ = opts.SpecificGravity;
            _ = opts.Accuracy;
            _ = opts.Trials;
            _ = opts.Pattern;
            _ = opts.Duration;
            _ = opts.TimeStep;
        }
    }
}
