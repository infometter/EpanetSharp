using System;
using EpanetSharp.Core;

internal static class Program
{
    private static int Main()
    {
        string path = "rede.inp";
        try
        {
            // Ensure sample file exists
            if (!System.IO.File.Exists(path))
            {
                System.IO.File.WriteAllText(path, "[TITLE]\nRede de teste\n");
            }

            if (!Project.IsNativeAvailable())
            {
                Console.WriteLine("Native library not available; skipping native test.");
                return 2;
            }

            using (var p = Project.Open(path))
            {
                Console.WriteLine(p.Network.NodeCount);
                Console.WriteLine(p.Network.LinkCount);
                Console.WriteLine(p.Network.PatternCount);
            }

            return 0;
        }
        catch (DllNotFoundException)
        {
            Console.WriteLine("Native epanet2.dll not found. Test skipped.");
            return 2;
        }
        catch (EntryPointNotFoundException)
        {
            Console.WriteLine("Native entry point missing. Test skipped.");
            return 3;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex}");
            return 1;
        }
    }
}
