using System;
using EpanetSharp.Core;
using EpanetSharp.Exceptions;

namespace TestRunner
{
    internal static class Program
    {
        private static int Main()
        {
            try
            {
                TestOpenClose();
                TestCollectionsAdd();
                TestRunWithoutOpen();
                TestDisposeBehavior();
                TestOpenFromFile();

                Console.WriteLine("All tests passed.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Test failed: {ex}");
                return 1;
            }
        }

        private static void TestOpenFromFile()
        {
            var temp = System.IO.Path.ChangeExtension(System.IO.Path.GetTempFileName(), ".inp");
            try
            {
                System.IO.File.WriteAllText(temp, "[TITLE]\nTest INP\n");

                try
                {
                    using var project = Project.Open(temp);
                    if (project is null) throw new Exception("Project.Open returned null.");
                    if (!project.IsOpen) throw new Exception("Project should be open after Open(file).");
                    if (project.InputFilePath != temp) throw new Exception("InputFilePath not set correctly.");

                    // Reload counts from native layer and validate minimum expected values
                    project.Network.ReloadCounts();
                    if (project.Network.NodeCount < 2) throw new Exception($"Unexpected NodeCount: {project.Network.NodeCount}");
                    if (project.Network.LinkCount < 1) throw new Exception($"Unexpected LinkCount: {project.Network.LinkCount}");
                    if (project.Network.ReservoirCount < 1) throw new Exception($"Unexpected ReservoirCount: {project.Network.ReservoirCount}");
                    if (project.Network.TankCount < 1) throw new Exception($"Unexpected TankCount: {project.Network.TankCount}");
                    if (project.Network.PatternCount < 1) throw new Exception($"Unexpected PatternCount: {project.Network.PatternCount}");
                    if (project.Network.CurveCount < 1) throw new Exception($"Unexpected CurveCount: {project.Network.CurveCount}");
                }
                catch (DllNotFoundException)
                {
                    Console.WriteLine("Native epanet2.dll not found — skipping native integration test.");
                }
                catch (EntryPointNotFoundException)
                {
                    Console.WriteLine("Native entry point missing — skipping native integration test.");
                }
            }
            finally
            {
                try { System.IO.File.Delete(temp); } catch { }
            }
        }

        private static void TestOpenClose()
        {
            using var project = new Project();

            if (project.IsOpen)
                throw new Exception("Project should not be open initially.");

            try
            {
                project.Open();
                if (!project.IsOpen)
                    throw new Exception("Project should be open after Open().");

                project.Close();
                if (project.IsOpen)
                    throw new Exception("Project should not be open after Close().");
            }
            catch (DllNotFoundException)
            {
                Console.WriteLine("Native epanet2.dll not found — skipping native integration test.");
                return;
            }
            catch (EntryPointNotFoundException)
            {
                Console.WriteLine("Native entry point missing — skipping native integration test.");
                return;
            }
            // Verify Network and collections are initialized and empty
            if (project.Network is null)
                throw new Exception("Project.Network should not be null.");

            if (project.Network.Nodes.Count != 0)
                throw new Exception("Nodes should be empty initially.");

            if (project.Network.Links.Count != 0)
                throw new Exception("Links should be empty initially.");

            if (project.Network.Patterns.Count != 0)
                throw new Exception("Patterns should be empty initially.");

            if (project.Network.Curves.Count != 0)
                throw new Exception("Curves should be empty initially.");
        }

        private static void TestCollectionsAdd()
        {
            using var project = new Project();

            // Create simple concrete implementations for abstract models
            var node = new TestNode { Id = "N1", Index = 1 };
            var link = new TestLink { Id = "L1", Index = 2 };
            var pattern = new TestPattern { Id = "P1", Index = 3 };
            var curve = new TestCurve { Id = "C1", Index = 4 };

            project.Network.Nodes.Add(node);
            project.Network.Links.Add(link);
            project.Network.Patterns.Add(pattern);
            project.Network.Curves.Add(curve);

            if (project.Network.Nodes.Count != 1) throw new Exception("Node not added.");
            if (project.Network.Links.Count != 1) throw new Exception("Link not added.");
            if (project.Network.Patterns.Count != 1) throw new Exception("Pattern not added.");
            if (project.Network.Curves.Count != 1) throw new Exception("Curve not added.");

            if (!project.Network.Nodes.Contains("N1")) throw new Exception("Node Contains by id failed.");
            if (!project.Network.Links.Contains("L1")) throw new Exception("Link Contains by id failed.");
            if (!project.Network.Patterns.Contains("P1")) throw new Exception("Pattern Contains by id failed.");
            if (!project.Network.Curves.Contains("C1")) throw new Exception("Curve Contains by id failed.");

            if (!project.Network.Nodes.TryGet("N1", out var nn) || nn is null) throw new Exception("TryGet node failed.");

            // Local concrete test types
            void cleanup()
            {
                // nothing for now
            }

            cleanup();
        }

        // Concrete test implementations for abstract models
        private sealed class TestNode : EpanetSharp.Elements.Node { }
        private sealed class TestLink : EpanetSharp.Elements.Link { }
        private sealed class TestPattern : EpanetSharp.Elements.Pattern { }
        private sealed class TestCurve : EpanetSharp.Elements.Curve { }

        private static void TestRunWithoutOpen()
        {
            using var project = new Project();
            try
            {
                project.Run();
                throw new Exception("Run() should have thrown when project is not open.");
            }
            catch (InvalidOperationException)
            {
                // expected
            }
        }

        private static void TestDisposeBehavior()
        {
            var project = new Project();
            project.Dispose();

            try
            {
                project.Open();
                throw new Exception("Open after Dispose should throw.");
            }
            catch (ObjectDisposedException)
            {
                // expected
            }

            try
            {
                var v = project.Version;
                throw new Exception("Accessing Version after Dispose should throw.");
            }
            catch (ObjectDisposedException)
            {
                // expected
            }
        }
    }
}
