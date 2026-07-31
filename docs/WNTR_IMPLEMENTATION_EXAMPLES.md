# Exemplos de Implementação - Funcionalidades WNTR

Este documento contém exemplos detalhados de como implementar as funcionalidades identificadas no WNTR.

---

## 1. Sistema de Conversão de Unidades

### Classe UnitConverter

```csharp
using System;
using System.Collections.Generic;

namespace EpanetSharp.Units
{
	/// <summary>
	/// Converte valores entre diferentes sistemas de unidades e SI
	/// Baseado em wntr.epanet.util
	/// </summary>
	public class UnitConverter
	{
		private readonly FlowUnit _flowUnit;
		private readonly HeadlossFormula _headlossFormula;

		// Fatores de conversão baseados no EPANET
		private static readonly Dictionary<FlowUnit, double> FlowFactors = new Dictionary<FlowUnit, double>
		{
			{ FlowUnit.CFS, 0.02831685 },      // cubic feet/sec
			{ FlowUnit.GPM, 0.000063090196 },  // gallons/min
			{ FlowUnit.MGD, 0.043812636 },     // million gallons/day
			{ FlowUnit.IMGD, 0.05261678 },     // imperial million gal/day
			{ FlowUnit.AFD, 0.014276410 },     // acre-feet/day
			{ FlowUnit.LPS, 0.001 },           // liters/sec
			{ FlowUnit.LPM, 0.000016666667 },  // liters/min
			{ FlowUnit.MLD, 0.011574074 },     // megaliter/day
			{ FlowUnit.CMH, 0.00027777778 },   // cubic meter/hr
			{ FlowUnit.SI, 1.0 }               // SI (m³/s)
		};

		private static readonly Dictionary<FlowUnit, double> PressureFactors = new Dictionary<FlowUnit, double>
		{
			{ FlowUnit.CFS, 0.3048 },    // feet
			{ FlowUnit.GPM, 0.3048 },    // feet
			{ FlowUnit.MGD, 0.3048 },    // feet
			{ FlowUnit.IMGD, 0.3048 },   // feet
			{ FlowUnit.AFD, 0.3048 },    // feet
			{ FlowUnit.LPS, 1.0 },       // meters
			{ FlowUnit.LPM, 1.0 },       // meters
			{ FlowUnit.MLD, 1.0 },       // meters
			{ FlowUnit.CMH, 1.0 },       // meters
			{ FlowUnit.SI, 1.0 }         // meters
		};

		public UnitConverter(FlowUnit flowUnit, HeadlossFormula headlossFormula)
		{
			_flowUnit = flowUnit;
			_headlossFormula = headlossFormula;
		}

		/// <summary>
		/// Converte valor para SI
		/// </summary>
		public double ToSI(double value, HydraulicParameter parameter)
		{
			if (double.IsNaN(value) || double.IsInfinity(value))
				return value;

			return parameter switch
			{
				HydraulicParameter.Flow => value * FlowFactors[_flowUnit],
				HydraulicParameter.Pressure => value * PressureFactors[_flowUnit],
				HydraulicParameter.Head => value * PressureFactors[_flowUnit],
				HydraulicParameter.Diameter => DiameterToSI(value),
				HydraulicParameter.Length => LengthToSI(value),
				HydraulicParameter.Roughness => RoughnessToSI(value),
				HydraulicParameter.Elevation => value * PressureFactors[_flowUnit],
				HydraulicParameter.Velocity => VelocityToSI(value),
				HydraulicParameter.Volume => VolumeToSI(value),
				HydraulicParameter.Demand => value * FlowFactors[_flowUnit],
				HydraulicParameter.Power => PowerToSI(value),
				_ => value // Sem conversão
			};
		}

		/// <summary>
		/// Converte valor de SI para unidade configurada
		/// </summary>
		public double FromSI(double value, HydraulicParameter parameter)
		{
			if (double.IsNaN(value) || double.IsInfinity(value))
				return value;

			return parameter switch
			{
				HydraulicParameter.Flow => value / FlowFactors[_flowUnit],
				HydraulicParameter.Pressure => value / PressureFactors[_flowUnit],
				HydraulicParameter.Head => value / PressureFactors[_flowUnit],
				HydraulicParameter.Diameter => DiameterFromSI(value),
				HydraulicParameter.Length => LengthFromSI(value),
				HydraulicParameter.Roughness => RoughnessFromSI(value),
				HydraulicParameter.Elevation => value / PressureFactors[_flowUnit],
				HydraulicParameter.Velocity => VelocityFromSI(value),
				HydraulicParameter.Volume => VolumeFromSI(value),
				HydraulicParameter.Demand => value / FlowFactors[_flowUnit],
				HydraulicParameter.Power => PowerFromSI(value),
				_ => value
			};
		}

		private double DiameterToSI(double value)
		{
			// CFS/GPM/etc usam polegadas, metric usam mm
			return IsMetric() ? value / 1000.0 : value * 0.0254;
		}

		private double DiameterFromSI(double value)
		{
			return IsMetric() ? value * 1000.0 : value / 0.0254;
		}

		private double LengthToSI(double value)
		{
			return value * PressureFactors[_flowUnit];
		}

		private double LengthFromSI(double value)
		{
			return value / PressureFactors[_flowUnit];
		}

		private double RoughnessToSI(double value)
		{
			// Rugosidade depende da fórmula de perda de carga!
			switch (_headlossFormula)
			{
				case HeadlossFormula.HazenWilliams:
					// H-W é adimensional, sem conversão
					return value;

				case HeadlossFormula.DarcyWeisbach:
					// D-W é em unidades de comprimento (ft ou mm)
					if (IsMetric())
						return value / 1000.0; // mm → m
					else
						return value * 0.0254; // millifeet → m (estranho mas é assim)

				case HeadlossFormula.ChezyManning:
					// C-M é em s/m^(1/3) ou s/ft^(1/3)
					return IsMetric() ? value : value / 1.486;

				default:
					return value;
			}
		}

		private double RoughnessFromSI(double value)
		{
			switch (_headlossFormula)
			{
				case HeadlossFormula.HazenWilliams:
					return value;

				case HeadlossFormula.DarcyWeisbach:
					if (IsMetric())
						return value * 1000.0;
					else
						return value / 0.0254;

				case HeadlossFormula.ChezyManning:
					return IsMetric() ? value : value * 1.486;

				default:
					return value;
			}
		}

		private double VelocityToSI(double value)
		{
			// ft/s ou m/s
			return IsMetric() ? value : value * 0.3048;
		}

		private double VelocityFromSI(double value)
		{
			return IsMetric() ? value : value / 0.3048;
		}

		private double VolumeToSI(double value)
		{
			// Volume de tanques: ft³ ou m³
			return IsMetric() ? value : value * 0.028316847;
		}

		private double VolumeFromSI(double value)
		{
			return IsMetric() ? value : value / 0.028316847;
		}

		private double PowerToSI(double value)
		{
			// hp ou kW
			return IsMetric() ? value * 1000.0 : value * 745.699872;
		}

		private double PowerFromSI(double value)
		{
			return IsMetric() ? value / 1000.0 : value / 745.699872;
		}

		private bool IsMetric()
		{
			return _flowUnit == FlowUnit.LPS ||
				   _flowUnit == FlowUnit.LPM ||
				   _flowUnit == FlowUnit.MLD ||
				   _flowUnit == FlowUnit.CMH ||
				   _flowUnit == FlowUnit.SI;
		}
	}

	public enum HydraulicParameter
	{
		Flow,
		Pressure,
		Head,
		Diameter,
		Length,
		Roughness,
		Elevation,
		Velocity,
		Volume,
		Demand,
		Power,
		Energy,
		Quality
	}
}
```

