# EpanetSharp

[![.NET Standard 2.0](https://img.shields.io/badge/.NET%20Standard-2.0-blue.svg)](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)
[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.6.1%2B-blue.svg)](https://dotnet.microsoft.com/download/dotnet-framework)
[![.NET](https://img.shields.io/badge/.NET-5%2B%20%7C%206%2B%20%7C%208%2B%20%7C%2010%2B-purple.svg)](https://dotnet.microsoft.com/)

**Biblioteca C# wrapper para o EPANET 2.2 Toolkit** - Simulação hidráulica e de qualidade da água em redes de abastecimento.

---

## ✨ Características

- 🔌 **Wrapper nativo** para a biblioteca EPANET 2.2/2.3
- 🎯 **API moderna** orientada a objetos em C#
- 📦 **Multi-plataforma**: .NET Framework 4.6.1+, .NET Core 2.0+, .NET 5-10+
- 🚀 **Alta performance** com P/Invoke direto para DLL nativa
- 🔒 **Type-safe** com validações e exceções gerenciadas
- 📚 **Documentação** abrangente com exemplos

---

## 🎯 Compatibilidade

### Frameworks Suportados

| Framework | Versão Mínima | Status |
|-----------|---------------|--------|
| .NET Framework | 4.6.1 | ✅ Suportado |
| .NET Framework | 4.7, 4.8 | ✅ Suportado |
| .NET Core | 2.0+ | ✅ Suportado |
| .NET | 5, 6, 8, 10+ | ✅ Suportado |

**A biblioteca é compilada como .NET Standard 2.0**, garantindo máxima compatibilidade entre frameworks legados e modernos.

---

## 📦 Instalação

### Via DLL direta

1. Compile o projeto ou baixe a release
2. Adicione referência ao `EpanetSharp.dll`
3. Copie as DLLs nativas do EPANET para o diretório de saída:
   - `runtimes/win-x64/native/epanet2.dll`
   - `runtimes/win-x86/native/epanet2.dll`

**Para .NET Framework 4.6+**, veja: [Guia de uso no .NET Framework](docs/NET_FRAMEWORK_USAGE.md)

---

## 🚀 Uso Rápido

```csharp
using System;
using EpanetSharp.Core;

namespace MeuProjeto
{
	class Program
	{
		static void Main()
		{
			string inpPath = "rede.inp";

			// Verificar disponibilidade da biblioteca nativa
			if (!Project.IsNativeAvailable())
			{
				Console.WriteLine("EPANET nativo não disponível!");
				return;
			}

			// Abrir projeto EPANET
			using (var project = Project.Open(inpPath))
			{
				Console.WriteLine($"Nós: {project.Network.NodeCount}");
				Console.WriteLine($"Links: {project.Network.LinkCount}");
				Console.WriteLine($"Versão EPANET: {project.Version}");

				// Executar simulação hidráulica
				project.RunHydraulicSimulation();

				Console.WriteLine("Simulação concluída!");
			}
		}
	}
}
```

---

## 🏗️ Arquitetura

```
EpanetSharp/
├── Core/               # Ponto de entrada principal (Project, Network)
├── Native/             # P/Invoke e wrapper da DLL nativa
├── Elements/           # Elementos da rede (Node, Link, Pattern, etc.)
├── Collections/        # Coleções tipadas (NodeCollection, LinkCollection)
├── Simulation/         # Controle de simulação hidráulica/qualidade
└── Reporting/          # Geração de relatórios
```

### Camadas

1. **NativeMethods** → Declarações P/Invoke puras
2. **NativeApi** → Wrapper gerenciado com tratamento de erros
3. **NativeContext** → Gerenciamento de lifecycle do projeto nativo
4. **Core (Project/Network)** → API pública de alto nível

---

## 📚 Documentação

- [Guia de uso no .NET Framework 4.6+](docs/NET_FRAMEWORK_USAGE.md)
- [QuickDemo - Exemplo funcional](tests/QuickDemo/)

---

## 🔧 Desenvolvimento

### Compilar

```bash
dotnet build
```

### Executar testes rápidos

```bash
cd tests/QuickDemo
dotnet run -- rede.inp
```

### Requisitos

- .NET SDK 8.0+ (para desenvolvimento)
- Visual Studio 2022+ ou VS Code
- Windows (devido à DLL nativa do EPANET)

---

## 🎯 Roadmap

- [x] Wrapper básico EPANET 2.2/2.3
- [x] Suporte a .NET Standard 2.0
- [x] API de simulação hidráulica
- [ ] API de qualidade da água completa
- [ ] Suporte a Linux/macOS (via EPANET cross-platform)
- [ ] Pacote NuGet
- [ ] Documentação XML completa
- [ ] Testes de integração abrangentes

---

## 📄 Licença

Este projeto é um wrapper para o EPANET, que é de domínio público (US EPA).

A biblioteca wrapper EpanetSharp é distribuída sob licença MIT (a definir).

---

## 🙏 Créditos

- **EPANET**: Desenvolvido pela US Environmental Protection Agency
- **OpenWaterAnalytics/EPANET**: Versão moderna open-source

---

## ⚠️ Avisos

- Requer DLL nativa do EPANET (`epanet2.dll`) no runtime
- Testado com EPANET 2.3.05
- Suporte apenas Windows no momento (DLL nativa é Windows-only)

---

## 📞 Suporte

Para bugs, sugestões ou dúvidas, abra uma issue no GitHub.

---

**Desenvolvido com ❤️ para a comunidade de engenharia hidráulica**
