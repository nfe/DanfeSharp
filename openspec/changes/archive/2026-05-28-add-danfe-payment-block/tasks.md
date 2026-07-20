## 1. Investigação de uso externo (pre-flight)

- [ ] 1.1 Buscar consumidores internos do NFe.io que usam `DanfeViewModel.Pagamento` ou `DetalheViewModel` para garantir que adicionar `Descricao` ao `DetalheViewModel` não conflita com nada existente. _(Manual — fora do working tree atual.)_
- [ ] 1.2 Confirmar com cliente Revenda Mais (via @Carolina Fagundes) duas dúvidas: (a) quando `tPag=01` (Dinheiro) e `xPag="Pagamento à vista"` ambos vêm preenchidos, mostrar `xPag` (mais específico) ou ignorar? (b) Largura aceitável para a coluna VALOR quando aparece R$ XX.XXX.XXX,XX. _(Default da spec: `xPag` prevalece; largura ajustável depois conforme feedback.)_

## 2. Schema XML — `ProcNFe.cs`

- [x] 2.1 Adicionada propriedade `public string xPag { get; set; }` à classe `detPag` em `ProcNFe.cs:411`, com XML doc YA03.
- [x] 2.2 Deserialização aceita XMLs sem `<xPag>` (`XmlSerializer` retorna `null`) — confirmado pelo teste `Schema_XmlSemXPag_RetornaNull`.
- [x] 2.3 `xPag` sem `[XmlElement(IsRequired=true)]` (opcional).

## 3. ViewModel — `PagamentoViewModel.cs`

- [x] 3.1 Adicionada `public string Descricao { get; set; }` ao `DetalheViewModel` em `PagamentoViewModel.cs:28`.
- [x] 3.2 XML doc do `Descricao` documenta "Quando preenchida, prevalece sobre a descrição padrão do enum FormaPagamento".

## 4. Producer — `DanfeViewModelCreator.cs`

- [x] 4.1 Adicionado `detalhe.Descricao = String.IsNullOrEmpty(detPag.xPag) ? null : detPag.xPag;` em `CreateFromProcNFCe`.
- [x] 4.2 **Bug raiz adicional descoberto e corrigido**: a população do grupo `<pag>` estava só em `CreateFromProcNFCe` (NFC-e modelo 65) — faltava em `CreateFromProcNFe` (NF-e modelo 55, escopo desta change). Adicionado o foreach `infNfe.pag` em `CreateFromProcNFe` antes do `return model;`. Sem essa correção o bloco renderer nunca dispararia. Cobertura por `IntegracaoPagamentoNFe.V4ComLocalEntrega_PopulaPagamentoDoGrupoPag`.
- [x] 4.3 Tratamento defensivo de null em `infNfe.pag` e `pag.detPag` (testado por `V3_10Retrato_SemPag_NaoQuebraEPagamentoFicaVazio`).

## 5. Helper — `FormaPagamentoExtensions`

- [x] 5.1 Criado `DanfeSharp/Modelo/FormaPagamentoExtensions.cs` com `public static string GetDescricao(this FormaPagamento fp)`.
- [x] 5.2 Retorna `string.Empty` para enum value inválido (testado por `GetDescricao_ValorInvalido_RetornaStringVazia`).
- [x] 5.3 Cobertura: testes `GetDescricao_*` para Dinheiro / CartaoCredito / CartaoDebito / Outro / PIX + caso inválido. **Achado**: existem 2 enums `FormaPagamento` no projeto (em `Enums.cs` e em `ProcNFe.cs`); helper atual opera no `DanfeSharp.FormaPagamento` (Modelo/Enums.cs), que é o usado por `DetalheViewModel.FormaPagamento`.

## 6. Renderer — `BlocoFormaPagamento.cs`

- [x] 6.1 Criado `DanfeSharp/Blocos/BlocoFormaPagamento.cs` herdando `BlocoBase`, seguindo padrão de `BlocoCalculoImposto`.
- [x] 6.2 `Cabecalho => "Forma de Pagamento"`, `Posicao => PosicaoBloco.Topo`.
- [x] 6.3 Iteração com `SelectMany` sobre `Pagamento[].DetalhePagamento`. Célula FORMA PAGAMENTO usa `d.Descricao ?? d.FormaPagamento.GetDescricao()`. Célula VALOR usa `ComCampoNumerico(...)` (sem prefixo `R$` — convenção do `BlocoCalculoImposto` & cia; spec atualizada para refletir).
- [x] 6.4 Auto-omissão: early return quando `detalhes == null || detalhes.Count == 0`.
- [x] 6.5 Suporta Retrato e Paisagem (herdado de `BlocoBase`).

## 7. Sequência — `Danfe.cs`

- [x] 7.1 Inserido `AdicionarBloco<BlocoFormaPagamento>()` em `Danfe.cs` entre `BlocoCalculoImposto` e `BlocoTransportador`.
- [x] 7.2 Condicionado à `ViewModel.Pagamento != null && Pagamento.Any(p => p.DetalhePagamento?.Count > 0)` + `using System.Linq;` adicionado.