### Uso do UnitConverter

```csharp
// Criar conversor
var converter = new UnitConverter(FlowUnit.GPM, HeadlossFormula.HazenWilliams);

// Converter vazão de GPM para SI (m³/s)
double flowGPM = 100.0; // 100 GPM
double flowSI = converter.ToSI(flowGPM, HydraulicParameter.Flow);
Console.WriteLine($"{flowGPM} GPM = {flowSI} m³/s");

// Converter pressão de ft para m
double pressureFt = 50.0;
double pressureM = converter.ToSI(pressureFt, HydraulicParameter.Pressure);

// Converter de volta
double backToGPM = converter.FromSI(flowSI, HydraulicParameter.Flow);
```

---

## 2. Validação de Rede

### Classe NetworkValidator

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

namespace EpanetSharp.Validation
{
	public class NetworkValidator
	{
		private readonly Network _network;

		public NetworkValidator(Network network)
		{
			_network = network ?? throw new ArgumentNullException(nameof(network));
		}

		public ValidationResult Validate()
		{
			var errors = new List<ValidationError>();

			ValidateNodeCount(errors);
			ValidateLinkCount(errors);
			ValidateConnectivity(errors);
			ValidateNodeProperties(errors);
			ValidateLinkProperties(errors);
			ValidateTanks(errors);
			ValidatePumps(errors);
			ValidateValves(errors);

			return new ValidationResult(errors);
		}

