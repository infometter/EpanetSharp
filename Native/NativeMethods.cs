using System;
using System.Runtime.InteropServices;
using EpanetSharp.Exceptions;

namespace EpanetSharp.Native
{
    /// <summary>
    /// Contém implementações de suporte que simulam chamadas nativas necessárias pelo contexto.
    /// Esta classe é <c>internal static partial</c> e deve ser usada apenas por <see cref="NativeContext"/>.
    /// </summary>
    internal static partial class NativeMethods
    {
        /// <summary>
        /// Cria um ponteiro de projeto nativo (simulado) e retorna um código de status.
        /// </summary>
        /// <param name="project">Recebe o handle do projeto nativo criado.</param>
        /// <returns>Código de status: 0 indica sucesso.</returns>
        internal static int CreateProject(out IntPtr project)
        {
            // Aloca um handle simulado. Implementação real deve usar P/Invoke para a biblioteca EPANET.
            project = Marshal.AllocHGlobal(1);
            return 0;
        }

        /// <summary>
        /// Destroi o ponteiro de projeto nativo (simulado) e retorna um código de status.
        /// </summary>
        /// <param name="project">Handle do projeto nativo a ser destruído.</param>
        /// <returns>Código de status: 0 indica sucesso; diferente de zero indica falha.</returns>
        internal static int DestroyProject(IntPtr project)
        {
            if (project == IntPtr.Zero)
            {
                return 1; // código de erro genérico para handle inválido
            }

            Marshal.FreeHGlobal(project);
            return 0;
        }

        /// <summary>
        /// Verifica o código de retorno de chamadas nativas e lança <see cref="EpanetException"/> em caso de erro.
        /// </summary>
        /// <param name="code">Código retornado pela chamada nativa.</param>
        internal static void CheckError(int code)
        {
            if (code != 0)
            {
                throw new EpanetException($"Erro nativo: código {code}");
            }
        }
    }
}
