using System;
using System.IO;
using EpanetSharp.Core;

internal static partial class Program
{
    private static void TryLoadLocalEpantDll()
    {
        try
        {
            var baseDir = AppContext.BaseDirectory;
            var dir = baseDir;
            for (int i = 0; i < 6; i++)
            {
                var candidate64 = Path.Combine(dir, "runtimes", "win-x64", "native", "epanet2.dll");
                var candidate86 = Path.Combine(dir, "runtimes", "win-x86", "native", "epanet2.dll");
                if (File.Exists(candidate64))
                {
                    Console.WriteLine($"Attempting to load native DLL from: {candidate64}");
                    System.Runtime.InteropServices.NativeLibrary.TryLoad(candidate64, out IntPtr h);
                    return;
                }
                if (File.Exists(candidate86))
                {
                    Console.WriteLine($"Attempting to load native DLL from: {candidate86}");
                    System.Runtime.InteropServices.NativeLibrary.TryLoad(candidate86, out IntPtr h);
                    return;
                }
                var parent = Path.GetDirectoryName(dir);
                if (string.IsNullOrEmpty(parent)) break;
                dir = parent;
            }
        }
        catch { }
    }

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

            // Attempt to run native-backed open; if native isn't available, report and continue.
            // Try to load epanet2.dll from repository runtimes folders if present to avoid loader issues
            TryLoadLocalEpantDll();

            // First, attempt to call EN_getversion via NativeApi to verify the native DLL responds
            try
            {
                var asmPath = Path.Combine(AppContext.BaseDirectory, "EpanetSharp.dll");
                if (File.Exists(asmPath))
                {
                    var asm = System.Reflection.Assembly.LoadFrom(asmPath);
                    var apiType = asm.GetType("EpanetSharp.Native.NativeApi");
                    if (apiType != null)
                    {
                        var apiInstance = Activator.CreateInstance(apiType);
                        var getVersion = apiType.GetMethod("GetVersion", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                        if (getVersion != null)
                        {
                            Console.WriteLine("Calling native EN_getversion...");
                            var version = getVersion.Invoke(apiInstance, null) as string;
                            Console.WriteLine("EN_getversion returned: " + (version ?? "(null)"));
                        }
                        else
                        {
                            Console.WriteLine("NativeApi.GetVersion not found via reflection.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("EpanetSharp.Native.NativeApi type not found in assembly.");
                    }
                }
                else
                {
                    Console.WriteLine($"Assembly not found at {asmPath}; cannot call NativeApi.GetVersion.");
                }
            }
            catch (System.Reflection.TargetInvocationException tie)
            {
                Console.WriteLine("Native call failed: " + tie.InnerException?.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inspecting native API: " + ex.Message);
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

                Console.WriteLine("\nTentando ler IDs via GetLinkId:");
                for (int i = 1; i <= 6; i++)
                {
                    try
                    {
                        var id = p.NativeContext.GetLinkId(i);
                        Console.WriteLine($"Link {i}: '{id}'");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Link {i}: erro -> {ex.Message}");
                        break;
                    }
                }
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