		private void ValidateNodeCount(List<ValidationError> errors)
		{
			if (_network.NodeCount == 0)
			{
				errors.Add(new ValidationError(
					ValidationSeverity.Error,
					"Network must have at least one node"
				));
			}
		}

		private void ValidateLinkCount(List<ValidationError> errors)
		{
			if (_network.LinkCount == 0)
			{
				errors.Add(new ValidationError(
					ValidationSeverity.Warning,
					"Network has no links"
				));
			}
		}

		private void ValidateConnectivity(List<ValidationError> errors)
		{
			// Verificar nós órfãos (sem links conectados)
			var ctx = _network.Context;

			for (int i = 1; i <= _network.NodeCount; i++)
			{
				string nodeId = ctx.GetNodeId(i);
				bool hasConnection = false;

				// Verificar se algum link se conecta a este nó
				for (int j = 1; j <= _network.LinkCount; j++)
				{
					var (node1, node2) = ctx.GetLinkNodes(j);
					if (node1 == i || node2 == i)
					{
						hasConnection = true;
						break;
					}
				}

				if (!hasConnection)
				{
					errors.Add(new ValidationError(
						ValidationSeverity.Warning,
						$"Node '{nodeId}' is not connected to any link (orphan node)"
					));
				}
			}
		}

		private void ValidateNodeProperties(List<ValidationError> errors)
		{
			var ctx = _network.Context;

			for (int i = 1; i <= _network.NodeCount; i++)
			{
				string nodeId = ctx.GetNodeId(i);

				// Verificar elevação
				double elevation = ctx.GetNodeValue(i, NodeProperty.Elevation);
				if (double.IsNaN(elevation) || double.IsInfinity(elevation))
				{
					errors.Add(new ValidationError(
						ValidationSeverity.Error,
						$"Node '{nodeId}' has invalid elevation"
					));
				}

				// Verificar demanda base (para junctions)
				int nodeType = (int)ctx.GetNodeValue(i, NodeProperty.Type);
				if (nodeType == 0) // Junction
				{
					double demand = ctx.GetNodeValue(i, NodeProperty.BaseDemand);
					if (double.IsNaN(demand))
					{
						errors.Add(new ValidationError(
							ValidationSeverity.Warning,
							$"Junction '{nodeId}' has invalid base demand"
						));
					}
				}
			}
		}

		private void ValidateLinkProperties(List<ValidationError> errors)
		{
			var ctx = _network.Context;

			for (int i = 1; i <= _network.LinkCount; i++)
			{
				string linkId = ctx.GetLinkId(i);

				// Verificar diâmetro
				double diameter = ctx.GetLinkValue(i, LinkProperty.Diameter);
				if (diameter <= 0)
				{
					errors.Add(new ValidationError(
						ValidationSeverity.Error,
						$"Link '{linkId}' has invalid diameter: {diameter}"
					));
				}

				// Verificar comprimento
				double length = ctx.GetLinkValue(i, LinkProperty.Length);
				if (length <= 0)
				{
					errors.Add(new ValidationError(
						ValidationSeverity.Error,
						$"Link '{linkId}' has invalid length: {length}"
					));
				}

				// Verificar rugosidade
				double roughness = ctx.GetLinkValue(i, LinkProperty.Roughness);
				if (roughness <= 0)
				{
					errors.Add(new ValidationError(
						ValidationSeverity.Warning,
						$"Link '{linkId}' has invalid roughness: {roughness}"
					));
				}
			}
		}

