# 🎨 Análise de Funcionalidades Visuais e Avançadas do WNTR-QGIS

## 📋 Resumo Executivo

Análise da parte visual/QGIS do projeto WNTR que foi inicialmente descartada. **Descoberta**: Não há funcionalidades específicas de **DMCs** (District Metered Areas) ou algoritmos de particionamento de rede no código WNTR-QGIS analisado.

---

## 🔍 O que Foi Encontrado

### 1. **Sistema de Visualização Avançada** ✅
**Localização**: `style.py`

**Funcionalidades**:
- **Graduated Symbol Renderer**: Coloração por faixas de valores
  - Usa classificação por quantis (5 classes)
  - Color ramp "Spectral" invertido (azul → vermelho)
  - Aplicado em:
	- **Nós**: Pressão (`pressure`)
	- **Links**: Velocidade (`velocity`)

- **Símbolos Customizados**:
  - **Junctions**: Círculo branco com borda fina
  - **Tanks**: Quadrado branco
  - **Reservoirs**: Trapézio (voltado para baixo)
  - **Pipes**: Linha com seta indicando direção de fluxo
  - **Pumps**: Símbolo de bomba (círculo + triângulo de saída)
  - **Valves**: Triângulo

**Exemplo de uso**:
```python
# Colorir nós por pressão em 5 faixas
renderer = QgsGraduatedSymbolRenderer()
renderer.setClassAttribute("pressure")
renderer.updateClasses(layer, 5)
color_ramp = QgsStyle().defaultStyle().colorRamp("Spectral")
color_ramp.invert()  # Azul (baixa pressão) → Vermelho (alta pressão)
renderer.updateColorRamp(color_ramp)
```

**Aplicável ao EpanetSharp?**
- ❌ Não diretamente (requer biblioteca GIS)
- ✅ Mas podemos exportar dados para visualização em outras ferramentas
- ✅ Podemos calcular as faixas de valores para coloração

---

### 2. **Visualização Temporal (Animação)** ✅
**Localização**: `style.py` (linha 140-145), `expressions.py`

**Funcionalidades**:
- **Temporal Properties**: Permite animar resultados ao longo do tempo
- **Interpolação Temporal**: Interpola valores entre timesteps
- **Expressão `wntr_result_at_current_time()`**: Retorna valor no timestamp atual da animação

**Como funciona**:
```python
# Ativar propriedades temporais
temporal_properties.setIsActive(True)
temporal_properties.setMode(Qgis.VectorTemporalMode.RedrawLayerOnly)

# Expressão QGIS para pegar valor no tempo atual
attribute_expression = 'wntr_result_at_current_time("pressure")'

# Interpolação linear entre timesteps
timestep = (current_time - start_time) / report_timestep
value = start_value + (timestep - floor(timestep)) * (end_value - start_value)
```

**Aplicável ao EpanetSharp?**
- ✅ **Sim!** Podemos implementar a mesma lógica de interpolação temporal
- ✅ Útil para criar APIs de "time travel" nos resultados
- ✅ Exemplo de uso:

```csharp
public class SimulationResults
{
	// Obter pressão de um nó em um timestamp específico (com interpolação)
	public double GetPressureAt(string nodeId, DateTime timestamp)
	{
		var series = Nodes.Pressure[nodeId];
		int index = series.Timestamps.BinarySearch(timestamp);

		if (index >= 0)
			return series.Values[index]; // Timestamp exato

		// Interpolar entre timesteps
		int nextIndex = ~index;
		int prevIndex = nextIndex - 1;

		if (prevIndex < 0 || nextIndex >= series.Count)
			throw new ArgumentOutOfRangeException();

		var t1 = series.Timestamps[prevIndex];
		var t2 = series.Timestamps[nextIndex];
		var v1 = series.Values[prevIndex];
		var v2 = series.Values[nextIndex];

		double fraction = (timestamp - t1).TotalSeconds / (t2 - t1).TotalSeconds;
		return v1 + fraction * (v2 - v1);
	}
}
```

---

### 3. **Análise de Water Quality** ✅
**Localização**: `elements.py` (linhas 392-396, 498-499)

**Campos suportados**:
- **Nós**:
  - `initial_quality`: Qualidade inicial da água
  - `mixing_fraction`: Fração de mistura (tanques)
  - `mixing_model`: Modelo de mistura
  - `bulk_coeff`: Coeficiente de reação bulk
  - `wall_coeff`: Coeficiente de reação na parede

- **Links**:
  - `quality`: Qualidade da água no link
  - `reaction_rate`: Taxa de reação

