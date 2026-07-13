# Usando EpanetSharp no .NET Framework 4.6+

## ✅ A biblioteca EpanetSharp agora é compatível com .NET Framework 4.6.1+

### Compilada para: .NET Standard 2.0

Isso significa que você pode usar a mesma DLL em:
- ✅ .NET Framework 4.6.1, 4.7, 4.8
- ✅ .NET Core 2.0+
- ✅ .NET 5, 6, 8, 10+

---

## 📦 Como usar no seu projeto .NET Framework 4.6

### 1. Adicionar a referência

No seu projeto .NET Framework 4.6, adicione a referência ao arquivo `EpanetSharp.dll` compilado.

**Via Visual Studio:**
- Botão direito em "Referências" → "Adicionar Referência"
- Procurar → Selecione `EpanetSharp.dll`

**Via .csproj manual:**
```xml
<ItemGroup>
  <Reference Include="EpanetSharp">
	<HintPath>..\path\to\EpanetSharp.dll</HintPath>
  </Reference>
</ItemGroup>
```

### 2. Copiar DLLs nativas

Certifique-se de que as DLLs nativas do EPANET estejam no mesmo diretório do executável ou em uma subpasta `runtimes`:

```
MeuApp.exe
├── EpanetSharp.dll
└── runtimes/
	├── win-x64/
	│   └── native/
	│       └── epanet2.dll
	└── win-x86/
		└── native/
			└── epanet2.dll
```

### 3. Exemplo de código (C# .NET Framework 4.6)

```csharp
using System;
using EpanetSharp.Core;

namespace ExemploFramework46
{
	class Program
	{
		static void Main(string[] args)
		{
			string inpPath = @"C:\caminho\para\seu\arquivo.inp";

			try
			{
				// Verificar se a biblioteca nativa está disponível
				if (!Project.IsNativeAvailable())
				{
					Console.WriteLine("EPANET nativo não disponível!");
					return;
				}

				// Abrir o projeto
				using (var project = Project.Open(inpPath))
				{
					Console.WriteLine($"Nós: {project.Network.NodeCount}");
					Console.WriteLine($"Links: {project.Network.LinkCount}");
					Console.WriteLine($"Patterns: {project.Network.PatternCount}");

					// Executar simulação hidráulica
					project.Run();

					Console.WriteLine("Simulação concluída!");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Erro: {ex.Message}");
			}
		}
	}
}
```

---

## 🔧 Configuração do projeto .NET Framework 4.6

### app.config (se necessário)

Se você tiver problemas com binding redirect, adicione:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <runtime>
	<assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
	  <dependentAssembly>
		<assemblyIdentity name="System.Runtime" 
						  publicKeyToken="b03f5f7f11d50a3a" 
						  culture="neutral" />
		<bindingRedirect oldVersion="0.0.0.0-4.1.2.0" 
						 newVersion="4.1.2.0" />
	  </dependentAssembly>
	</assemblyBinding>
  </runtime>
</configuration>
```

---

## 📋 Requisitos

- **.NET Framework 4.6.1 ou superior** (recomendado 4.8 para melhor compatibilidade)
- **Windows** (devido à DLL nativa epanet2.dll do EPANET)
- **Visual C++ Redistributable** pode ser necessário para a DLL nativa

---

## 🚀 Migração futura para .NET 10+

Quando você migrar seu sistema para .NET 10:
1. **Não será necessário recompilar** a DLL do EpanetSharp
2. A mesma DLL continuará funcionando
3. Basta atualizar o TargetFramework do seu projeto consumidor

```xml
<!-- De .NET Framework 4.6 -->
<TargetFramework>net46</TargetFramework>

<!-- Para .NET 10 -->
<TargetFramework>net10.0</TargetFramework>
```

A DLL do EpanetSharp (.NET Standard 2.0) funcionará sem alterações! ✅

---

## ⚠️ Observação sobre .NET Framework 4.6.0

Se você **realmente precisa** de .NET Framework 4.6.0 (não 4.6.1):
- .NET Standard 2.0 oficialmente requer 4.6.1+
- Na prática, pode funcionar em 4.6.0, mas não é garantido pela Microsoft
- **Recomendação:** Atualize para 4.6.1 (foi lançado apenas 6 meses depois do 4.6.0)

---

## 📞 Suporte

Para problemas ou dúvidas, abra uma issue no repositório GitHub.