		private void ValidateTanks(List<ValidationError> errors)
		{
			var ctx = _network.Context;

			for (int i = 1; i <= _network.NodeCount; i++)
			{
				int nodeType = (int)ctx.GetNodeValue(i, NodeProperty.Type);
				if (nodeType == 2) // Tank
				{
					string nodeId = ctx.GetNodeId(i);

					double minLevel = ctx.GetNodeValue(i, NodeProperty.TankMinLevel);
					double maxLevel = ctx.GetNodeValue(i, NodeProperty.TankMaxLevel);
					double initLevel = ctx.GetNodeValue(i, NodeProperty.TankInitLevel);

					if (minLevel >= maxLevel)
					{
						errors.Add(new ValidationError(
							ValidationSeverity.Error,
							$"Tank '{nodeId}': minimum level must be less than maximum level"
						));
					}

					if (initLevel < minLevel || initLevel > maxLevel)
					{
						errors.Add(new ValidationError(
							ValidationSeverity.Error,
							$"Tank '{nodeId}': initial level must be between min and max levels"
						));
					}
				}
			}
		}

		private void ValidatePumps(List<ValidationError> errors)
		{
			// TODO: Validar curvas de bombas
		}

		private void ValidateValves(List<ValidationError> errors)
		{
			// TODO: Validar configurações de válvulas
		}
	}

	public class ValidationResult
	{
		public List<ValidationError> Errors { get; }
		public bool IsValid => !Errors.Any(e => e.Severity == ValidationSeverity.Error);
		public bool HasWarnings => Errors.Any(e => e.Severity == ValidationSeverity.Warning);

		public ValidationResult(List<ValidationError> errors)
		{
			Errors = errors ?? new List<ValidationError>();
		}

		public void ThrowIfInvalid()
		{
			if (!IsValid)
			{
				var errorMessages = string.Join("\n", 
					Errors.Where(e => e.Severity == ValidationSeverity.Error)
						  .Select(e => e.Message));
				throw new NetworkValidationException(
					$"Network validation failed:\n{errorMessages}");
			}
		}
	}

	public class ValidationError
	{
		public ValidationSeverity Severity { get; }
		public string Message { get; }

		public ValidationError(ValidationSeverity severity, string message)
		{
			Severity = severity;
			Message = message;
		}
	}

	public enum ValidationSeverity
	{
		Info,
		Warning,
		Error
	}

	public class NetworkValidationException : Exception
	{
		public NetworkValidationException(string message) : base(message) { }
	}
}
```

### Uso do NetworkValidator

```csharp
using (var project = Project.Open("rede.inp"))
{
	var validator = new NetworkValidator(project.Network);
	var result = validator.Validate();

	if (!result.IsValid)
	{
		Console.WriteLine("❌ Network validation failed:");
		foreach (var error in result.Errors.Where(e => e.Severity == ValidationSeverity.Error))
		{
			Console.WriteLine($"  ERROR: {error.Message}");
		}
	}

	if (result.HasWarnings)
	{
		Console.WriteLine("⚠️  Warnings:");
		foreach (var warning in result.Errors.Where(e => e.Severity == ValidationSeverity.Warning))
		{
			Console.WriteLine($"  WARN: {warning.Message}");
		}
	}

	if (result.IsValid)
	{
		Console.WriteLine("✅ Network is valid!");

		// Lançar exceção se houver erros
		result.ThrowIfInvalid();

		// Prosseguir com a simulação
		project.Run();
	}
}
```

---

## 3. SimulationOptions - Configuração Programática

```csharp
using System;

namespace EpanetSharp.Simulation
{
	public class SimulationOptions
	{
		public TimeSpan Duration { get; set; } = TimeSpan.Zero;
		public TimeSpan HydraulicTimestep { get; set; } = TimeSpan.FromHours(1);
		public TimeSpan QualityTimestep { get; set; } = TimeSpan.FromMinutes(5);
		public TimeSpan PatternTimestep { get; set; } = TimeSpan.FromHours(1);
		public TimeSpan ReportTimestep { get; set; } = TimeSpan.FromHours(1);
		public TimeSpan ReportStart { get; set; } = TimeSpan.Zero;
		public TimeSpan RuleTimestep { get; set; } = TimeSpan.FromMinutes(6);

