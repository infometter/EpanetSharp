using System;
using System.Runtime.InteropServices;

namespace EpanetSharp.Native
{
    /// <summary>
    /// Declarações P/Invoke para a biblioteca nativa do EPANET.
    /// Esta classe deve conter APENAS assinaturas externas (DllImport).
    /// Implementações gerenciadas e adaptadores devem residir em <see cref="NativeApi"/>.
    /// </summary>
    internal static partial class NativeMethods
    {
        /// <summary>
        /// Assinatura P/Invoke para criar um projeto nativo (EN_Project).
        /// Implementação real depende da biblioteca nativa do EPANET.
        /// </summary>
        /// <param name="project">Recebe o ponteiro nativo do projeto.</param>
        /// <returns>Código de status nativo.</returns>
        [DllImport("epanet2.dll", EntryPoint = "EN_createproject", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int EN_createproject(out IntPtr project);

        /// <summary>
        /// Assinatura P/Invoke para deletar um projeto nativo (EN_deleteproject).
        /// </summary>
        /// <param name="project">Ponteiro nativo do projeto.</param>
        /// <returns>Código de status nativo.</returns>
        [DllImport("epanet2.dll", EntryPoint = "EN_deleteproject", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int EN_deleteproject(IntPtr project);

        /// <summary>
        /// Assinatura P/Invoke para abrir um arquivo INP no projeto nativo (EN_open).
        /// A API do EPANET tradicionalmente aceita três strings: input, report e binary filenames.
        /// Mantemos CharSet.Ansi e CallingConvention.Cdecl.
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_open", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int EN_open(IntPtr project, string inpFile, string rptFile, string outFile);

        /// <summary>
        /// Assinatura P/Invoke para fechar um projeto nativo (EN_close).
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_close", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int EN_close(IntPtr project);

        /// <summary>
        /// Assinatura P/Invoke para obter a versão da biblioteca EPANET.
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_getversion", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int EN_getversion(System.Text.StringBuilder buffer, int bufferLength);

        /// <summary>
        /// Assinatura P/Invoke para obter contadores relacionados ao projeto (EN_getcount).
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_getcount", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int EN_getcount(IntPtr project, int countCode, out int value);

        /// <summary>
        /// Assinatura P/Invoke para obter o ID de um nó dado seu índice (EN_getnodeid).
        /// Preenche um buffer com o identificador do nó.
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_getnodeid", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int EN_getnodeid(IntPtr project, int index, System.Text.StringBuilder idBuffer, int bufferLength);

        /// <summary>
        /// Assinatura P/Invoke para obter o índice de um nó dado seu ID (EN_getnodeindex).
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_getnodeindex", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int EN_getnodeindex(IntPtr project, string id, out int index);

        /// <summary>
        /// Assinatura P/Invoke para obter o tipo do nó (EN_getnodetype).
        /// Retorna um código inteiro representando Junction/Reservoir/Tank.
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_getnodetype", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int EN_getnodetype(IntPtr project, int index, out int type);

        /// <summary>
        /// Obtém um valor numérico associado a um nó (EN_getnodevalue).
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_getnodevalue", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int EN_getnodevalue(IntPtr project, int index, int paramCode, out double value);

        /// <summary>
        /// Define um valor numérico associado a um nó (EN_setnodevalue).
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_setnodevalue", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int EN_setnodevalue(IntPtr project, int index, int paramCode, double value);

        /// <summary>
        /// Obtém o comentário associado a um nó (string).
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_getnodecomment", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int EN_getnodecomment(IntPtr project, int index, System.Text.StringBuilder buffer, int bufferLength);

        /// <summary>
        /// Define o comentário associado a um nó.
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_setnodecomment", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int EN_setnodecomment(IntPtr project, int index, string comment);

        /// <summary>
        /// Obtém a tag associada a um nó (string).
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_getnodetag", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int EN_getnodetag(IntPtr project, int index, System.Text.StringBuilder buffer, int bufferLength);

        /// <summary>
        /// Define a tag associada a um nó.
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_setnodetag", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int EN_setnodetag(IntPtr project, int index, string tag);

        /// <summary>
        /// Obtém o ID de um link dado seu índice (EN_getlinkid).
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_getlinkid", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int EN_getlinkid(IntPtr project, int index, System.Text.StringBuilder idBuffer, int bufferLength);

        /// <summary>
        /// Obtém o índice de um link dado seu ID (EN_getlinkindex).
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_getlinkindex", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int EN_getlinkindex(IntPtr project, string id, out int index);

        /// <summary>
        /// Obtém o tipo do link (EN_getlinktype).
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_getlinktype", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int EN_getlinktype(IntPtr project, int index, out int type);

        // Hydraulic simulation API (EN_openH, EN_initH, EN_runH, EN_nextH, EN_closeH)
        [DllImport("epanet2.dll", EntryPoint = "EN_openH", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int EN_openH(IntPtr project);

        [DllImport("epanet2.dll", EntryPoint = "EN_initH", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int EN_initH(IntPtr project);

        [DllImport("epanet2.dll", EntryPoint = "EN_runH", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int EN_runH(IntPtr project);

        [DllImport("epanet2.dll", EntryPoint = "EN_nextH", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int EN_nextH(IntPtr project, out int tstep);

        [DllImport("epanet2.dll", EntryPoint = "EN_closeH", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int EN_closeH(IntPtr project);

        // Water quality simulation API (EN_openQ, EN_initQ, EN_runQ, EN_nextQ, EN_closeQ)
        [DllImport("epanet2.dll", EntryPoint = "EN_openQ", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int EN_openQ(IntPtr project);

        [DllImport("epanet2.dll", EntryPoint = "EN_initQ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int EN_initQ(IntPtr project, int traceNodeIndex, int traceType);

        [DllImport("epanet2.dll", EntryPoint = "EN_runQ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int EN_runQ(IntPtr project);

        [DllImport("epanet2.dll", EntryPoint = "EN_nextQ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int EN_nextQ(IntPtr project, out int tstep);

        [DllImport("epanet2.dll", EntryPoint = "EN_closeQ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int EN_closeQ(IntPtr project);

        /// <summary>
        /// Obtém um valor numérico associado a um link (EN_getlinkvalue).
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_getlinkvalue", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int EN_getlinkvalue(IntPtr project, int index, int paramCode, out double value);

        /// <summary>
        /// Define um valor numérico associado a um link (EN_setlinkvalue).
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_setlinkvalue", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int EN_setlinkvalue(IntPtr project, int index, int paramCode, double value);

        /// <summary>
        /// Obtém o índice de um pattern dado seu identificador (EN_getpatternindex).
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_getpatternindex", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int EN_getpatternindex(IntPtr project, string id, out int index);

        /// <summary>
        /// Obtém o ID de um pattern dado seu índice (EN_getpatternid).
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_getpatternid", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int EN_getpatternid(IntPtr project, int index, System.Text.StringBuilder idBuffer, int bufferLength);

        /// <summary>
        /// Obtém o índice de um controle dado seu identificador (EN_getcontrolindex).
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_getcontrolindex", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int EN_getcontrolindex(IntPtr project, string id, out int index);

        /// <summary>
        /// Obtém o ID de um controle dado seu índice (EN_getcontrolid).
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_getcontrolid", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int EN_getcontrolid(IntPtr project, int index, System.Text.StringBuilder idBuffer, int bufferLength);

        /// <summary>
        /// Lê a definição textual de um controle (EN_getcontrol).
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_getcontrol", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int EN_getcontrol(IntPtr project, int index, System.Text.StringBuilder buffer, int bufferLength);

        /// <summary>
        /// Adiciona um controle textual ao projeto (EN_addcontrol).
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_addcontrol", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int EN_addcontrol(IntPtr project, string controlText);

        /// <summary>
        /// Remove um controle dado seu índice (EN_deletecontrol).
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_deletecontrol", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int EN_deletecontrol(IntPtr project, int index);

        /// <summary>
        /// Obtém o comprimento (número de períodos) de um pattern (EN_getpatternlen).
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_getpatternlen", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int EN_getpatternlen(IntPtr project, int index, out int length);

        /// <summary>
        /// Obtém o valor de um pattern no período especificado (EN_getpatternvalue).
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_getpatternvalue", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int EN_getpatternvalue(IntPtr project, int index, int period, out double value);

        /// <summary>
        /// Define o valor de um pattern no período especificado (EN_setpatternvalue).
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_setpatternvalue", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int EN_setpatternvalue(IntPtr project, int index, int period, double value);

        /// <summary>
        /// Obtém o índice de uma curva dado seu identificador (EN_getcurveindex).
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_getcurveindex", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int EN_getcurveindex(IntPtr project, string id, out int index);

        /// <summary>
        /// Obtém o ID de uma curva dado seu índice (EN_getcurveid).
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_getcurveid", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int EN_getcurveid(IntPtr project, int index, System.Text.StringBuilder idBuffer, int bufferLength);

        /// <summary>
        /// Lê os pontos de uma curva (EN_getcurve). Recebe arrays X e Y e um ref n com tamanho.
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_getcurve", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int EN_getcurve(IntPtr project, int index, [Out] double[] x, [Out] double[] y, ref int n);

        /// <summary>
        /// Define os pontos de uma curva (EN_setcurve).
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_setcurve", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int EN_setcurve(IntPtr project, int index, double[] x, double[] y, int n);

        /// <summary>
        /// Obtém uma opção de simulação (EN_getoption).
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_getoption", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int EN_getoption(IntPtr project, int option, out double value);

        /// <summary>
        /// Define uma opção de simulação (EN_setoption).
        /// </summary>
        [DllImport("epanet2.dll", EntryPoint = "EN_setoption", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int EN_setoption(IntPtr project, int option, double value);
    }
}
