# 🎨 Graduated Symbol Renderer - Detalhamento das Faixas

## 📊 Resumo Executivo

O **Graduated Symbol Renderer** do WNTR-QGIS usa classificação por **Quantis** para dividir valores em **5 faixas**, com coloração usando a escala **Spectral invertida** (azul → amarelo → vermelho).

---

## 🔢 Método de Classificação: Quantis

### O que são Quantis?

**Quantis** dividem os dados de forma que **cada faixa tenha aproximadamente a mesma quantidade de elementos**.

**Código no WNTR**:
```python
classification_method = QgsClassificationQuantile()
renderer.setClassificationMethod(classification_method)
renderer.updateClasses(layer, 5)  # 5 faixas
```

### Exemplo Prático

Suponha que temos **100 nós** com as seguintes pressões (em metros):

```
Dados: [2.1, 3.4, 5.6, 7.8, 9.2, 10.5, 12.3, 15.6, 18.9, 21.2, ..., 145.7]
	   ↓
Ordenados: [2.1, 3.4, 5.6, ..., 145.7]
```

Com **5 faixas por quantis**, cada faixa terá **20 elementos** (100/5):

| Faixa | Nome | Intervalo | Qtd Elementos | Cor |
|-------|------|-----------|---------------|-----|
| 1 | Muito Baixo | 2.1 - 15.3 | 20 elementos | 🔵 Azul escuro |
| 2 | Baixo | 15.4 - 28.7 | 20 elementos | 🔵 Azul claro |
| 3 | Médio | 28.8 - 42.1 | 20 elementos | 🟢 Verde/Amarelo |
| 4 | Alto | 42.2 - 68.9 | 20 elementos | 🟠 Laranja |
| 5 | Muito Alto | 69.0 - 145.7 | 20 elementos | 🔴 Vermelho |

### Diferença vs Intervalo Igual

**Intervalo Igual** (não usado pelo WNTR):
- Divide o **range** em partes iguais
- Exemplo: min=2, max=150 → faixas de ~30 em 30
- Problema: pode ter faixas vazias ou muito cheias

**Quantis** (usado pelo WNTR):
- Divide os **elementos** em partes iguais
- Cada faixa tem ~20% dos dados
- Vantagem: distribuição visual equilibrada

---

## 🎨 Escala de Cores: Spectral Invertida

### Cores Padrão do Spectral (QGIS)

O color ramp "Spectral" padrão vai de:
```
Vermelho → Laranja → Amarelo → Verde → Azul
(quente → frio)
```

### Spectral Invertida (usada pelo WNTR)

```python
color_ramp = QgsStyle().defaultStyle().colorRamp("Spectral")
color_ramp.invert()  # Inverte as cores
```

Resultado:
```
Azul → Verde → Amarelo → Laranja → Vermelho
(frio → quente)
```

**Por que inverter?**
- 🔵 **Azul** = Valores baixos (baixa pressão = problema)
- 🔴 **Vermelho** = Valores altos (alta pressão = bom)

Ou no caso de velocidade:
- 🔵 **Azul** = Velocidade baixa (estagnação)
- 🔴 **Vermelho** = Velocidade alta (fluxo intenso)

### Mapeamento RGB Aproximado

| Faixa | Percentil | Cor | RGB Aproximado | Hex |
|-------|-----------|-----|----------------|-----|
| 1 | 0-20% | 🔵 Azul escuro | (68, 1, 84) | #440154 |
| 2 | 20-40% | 🔵 Azul claro | (49, 104, 142) | #31688e |
| 3 | 40-60% | 🟢 Verde/Amarelo | (53, 183, 121) | #35b779 |
| 4 | 60-80% | 🟠 Laranja | (253, 231, 37) | #fde725 |
| 5 | 80-100% | 🔴 Vermelho | (234, 51, 35) | #ea3323 |

*(Nota: Cores exatas podem variar ligeiramente dependendo da versão do QGIS)*

---

## 🔍 Parâmetros de Visualização

### Para Nós (Nodes)
```python
if self.layer_type is ResultLayer.NODES:
	attribute_expression = 'wntr_result_at_current_time("pressure")' 
	# ou simplesmente "pressure" se não houver animação temporal
```

**Parâmetro colorido**: `pressure` (Pressão em metros ou pés)

### Para Links
```python
else:
	attribute_expression = 'wntr_result_at_current_time("velocity")'
	# ou simplesmente "velocity"
```

**Parâmetro colorido**: `velocity` (Velocidade em m/s ou ft/s)

---

## 💡 Implementação no EpanetSharp

### Classe para Calcular Quantis

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

