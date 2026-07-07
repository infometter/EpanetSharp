# API Roadmap — EpanetSharp

Este documento descreve a API pública planejada para a biblioteca EpanetSharp. O objetivo é fornecer
uma visão clara das classes, propriedades, métodos e eventos que farão parte da API pública
quando a implementação estiver completa. O documento não implementa funcionalidade — é um
guia de design e contrato.

Todas as assinaturas apresentadas são propostas e poderão ser ajustadas durante a implementação,
mantendo-se a compatibilidade semântica sempre que possível.

Observação: nomes de tipos e membros estão em PascalCase conforme convenção .NET.

---

## Project

Representa um projeto EPANET em memória e orquestra operações de carregamento, execução e
exportação.

Principais responsabilidades:
- Criar/abrir/fechar projetos
- Acessar Network associada
- Executar simulações e controlar o ciclo de vida
- Ler/escrever arquivos INP, RPT, BIN (quando aplicável)

API pública (exemplos de assinaturas):
- static Project Open(string inpFile)
- void Open()
- void Close()
- void Run()
- Task RunAsync(CancellationToken ct)
- string? InputFilePath { get; }
- Network Network { get; }
- void Save(string path)
- void ExportReport(string path)
- string Version { get; }

Comportamento:
- Lançar ObjectDisposedException quando usado após Dispose.
- Validar argumentos com ArgumentNullException / ArgumentException.
- Erros provenientes da camada nativa EPANET geram EpanetException.

---

## Network

Representa a rede hidráulica do projeto: conjunto de elementos e configurações.

Responsabilidades:
- Manter coleções de elementos (Nodes, Links, Patterns, Curves)
- Fornecer APIs de consulta (busca por Id, por índice)
- Aplicar alterações estruturais à rede (adicionar/remover)

API pública proposta:
- NodeCollection Nodes { get; }
- LinkCollection Links { get; }
- PatternCollection Patterns { get; }
- CurveCollection Curves { get; }
- Options Options { get; }
- void Clear()
- IEnumerable<T> Find<T>(Func<T,bool> predicate) where T : NetworkElement

Observações:
- Coleções expostas como tipos especializados que implementam IReadOnlyCollection para leitura,
  com métodos controlados para modificação.

---

## Simulation

Responsabilidades:
- Configurar e executar simulações hidráulicas e de qualidade de água
- Fornecer hooks para monitoramento em tempo de execução e coleta de resultados

API pública planejada:
- SimulationOptions SimulationOptions { get; set; }
- SimulationResults RunSimulation()
- IAsyncEnumerable<SimulationProgress> RunSimulationAsync(SimulationOptions options, CancellationToken ct)
- event EventHandler<SimulationProgressEventArgs> ProgressChanged
- event EventHandler<SimulationCompletedEventArgs> SimulationCompleted

Notas:
- SimulationResults conterá séries temporais, valores instantâneos, e sumários.

---

## Nodes

Tipos e responsabilidades:
- Node (base abstrata) — propriedades comuns (Id, Index, Tag, Comment)
- Junction, Reservoir, Tank — especializações com propriedades próprias

Propriedades comuns (ex.):
- string Id { get; set; }
- int Index { get; set; }
- string? Tag { get; set; }
- string? Comment { get; set; }
- double? Elevation { get; set; } // em Junction
- double? InitialLevel { get; set; } // em Tank

Operações:
- void SetDemand(Pattern pattern, double factor)
- IEnumerable<TimeSeriesPoint> GetTimeSeries(SeriesType type)

---

## Links

Tipos e responsabilidades:
- Link (base abstrata)
- Pipe, Pump, Valve — especializações com propriedades próprias

Propriedades comuns (ex.):
- string Id { get; set; }
- int Index { get; set; }
- string? Tag { get; set; }
- string? Comment { get; set; }
- double Length { get; set; }
- double Diameter { get; set; }
- double Roughness { get; set; }

Operações/consultas:
- void SetStatus(LinkStatus status)
- LinkStatus GetStatus()
- IEnumerable<TimeSeriesPoint> GetTimeSeries(SeriesType type)

---

## Patterns

Representam fatores temporais de demanda.

Propriedades:
- string Id { get; set; }
- IReadOnlyList<double> Factors { get; }

Operações:
- void SetFactors(IEnumerable<double> values)
- double GetFactorAt(int step)

---

## Curves

Curvas usadas por bombas e perdas.

Propriedades:
- string Id { get; set; }
- IReadOnlyList<(double X,double Y)> Points { get; }

Operações:
- void AddPoint(double x, double y)
- (double X,double Y)[] GetPoints()

---

## Rules

Regras operacionais (opcional, para controlar comportamentos dependentes de tempo ou condição).

Conceito/assinaturas:
- RuleCollection Rules { get; }
- Rule: condicional + ações (ex.: quando pressão < X então abrir válvula)
- Interfaces para criação e validação de regras

---

## Controls

Representação de controles discretos: válvulas, bombas controladas por regras, timers.

API prevista:
- ControlCollection Controls { get; }
- Control: TargetId, Condition, Action

---

## Reporting

Responsabilidades:
- Gerar relatórios sumarizados e detalhados (formato texto/CSV/JSON)
- Exportar resultados e estados finais

API pública proposta:
- ReportGenerator CreateReport(ReportOptions options)
- void ExportReport(string path, ReportOptions options)
- event EventHandler<ReportProgressEventArgs> ReportProgress

---

## Opções (Options)

Configurações globais da simulação/rede.

Propriedades previstas:
- TimeStepSeconds
- DurationSeconds
- ReportStart
- ReportStep
- HydraulicsSolverOptions

---

## Eventos

Eventos gerais esperados:
- ProjectOpened / ProjectClosed
- SimulationStarted / SimulationProgress / SimulationCompleted
- ReportGenerated
- ElementAdded / ElementRemoved (para Network)

Assinaturas típicas:
- event EventHandler<ProjectEventArgs> ProjectOpened

---

## Versionamento

Política inicial proposta:
- Assembly version (semântico): Major.Minor.Patch
- Public API: manter compatibilidade binária dentro de Minor/patch releases; quebrar Major quando necessário
- Documentar breaking changes no changelog

---

Notas finais

- Este roadmap prioriza clareza da API pública e separação de responsabilidades (Core, Elements, Simulation,
  Reporting, Native). Implementações concretas (P/Invoke, algoritmos de solução) serão adicionadas em etapas posteriores.
- Antes de qualquer mudança que quebre API pública, atualizar este documento e seguir um plano de migração.
