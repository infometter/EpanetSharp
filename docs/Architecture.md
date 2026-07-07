# Arquitetura — EpanetSharp

Este documento descreve a arquitetura da biblioteca EpanetSharp, os módulos principais, seus
responsabilidades e como eles se relacionam. O objetivo é dar visão clara para manutenção,
extensão e integração com a biblioteca nativa EPANET.

Observação: o diagrama abaixo usa sintaxe Mermaid para visualização em renderizadores que a
suportem (GitHub, diversos editores, etc.).

```mermaid
flowchart TD
  Project[Project]
  Network[Network]
  Collections[Collections (NodeCollection, LinkCollection,...)]
  Elements[Elements (Node, Link, Pattern, Curve)]
  NativeApi[NativeApi (adaptador)]
  NativeMethods[NativeMethods (P/Invoke signatures)]
  EPANET[epanet2.dll / biblioteca nativa]

  Project --> Network
  Network --> Collections
  Collections --> Elements
  Project --> NativeApi
  NativeApi --> NativeMethods
  NativeMethods --> EPANET

  style EPANET fill:#f9f,stroke:#333,stroke-width:1px
  style NativeMethods fill:#fee,stroke:#333,stroke-width:1px
  style NativeApi fill:#efe,stroke:#333,stroke-width:1px
  style Project fill:#eef,stroke:#333,stroke-width:1px
  style Network fill:#fff,stroke:#333,stroke-width:1px
  style Collections fill:#fff,stroke:#333,stroke-width:1px
  style Elements fill:#fff,stroke:#333,stroke-width:1px
```

Resumo em palavras: Project é a API de alto nível usada pelo consumidor. Ele contém uma
instância de Network que agrega as coleções especializadas (NodeCollection, LinkCollection,
PatternCollection, CurveCollection). As coleções mantêm instâncias de elementos (Nodes, Links,
Patterns, Curves). Quando é necessário executar operações nativas (criar projeto, rodar
simulação, consultar resultados), Project/Network delegam para NativeApi — o adaptador gerenciado.
NativeApi traduz chamadas em invocações às assinaturas P/Invoke definidas em NativeMethods, que
por sua vez chamam a DLL nativa (por exemplo, epanet2.dll).

Detalhamento das camadas

- Project (Camada de aplicação / fachada)
  - Responsabilidades:
	- Orquestrar o ciclo de vida do projeto (Open/Close/Dispose).
	- Expor API de alto nível para execução de simulação, leitura/escrita de arquivos e
	  operação sobre a Network.
	- Gerenciar eventos de nível alto (ProjectOpened, ProjectClosed, SimulationStarted, ...).
  - Observações:
	- Deve validar argumentos e lançar exceções .NET apropriadas (ArgumentNullException,
	  ArgumentException, InvalidOperationException, ObjectDisposedException).
	- Erros nativos são apresentados como EpanetException.

- Network (Modelo da rede)
  - Responsabilidades:
	- Agregar coleções de elementos e opções globais.
	- Fornecer operações de consulta de alto nível e alterações estruturadas na rede.
  - Observações:
	- Expõe coleções especializadas (NodeCollection, LinkCollection, ...).
	- Mantém referência ao Project que a contém.

- Collections (Camada de coleções especializadas)
  - Responsabilidades:
	- Implementar IReadOnlyCollection<T> para leitura eficiente.
	- Fornecer buscas por Id, indexer por Id, TryGet, Contains e enumeradores.
	- Controlar a inserção/removal de elementos (métodos Add/Remove internos ou controlados).
  - Observações:
	- Mantêm índice e mapa interno (List + Dictionary) para desempenho e ordenação.
	- Podem atribuir referências Network/Project nos elementos ao adicioná-los.

- Elements (Modelos de domínio)
  - Responsabilidades:
	- Representar os tipos de elementos de rede: Node, Link, Pattern, Curve (abstratos), e
	  implementações concretas respectivas (Junction, Pipe, Pump, ...).
	- Conter propriedades comuns (Id, Index, Tag, Comment, referências a Project/Network).
  - Observações:
	- Classes abstratas permitem criar variações específicas sem mudar a API pública.

