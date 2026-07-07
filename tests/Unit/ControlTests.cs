using System;
using Xunit;
using EpanetSharp.Core;

namespace EpanetSharp.Tests.Unit
{
    public class ControlTests
    {
        [Fact]
        public void EnumerateControls_SkipsIfNativeMissing()
        {
            var proj = new Project();
            if (!proj.IsNativeAvailable())
            {
                // apenas garante que a chamada não lança quando não há native
                Assert.Empty(proj.Network.Controls);
                return;
            }

            // se native disponível, apenas exercita a enumeração sem falhar
            var ctrls = proj.Network.Controls;
            foreach (var c in ctrls)
            {
                Assert.NotNull(c.Id);
            }
        }
    }
}
