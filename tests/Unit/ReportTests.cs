using System;
using Xunit;
using EpanetSharp.Core;
using EpanetSharp.Reporting;

namespace EpanetSharp.Tests.Unit
{
    public class ReportTests
    {
        [Fact]
        public void GenerateReport_SucceedsWithoutNative()
        {
            var proj = new Project();
            var report = new Report(proj);
            report.Generate();
            var summary = report.Summary();
            Assert.NotNull(summary);
            Assert.Contains("EPANET Report Generated", summary);
        }
    }
}
