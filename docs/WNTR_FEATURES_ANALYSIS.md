# Análise de Funcionalidades WNTR para EpanetSharp

## 📋 Resumo Executivo

O projeto **WNTR-QGIS** é um plugin para QGIS que usa a biblioteca Python **WNTR** (Water Network Tool for Resilience) como wrapper do EPANET. Identificamos funcionalidades valiosas que podem ser incorporadas ao EpanetSharp.

---

## 🎯 Funcionalidades Principais Identificadas

### 1. **Sistema de Conversão de Unidades Automático**
**Localização**: `interface.py` - Classe `_Converter`

**O que faz**:
- Converte automaticamente entre unidades SI e outros sistemas (LPS, GPM, MGD, etc.)
- Suporta conversão de:
  - Vazão (flow)
  - Pressão (pressure/head)
  - Comprimento (length)
  - Diâmetro (diameter)
  - Coeficiente de rugosidade (roughness) - ajustado por fórmula de perda de carga
  - Energia (energy)
  - Potência (power)

**Benefício para EpanetSharp**:
- Usuários podem trabalhar em suas unidades preferidas
- API converte automaticamente para SI antes de enviar ao EPANET
- Resultados convertidos de volta para unidades do usuário

**Implementação sugerida**:
```csharp
public class UnitConverter
{
	private readonly FlowUnit _flowUnit;
	private readonly HeadlossFormula _headlossFormula;

	public double ToSI(double value, HydraulicParameter param) { }
	public double FromSI(double value, HydraulicParameter param) { }

	// Conversões específicas
	public double FlowToSI(double value) { }
	public double PressureToSI(double value) { }
	public double DiameterToSI(double value) { }
	public double RoughnessToSI(double value) { }
}
```

---

### 2. **Validação de Rede (Network Check)**
**Localização**: `interface.py` - Função `check_network(wn)`

**O que faz**:
- Valida a integridade da rede antes de simular
- Verifica:
  - Conectividade dos nós
  - Links sem nós
  - Nós órfãos (sem conexão)
  - Valores inválidos (diâmetros negativos, etc.)
  - Consistência de dados

**Benefício para EpanetSharp**:
- Erros detectados **antes** de chamar EN_run
- Mensagens de erro mais claras e específicas
- Melhor experiência do desenvolvedor

**Implementação sugerida**:
```csharp
public class NetworkValidator
{
	public ValidationResult Validate(Network network)
	{
		var errors = new List<ValidationError>();

		// Verificar conectividade
		ValidateConnectivity(network, errors);

		// Verificar valores
		ValidateNodeValues(network, errors);
		ValidateLinkValues(network, errors);

		return new ValidationResult(errors);
	}
}
```

---

### 3. **Import/Export de INP com Conversão de Unidades**
**Localização**: `import_inp.py`

**O que faz**:
- Lê arquivo INP do EPANET
- Converte automaticamente para unidades desejadas
- Preserva metadados e opções

**Benefício para EpanetSharp**:
- Usuário pode importar INP em unidades imperiais e trabalhar em SI
- Exportar em formato compatível com EPANET original
- Flexibilidade para diferentes mercados (US vs Internacional)

**Implementação sugerida**:
```csharp
public class InpImporter
{
	public Network ImportInp(string filePath, FlowUnit targetUnit = FlowUnit.SI)
	{
		// Usar EN_open
		var network = Project.Open(filePath).Network;

		// Se targetUnit != unidade do arquivo, converter
		if (targetUnit != network.FlowUnit)
		{
			ConvertUnits(network, targetUnit);
		}

		return network;
	}
}
```

---

### 4. **Enumerações e Tipos Fortemente Tipados**
**Localização**: `elements.py`

