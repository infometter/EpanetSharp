QuickDemo - instruções
======================

Objetivo
--------
Este diretório contém uma pequena aplicação console (QuickDemo) que abre um arquivo INP usando a biblioteca nativa epanet2.dll e imprime contadores básicos (Nodes, Links, PatternCount).

Pré-requisitos
--------------
- .NET 10 SDK instalado.
- A DLL nativa epanet2.dll compatível com sua arquitetura (x64 ou x86). Não incluímos a DLL no repositório.

Onde obter epanet2.dll
----------------------
Baixe a distribuição oficial do EPANET (ou compile a partir do código-fonte) a partir do site oficial do EPA:

https://www.epa.gov/water-research/epanet

Após obter a DLL, coloque o arquivo epanet2.dll em uma pasta local. O script abaixo ajudará a copiar para o local correto do projeto.

Uso rápido (PowerShell)
-----------------------
No prompt PowerShell execute este script a partir do diretório tests/QuickDemo:

.
\setup-and-run.ps1 -SourcePath "C:\caminho\para\epanet2.dll" -InpPath "example.inp" -ExpectedNodes 3 -ExpectedLinks 3

Parâmetros:
- SourcePath: (opcional) caminho para a DLL epanet2.dll a ser copiada para o diretório runtimes do projeto.
- InpPath: (opcional) caminho para o arquivo INP que será passado ao QuickDemo. Se omitido, example.inp do diretório será usado.
- ExpectedNodes / ExpectedLinks: (opcionais) valores esperados para verificação automática; se informados o processo retornará código 0 quando corresponderem.

O que o script faz
- Detecta a arquitetura do sistema (x64/x86).
- Copia epanet2.dll para runtimes\win-x64\native ou runtimes\win-x86\native conforme apropriado (cria diretórios se necessário).
- Executa: dotnet run --project QuickDemo.csproj -- <inp> [expectedNodes] [expectedLinks]

Notas de segurança e licença
---------------------------
Não incluímos a DLL epanet2.dll por motivos de licença e tamanho. Obtenha a DLL de fontes oficiais ou compile a partir do código-fonte do EPANET.