namespace EpanetSharp.Visualization
{
	public class QuantileClassifier
	{
		/// <summary>
		/// Classifica valores em N faixas usando quantis
		/// </summary>
		public List<ValueRange> ClassifyByQuantile(
			List<double> values, 
			int numberOfClasses = 5)
		{
			if (values == null || values.Count == 0)
				throw new ArgumentException("Values cannot be null or empty");

			if (numberOfClasses < 2)
				throw new ArgumentException("Number of classes must be at least 2");

			// Remover NaN e Infinity
			var cleanValues = values.Where(v => !double.IsNaN(v) && !double.IsInfinity(v))
								   .OrderBy(v => v)
								   .ToList();

			if (cleanValues.Count == 0)
				throw new ArgumentException("No valid values to classify");

			var ranges = new List<ValueRange>();
			int elementsPerClass = cleanValues.Count / numberOfClasses;
			int remainder = cleanValues.Count % numberOfClasses;

			int startIndex = 0;
			for (int i = 0; i < numberOfClasses; i++)
			{
				// Distribuir elementos extras nas primeiras classes
				int classSize = elementsPerClass + (i < remainder ? 1 : 0);
				int endIndex = Math.Min(startIndex + classSize - 1, cleanValues.Count - 1);

				double minValue = cleanValues[startIndex];
				double maxValue = cleanValues[endIndex];

				ranges.Add(new ValueRange
				{
					ClassNumber = i + 1,
					MinValue = minValue,
					MaxValue = maxValue,
					ElementCount = classSize,
					Label = $"{minValue:F1} - {maxValue:F1}",
					Color = GetSpectralColor(i, numberOfClasses)
				});

				startIndex = endIndex + 1;
			}

			return ranges;
		}

		/// <summary>
		/// Retorna cor Spectral invertida para uma classe
		/// </summary>
		private string GetSpectralColor(int classIndex, int totalClasses)
		{
			// Spectral invertido: Azul → Verde → Amarelo → Laranja → Vermelho
			var spectralColors = new Dictionary<int, string>
			{
				{ 0, "#3288bd" }, // Azul escuro
				{ 1, "#66c2a5" }, // Azul-verde
				{ 2, "#abdda4" }, // Verde claro
				{ 3, "#fee08b" }, // Amarelo
				{ 4, "#f46d43" }, // Laranja
				{ 5, "#d53e4f" }  // Vermelho
			};

			// Mapear classIndex para cor (interpolar se necessário)
			double position = (double)classIndex / (totalClasses - 1);
			int colorIndex = (int)Math.Round(position * (spectralColors.Count - 1));

			return spectralColors[colorIndex];
		}
	}

	public class ValueRange
	{
		public int ClassNumber { get; set; }
		public double MinValue { get; set; }
		public double MaxValue { get; set; }
		public int ElementCount { get; set; }
		public string Label { get; set; }
		public string Color { get; set; } // Hex color

		public bool Contains(double value)
		{
			return value >= MinValue && value <= MaxValue;
		}
	}
}
```

### Uso Prático

```csharp
// Obter todas as pressões dos nós
var pressures = new List<double>();
foreach (var (nodeId, series) in results.Nodes.Pressure)
{
	// Usar último valor ou valor médio
	pressures.Add(series.Values.Last());
}

// Classificar em 5 faixas
var classifier = new QuantileClassifier();
var ranges = classifier.ClassifyByQuantile(pressures, numClasses: 5);

// Exibir faixas
Console.WriteLine("Classificação de Pressões (Quantis):");
foreach (var range in ranges)
{
	Console.WriteLine($"Faixa {range.ClassNumber}: {range.Label}");
	Console.WriteLine($"  Cor: {range.Color}");
	Console.WriteLine($"  Elementos: {range.ElementCount}");
}