**O que define**:
```python
class FlowUnit(Enum):
	LPS, LPM, MLD, CMH, CFS, GPM, MGD, IMGD, AFD, SI

class HeadlossFormula(Enum):
	HAZEN_WILLIAMS = "H-W"
	DARCY_WEISBACH = "D-W"
	CHEZY_MANNING = "C-M"

class ValveType(Enum):
	PRV, PSV, PBV, FCV, TCV, GPV

class InitialStatus(Enum):
	OPEN, CLOSED, ACTIVE

class PumpTypes(Enum):
	POWER, HEAD
```

**Benefício para EpanetSharp**:
- Já temos algumas enums, mas expandir cobertura
- Adicionar métodos de conversão string ↔ enum
- Adicionar `FriendlyName` para exibição

**Já temos parcialmente no EpanetSharp**, mas podemos:
```csharp
public enum FlowUnit
{
	[DisplayName("Litres per Second")]
	LPS,

	[DisplayName("Gallons per Minute")]
	GPM,

	[DisplayName("International System (SI)")]
	SI,
	// ... etc
}

public static class FlowUnitExtensions
{
	public static string GetFriendlyName(this FlowUnit unit) { }
	public static FlowUnit Parse(string value) { }
}
```

---

### 5. **Writer para Resultados de Simulação**
**Localização**: `interface.py` - Classe `Writer`

**O que faz**:
- Extrai resultados da simulação
- Organiza por:
  - **Nodes** (junctions, tanks, reservoirs)
	- Pressure, Head, Demand, Quality
  - **Links** (pipes, pumps, valves)
	- Flow, Velocity, Headloss, Status
- Suporta séries temporais (múltiplos timesteps)
- Converte unidades dos resultados

**Benefício para EpanetSharp**:
- API mais amigável para acessar resultados
- Typed accessors para cada propriedade
- Suporte a análise temporal

**Implementação sugerida**:
```csharp
public class SimulationResults
{
	public NodeResults Nodes { get; }
	public LinkResults Links { get; }

	public class NodeResults
	{
		public Dictionary<string, TimeSeries<double>> Pressure { get; }
		public Dictionary<string, TimeSeries<double>> Head { get; }
		public Dictionary<string, TimeSeries<double>> Demand { get; }
		public Dictionary<string, TimeSeries<double>> Quality { get; }
	}

	public class LinkResults
	{
		public Dictionary<string, TimeSeries<double>> Flow { get; }
		public Dictionary<string, TimeSeries<double>> Velocity { get; }
		public Dictionary<string, TimeSeries<double>> Headloss { get; }
		public Dictionary<string, TimeSeries<LinkStatus>> Status { get; }
	}
}

public class TimeSeries<T>
{
	public List<DateTime> Timestamps { get; }
	public List<T> Values { get; }

	public T GetValueAt(DateTime time) { }
	public T Min() { }
	public T Max() { }
	public T Average() { } // where T : numeric
}
```

---

### 6. **Configurações e Opções Avançadas**
**Localização**: `run_simulation.py`, `settings.py`

**O que oferece**:
- Duração da simulação configurável
- Unidades de vazão configuráveis
- Fórmula de perda de carga configurável
- Persistência de configurações entre execuções
- Patterns e Curves

**Benefício para EpanetSharp**:
- API mais rica para configurar simulações
- Não precisar editar INP manualmente
- Configuração programática de todos os aspectos

**Implementação sugerida**:
```csharp
public class SimulationOptions
{
	public TimeSpan Duration { get; set; }
	public TimeSpan HydraulicTimestep { get; set; }
	public TimeSpan QualityTimestep { get; set; }
	public TimeSpan ReportTimestep { get; set; }
	public FlowUnit FlowUnits { get; set; }
	public HeadlossFormula HeadlossFormula { get; set; }
	public double SpecificGravity { get; set; } = 1.0;
	public double Viscosity { get; set; } = 1.0;
	public int MaxTrials { get; set; } = 200;
	public double Accuracy { get; set; } = 0.001;

	public void ApplyTo(Project project) { }
}

// Uso
var options = new SimulationOptions
{
	Duration = TimeSpan.FromHours(24),
	HydraulicTimestep = TimeSpan.FromHours(1),
	FlowUnits = FlowUnit.LPS,
	HeadlossFormula = HeadlossFormula.HazenWilliams
};

project.Configure(options);
project.Run();
```

