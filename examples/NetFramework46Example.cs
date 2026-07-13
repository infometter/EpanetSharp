using System;
using EpanetSharp.Core;

namespace ExemploFramework46
{
    /// <summary>
    /// Exemplo mínimo de uso do EpanetSharp em .NET Framework 4.6+
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Teste EpanetSharp no .NET Framework 4.6 ===");
            Console.WriteLine();

            // 1. Verificar se a biblioteca nativa está disponível
            Console.WriteLine("1. Verificando disponibilidade da biblioteca nativa...");
            if (!Project.IsNativeAvailable())
            {
                Console.WriteLine("   ❌ ERRO: EPANET nativo não disponível!");
                Console.WriteLine("   Certifique-se que epanet2.dll está no mesmo diretório.");
                Console.WriteLine();
                Console.WriteLine("Pressione qualquer tecla para sair...");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("   ✅ Biblioteca nativa encontrada!");
            Console.WriteLine();

            // 2. Definir caminho do arquivo .inp
            string inpPath = args.Length > 0 ? args[0] : "rede.inp";

            if (!System.IO.File.Exists(inpPath))
            {
                Console.WriteLine($"   ❌ ERRO: Arquivo '{inpPath}' não encontrado!");
                Console.WriteLine("   Uso: ExemploFramework46.exe <caminho-para-arquivo.inp>");
                Console.WriteLine();
                Console.WriteLine("Pressione qualquer tecla para sair...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"2. Abrindo arquivo: {inpPath}");
            Console.WriteLine();

            try
            {
                // 3. Abrir o projeto EPANET
                using (var project = Project.Open(inpPath))
                {
                    Console.WriteLine("3. Projeto aberto com sucesso!");
                    Console.WriteLine();

                    // 4. Exibir informações da rede
                    Console.WriteLine("=== Informações da Rede ===");
                    Console.WriteLine($"   Nós (Nodes):       {project.Network.NodeCount}");
                    Console.WriteLine($"   Links:             {project.Network.LinkCount}");
                    Console.WriteLine($"   Tanques:           {project.Network.TankCount}");
                    Console.WriteLine($"   Patterns:          {project.Network.PatternCount}");
                    Console.WriteLine($"   Curvas:            {project.Network.CurveCount}");
                    Console.WriteLine($"   Controles:         {project.Network.ControlCount}");
                    Console.WriteLine();

                    // 5. Listar IDs dos links (opcional)
                    Console.WriteLine("=== Links da Rede ===");
                    try
                    {
                        for (int i = 1; i <= project.Network.LinkCount; i++)
                        {
                            var linkId = project.NativeContext.GetLinkId(i);
                            var linkType = project.NativeContext.GetLinkType(i);
                            Console.WriteLine($"   Link {i}: '{linkId}' (Tipo: {linkType})");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"   ⚠️ Erro ao listar links: {ex.Message}");
                    }
                    Console.WriteLine();

                    // 6. Executar simulação hidráulica
                    Console.WriteLine("4. Executando  simulação hidráulica...");
                    try
                    {
                        project.Run();
                        Console.WriteLine("   ✅ Simulação concluída com sucesso!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"   ❌ Erro na simulação: {ex.Message}");
                    }
                    Console.WriteLine();

                    Console.WriteLine("=== Teste Concluído ===");
                    Console.WriteLine("✅ Tudo funcionou corretamente!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERRO: {ex.Message}");
                Console.WriteLine();
                Console.WriteLine("Stack Trace:");
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine();
            Console.WriteLine("Pressione qualquer tecla para sair...");
            Console.ReadKey();
        }
    }
}
