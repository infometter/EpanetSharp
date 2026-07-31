# 📋 Checklist de Implementação - Features WNTR

Use este checklist para acompanhar a implementação das funcionalidades identificadas na análise do WNTR.

---

## 🎯 Sprint 1 - Fundação (Duração: 1 semana)

### ✅ Unit Converter (3 dias)

- [ ] **Dia 1: Estrutura Base**
  - [ ] Criar namespace `EpanetSharp.Units`
  - [ ] Criar enum `FlowUnit` com todos os valores
	- [ ] LPS, LPM, MLD, CMH (métrico)
	- [ ] CFS, GPM, MGD, IMGD, AFD (imperial)
	- [ ] SI (sistema internacional)
  - [ ] Criar enum `HydraulicParameter`
	- [ ] Flow, Pressure, Head, Diameter, Length
	- [ ] Roughness, Elevation, Velocity, Volume
	- [ ] Demand, Power, Energy, Quality
  - [ ] Criar classe `UnitConverter`
	- [ ] Constructor(FlowUnit, HeadlossFormula)
	- [ ] Método `ToSI(double value, HydraulicParameter param)`
	- [ ] Método `FromSI(double value, HydraulicParameter param)`

- [ ] **Dia 2: Implementar Conversões**
  - [ ] Implementar conversão de Flow
  - [ ] Implementar conversão de Pressure/Head
  - [ ] Implementar conversão de Diameter
  - [ ] Implementar conversão de Length
  - [ ] Implementar conversão de Roughness (especial!)
	- [ ] Hazen-Williams (adimensional)
	- [ ] Darcy-Weisbach (mm ou ft)
	- [ ] Chezy-Manning (s/m^1/3 ou s/ft^1/3)
  - [ ] Implementar conversão de Velocity
  - [ ] Implementar conversão de Volume
  - [ ] Implementar conversão de Power

- [ ] **Dia 3: Testes e Validação**
  - [ ] Criar `UnitConverterTests.cs`
  - [ ] Testar conversão Flow: GPM → SI → GPM (round-trip)
  - [ ] Testar conversão Flow: LPS → SI → LPS
  - [ ] Testar conversão Pressure: ft → m → ft
  - [ ] Testar conversão Diameter: in → mm → in
  - [ ] Testar conversão Roughness com H-W
  - [ ] Testar conversão Roughness com D-W
  - [ ] Testar valores especiais (NaN, Infinity, 0)
  - [ ] Documentar uso no README
  - [ ] Criar exemplo em `examples/UnitConversionExample.cs`

---

### ✅ Enums Expandidos (1 dia)

- [ ] **Expandir FlowUnit**
  - [ ] Adicionar atributo `[DisplayName("...")]`
  - [ ] Criar extension method `GetFriendlyName()`
  - [ ] Criar extension method `Parse(string value)`
  - [ ] Criar extension method `IsMetric()`

- [ ] **Criar ValveType enum**
  - [ ] PRV - Pressure Reducing Valve
  - [ ] PSV - Pressure Sustaining Valve
  - [ ] PBV - Pressure Breaking Valve
  - [ ] FCV - Flow Control Valve
  - [ ] TCV - Throttle Control Valve
  - [ ] GPV - General Purpose Valve

- [ ] **Criar InitialStatus enum**
  - [ ] Open
  - [ ] Closed
  - [ ] Active (para bombas)

- [ ] **Criar PumpType enum**
  - [ ] Power
  - [ ] Head

- [ ] **Testes**
  - [ ] Testar parse de string para enum
  - [ ] Testar friendly names
  - [ ] Documentar no README

---

### ✅ SimulationOptions (1 dia)

- [ ] **Criar classe `SimulationOptions`**
  - [ ] Propriedades de tempo
	- [ ] Duration (TimeSpan)
	- [ ] HydraulicTimestep (TimeSpan)
	- [ ] QualityTimestep (TimeSpan)
	- [ ] PatternTimestep (TimeSpan)
	- [ ] ReportTimestep (TimeSpan)
	- [ ] ReportStart (TimeSpan)
	- [ ] RuleTimestep (TimeSpan)
  - [ ] Propriedades hidráulicas
	- [ ] FlowUnits (FlowUnit)
	- [ ] HeadlossFormula (HeadlossFormula)
	- [ ] SpecificGravity (double)
	- [ ] Viscosity (double)
	- [ ] MaxTrials (int)
	- [ ] Accuracy (double)
	- [ ] UnbalancedContinue (bool)
	- [ ] UnbalancedTrials (int)

- [ ] **Métodos**
  - [ ] `ApplyTo(NativeContext context)` - Aplica ao projeto EPANET
  - [ ] `FromProject(NativeContext context)` - Lê do projeto atual

- [ ] **Testes**
  - [ ] Testar set/get de cada propriedade
  - [ ] Testar aplicação ao NativeContext
  - [ ] Testar leitura do NativeContext
  - [ ] Documentar exemplo de uso

---

## 🎯 Sprint 2 - Qualidade (Duração: 1 semana)