		public FlowUnit FlowUnits { get; set; } = FlowUnit.SI;
		public HeadlossFormula HeadlossFormula { get; set; } = HeadlossFormula.HazenWilliams;

		public double SpecificGravity { get; set; } = 1.0;
		public double Viscosity { get; set; } = 1.0;
		public int MaxTrials { get; set; } = 200;
		public double Accuracy { get; set; } = 0.001;
		public bool UnbalancedContinue { get; set; } = false;
		public int UnbalancedTrials { get; set; } = 10;

		public string DemandMultiplier { get; set; } = "1.0";
		public string EmitterExponent { get; set; } = "0.5";

		public QualityAnalysisType QualityAnalysis { get; set; } = QualityAnalysisType.None;
		public string QualityChemicalName { get; set; } = "";
		public string QualityChemicalUnits { get; set; } = "mg/L";
		public double QualityTolerance { get; set; } = 0.01;

		/// <summary>
		/// Aplica as opções ao projeto EPANET
		/// </summary>
		public void ApplyTo(NativeContext context)
		{
			// Tempo
			context.SetTimeParameter(TimeParameter.Duration, (int)Duration.TotalSeconds);
			context.SetTimeParameter(TimeParameter.HydraulicTimestep, (int)HydraulicTimestep.TotalSeconds);
			context.SetTimeParameter(TimeParameter.QualityTimestep, (int)QualityTimestep.TotalSeconds);
			context.SetTimeParameter(TimeParameter.PatternTimestep, (int)PatternTimestep.TotalSeconds);
			context.SetTimeParameter(TimeParameter.ReportTimestep, (int)ReportTimestep.TotalSeconds);
			context.SetTimeParameter(TimeParameter.ReportStart, (int)ReportStart.TotalSeconds);
			context.SetTimeParameter(TimeParameter.RuleTimestep, (int)RuleTimestep.TotalSeconds);

			// Opções hidráulicas
			context.SetOption(OptionType.Trials, MaxTrials);
			context.SetOption(OptionType.Accuracy, Accuracy);
			context.SetOption(OptionType.UnbalancedContinue, UnbalancedContinue ? 1 : 0);
			context.SetOption(OptionType.UnbalancedTrials, UnbalancedTrials);
			context.SetOption(OptionType.SpecificGravity, SpecificGravity);
			context.SetOption(OptionType.Viscosity, Viscosity);

			// TODO: Aplicar FlowUnits e HeadlossFormula (requer EN_setoption)
		}

		/// <summary>
		/// Cria SimulationOptions a partir do projeto atual
		/// </summary>
		public static SimulationOptions FromProject(NativeContext context)
		{
			return new SimulationOptions
			{
				Duration = TimeSpan.FromSeconds(context.GetTimeParameter(TimeParameter.Duration)),
				HydraulicTimestep = TimeSpan.FromSeconds(context.GetTimeParameter(TimeParameter.HydraulicTimestep)),
				QualityTimestep = TimeSpan.FromSeconds(context.GetTimeParameter(TimeParameter.QualityTimestep)),
				// ... continuar leitura de todos os parâmetros
			};
		}
	}

	public enum QualityAnalysisType
	{
		None,
		Chemical,
		Age,
		Trace
	}
}
```

### Uso de SimulationOptions

```csharp
using (var project = Project.Open("rede.inp"))
{
	// Configurar simulação de 24 horas
	var options = new SimulationOptions
	{
		Duration = TimeSpan.FromHours(24),
		HydraulicTimestep = TimeSpan.FromHours(1),
		ReportTimestep = TimeSpan.FromHours(1),
		FlowUnits = FlowUnit.LPS,
		HeadlossFormula = HeadlossFormula.HazenWilliams,
		Accuracy = 0.001,
		MaxTrials = 200
	};

	options.ApplyTo(project.NativeContext);

	var results = project.Run();
}
```

---

## 4. SimulationResults - API Estruturada

```csharp
using System;
using System.Collections.Generic;

namespace EpanetSharp.Results
{
	public class SimulationResults
	{
		public NodeResults Nodes { get; }
		public LinkResults Links { get; }
		public List<DateTime> Timestamps { get; }
		public TimeSpan Duration { get; }