**Aplicável ao EpanetSharp?**
- ✅ **Sim!** EPANET 2.2 já suporta análise de qualidade
- ✅ Precisamos expor as APIs nativas de qualidade:
  - `EN_getqualitytype`
  - `EN_setqualitytype`
  - `EN_getnodevalue(index, EN_QUALITY)`
  - `EN_getlinkvalue(index, EN_QUALITY)`

---

### 4. **Análise de Pressure Dependent Demand (PDD)** ✅
**Localização**: `elements.py`, `empty_model.py`

**Campos suportados**:
- `minimum_pressure`: Pressão mínima para demanda
- `required_pressure`: Pressão requerida para demanda total
- `pressure_exponent`: Expoente da relação pressão-demanda

**O que é PDD?**
Modelo realista onde a demanda de água **depende da pressão disponível**:
- Pressão baixa → Demanda reduzida (torneiras com pouco fluxo)
- Pressão muito baixa → Sem demanda (torneiras secas)

**Aplicável ao EpanetSharp?**
- ✅ **Sim!** EPANET 2.2 suporta PDD nativamente
- ✅ APIs nativas:
  - `EN_setdemandmodel(type, pmin, preq, pexp)`
  - `EN_getdemandmodel()`

---

### 5. **Análise de Energia (Bombas)** ✅
**Localização**: `elements.py`, `empty_model.py`

**Campos para bombas**:
- `efficiency`: Curva de eficiência
- `energy_price`: Preço da energia
- `energy_pattern`: Padrão de preço de energia
- `speed`: Velocidade da bomba

**Aplicável ao EpanetSharp?**
- ✅ **Sim!** EPANET 2.2 calcula custo de energia
- ✅ APIs nativas:
  - `EN_getenergy(pump_index, …)`
  - `EN_setoption(EN_EPUMP, value)`

---

## ❌ O que NÃO Foi Encontrado

### 1. **DMCs / District Metered Areas**
- ❌ Nenhuma função de particionamento de rede
- ❌ Nenhum algoritmo de detecção de áreas/zonas
- ❌ Nenhuma análise de comunidades/clusters
- ❌ Nenhuma coloração por "distrito" ou "área"

**Provável origem da confusão**:
A **biblioteca WNTR Python completa** (não o plugin QGIS) tem módulos de análise de rede avançada, incluindo:
- `wntr.metrics.topographic`: Análise topológica
- `wntr.network.graph`: Análise de grafos
- Mas isso **não está no WNTR-QGIS** (que é só um wrapper UI)

### 2. **Análise de Resiliência / Criticidade**
- ❌ Não há cálculo de métricas de resiliência
- ❌ Não há análise de criticidade de componentes
- ❌ Não há simulação de falhas

### 3. **Algoritmos de Otimização**
- ❌ Não há otimização de calibração
- ❌ Não há otimização de design
- ❌ Não há alocação ótima de sensores

---

## 🎯 Funcionalidades Visuais Aplicáveis ao EpanetSharp

### Prioridade Alta 🔴

#### 1. **Interpolação Temporal de Resultados**
```csharp
public double GetValueAt(string elementId, DateTime timestamp, HydraulicParameter param)
{
	// Interpolar linearmente entre timesteps
}
```
**Esforço**: 1 dia  
**Benefício**: Análise temporal suave, exportação de vídeos

---

#### 2. **Classificação de Valores para Visualização**
```csharp
public class ValueClassifier
{
	public List<ValueRange> Quantile(List<double> values, int numClasses = 5)
	{
		// Dividir valores em N faixas com mesma quantidade de elementos
	}

	public List<ValueRange> EqualInterval(List<double> values, int numClasses = 5)
	{
		// Dividir valores em N faixas com mesmo intervalo
	}

	public class ValueRange
	{
		public double Min { get; set; }
		public double Max { get; set; }
		public string Label { get; set; } // "0.0 - 5.0"
		public string SuggestedColor { get; set; } // "#0000FF" (azul)
	}
}
```
**Esforço**: 2 dias  
**Benefício**: Preparar dados para visualização externa (Power BI, Python, etc.)

---

### Prioridade Média 🟡

#### 3. **Export para Formatos Visuais**
```csharp
public class ResultsExporter
{
	// Export para GeoJSON (pode ser aberto em QGIS, ArcGIS, etc.)
	public void ExportToGeoJSON(string filePath, SimulationResults results)

	// Export para CSV com coordenadas
	public void ExportToCSV(string filePath, SimulationResults results)

	// Export para formato temporal (para criar animações)
	public void ExportTemporalGeoJSON(string filePath, SimulationResults results)
}
```
**Esforço**: 3 dias  
**Benefício**: Integração com ferramentas GIS profissionais

---