### ✅ Network Validator (3 dias)

- [ ] **Dia 1: Estrutura**
  - [ ] Criar namespace `EpanetSharp.Validation`
  - [ ] Criar classe `NetworkValidator`
	- [ ] Constructor(Network network)
	- [ ] Método `Validate()` → `ValidationResult`
  - [ ] Criar classe `ValidationResult`
	- [ ] Propriedade `List<ValidationError> Errors`
	- [ ] Propriedade bool `IsValid`
	- [ ] Propriedade bool `HasWarnings`
	- [ ] Método `ThrowIfInvalid()`
  - [ ] Criar classe `ValidationError`
	- [ ] Propriedade `ValidationSeverity Severity`
	- [ ] Propriedade `string Message`
  - [ ] Criar enum `ValidationSeverity`
	- [ ] Info, Warning, Error
  - [ ] Criar exceção `NetworkValidationException`

- [ ] **Dia 2: Validações de Nodes**
  - [ ] Validar contagem de nós (> 0)
  - [ ] Validar nós órfãos (sem links)
  - [ ] Validar elevação (não NaN/Infinity)
  - [ ] Validar demanda base (junctions)
  - [ ] Validar tanques
	- [ ] MinLevel < MaxLevel
	- [ ] MinLevel <= InitLevel <= MaxLevel
	- [ ] Diameter > 0
  - [ ] Validar reservatórios
	- [ ] Total head > 0

- [ ] **Dia 3: Validações de Links e Testes**
  - [ ] Validar contagem de links
  - [ ] Validar diâmetro > 0
  - [ ] Validar comprimento > 0
  - [ ] Validar rugosidade > 0
  - [ ] Validar bombas
	- [ ] Curva de bomba válida
  - [ ] Validar válvulas
	- [ ] Tipo válido
	- [ ] Setting apropriado ao tipo
  - [ ] **Testes**
	- [ ] Testar validação com rede válida
	- [ ] Testar detecção de nó órfão
	- [ ] Testar detecção de diâmetro negativo
	- [ ] Testar detecção de tanque inválido
	- [ ] Documentar uso

---

### ✅ Melhorias de Error Messages (1 dia)

- [ ] **Criar exceções customizadas**
  - [ ] `EpanetException` (base)
  - [ ] `NetworkValidationException : EpanetException`
  - [ ] `SimulationException : EpanetException`
  - [ ] `NativeLibraryException : EpanetException`

- [ ] **Melhorar mensagens**
  - [ ] Incluir código de erro EPANET
  - [ ] Incluir contexto (node/link ID)
  - [ ] Sugerir solução quando possível

- [ ] **Testes**
  - [ ] Testar cada tipo de exceção
  - [ ] Verificar mensagens claras

---

### ✅ Integration Tests (2 dias)

- [ ] **Criar testes de integração**
  - [ ] Testar validação em Net1.inp
  - [ ] Testar conversão de unidades end-to-end
  - [ ] Testar aplicação de SimulationOptions
  - [ ] Testar detecção de erros comuns

- [ ] **Criar redes de teste**
  - [ ] Rede válida simples
  - [ ] Rede com nó órfão
  - [ ] Rede com diâmetro inválido
  - [ ] Rede com tanque inválido

---

## 🎯 Sprint 3 - Resultados (Duração: 1-2 semanas)

### ✅ SimulationResults & TimeSeries (5 dias)

- [ ] **Dia 1: Estrutura Base**
  - [ ] Criar namespace `EpanetSharp.Results`
  - [ ] Criar classe `TimeSeries<T>`
	- [ ] `List<DateTime> Timestamps`
	- [ ] `List<T> Values`
	- [ ] Método `Add(DateTime, T)`
	- [ ] Método `GetValueAt(DateTime)` e `GetValueAt(int)`
	- [ ] Propriedade `Count`
  - [ ] Criar extension methods `TimeSeriesExtensions`
	- [ ] `Min()` para `TimeSeries<double>`
	- [ ] `Max()` para `TimeSeries<double>`
	- [ ] `Average()` para `TimeSeries<double>`

- [ ] **Dia 2: SimulationResults**
  - [ ] Criar classe `SimulationResults`
	- [ ] Propriedade `NodeResults Nodes`
	- [ ] Propriedade `LinkResults Links`
	- [ ] Propriedade `List<DateTime> Timestamps`
	- [ ] Propriedade `TimeSpan Duration`
  - [ ] Criar classe `NodeResults`
	- [ ] `Dictionary<string, TimeSeries<double>> Pressure`
	- [ ] `Dictionary<string, TimeSeries<double>> Head`
	- [ ] `Dictionary<string, TimeSeries<double>> Demand`
	- [ ] `Dictionary<string, TimeSeries<double>> Quality`
  - [ ] Criar classe `LinkResults`
	- [ ] `Dictionary<string, TimeSeries<double>> Flow`
	- [ ] `Dictionary<string, TimeSeries<double>> Velocity`
	- [ ] `Dictionary<string, TimeSeries<double>> Headloss`
	- [ ] `Dictionary<string, TimeSeries<LinkStatus>> Status`
  - [ ] Criar enum `LinkStatus`
	- [ ] Closed, Open, Active

