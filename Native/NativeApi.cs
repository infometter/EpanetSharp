using System;
using System.Runtime.InteropServices;
using System.Text;
using EpanetSharp.Exceptions;

namespace EpanetSharp.Native
{
    /// <summary>
    /// Adaptador para a camada nativa do EPANET. Fornece métodos gerenciados que encapsulam
    /// chamadas P/Invoke declaradas em <see cref="NativeMethods"/>.
    /// Esta classe é internal e deve ser a única a utilizar <see cref="NativeMethods"/>.
    /// </summary>
    internal sealed class NativeApi
    {
        private const int VersionBufferSize = 256;

        /// <summary>
        /// Cria um projeto nativo e retorna o ponteiro associado.
        /// </summary>
        /// <returns>Handle nativo do projeto.</returns>
        public IntPtr CreateProject()
        {
            int rc = NativeMethods.EN_createproject(out IntPtr project);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_createproject), $"EN_createproject returned {rc}");
            }
            if (project == IntPtr.Zero)
            {
                throw new InvalidOperationException("EN_createproject returned a null handle.");
            }

            return project;
        }

        /// <summary>
        /// Deleta (libera) um projeto nativo previamente criado.
        /// </summary>
        public void DeleteProject(IntPtr project)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");

            int rc = NativeMethods.EN_deleteproject(project);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_deleteproject), $"EN_deleteproject returned {rc}");
            }
        }

        /// <summary>
        /// Verifica se a biblioteca nativa está disponível e responde a chamadas básicas.
        /// </summary>
        /// <returns><c>true</c> se a biblioteca nativa puder ser invocada; caso contrário, <c>false</c>.</returns>
        public bool IsAvailable()
        {
            try
            {
                var sb = new System.Text.StringBuilder(VersionBufferSize);
                int rc = NativeMethods.EN_getversion(sb, sb.Capacity);
                return rc == 0;
            }
            catch (DllNotFoundException)
            {
                return false;
            }
            catch (EntryPointNotFoundException)
            {
                return false;
            }
            catch (BadImageFormatException)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Abre um arquivo INP no projeto nativo.
        /// </summary>
        public void Open(IntPtr project, string inpFile)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");
            if (inpFile is null) throw new ArgumentNullException(nameof(inpFile));
            // Call EN_open using input, report and binary filenames. Pass empty strings for report and binary
            // which is compatible with EPANET C API expecting three string args.
            int rc = NativeMethods.EN_open(project, inpFile, string.Empty, string.Empty);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_open), $"EN_open returned {rc}");
            }

            // Diagnostic hook: when environment variable EPANET_DIAG is set, write basic counts
            // to a .diag.txt file next to the input file. This helps debugging discrepancies
            // between EPANET report and EN_getcount values without changing public API.
            try
            {
                var diag = System.Environment.GetEnvironmentVariable("EPANET_DIAG");
                if (!string.IsNullOrEmpty(diag))
                {
                    try
                    {
                        int nodeCount = GetCount(project, NativeConstants.EN_NODECOUNT);
                        int linkCount = GetCount(project, NativeConstants.EN_LINKCOUNT);
                        string path = inpFile;
                        string diagPath = System.IO.Path.ChangeExtension(path, ".diag.txt");
                        var sb = new System.Text.StringBuilder();
                        sb.AppendLine($"EPANET_DIAG: {DateTime.UtcNow:u}");
                        sb.AppendLine($"InputFile: {path}");
                        sb.AppendLine($"NodeCount: {nodeCount}");
                        sb.AppendLine($"LinkCount: {linkCount}");
                        System.IO.File.WriteAllText(diagPath, sb.ToString());
                    }
                    catch
                    {
                        // swallow diagnostic errors to avoid affecting normal execution
                    }
                }
            }
            catch
            {
                // ignore any unexpected issues reading env
            }
        }

        /// <summary>
        /// Fecha o projeto nativo.
        /// </summary>
        public void Close(IntPtr project)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");

            int rc = NativeMethods.EN_close(project);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_close), $"EN_close returned {rc}");
            }
        }

        /// <summary>
        /// Obtém a versão da biblioteca EPANET.
        /// </summary>
        public string GetVersion()
        {
            var sb = new StringBuilder(VersionBufferSize);
            int rc = NativeMethods.EN_getversion(sb, sb.Capacity);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_getversion), $"EN_getversion returned {rc}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Obtém uma opção de simulação (EN_getoption).
        /// </summary>
        public double GetOption(IntPtr project, int option)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");

            int rc = NativeMethods.EN_getoption(project, option, out double value);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_getoption), $"EN_getoption returned {rc}");
            }
            return value;
        }

        /// <summary>
        /// Define uma opção de simulação (EN_setoption).
        /// </summary>
        public void SetOption(IntPtr project, int option, double value)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");

            int rc = NativeMethods.EN_setoption(project, option, value);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_setoption), $"EN_setoption returned {rc}");
            }
        }

        /// <summary>
        /// Obtém um contador associado ao projeto (por exemplo número de nós, links, etc.).
        /// </summary>
        public int GetCount(IntPtr project, int countCode)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");

            int rc = NativeMethods.EN_getcount(project, countCode, out int value);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_getcount), $"EN_getcount returned {rc}");
            }

            return value;
        }

        /// <summary>
        /// Obtém o ID do nó no índice especificado.
        /// </summary>
        public string GetNodeId(IntPtr project, int index)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");

            var sb = new StringBuilder(128);
            int rc = NativeMethods.EN_getnodeid(project, index, sb, sb.Capacity);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_getnodeid), $"EN_getnodeid returned {rc}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Obtém o índice de um nó dado seu ID.
        /// </summary>
        public int GetNodeIndex(IntPtr project, string id)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");
            if (id is null) throw new ArgumentNullException(nameof(id));

            int rc = NativeMethods.EN_getnodeindex(project, id, out int index);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_getnodeindex), $"EN_getnodeindex returned {rc}");
            }

            return index;
        }

        /// <summary>
        /// Obtém o tipo de nó.
        /// </summary>
        public int GetNodeType(IntPtr project, int index)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");

            int rc = NativeMethods.EN_getnodetype(project, index, out int type);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_getnodetype), $"EN_getnodetype returned {rc}");
            }

            return type;
        }

        /// <summary>
        /// Obtém um valor numérico de nó (EN_getnodevalue).
        /// </summary>
        public double GetNodeValue(IntPtr project, int index, int paramCode)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");

            int rc = NativeMethods.EN_getnodevalue(project, index, paramCode, out double value);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_getnodevalue), $"EN_getnodevalue returned {rc}");
            }

            return value;
        }

        /// <summary>
        /// Define um valor numérico de nó (EN_setnodevalue).
        /// </summary>
        public void SetNodeValue(IntPtr project, int index, int paramCode, double value)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");

            int rc = NativeMethods.EN_setnodevalue(project, index, paramCode, value);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_setnodevalue), $"EN_setnodevalue returned {rc}");
            }
        }

        /// <summary>
        /// Obtém comentário do nó.
        /// </summary>
        public string GetNodeComment(IntPtr project, int index)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");

            var sb = new StringBuilder(256);
            int rc = NativeMethods.EN_getnodecomment(project, index, sb, sb.Capacity);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_getnodecomment), $"EN_getnodecomment returned {rc}");
            }
            return sb.ToString();
        }

        public void SetNodeComment(IntPtr project, int index, string comment)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");
            if (comment is null) throw new ArgumentNullException(nameof(comment));

            int rc = NativeMethods.EN_setnodecomment(project, index, comment);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_setnodecomment), $"EN_setnodecomment returned {rc}");
            }
        }

        public string GetNodeTag(IntPtr project, int index)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");

            var sb = new StringBuilder(128);
            int rc = NativeMethods.EN_getnodetag(project, index, sb, sb.Capacity);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_getnodetag), $"EN_getnodetag returned {rc}");
            }
            return sb.ToString();
        }

        public void SetNodeTag(IntPtr project, int index, string tag)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");
            if (tag is null) throw new ArgumentNullException(nameof(tag));

            int rc = NativeMethods.EN_setnodetag(project, index, tag);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_setnodetag), $"EN_setnodetag returned {rc}");
            }
        }

        /// <summary>
        /// Obtém o ID de um link dado seu índice.
        /// </summary>
        public string GetLinkId(IntPtr project, int index)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");

            var sb = new StringBuilder(128);
            int rc = NativeMethods.EN_getlinkid(project, index, sb, sb.Capacity);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_getlinkid), $"EN_getlinkid returned {rc}");
            }
            return sb.ToString();
        }

        public int GetLinkIndex(IntPtr project, string id)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");
            if (id is null) throw new ArgumentNullException(nameof(id));

            int rc = NativeMethods.EN_getlinkindex(project, id, out int index);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_getlinkindex), $"EN_getlinkindex returned {rc}");
            }
            return index;
        }

        public int GetLinkType(IntPtr project, int index)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");
            int rc = NativeMethods.EN_getlinktype(project, index, out int type);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_getlinktype), $"EN_getlinktype returned {rc}");
            }
            return type;
        }

        public double GetLinkValue(IntPtr project, int index, int paramCode)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");
            int rc = NativeMethods.EN_getlinkvalue(project, index, paramCode, out double value);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_getlinkvalue), $"EN_getlinkvalue returned {rc}");
            }
            return value;
        }

        public void SetLinkValue(IntPtr project, int index, int paramCode, double value)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");
            int rc = NativeMethods.EN_setlinkvalue(project, index, paramCode, value);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_setlinkvalue), $"EN_setlinkvalue returned {rc}");
            }
        }

        public int GetPatternIndex(IntPtr project, string id)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");
            if (id is null) throw new ArgumentNullException(nameof(id));

            int rc = NativeMethods.EN_getpatternindex(project, id, out int index);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_getpatternindex), $"EN_getpatternindex returned {rc}");
            }

            return index;
        }

        public int GetControlIndex(IntPtr project, string id)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");
            if (id is null) throw new ArgumentNullException(nameof(id));

            int rc = NativeMethods.EN_getcontrolindex(project, id, out int index);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_getcontrolindex), $"EN_getcontrolindex returned {rc}");
            }
            return index;
        }

        public string GetControlId(IntPtr project, int index)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");

            var sb = new StringBuilder(256);
            int rc = NativeMethods.EN_getcontrolid(project, index, sb, sb.Capacity);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_getcontrolid), $"EN_getcontrolid returned {rc}");
            }
            return sb.ToString();
        }

        public string GetControl(IntPtr project, int index)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");

            var sb = new StringBuilder(1024);
            int rc = NativeMethods.EN_getcontrol(project, index, sb, sb.Capacity);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_getcontrol), $"EN_getcontrol returned {rc}");
            }
            return sb.ToString();
        }

        public void AddControl(IntPtr project, string controlText)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");
            if (controlText is null) throw new ArgumentNullException(nameof(controlText));

            int rc = NativeMethods.EN_addcontrol(project, controlText);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_addcontrol), $"EN_addcontrol returned {rc}");
            }
        }

        public void DeleteControl(IntPtr project, int index)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");

            int rc = NativeMethods.EN_deletecontrol(project, index);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_deletecontrol), $"EN_deletecontrol returned {rc}");
            }
        }

        public string GetPatternId(IntPtr project, int index)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");

            var sb = new StringBuilder(128);
            int rc = NativeMethods.EN_getpatternid(project, index, sb, sb.Capacity);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_getpatternid), $"EN_getpatternid returned {rc}");
            }
            return sb.ToString();
        }

        public int GetPatternLength(IntPtr project, int index)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");

            int rc = NativeMethods.EN_getpatternlen(project, index, out int length);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_getpatternlen), $"EN_getpatternlen returned {rc}");
            }

            return length;
        }

        public double GetPatternValue(IntPtr project, int index, int period)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");

            int rc = NativeMethods.EN_getpatternvalue(project, index, period, out double value);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_getpatternvalue), $"EN_getpatternvalue returned {rc}");
            }

            return value;
        }

        public void SetPatternValue(IntPtr project, int index, int period, double value)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");

            int rc = NativeMethods.EN_setpatternvalue(project, index, period, value);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_setpatternvalue), $"EN_setpatternvalue returned {rc}");
            }
        }

        public int GetCurveIndex(IntPtr project, string id)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");
            if (id is null) throw new ArgumentNullException(nameof(id));

            int rc = NativeMethods.EN_getcurveindex(project, id, out int index);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_getcurveindex), $"EN_getcurveindex returned {rc}");
            }
            return index;
        }

        public string GetCurveId(IntPtr project, int index)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");

            var sb = new StringBuilder(128);
            int rc = NativeMethods.EN_getcurveid(project, index, sb, sb.Capacity);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_getcurveid), $"EN_getcurveid returned {rc}");
            }
            return sb.ToString();
        }

        public (double[] x, double[] y) GetCurve(IntPtr project, int index)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");

            // First call to determine length: EN_getcurve typically expects n set to size of arrays.
            int n = 0;
            int rc = NativeMethods.EN_getcurve(project, index, null, null, ref n);
            if (rc != 0 && n <= 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_getcurve), $"EN_getcurve returned {rc}");
            }

            var xs = new double[n];
            var ys = new double[n];
            rc = NativeMethods.EN_getcurve(project, index, xs, ys, ref n);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_getcurve), $"EN_getcurve returned {rc}");
            }

            return (xs, ys);
        }

        public void SetCurve(IntPtr project, int index, double[] x, double[] y)
        {
            if (project == IntPtr.Zero) throw new ArgumentException("project", "Handle nativo inválido.");
            if (x is null) throw new ArgumentNullException(nameof(x));
            if (y is null) throw new ArgumentNullException(nameof(y));
            if (x.Length != y.Length) throw new ArgumentException("x and y must have same length");

            int rc = NativeMethods.EN_setcurve(project, index, x, y, x.Length);
            if (rc != 0)
            {
                throw new EpanetException(rc, nameof(NativeMethods.EN_setcurve), $"EN_setcurve returned {rc}");
            }
        }

        /// <summary>
        /// Verifica um código de retorno nativo e lança <see cref="EpanetException"/> quando não for sucesso.
        /// </summary>
        /// <param name="code">Código de retorno nativo.</param>
        public void CheckError(int code)
        {
            if (code == 0) return;
            throw new EpanetException(code, "native", $"Native call returned {code}");
        }
    }
}