#### 4. **Análise de Water Quality**
```csharp
public class WaterQualityAnalysis
{
	public QualityAnalysisType Type { get; set; } // Chemical, Age, Trace
	public string ChemicalName { get; set; }
	public string ChemicalUnits { get; set; }
	public double Tolerance { get; set; }

	// Campos de nó
	public Dictionary<string, double> NodeInitialQuality { get; }
	public Dictionary<string, double> NodeBulkCoeff { get; }

	// Resultados
	public Dictionary<string, TimeSeries<double>> NodeQuality { get; }
	public Dictionary<string, TimeSeries<double>> LinkQuality { get; }
}
```
**Esforço**: 3 dias  
**Benefício**: Análise de contaminação, idade da água

---

### Prioridade Baixa 🟢

#### 5. **Pressure Dependent Demand (PDD)**
```csharp
public class PressureDependentDemandOptions
{
	public DemandModelType Type { get; set; } // DDA vs PDD
	public double MinimumPressure { get; set; }
	public double RequiredPressure { get; set; }
	public double PressureExponent { get; set; } = 0.5;
}
```
**Esforço**: 2 dias  
**Benefício**: Modelo mais realista para áreas com pressão baixa

---

#### 6. **Análise de Energia de Bombas**
```csharp
public class PumpEnergyAnalysis
{
	public double TotalEnergyUsed { get; }
	public double AveragePower { get; }
	public double PeakPower { get; }
	public double TotalCost { get; }
	public Dictionary<string, TimeSeriesEnergy> PumpEnergy { get; }
}

public class TimeSeriesEnergy
{
	public TimeSeries<double> Power { get; }    // kW
	public TimeSeries<double> Energy { get; }   // kWh
	public TimeSeries<double> Cost { get; }     // $
	public TimeSeries<double> Efficiency { get; } // %
}
```
**Esforço**: 3 dias  
**Benefício**: Otimização de custos operacionais

---

## 📊 Resumo de Aplicabilidade

| Funcionalidade WNTR-QGIS | Aplicável ao EpanetSharp? | Esforço | Prioridade | Notas |
|---------------------------|---------------------------|---------|------------|-------|
| Graduated Coloring | 🟡 Parcial | 2 dias | Média | Calcular faixas, não renderizar |
| Temporal Animation | ✅ Sim | 1 dia | Alta | Interpolação temporal útil |
| Water Quality Analysis | ✅ Sim | 3 dias | Média | EPANET já suporta |
| PDD Analysis | ✅ Sim | 2 dias | Baixa | EPANET 2.2+ suporta |
| Energy Analysis | ✅ Sim | 3 dias | Baixa | Útil para otimização |
| GeoJSON Export | ✅ Sim | 3 dias | Média | Integração com GIS |
| DMCs / Partitioning | ❌ Não existe | N/A | N/A | Não no WNTR-QGIS |

---

## 💡 Conclusão

### Sobre DMCs (District Metered Areas)
**Não há implementação de DMCs no WNTR-QGIS analisado.** A pessoa pode estar confundindo com:
1. **Biblioteca WNTR completa** (Python) que tem análise de grafos
2. **Coloração por propriedades** (pressão, vazão) que pode dar impressão de "áreas"
3. **Funcionalidade de outro software** (EPA SWMM, WaterGEMS, etc.)

### Funcionalidades Visuais Úteis
As seguintes funcionalidades **valem a pena** implementar no EpanetSharp:

🔴 **Alta prioridade**:
- Interpolação temporal de resultados (1 dia)
- Classificação de valores para visualização (2 dias)

🟡 **Média prioridade**:
- Export para GeoJSON/CSV (3 dias)
- Water Quality Analysis APIs (3 dias)

🟢 **Baixa prioridade** (implementar se houver demanda):
- Pressure Dependent Demand (2 dias)
- Energy Analysis (3 dias)

**Total para prioridades Alta+Média**: ~9 dias de desenvolvimento

---

## 🚀 Recomendação Final

**Para visualização de redes no EpanetSharp**:

1. **Foco**: Exportar dados estruturados (JSON, CSV, GeoJSON)
2. **Deixar renderização para ferramentas especializadas**:
   - QGIS (plugin WNTR-QGIS)
   - Python + Matplotlib/Plotly
   - Power BI / Tableau
   - D3.js / JavaScript

3. **Implementar no EpanetSharp**:
   - ✅ Interpolação temporal
   - ✅ Classificação de valores
   - ✅ APIs de Water Quality
   - ✅ Exporters para formatos padrão

**Não reinventar a roda de visualização** — fornecer dados ricos e deixar visualização para ferramentas certas. 🎯

---

**Documento criado**: 2026-07-13  
**Baseado em**: Análise do código-fonte WNTR-QGIS (parte visual)  
**Versão**: 1.0
