## 1. Investigação de uso externo (pre-flight)

- [ ] 1.1 Buscar consumidores internos do NFe.io que usam `DanfeViewModel.Pagamento` ou `DetalheViewModel` para garantir que adicionar `Descricao` ao `DetalheViewModel` não conflita com nada existente. _(Manual — fora do working tree atual.)_
- [ ] 1.2 Confirmar com cliente Revenda Mais (via @Carolina Fagundes) duas dúvidas: (a) quando `tPag=01` (Dinheiro) e `xPag="Pagamento à vista"` ambos vêm preenchidos, mostrar `xPag` (mais específico) ou ignorar? (b) Largura aceitável para a coluna VALOR quando aparece R$ XX.XXX.XXX,XX. _(Default da spec: `xPag` prevalece; largura ajustável depois conforme feedback.)_

## 2. Schema XML — `ProcNFe.cs`

- [ ] 2.1 Adicionar propriedade pública `public string xPag { get; set; }` à classe `detPag` em `DanfeSharp/Esquemas/ProcNFe.cs:402`, com comentário XML doc apontando para a tag `<xPag>` (Grupo YA03 — Descrição do pagamento).
- [ ] 2.2 Garantir que a deserialização aceita XMLs sem `<xPag>` sem erro — `XmlSerializer` trata elemento ausente como `null` por padrão; verificar com fixture existente.
- [ ] 2.3 Não usar `[XmlElement(IsRequired=true)]` — `<xPag>` é opcional pelo schema da NF-e.

## 3. ViewModel — `PagamentoViewModel.cs`

- [ ] 3.1 Adicionar propriedade pública `public string Descricao { get; set; }` à classe `DetalheViewModel` em `DanfeSharp/Modelo/PagamentoViewModel.cs:20`, com XML doc apontando para a tag `<xPag>` do XML.
- [ ] 3.2 Documentar a regra "Descricao prevalece sobre o `[Description]` do enum quando preenchido" no XML doc, fazendo cross-reference com a spec em `openspec/specs/danfe-payment-block/`.

## 4. Producer — `DanfeViewModelCreator.cs`

