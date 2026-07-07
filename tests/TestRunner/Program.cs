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
                TestRunWithoutOpen();
                TestDisposeBehavior();

                Console.WriteLine("All tests passed.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Test failed: {ex}");
                return 1;
            }
        }

        private static void TestOpenClose()
        {
            using var project = new Project();

            if (project.IsOpen)
                throw new Exception("Project should not be open initially.");

            project.Open();
            if (!project.IsOpen)
                throw new Exception("Project should be open after Open().");

            project.Close();
            if (project.IsOpen)
                throw new Exception("Project should not be open after Close().");
        }

        private static void TestRunWithoutOpen()
        {
            using var project = new Project();
            try
            {
                project.Run();
                throw new Exception("Run() should have thrown when project is not open.");
            }
            catch (EpanetException)
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
            catch (EpanetException)
            {
                // expected
            }

            try
            {
                var v = project.Version;
                throw new Exception("Accessing Version after Dispose should throw.");
            }
            catch (EpanetException)
            {
                // expected
            }
        }
    }
}
