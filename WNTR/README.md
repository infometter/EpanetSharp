# WNTR-QGIS - Water Network Tool for Resilience

## 📋 O que é este diretório?

Este diretório contém o código-fonte do projeto **WNTR-QGIS**, um plugin para QGIS que usa a biblioteca Python **WNTR** (Water Network Tool for Resilience) como wrapper do EPANET 2.

**Objetivo**: Analisar as funcionalidades extras do WNTR para incorporar no **EpanetSharp**.

---

## 🏗️ Estrutura do Projeto WNTR-QGIS

```
wntrqgis/
├── elements.py              # Definições: FlowUnit, HeadlossFormula, ModelLayer, etc.
├── interface.py             # Conversão WNTR ↔ QGIS, UnitConverter
├── settings.py              # Persistência de configurações
├── style.py                 # Estilos de visualização (ignorar para EpanetSharp)
├── plugin.py                # Plugin QGIS (ignorar)
├── wntrqgis_processing/
│   ├── run_simulation.py    # Orquestração de simulações
│   ├── import_inp.py        # Import de arquivos .inp com conversão
│   ├── empty_model.py       # Criação de modelo vazio
│   └── common.py            # ProgressTracker, helpers
└── resources/               # UI, i18n, icons (ignorar)
```

---

## 🎯 Funcionalidades Principais

### 1. **Sistema de Conversão de Unidades**
- Converte automaticamente entre sistemas (GPM, LPS, CFS, etc.)
- Ajusta conversão de roughness por headloss formula
- Arquivo: `interface.py` → Classe `_Converter`

### 2. **Validação de Rede**
- Valida integridade antes de simular
- Detecta nós órfãos, valores inválidos
- Arquivo: `interface.py` → Função `check_network()`

### 3. **Import/Export INP**
- Lê INP e converte unidades on-the-fly
- Arquivo: `import_inp.py`

### 4. **Organização de Resultados**
- Estrutura typed para resultados por nó/link
- Suporte a séries temporais
- Arquivo: `interface.py` → Classe `Writer`

### 5. **Enums Fortemente Tipados**
- FlowUnit, HeadlossFormula, ValveType, etc.
- Com `.friendly_name` para display
- Arquivo: `elements.py`

---

## 📚 Documentação da Análise

A análise completa das funcionalidades WNTR está em:

📁 **`docs/`** (na raiz do projeto)
- `WNTR_ANALYSIS_INDEX.md` - Início aqui! 👈
- `WNTR_EXECUTIVE_SUMMARY.md` - Resumo executivo
- `WNTR_FEATURES_ANALYSIS.md` - Análise técnica detalhada
- `WNTR_IMPLEMENTATION_EXAMPLES.md` - Exemplos de código C#

---

## 🔗 Links Úteis

- **WNTR GitHub**: https://github.com/USEPA/WNTR
- **WNTR Documentation**: https://usepa.github.io/WNTR/
- **WNTR-QGIS Plugin**: https://github.com/angusmcb/wntr-qgis
- **EPANET 2.2**: https://epanet22.readthedocs.io/

---

## ⚠️ Nota Importante

Este código Python **não será executado** no EpanetSharp. Ele serve apenas como **referência** para:
- Entender como WNTR organiza funcionalidades
- Identificar patterns úteis
- Extrair fórmulas de conversão
- Inspirar arquitetura do EpanetSharp

O EpanetSharp continuará usando **P/Invoke direto** para `epanet2.dll`, mas com APIs de mais alto nível inspiradas no WNTR.

---

## 🚀 Próximos Passos

1. ✅ **Análise completa do código** (FEITO)
2. ✅ **Documentação das features** (FEITO)
3. ✅ **Exemplos de implementação C#** (FEITO)
4. ⏳ **Implementação no EpanetSharp** (A FAZER)

Consulte `docs/WNTR_ANALYSIS_INDEX.md` para começar!

---

**Última atualização**: 2026-07-13  
**Status**: 📚 Análise completa, pronto para implementação