- [ ] 4.1 Em `DanfeSharp/Modelo/DanfeViewModelCreator.cs:328`, ao popular o `DetalheViewModel`, adicionar `detalhe.Descricao = String.IsNullOrEmpty(detPag.xPag) ? null : detPag.xPag;` (mesma normalização aplicada no change #38 para origem/CST/CSOSN).
- [ ] 4.2 Verificar se o caminho `detPag.xPag` está acessível na linha onde `tPag`/`vPag` são populados; se não, ajustar a ordem do bloco.
- [ ] 4.3 Garantir que a propriedade ordem de inicialização não introduz NullReferenceException quando `detPag.xPag` é null (testes cobrem).

## 5. Helper — `FormaPagamentoExtensions`

- [ ] 5.1 Criar arquivo `DanfeSharp/Modelo/FormaPagamentoExtensions.cs` (ou anexar como static class no PagamentoViewModel.cs se o codestyle preferir) com método estático `public static string GetDescricao(this FormaPagamento fp)` que lê o `[DescriptionAttribute]` via reflexão e retorna a string.
- [ ] 5.2 Garantir que retorna string vazia (`""`) — não null nem exceção — para valor não definido no enum (defensivo para o caso de XML com `tPag` novo antes do enum estar atualizado).
- [ ] 5.3 Cobertura por testes unitários: helper retorna a descrição correta para cada valor do enum (`01` a `19`, `90`, `99`).

## 6. Renderer — `BlocoFormaPagamento.cs`

- [ ] 6.1 Criar arquivo `DanfeSharp/Blocos/BlocoFormaPagamento.cs` herdando de `BlocoBase`, seguindo o padrão de `BlocoCalculoImposto` (mesma pasta, mesmo estilo).
- [ ] 6.2 Implementar: `Cabecalho => "Forma de Pagamento"`, `Posicao => PosicaoBloco.Topo`. Construtor recebe `(DanfeViewModel viewModel, Estilo estilo)`.
- [ ] 6.3 No construtor, iterar sobre `ViewModel.Pagamento?.SelectMany(p => p.DetalhePagamento ?? Enumerable.Empty<DetalheViewModel>())` e adicionar uma linha por `DetalheViewModel`:
  - Coluna FORMA PAGAMENTO: `detalhe.Descricao ?? detalhe.FormaPagamento.GetDescricao()`
  - Coluna VALOR: formatado em moeda brasileira (`R$ {valor:N2}`, com cultura `pt-BR`)
- [ ] 6.4 Auto-omissão: se `ViewModel.Pagamento` é null OU vazio OU não tem nenhum `DetalheViewModel`, não adicionar nenhuma linha ao bloco (resulta em bloco com height efetivo zero — não desenha cabeçalho nem corpo).
- [ ] 6.5 Suportar Orientação Retrato e Paisagem — mesmo `BlocoBase` que os demais já fazem isso transparentemente.

## 7. Sequência — `Danfe.cs`

- [ ] 7.1 Em `DanfeSharp/Danfe.cs:71` (entre `AdicionarBloco<BlocoCalculoImposto>` e `AdicionarBloco<BlocoTransportador>`), adicionar `AdicionarBloco<BlocoFormaPagamento>();`.
- [ ] 7.2 Condicionar à presença de pagamento: `if (ViewModel.Pagamento?.Any(p => p.DetalhePagamento?.Any() == true) == true) AdicionarBloco<BlocoFormaPagamento>();` — defense in depth com a auto-omissão do bloco.

## 8. Testes unitários

- [ ] 8.1 Criar `DanfeSharp.Test/FormaPagamentoTests.cs` cobrindo:
  - (a) Schema parsing: XML com `<xPag>` é deserializado corretamente (uso de `XmlSerializer` em memória)
  - (b) Schema parsing: XML sem `<xPag>` resulta em `xPag == null`
  - (c) Helper `GetDescricao` retorna a string esperada para `fpDinheiro`, `fpCartaoCredito`, `fpOutros`, etc.
  - (d) Helper retorna `""` para valor inválido `(FormaPagamento)999`
  - (e) Producer popula `DetalheViewModel.Descricao` quando XML tem `<xPag>`
  - (f) Producer normaliza `<xPag></xPag>` para `null`
- [ ] 8.2 Criar `DanfeSharp.Test/BlocoFormaPagamentoTests.cs` cobrindo:
  - (a) Linha com FormaPagamento + Descricao preenchidos → célula mostra Descricao
  - (b) Linha sem Descricao → célula mostra descrição do enum
  - (c) tPag=99 + Descricao=null → célula mostra "Outros"
  - (d) Múltiplas linhas → uma por DetalheViewModel
  - (e) Valor formatado em R$ XX.XXX,XX
- [ ] 8.3 Atualizar `DanfeSharp.Test/DanfeSharp.Test.csproj` com `<Compile Include>` dos arquivos novos.
- [ ] 8.4 Rodar `dotnet build` para garantir 0 erros + `dotnet vstest DanfeSharp.Test.dll --TestCaseFilter:"FullyQualifiedName~FormaPagamento OR FullyQualifiedName~BlocoFormaPagamento"` para garantir 100% verde.
- [ ] 8.5 Rodar suite completa de `DanfeXmlTests` para confirmar zero regressão visual nos PDFs gerados das fixtures atuais.

## 9. Fixture XML (opcional, recomendado)

- [ ] 9.1 Adicionar `DanfeSharp.Test/Xml/NFe/v4.00/v4_ComPagamento.xml` — XML de NF-e modelo 55 com grupo `<pag>` preenchido com 2-3 `<detPag>` variados (ex.: Dinheiro + Cartão de Crédito + Outros com `<xPag>`).
- [ ] 9.2 Atualizar `DanfeXmlTests` com novo test method `v4_ComPagamento` apontando para a nova fixture.
- [ ] 9.3 Adicionar a fixture XML no `<ItemGroup>` da `DanfeSharp.Test.csproj` com `<Content Include>` + `<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>`.

## 10. Validação visual (manual, fora do CI)

- [ ] 10.1 Gerar PDF da invoice de referência `9175b02ac0cf4ed898025c4bad09e2fe` (company `150a90afdf3e468f9feb7f8973707ce9` do cliente Revenda Mais). Confirmar visualmente que o bloco "Forma de Pagamento" aparece entre Cálculo do Imposto e Transportador, com `RECURSOS PROPRIOS / R$ 48.000,00`.
- [ ] 10.2 Gerar PDFs antes/depois das fixtures `v4_ComLocalEntrega.xml` e `v4_ComLocalRetirada.xml`. Confirmar: (a) se essas fixtures têm `<pag>`, o bloco passa a aparecer; (b) se não têm, PDF é idêntico ao baseline.
- [ ] 10.3 Tirar prints antes/depois e anexar no PR como evidência.

## 11. Documentação e changelog

- [ ] 11.1 Verificar se há `README.md` ou `CHANGELOG.md` no repo que precisa entrada explicativa. _(Confirmado em #38: repo não mantém CHANGELOG; documentação fica no body do PR.)_
- [ ] 11.2 XML doc no `BlocoFormaPagamento.cs` referenciando a spec OpenSpec (`openspec/specs/danfe-payment-block/spec.md`) e citando a NT 2016.002 como motivação histórica.

## 12. PR e revisão

- [ ] 12.1 PR body com link `Fixes #39`, screenshots antes/depois (do tasks §10.3), e nota sobre base normativa calibrada (MOC não manda, NT obrigou o XML, bloco visual é convenção).
- [ ] 12.2 Solicitar review do @joaokita (co-assignee automático no `nfe/DanfeSharp`) + alguém fiscal-aware do time NFe.io.
- [ ] 12.3 Após approval, retirar o draft e mergear (squash) em `main`.

## 13. Arquivamento OpenSpec (post-merge)

- [ ] 13.1 Após merge, rodar `openspec validate fix-danfe-csosn-rendering... err... add-danfe-payment-block` para confirmar consistência.
- [ ] 13.2 Rodar `openspec archive add-danfe-payment-block -y` para mover a change para `openspec/changes/archive/` e promover os requirements ADDED para `openspec/specs/danfe-payment-block/spec.md`. Preencher o `Purpose` no spec.md (substituindo o boilerplate "TBD") com a base normativa.
