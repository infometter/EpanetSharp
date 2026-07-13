using System;
using System.IO;
using EpanetSharp.Core;
using EpanetSharp.Native;

class Program
{
    static int Main(string[] args)
    {
        if (args.Length == 0) { Console.WriteLine("uso: TempDiag <caminho-rede.inp>"); return 2; }
        var inp = Path.GetFullPath(args[0]);
        if (!File.Exists(inp)) { Console.WriteLine("INP não encontrado: " + inp); return 2; }

        var ctx = new NativeContext();
        try
        {
            ctx.OpenProject(inp);
            // EN_NODECOUNT=0, EN_LINKCOUNT=1 (from NativeConstants which is internal)
            Console.WriteLine("GetCount EN_NODECOUNT: " + ctx.GetCount(0));
            Console.WriteLine("GetCount EN_LINKCOUNT: " + ctx.GetCount(1));
            // Tente ler IDs nos índices 1..6
            for (int i = 1; i <= 6; i++)
            {
                try
                {
                    var id = ctx.GetLinkId(i);
                    Console.WriteLine($"Link index {i}: id='{id}'");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Link index {i}: erro -> {ex.Message}");
                }
            }
        }
        finally
        {
            try { ctx.CloseProject(); } catch { }
            try { ctx.DestroyProject(); } catch { }
        }
        return 0;
    }
}
