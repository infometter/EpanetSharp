using System;
using System.Collections.Generic;
using EpanetSharp.Core;
using EpanetSharp.Native;

namespace EpanetSharp.Validation
{
    /// <summary>
    /// Valida a integridade de uma rede hidráulica EPANET antes de rodar a simulação.
    /// Inspirado na função check_network() do WNTR-QGIS.
    /// </summary>
    public class NetworkValidator
    {
        private readonly NativeContext _ctx;
        private readonly Network _network;

        /// <summary>
        /// Cria um validador para a rede associada ao projeto.
        /// </summary>
        public NetworkValidator(Project project)
        {
            if (project == null) throw new ArgumentNullException("project");
            _ctx     = project.NativeContext;
            _network = project.Network;
        }

        /// <summary>
        /// Executa todas as validações e retorna o resultado consolidado.
        /// </summary>
        public ValidationResult Validate()
        {
            var errors = new List<ValidationError>();

            ValidateNodeCount(errors);
            ValidateLinkCount(errors);
            ValidateNodes(errors);
            ValidateLinks(errors);
            ValidateConnectivity(errors);

            return new ValidationResult(errors);
        }

        // ── Validações ────────────────────────────────────────────────────────

        private void ValidateNodeCount(List<ValidationError> errors)
        {
            if (_network.NodeCount == 0)
                errors.Add(new ValidationError(ValidationSeverity.Error,
                    "A rede não possui nenhum nó."));
        }

        private void ValidateLinkCount(List<ValidationError> errors)
        {
            if (_network.LinkCount == 0)
                errors.Add(new ValidationError(ValidationSeverity.Warning,
                    "A rede não possui nenhum link (tubulação/bomba/válvula)."));
        }

        private void ValidateNodes(List<ValidationError> errors)
        {
            for (int i = 1; i <= _network.NodeCount; i++)
            {
                string id = TryGetNodeId(i);

                // Elevação
                double elevation = TryGetNodeValue(i, NativeConstants.EN_ELEVATION);
                if (double.IsNaN(elevation) || double.IsInfinity(elevation))
                    errors.Add(new ValidationError(ValidationSeverity.Error,
                        "Elevação inválida (NaN ou Infinity).", id));

                // Tipo do nó
                int nodeType = _ctx.GetNodeType(i);

                if (nodeType == 0) // Junction
                {
                    double demand = TryGetNodeValue(i, NativeConstants.EN_BASEDEMAND);
                    if (double.IsNaN(demand))
                        errors.Add(new ValidationError(ValidationSeverity.Warning,
                            "Demanda base inválida (NaN).", id));
                }
                else if (nodeType == 2) // Tank
                {
                    ValidateTank(i, id, errors);
                }
            }
        }

        private void ValidateTank(int index, string id, List<ValidationError> errors)
        {
            double minLevel  = TryGetNodeValue(index, 11); // EN_TANKMINLEVEL
            double maxLevel  = TryGetNodeValue(index, 12); // EN_TANKMAXLEVEL
            double initLevel = TryGetNodeValue(index, 10); // EN_TANKLEVEL (initial)

            if (!double.IsNaN(minLevel) && !double.IsNaN(maxLevel) && minLevel >= maxLevel)
                errors.Add(new ValidationError(ValidationSeverity.Error,
                    string.Format("Nível mínimo ({0:F2}) >= nível máximo ({1:F2}).", minLevel, maxLevel), id));

            if (!double.IsNaN(minLevel) && !double.IsNaN(maxLevel) && !double.IsNaN(initLevel))
            {
                if (initLevel < minLevel || initLevel > maxLevel)
                    errors.Add(new ValidationError(ValidationSeverity.Error,
                        string.Format("Nível inicial ({0:F2}) fora do intervalo [{1:F2}, {2:F2}].",
                            initLevel, minLevel, maxLevel), id));
            }

            double diameter = TryGetNodeValue(index, 13); // EN_TANKDIAM
            if (!double.IsNaN(diameter) && diameter <= 0)
                errors.Add(new ValidationError(ValidationSeverity.Error,
                    string.Format("Diâmetro do tanque inválido ({0:F2}).", diameter), id));
        }

        private void ValidateLinks(List<ValidationError> errors)
        {
            for (int i = 1; i <= _network.LinkCount; i++)
            {
                string id = TryGetLinkId(i);
                int linkType = TryGetLinkType(i);

                if (linkType == 1) // PIPE
                {
                    double diameter  = TryGetLinkValue(i, NativeConstants.EN_LINK_DIAMETER);
                    double length    = TryGetLinkValue(i, NativeConstants.EN_LINK_LENGTH);
                    double roughness = TryGetLinkValue(i, NativeConstants.EN_LINK_ROUGHNESS);

                    if (!double.IsNaN(diameter) && diameter <= 0)
                        errors.Add(new ValidationError(ValidationSeverity.Error,
                            string.Format("Diâmetro inválido ({0:F2}).", diameter), id));

                    if (!double.IsNaN(length) && length <= 0)
                        errors.Add(new ValidationError(ValidationSeverity.Error,
                            string.Format("Comprimento inválido ({0:F2}).", length), id));

                    if (!double.IsNaN(roughness) && roughness <= 0)
                        errors.Add(new ValidationError(ValidationSeverity.Warning,
                            string.Format("Rugosidade inválida ({0:F2}).", roughness), id));
                }
            }
        }

        private void ValidateConnectivity(List<ValidationError> errors)
        {
            if (_network.NodeCount == 0 || _network.LinkCount == 0) return;

            // Marca quais nós têm pelo menos um link
            var connected = new bool[_network.NodeCount + 1];

            for (int j = 1; j <= _network.LinkCount; j++)
            {
                try
                {
                    _ctx.GetLinkNodes(j, out int node1, out int node2);
                    if (node1 >= 1 && node1 <= _network.NodeCount) connected[node1] = true;
                    if (node2 >= 1 && node2 <= _network.NodeCount) connected[node2] = true;
                }
                catch { /* ignora erros de leitura individuais */ }
            }

            for (int i = 1; i <= _network.NodeCount; i++)
            {
                if (!connected[i])
                {
                    string id = TryGetNodeId(i);
                    errors.Add(new ValidationError(ValidationSeverity.Warning,
                        "Nó não conectado a nenhum link (nó órfão).", id));
                }
            }
        }

        // ── Helpers seguros ────────────────────────────────────────────────────

        private string TryGetNodeId(int index)
        {
            try { return _ctx.GetNodeId(index); } catch { return "#" + index; }
        }

        private string TryGetLinkId(int index)
        {
            try { return _ctx.GetLinkId(index); } catch { return "#" + index; }
        }

        private double TryGetNodeValue(int index, int param)
        {
            try { return _ctx.GetNodeValue(index, param); } catch { return double.NaN; }
        }

        private double TryGetLinkValue(int index, int param)
        {
            try { return _ctx.GetLinkValue(index, param); } catch { return double.NaN; }
        }

        private int TryGetLinkType(int index)
        {
            try { return _ctx.GetLinkType(index); } catch { return -1; }
        }
    }
}