		internal SimulationResults(
			NodeResults nodes, 
			LinkResults links, 
			List<DateTime> timestamps,
			TimeSpan duration)
		{
			Nodes = nodes;
			Links = links;
			Timestamps = timestamps;
			Duration = duration;
		}

		public class NodeResults
		{
			// Dicionário: NodeId → TimeSeries
			public Dictionary<string, TimeSeries<double>> Pressure { get; }
			public Dictionary<string, TimeSeries<double>> Head { get; }
			public Dictionary<string, TimeSeries<double>> Demand { get; }
			public Dictionary<string, TimeSeries<double>> Quality { get; }

			internal NodeResults()
			{
				Pressure = new Dictionary<string, TimeSeries<double>>();
				Head = new Dictionary<string, TimeSeries<double>>();
				Demand = new Dictionary<string, TimeSeries<double>>();
				Quality = new Dictionary<string, TimeSeries<double>>();
			}
		}

		public class LinkResults
		{
			public Dictionary<string, TimeSeries<double>> Flow { get; }
			public Dictionary<string, TimeSeries<double>> Velocity { get; }
			public Dictionary<string, TimeSeries<double>> Headloss { get; }
			public Dictionary<string, TimeSeries<LinkStatus>> Status { get; }

			internal LinkResults()
			{
				Flow = new Dictionary<string, TimeSeries<double>>();
				Velocity = new Dictionary<string, TimeSeries<double>>();
				Headloss = new Dictionary<string, TimeSeries<double>>();
				Status = new Dictionary<string, TimeSeries<LinkStatus>>();
			}
		}
	}

	public class TimeSeries<T>
	{
		public List<DateTime> Timestamps { get; }
		public List<T> Values { get; }

		public TimeSeries()
		{
			Timestamps = new List<DateTime>();
			Values = new List<T>();
		}

		public void Add(DateTime timestamp, T value)
		{
			Timestamps.Add(timestamp);
			Values.Add(value);
		}

		public T GetValueAt(DateTime time)
		{
			int index = Timestamps.IndexOf(time);
			if (index < 0)
				throw new ArgumentException($"No value at timestamp {time}");
			return Values[index];
		}

		public T GetValueAt(int index)
		{
			return Values[index];
		}

		public int Count => Values.Count;
	}

	// Extensões para análise numérica
	public static class TimeSeriesExtensions
	{
		public static double Min(this TimeSeries<double> series)
		{
			if (series.Count == 0)
				throw new InvalidOperationException("TimeSeries is empty");

			double min = double.MaxValue;
			foreach (var value in series.Values)
			{
				if (value < min) min = value;
			}
			return min;
		}

		public static double Max(this TimeSeries<double> series)
		{
			if (series.Count == 0)
				throw new InvalidOperationException("TimeSeries is empty");

			double max = double.MinValue;
			foreach (var value in series.Values)
			{
				if (value > max) max = value;
			}
			return max;
		}

		public static double Average(this TimeSeries<double> series)
		{
			if (series.Count == 0)
				throw new InvalidOperationException("TimeSeries is empty");

			double sum = 0;
			foreach (var value in series.Values)
			{
				sum += value;
			}
			return sum / series.Count;
		}
	}

	public enum LinkStatus
	{
		Closed = 0,
		Open = 1,
		Active = 2
	}
}
```

### Collector de Resultados

```csharp
public class ResultsCollector
{
	private readonly NativeContext _context;
	private readonly Network _network;
	private readonly DateTime _startTime;

	public ResultsCollector(NativeContext context, Network network, DateTime? startTime = null)
	{
		_context = context;
		_network = network;
		_startTime = startTime ?? DateTime.Now;
	}