- [ ] **Dia 3-4: ResultsCollector**
  - [ ] Criar classe `ResultsCollector`
	- [ ] Constructor(NativeContext, Network, DateTime?)
	- [ ] Método `Collect()` → `SimulationResults`
  - [ ] Implementar coleta durante simulação
	- [ ] Loop por timesteps
	- [ ] Coletar valores de cada nó
	- [ ] Coletar valores de cada link
	- [ ] Armazenar em TimeSeries

- [ ] **Dia 5: Testes e Documentação**
  - [ ] Testar coleta em simulação steady-state
  - [ ] Testar coleta em simulação extended period
  - [ ] Testar análise Min/Max/Average
  - [ ] Documentar uso
  - [ ] Criar exemplo `ResultsAnalysisExample.cs`

---

## 🎯 Sprint 4 - Polimento (Duração: 1 semana)

### ✅ Documentação (2 dias)

- [ ] **Atualizar README.md**
  - [ ] Adicionar seção "Unit Conversion"
  - [ ] Adicionar seção "Network Validation"
  - [ ] Adicionar seção "Simulation Results"
  - [ ] Adicionar quickstart atualizado

- [ ] **Criar exemplos completos**
  - [ ] `examples/UnitConversionExample.cs`
  - [ ] `examples/NetworkValidationExample.cs`
  - [ ] `examples/ResultsAnalysisExample.cs`
  - [ ] `examples/ConfigureSimulationExample.cs`

- [ ] **Documentação XML**
  - [ ] Adicionar XML docs em todas as classes públicas
  - [ ] Gerar arquivo de IntelliSense

---

### ✅ Async/Await (2 dias)

- [ ] **Criar métodos assíncronos**
  - [ ] `Project.RunAsync(CancellationToken)`
  - [ ] `Project.ValidateAsync()`
  - [ ] `ResultsCollector.CollectAsync()`

- [ ] **Thread safety**
  - [ ] Verificar se DLL é thread-safe
  - [ ] Implementar locks se necessário

- [ ] **Testes**
  - [ ] Testar cancelamento
  - [ ] Testar múltiplas simulações concorrentes

---

### ✅ Progress Tracking (1 dia)

- [ ] **Criar sistema de eventos**
  - [ ] Criar enum `SimulationPhase`
	- [ ] Loading, PreparingModel, RunningSimulation
	- [ ] CreatingOutputs, Finished
  - [ ] Criar classe `SimulationProgress`
	- [ ] `SimulationPhase Phase`
	- [ ] `double Percentage`
	- [ ] `string Message`
  - [ ] Adicionar event `Project.ProgressChanged`

- [ ] **Implementar reportes**
  - [ ] Reportar progresso em cada fase
  - [ ] Calcular percentual baseado em timesteps

- [ ] **Testes**
  - [ ] Verificar eventos disparados
  - [ ] Testar UI simples com barra de progresso

---

### ✅ Performance (2 dias)

- [ ] **Profiling**
  - [ ] Medir performance de conversão de unidades
  - [ ] Medir performance de validação
  - [ ] Medir performance de coleta de resultados

- [ ] **Otimizações**
  - [ ] Cache de fatores de conversão
  - [ ] Lazy loading de resultados
  - [ ] Parallel processing (se aplicável)

- [ ] **Benchmarks**
  - [ ] Criar benchmarks com BenchmarkDotNet
  - [ ] Comparar antes/depois de otimizações

---

## 📊 Métricas de Conclusão

### Fase 1 - Fundação
- [ ] 100% de cobertura de testes em UnitConverter
- [ ] 100% de cobertura de testes em Enums
- [ ] README atualizado com exemplos

### Fase 2 - Qualidade
- [ ] NetworkValidator detecta 10+ tipos de erro
- [ ] 90%+ de cobertura de testes
- [ ] Mensagens de erro claras e acionáveis

### Fase 3 - Resultados
- [ ] SimulationResults cobre todos os parâmetros principais
- [ ] TimeSeries com análise estatística
- [ ] Exemplos funcionais

### Fase 4 - Polimento
- [ ] Documentação XML completa
- [ ] 5+ exemplos práticos
- [ ] Performance <5% overhead vs wrapper básico

---

## ✅ Critérios de Aceitação Geral

- [ ] **Build sem warnings**
- [ ] **Todas as features com testes unitários**
- [ ] **Cobertura de testes > 80%**
- [ ] **Documentação XML em APIs públicas**
- [ ] **README com quickstart completo**
- [ ] **Compatibilidade .NET Framework 4.6.1+**
- [ ] **Compatibilidade .NET 10+**
- [ ] **Zero breaking changes na API existente**

---

**Última atualização**: 2026-07-13  
**Versão**: 1.0  
**Status**: 📋 Pronto para execução