---

### 7. **Progress Tracking**
**Localização**: `common.py` - Classes `ProgressTracker`, `Progression`

**O que faz**:
- Reporta progresso da simulação
- Fases: Loading → Preparing → Running → Creating Outputs → Finished
- Permite cancelamento

**Benefício para EpanetSharp**:
- UIs podem mostrar barra de progresso
- Apps console podem mostrar status
- Possibilidade de cancelar simulações longas

**Implementação sugerida**:
```csharp
public enum SimulationPhase
{
	Loading,
	PreparingModel,
	RunningSimulation,
	CreatingOutputs,
	Finished
}

public class SimulationProgress
{
	public SimulationPhase Phase { get; set; }
	public double Percentage { get; set; }
	public string Message { get; set; }
}

public class Project
{
	public event EventHandler<SimulationProgress> ProgressChanged;

	public async Task<SimulationResults> RunAsync(
		CancellationToken cancellationToken = default)
	{
		ReportProgress(SimulationPhase.Loading, 0, "Loading network...");
		// ...
		ReportProgress(SimulationPhase.RunningSimulation, 50, "Running hydraulic simulation...");
		// ...
	}
}
```

---

### 8. **Field Groups (Agrupamento de Campos)**
**Localização**: `elements.py` - `FieldGroup`

**O que faz**:
- Organiza propriedades por categoria:
  - **BASE**: Propriedades essenciais (ID, coordenadas, demanda base)
  - **WATER_QUALITY_ANALYSIS**: Campos de qualidade de água
  - **PRESSURE_DEPENDENT_DEMAND**: Análise PDD
  - **ENERGY**: Análise energética
  - **EXTRA**: Campos opcionais/avançados

**Benefício para EpanetSharp**:
- APIs podem oferecer diferentes níveis de complexidade
- Iniciantes veem só campos BASE
- Avançados acessam ENERGY, WQ, etc.

**Implementação sugerida**:
```csharp
public class Junction
{
	// BASE
	public string Id { get; set; }
	public Point Coordinates { get; set; }
	public double Elevation { get; set; }
	public double BaseDemand { get; set; }

	// WATER_QUALITY_ANALYSIS
	[Advanced]
	public double? InitialQuality { get; set; }

	// PRESSURE_DEPENDENT_DEMAND
	[Advanced]
	public double? MinimumPressure { get; set; }
	[Advanced]
	public double? RequiredPressure { get; set; }

	// EXTRA
	[Advanced]
	public string Tag { get; set; }
}
```

---

## 🚀 Roadmap de Implementação Sugerido

### Fase 1 - Fundação (Mais Importante)
1. ✅ **Unit Converter** - Sistema de conversão de unidades
2. ✅ **Enums & Constants** - Expandir enumerações existentes
3. ✅ **SimulationOptions** - Classe de configuração

### Fase 2 - Validação e Qualidade
4. **NetworkValidator** - Validação de rede antes da simulação
5. **Error Messages** - Mensagens de erro mais amigáveis

### Fase 3 - Resultados
6. **SimulationResults** - API estruturada para resultados
7. **TimeSeries** - Suporte a séries temporais

### Fase 4 - Experiência do Desenvolvedor
8. **Progress Tracking** - Reporte de progresso
9. **Async/Await** - Simulações assíncronas
10. **InpImporter/Exporter** - Import/export com conversão de unidades

### Fase 5 - Avançado
11. **Field Groups** - Organização de propriedades por nível
12. **Patterns & Curves** - Suporte completo a padrões e curvas

---

## 📊 Comparação Funcional