	public SimulationResults Collect()
	{
		var nodeResults = new SimulationResults.NodeResults();
		var linkResults = new SimulationResults.LinkResults();
		var timestamps = new List<DateTime>();

		long currentTime = 0;
		int timestepIndex = 0;

		// Executar simulação hidráulica
		_context.OpenHydraulics();
		_context.InitHydraulics(InitHydOption.SaveAndInit);

		do
		{
			// Resolver timestep
			_context.RunHydraulics(ref currentTime);

			// Coletar timestamp
			var timestamp = _startTime.AddSeconds(currentTime);
			timestamps.Add(timestamp);

			// Coletar resultados dos nós
			for (int i = 1; i <= _network.NodeCount; i++)
			{
				string nodeId = _context.GetNodeId(i);

				// Criar séries temporais se não existirem
				if (!nodeResults.Pressure.ContainsKey(nodeId))
				{
					nodeResults.Pressure[nodeId] = new TimeSeries<double>();
					nodeResults.Head[nodeId] = new TimeSeries<double>();
					nodeResults.Demand[nodeId] = new TimeSeries<double>();
					nodeResults.Quality[nodeId] = new TimeSeries<double>();
				}

				// Coletar valores
				double pressure = _context.GetNodeValue(i, NodeProperty.Pressure);
				double head = _context.GetNodeValue(i, NodeProperty.Head);
				double demand = _context.GetNodeValue(i, NodeProperty.Demand);
				double quality = _context.GetNodeValue(i, NodeProperty.Quality);

				nodeResults.Pressure[nodeId].Add(timestamp, pressure);
				nodeResults.Head[nodeId].Add(timestamp, head);
				nodeResults.Demand[nodeId].Add(timestamp, demand);
				nodeResults.Quality[nodeId].Add(timestamp, quality);
			}

			// Coletar resultados dos links
			for (int i = 1; i <= _network.LinkCount; i++)
			{
				string linkId = _context.GetLinkId(i);

				if (!linkResults.Flow.ContainsKey(linkId))
				{
					linkResults.Flow[linkId] = new TimeSeries<double>();
					linkResults.Velocity[linkId] = new TimeSeries<double>();
					linkResults.Headloss[linkId] = new TimeSeries<double>();
					linkResults.Status[linkId] = new TimeSeries<LinkStatus>();
				}

				double flow = _context.GetLinkValue(i, LinkProperty.Flow);
				double velocity = _context.GetLinkValue(i, LinkProperty.Velocity);
				double headloss = _context.GetLinkValue(i, LinkProperty.Headloss);
				int status = (int)_context.GetLinkValue(i, LinkProperty.Status);

				linkResults.Flow[linkId].Add(timestamp, flow);
				linkResults.Velocity[linkId].Add(timestamp, velocity);
				linkResults.Headloss[linkId].Add(timestamp, headloss);
				linkResults.Status[linkId].Add(timestamp, (LinkStatus)status);
			}

			timestepIndex++;

		} while (_context.NextHydraulicAnalysisTime(ref currentTime) > 0);

		_context.CloseHydraulics();

		var duration = TimeSpan.FromSeconds(currentTime);
		return new SimulationResults(nodeResults, linkResults, timestamps, duration);
	}
}
```

### Uso de SimulationResults

```csharp
using (var project = Project.Open("rede.inp"))
{
	// Configurar simulação
	var options = new SimulationOptions
	{
		Duration = TimeSpan.FromHours(24),
		HydraulicTimestep = TimeSpan.FromHours(1)
	};
	options.ApplyTo(project.NativeContext);

	// Executar e coletar resultados
	var collector = new ResultsCollector(project.NativeContext, project.Network);
	var results = collector.Collect();

	// Analisar resultados
	Console.WriteLine($"Simulation duration: {results.Duration}");
	Console.WriteLine($"Number of timesteps: {results.Timestamps.Count}");

	// Pressão mínima/máxima em cada nó
	foreach (var (nodeId, pressureSeries) in results.Nodes.Pressure)
	{
		Console.WriteLine($"Node {nodeId}:");
		Console.WriteLine($"  Min Pressure: {pressureSeries.Min():F2} m");
		Console.WriteLine($"  Max Pressure: {pressureSeries.Max():F2} m");
		Console.WriteLine($"  Avg Pressure: {pressureSeries.Average():F2} m");
	}

	// Vazão ao longo do tempo em um link específico
	if (results.Links.Flow.TryGetValue("P1", out var flowSeries))
	{
		Console.WriteLine("\nFlow in pipe P1 over time:");
		for (int i = 0; i < flowSeries.Count; i++)
		{
			Console.WriteLine($"  {results.Timestamps[i]:HH:mm}: {flowSeries.Values[i]:F3} m³/s");
		}
	}
}
```

---

**Continua...**

Este documento demonstra as implementações principais. Há muito mais a fazer, mas estes exemplos fornecem a base sólida para expandir o EpanetSharp com funcionalidades do WNTR.