## 8. Testes unitários

- [x] 8.1 Criado `DanfeSharp.Test/FormaPagamentoTests.cs` (14 testes) cobrindo: helper para Dinheiro/CartaoCredito/CartaoDebito/Outro/PIX/inválido, schema parsing com/sem `<xPag>` (e `<xPag></xPag>` vazio), `DetalheViewModel.Descricao`, regra "Descricao prevalece sobre enum description". Aliases `FormaPagamentoVm`/`FormaPagamentoSchema` para desambiguar os 2 enums coexistentes.
- [x] 8.2 Criado `IntegracaoPagamentoNFe` (2 testes) no mesmo arquivo cobrindo: NF-e v4 com `<pag>` popula `model.Pagamento` (regressão direta do bug raiz §4.2); NF-e v3.10 sem `<pag>` não quebra e mantém `Pagamento.Count == 0`. **Substitui o BlocoFormaPagamentoTests.cs originalmente planejado** — o renderer foi testado via DanfeXmlTests (integração end-to-end mais robusta que mockar PdfClown internals).
- [x] 8.3 `DanfeSharp.Test.csproj` atualizado: `<Compile Include="FormaPagamentoTests.cs" />` + `<Reference Include="System.Xml" />` (necessário para `XmlSerializer` nos testes de schema parsing).
- [x] 8.4 `dotnet build`: 0 erros. `dotnet vstest --TestCaseFilter:"FullyQualifiedName~FormaPagamento|FullyQualifiedName~IntegracaoPagamentoNFe"` → **16/16 aprovados**, 748 ms.
- [x] 8.5 `dotnet vstest --TestCaseFilter:"FullyQualifiedName~DanfeXmlTests"` → **5/5 aprovados**, sem regressão. PDF re-renderizado de `v4_ComLocalEntrega.xml` exibe o novo bloco corretamente posicionado.

## 9. Fixture XML (opcional, recomendado)

- [x] 9.1-9.3 **Não necessário criar fixture nova**: descobri durante a implementação que as fixtures `v4_ComLocalEntrega.xml` e `v4_ComLocalRetirada.xml` **já têm grupo `<pag>` preenchido** (`<detPag><tPag>99</tPag><vPag>3977.00</vPag></detPag>`). O efeito da mudança já é coberto pelos `DanfeXmlTests.v4_ComLocalEntrega` e `.v4_ComLocalRetirada` existentes — eles renderizam o bloco novo automaticamente. Se algum dia quisermos cobrir caso com `<xPag>` preenchido + múltiplos `<detPag>`, basta criar essa fixture nova.

## 10. Validação visual (manual, fora do CI)

- [ ] 10.1 Gerar PDF da invoice de referência `9175b02ac0cf4ed898025c4bad09e2fe` (company `150a90afdf3e468f9feb7f8973707ce9` do cliente Revenda Mais). Confirmar visualmente que o bloco "Forma de Pagamento" aparece entre Cálculo do Imposto e Transportador, com `RECURSOS PROPRIOS / R$ 48.000,00`.
- [ ] 10.2 Gerar PDFs antes/depois das fixtures `v4_ComLocalEntrega.xml` e `v4_ComLocalRetirada.xml`. Confirmar: (a) se essas fixtures têm `<pag>`, o bloco passa a aparecer; (b) se não têm, PDF é idêntico ao baseline.
- [ ] 10.3 Tirar prints antes/depois e anexar no PR como evidência.

## 11. Documentação e changelog

- [x] 11.1 Repo não mantém CHANGELOG (confirmado em #38); doc fica no PR body.
- [x] 11.2 XML doc em `BlocoFormaPagamento.cs` referencia `openspec/specs/danfe-payment-block/spec.md`. NT 2016.002 está citada no `proposal.md`/`design.md`/`spec.md`.

## 12. PR e revisão

- [ ] 12.1 PR body com link `Fixes #39`, screenshots antes/depois (do tasks §10.3), e nota sobre base normativa calibrada (MOC não manda, NT obrigou o XML, bloco visual é convenção).
- [ ] 12.2 Solicitar review do @joaokita (co-assignee automático no `nfe/DanfeSharp`) + alguém fiscal-aware do time NFe.io.
- [ ] 12.3 Após approval, retirar o draft e mergear (squash) em `main`.

## 13. Arquivamento OpenSpec (post-merge)

- [ ] 13.1 Após merge, rodar `openspec validate fix-danfe-csosn-rendering... err... add-danfe-payment-block` para confirmar consistência.
- [ ] 13.2 Rodar `openspec archive add-danfe-payment-block -y` para mover a change para `openspec/changes/archive/` e promover os requirements ADDED para `openspec/specs/danfe-payment-block/spec.md`. Preencher o `Purpose` no spec.md (substituindo o boilerplate "TBD") com a base normativa.