| Funcionalidade | WNTR-QGIS | EpanetSharp Atual | Prioridade |
|----------------|-----------|-------------------|------------|
| Conversão de Unidades | ✅ Completo | ❌ Não tem | 🔴 Alta |
| Validação de Rede | ✅ Completo | ❌ Não tem | 🔴 Alta |
| Enums Fortemente Tipados | ✅ Completo | 🟡 Parcial | 🟡 Média |
| Resultados Estruturados | ✅ Completo | ❌ Não tem | 🔴 Alta |
| Séries Temporais | ✅ Completo | ❌ Não tem | 🟡 Média |
| Progress Tracking | ✅ Completo | ❌ Não tem | 🟢 Baixa |
| Simulação Assíncrona | ❌ Não tem | ❌ Não tem | 🟢 Baixa |
| Import INP com Conversão | ✅ Completo | ❌ Não tem | 🟡 Média |
| Options Programáticas | ✅ Completo | 🟡 Parcial | 🔴 Alta |

---

## 💡 Insights Arquiteturais

### 1. Separação de Responsabilidades
WNTR separa claramente:
- **Elements**: Definições de tipos, enums
- **Interface**: Conversão WNTR ↔ formato externo
- **Processing**: Lógica de processamento/simulação

Podemos aplicar no EpanetSharp:
```
EpanetSharp/
  ├── Core/           # Project, Network (mantém)
  ├── Elements/       # Entities: Junction, Pipe, Tank, etc.
  ├── Simulation/     # Options, Results, Validator
  ├── Units/          # UnitConverter, FlowUnit, etc.
  └── Native/         # P/Invoke (mantém)
```

### 2. Lazy Loading de Dependências
WNTR carrega numpy/pandas apenas quando necessário.

No EpanetSharp podemos:
- Carregar DLL nativa apenas quando `Project.Open()` é chamado
- Lazy load de patterns/curves
- Resultados sob demanda

### 3. Error Handling Rico
WNTR cria exceções customizadas:
- `NetworkModelError`
- `EpanetException`

EpanetSharp pode ter:
```csharp
public class EpanetException : Exception { }
public class NetworkValidationException : EpanetException { }
public class SimulationException : EpanetException { }
public class NativeLibraryException : EpanetException { }
```

---

## 🎯 Próximos Passos Recomendados

1. **Criar Issue/Milestone no GitHub** para cada fase
2. **Começar pela Fase 1** (Unit Converter + Enums)
3. **Escrever testes** para cada nova funcionalidade
4. **Manter compatibilidade** com API atual (não quebrar existing code)
5. **Documentar** cada feature com exemplos

---

## 📚 Referências

- WNTR Documentation: https://usepa.github.io/WNTR/
- EPANET 2.2 Documentation: https://epanet22.readthedocs.io/
- Código fonte WNTR-QGIS analisado: `C:\ProjetosClaude\EpanetSharp\WNTR\wntrqgis\`

---

## ⚠️ Notas de Implementação

### Cuidados ao Implementar Unit Conversion

O WNTR usa a biblioteca oficial `wntr.epanet.util.to_si()` e `from_si()` que é baseada no código do próprio EPANET. **Devemos usar as mesmas fórmulas de conversão** para garantir compatibilidade.

### Roughness Coefficient

**Atenção especial**: O coeficiente de rugosidade tem conversão diferente dependendo da fórmula:
- **Hazen-Williams**: Adimensional (mesmo valor em qualquer unidade)
- **Darcy-Weisbach**: Em milímetros ou pés dependendo do sistema
- **Chezy-Manning**: Diferente ainda

O `UnitConverter` precisa conhecer a `HeadlossFormula` ativa.

### Thread Safety

Se implementarmos simulações assíncronas, precisamos garantir:
- DLL nativa pode não ser thread-safe
- Um projeto por thread
- Ou lock global

---

**Documento criado em**: 2026-07-13  
**Autor**: Análise automatizada do código WNTR-QGIS  
**Versão**: 1.0