- NativeApi (Adaptador gerenciado)
  - Responsabilidades:
	- Ser a única camada a interagir diretamente com NativeMethods.
	- Traduzir parâmetros gerenciados para chamadas nativas e traduzir códigos de retorno
	  para EpanetException com mensagens nativas.
	- Fornecer fallbacks controlados (somente para desenvolvimento e testes locais quando a
	  DLL nativa não estiver disponível). Em produção a expectativa é usar a DLL nativa real.
  - Observações:
	- Centraliza tratamento de erros, formatação de mensagens nativas e encapsula detalhes de
	  interoperabilidade (charset, buffers, convenções de chamada).

- NativeMethods (Assinaturas P/Invoke)
  - Responsabilidades:
	- Declarar assinaturas extern (DllImport) exclusivamente — sem lógica gerenciada.
	- Expor funções mínimas necessárias (criação/destruição de projeto, obtenção de mensagens
	  de erro, execução de passos, leitura de resultados).
  - Observações:
	- Deve permanecer thin: mudanças na DLL nativa ou seu nome exigirão apenas ajustes aqui.

- epanet2.dll (biblioteca nativa)
  - Responsabilidades:
	- Implementar os algoritmos hidráulicos e de qualidade de água.
	- Expor uma API C compatível que será consumida via P/Invoke.

Fluxo de chamadas (exemplo):
1. Usuário chama Project.Open(file).
2. Project cria Network e chama NativeApi.CreateProject().
3. NativeApi invoca NativeMethods.EN_createproject (P/Invoke).
4. NativeMethods chama a função na DLL nativa (epanet2.dll) que aloca/retorna um handle.
5. Em caso de erro, o código retornado é consultado via NativeApi.GetNativeMessage e um
   EpanetException é lançado com ErrorCode/Function/NativeMessage.

Tratamento de erros e política de exceções
- Validação de API e condições de uso geram exceções .NET padrão (ArgumentNullException,
  ArgumentException, InvalidOperationException e ObjectDisposedException).
- Erros originados pela biblioteca EPANET (códigos nativos != 0) são encapsulados em
  EpanetException, que contém ErrorCode, Function e NativeMessage.

Gerenciamento de recursos
- Todos os objetos que envolvem handles nativos implementam IDisposable seguindo o padrão
  recomendado pela Microsoft (Dispose(bool), GC.SuppressFinalize em Dispose(), finalizador
  que chama Dispose(false)).

Considerações de concorrência
- A arquitetura não assume thread-safety intrínseca. Se API pública precisar ser thread-safe,
  será definida a estratégia (imobilização interna, objetos imutáveis, locks, ou política clara
  de ownership).

Testabilidade
- NativeApi é o ponto único para isolar chamadas nativas. Para testar lógicas de alto nível,
  recomenda-se introduzir uma interface INativeApi e injetá-la no NativeContext/Project em
  futuros refactors (facilita mocks e testes unitários sem dependência da DLL nativa).

Extensibilidade
- A separação entre Elements, Collections e NativeApi permite:
  - Adicionar novos tipos de elementos (p.ex. novos tipos de válvulas) sem impactar P/Invoke.
  - Implementar carregadores/serializadores diferentes (INP parser, JSON, banco de dados).
  - Substituir ou evoluir a camada nativa sem alterar a API de alto nível.

Conclusão

Esta arquitetura segue princípios de separação de responsabilidades, encapsulamento da
interop nativa e API amigável ao consumidor. O adaptador NativeApi garante que somente um
ponto tradutor lide com peculiaridades da DLL nativa, enquanto Project e Network expõem a API
de domínio.

Para evoluções imediatas recomenda-se:
- Introduzir INativeApi para facilitar mocking e testes.
- Mapear e documentar as funções exatas da DLL nativa que serão usadas (entry points e contratos).
- Adicionar testes unitários para cada camada (Collections, Network, Project) com NativeApi
  stubbed/mocked.