// Output:
// Faixa 1: 2.1 - 15.3
//   Cor: #3288bd (Azul escuro)
//   Elementos: 20
// Faixa 2: 15.4 - 28.7
//   Cor: #66c2a5 (Azul-verde)
//   Elementos: 20
// ...
```

### Exportar para GeoJSON com Cores

```csharp
public class GeoJsonExporter
{
	public void ExportNodesWithColors(
		SimulationResults results, 
		Network network,
		string outputPath)
	{
		// Classificar pressões
		var pressures = results.Nodes.Pressure
			.Select(kvp => kvp.Value.Values.Last())
			.ToList();

		var classifier = new QuantileClassifier();
		var ranges = classifier.ClassifyByQuantile(pressures, 5);

		// Criar GeoJSON
		var features = new List<object>();

		foreach (var (nodeId, pressureSeries) in results.Nodes.Pressure)
		{
			double pressure = pressureSeries.Values.Last();
			var range = ranges.First(r => r.Contains(pressure));

			// TODO: Obter coordenadas do nó
			double lat = 0, lon = 0; // Substituir por coordenadas reais

			features.Add(new
			{
				type = "Feature",
				properties = new
				{
					id = nodeId,
					pressure = pressure,
					pressureClass = range.ClassNumber,
					pressureLabel = range.Label,
					color = range.Color
				},
				geometry = new
				{
					type = "Point",
					coordinates = new[] { lon, lat }
				}
			});
		}

		var geoJson = new
		{
			type = "FeatureCollection",
			features = features
		};

		// Salvar JSON
		var json = System.Text.Json.JsonSerializer.Serialize(geoJson, 
			new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
		System.IO.File.WriteAllText(outputPath, json);
	}
}
```

---

## 📐 Configurações Adicionais no WNTR

### Precisão dos Labels

```python
classification_method.setLabelPrecision(1)
classification_method.setLabelTrimTrailingZeroes(False)
```

**Efeito**:
- `setLabelPrecision(1)`: 1 casa decimal nos labels
  - Exemplo: "2.1 - 15.3" (não "2.123 - 15.345")
- `setLabelTrimTrailingZeroes(False)`: Mantém zeros
  - Exemplo: "10.0 - 20.0" (não "10 - 20")

### Implementação no EpanetSharp

```csharp
public class ValueRange
{
	private int _precision = 1;

	public string Label => FormatLabel(_precision);

	private string FormatLabel(int precision)
	{
		string format = $"F{precision}";
		return $"{MinValue.ToString(format)} - {MaxValue.ToString(format)}";
	}
}
```

---

## 🎯 Diferentes Métodos de Classificação

O WNTR usa **Quantis**, mas existem outros métodos:

### 1. Quantile (Quantis) ✅ Usado pelo WNTR
- Divide **elementos** em partes iguais
- Cada classe tem ~20% dos dados
- Melhor para distribuição equilibrada

### 2. Equal Interval (Intervalo Igual)
- Divide **range** em partes iguais
- Classes podem ter quantidades diferentes
- Melhor quando há distribuição uniforme

### 3. Natural Breaks (Jenks)
- Maximiza diferença entre classes
- Minimiza variância dentro da classe
- Melhor para destacar clusters naturais

### 4. Standard Deviation
- Classes baseadas em desvio padrão
- Útil para detectar outliers
- Melhor para análise estatística

### Implementação de Outros Métodos

```csharp
public class Classifier
{
	public List<ValueRange> Quantile(List<double> values, int classes) { ... }

	public List<ValueRange> EqualInterval(List<double> values, int classes)
	{
		var sorted = values.OrderBy(v => v).ToList();
		double min = sorted.First();
		double max = sorted.Last();
		double interval = (max - min) / classes;

		var ranges = new List<ValueRange>();
		for (int i = 0; i < classes; i++)
		{
			double rangeMin = min + i * interval;
			double rangeMax = min + (i + 1) * interval;

			var elementsInRange = sorted.Count(v => v >= rangeMin && v <= rangeMax);

			ranges.Add(new ValueRange
			{
				MinValue = rangeMin,
				MaxValue = rangeMax,
				ElementCount = elementsInRange,
				Label = $"{rangeMin:F1} - {rangeMax:F1}"
			});
		}

		return ranges;
	}
}
```

---

## 📊 Resumo Visual

```
WNTR-QGIS Graduated Symbol Renderer
═══════════════════════════════════════

Método: Quantis (Quantile)
Número de faixas: 5
Cor: Spectral Invertida

┌─────────────────────────────────────┐
│ Faixa 1 (0-20%)   🔵 Azul escuro   │
│ Faixa 2 (20-40%)  🔵 Azul-verde    │
│ Faixa 3 (40-60%)  🟢 Verde-amarelo │
│ Faixa 4 (60-80%)  🟠 Laranja       │
│ Faixa 5 (80-100%) 🔴 Vermelho      │
└─────────────────────────────────────┘

Parâmetros coloridos:
  • Nós: pressure (Pressão)
  • Links: velocity (Velocidade)

Precisão: 1 casa decimal
Zeros: Mantidos (10.0 não vira 10)
```

---

## 🚀 Aplicação no EpanetSharp

**Prioridade**: 🟡 Média  
**Esforço**: 2 dias  
**Benefício**: Preparar dados para visualização externa

**Implementar**:
1. ✅ `QuantileClassifier` (calcular faixas)
2. ✅ `ValueRange` (representar cada faixa)
3. ✅ Cores Spectral (palette de cores)
4. ✅ `GeoJsonExporter` (exportar com cores)

**Usar em**:
- Exportação para QGIS, ArcGIS
- Visualização em Python (Matplotlib, Plotly)
- Dashboards web (D3.js, Leaflet)
- Relatórios (gerar legenda de cores)

---

**Documento criado**: 2026-07-13  
**Versão**: 1.0
