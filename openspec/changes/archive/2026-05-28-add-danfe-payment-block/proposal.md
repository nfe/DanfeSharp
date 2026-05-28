## Why

O `nfe/DanfeSharp` não renderiza nenhum bloco visual de forma de pagamento na DANFE de NF-e modelo 55, mesmo quando o XML traz o grupo `<pag><detPag>` preenchido — situação tornada universal a partir da NT 2016.002 / v4.0 que tornou `<pag>` obrigatório na NF-e. Cliente Revenda Mais (issue [#39](https://github.com/nfe/DanfeSharp/issues/39), umbrella [#37](https://github.com/nfe/DanfeSharp/issues/37)) reportou que o XML traz `<pag><detPag><tPag>99</tPag><xPag>RECURSOS PROPRIOS</xPag><vPag>48000.00</vPag></detPag></pag>` mas a DANFE termina em "DADOS DOS PRODUTOS / SERVIÇOS" sem exibir a forma de pagamento — visualmente inconsistente com o que receptores fiscais e contadores esperam ver em NF-e modelo 55 emitidas após 2018. **Importante: o MOC 7.0 Anexo II (Manual de Especificações Técnicas do DANFE) não tem seção dedicada a forma de pagamento — verifiquei literalmente o PDF do CONFAZ em 2026-05-28; a renderização desse bloco em DANFE de NF-e é convenção universal de mercado (TOTVS, SAP, SmartGo, eMissor), análoga ao caso do change [`fix-danfe-csosn-rendering`](../archive/2026-05-28-fix-danfe-csosn-rendering/proposal.md). A NT 2016.002 obrigou o grupo `<pag>` no XML; o manual do DANFE NFC-e (modelo 65) mandou exibir o bloco no cupom; para NF-e modelo 55 o uso visual é convenção consolidada, não mandato literal.

O fork `nfe/DanfeSharp` descende do upstream `SilverCard/DanfeSharp` que precede a NT 2016.002 — o bloco provavelmente nunca foi implementado neste renderer. Curiosamente, **o trabalho de schema XML + ViewModel já está parcialmente feito** (classes `pag`/`detPag`/`FormaPagamento` em `ProcNFe.cs`, `PagamentoViewModel`/`DetalheViewModel` em `Modelo/`, e o `DanfeViewModelCreator` já popula a propriedade `model.Pagamento`) — falta apenas o campo `xPag`, o bloco visual e a inserção no fluxo da DANFE.

## What Changes

- Adicionar campo `xPag` (descrição da forma de pagamento, usado quando `tPag=99` ou para complementar `tPag` conhecidos) ao schema XML `detPag` em `DanfeSharp/Esquemas/ProcNFe.cs` e ao `DetalheViewModel` em `DanfeSharp/Modelo/PagamentoViewModel.cs`.
- `DanfeSharp/Modelo/DanfeViewModelCreator.cs` passa a popular `detalhe.Descricao` (mapeada de `detPag.xPag`).
- Criar novo bloco renderer `DanfeSharp/Blocos/BlocoFormaPagamento.cs` que desenha uma tabela com cabeçalho "FORMA DE PAGAMENTO" e colunas "FORMA PAGAMENTO" + "VALOR", uma linha por `DetalheViewModel`.
- Mapeamento de `tPag` numérico para descrição legível usa o `DescriptionAttribute` já presente no enum `FormaPagamento` (`[Description("Dinheiro")]`, `[Description("Cartão de Crédito")]`, etc.). Quando `tPag=99` ("Outros"), o renderer exibe o conteúdo de `xPag` no lugar; quando `xPag` está presente mesmo com tPag conhecido, prevalece `xPag` (mais específico).
- Valor formatado em moeda brasileira (`R$ 48.000,00`) usando os helpers de formatação existentes no projeto.
- Inserir o bloco no fluxo de renderização em `DanfeSharp/Danfe.cs`, posicionado **antes do bloco Transportador** (ordem visual convencional: Produtos → Cálculo do Imposto → Forma de Pagamento → Transportador → Dados Adicionais).
- DANFE de NF-e sem o grupo `<pag>` no XML continua renderizando idêntica — o bloco é omitido (não desenha nada se `ViewModel.Pagamento` está vazio).
- Cobertura de testes unitários cobrindo: (a) tPag numérico mapeado via Description, (b) tPag=99 + xPag, (c) múltiplos detPag, (d) XML sem `<pag>`, (e) valor formatado em R$.
- **Nenhuma mudança de comportamento** para emitentes que hoje geram DANFEs sem `<pag>` no XML.

## Capabilities

### New Capabilities

- `danfe-payment-block`: regras de renderização do bloco "FORMA DE PAGAMENTO" no DANFE de NF-e modelo 55 — incluindo schema do grupo `<pag>` (campos `tPag`/`xPag`/`vPag`/`vTroco`), mapeamento `tPag → descrição`, tratamento de tPag=99 (Outros), tabela visual (cabeçalho + colunas FORMA PAGAMENTO + VALOR + 1 linha por detPag), e omissão graciosa quando o XML não tem `<pag>`.

### Modified Capabilities

_Nenhuma — a spec `danfe-icms-column` (do change anterior, #38) não é tocada por esta change._

## Impact

- **Código afetado** (a confirmar no design):
  - `DanfeSharp/Esquemas/ProcNFe.cs` — adicionar `xPag` à class `detPag` (string, opcional, mapeada com `[XmlElement("xPag")]`)
  - `DanfeSharp/Modelo/PagamentoViewModel.cs` — adicionar `Descricao` em `DetalheViewModel`
  - `DanfeSharp/Modelo/DanfeViewModelCreator.cs` — popular `detalhe.Descricao = detPag.xPag` (em torno da linha 326-329, onde já popula `FormaPagamento` e `Valor`)
  - `DanfeSharp/Blocos/BlocoFormaPagamento.cs` — **arquivo novo**, segue padrão dos outros blocos (herda `ElementoBase`, tem `Tabela`, override `Draw`)
  - `DanfeSharp/Danfe.cs` — inserir `AdicionarBloco<BlocoFormaPagamento>()` no fluxo do construtor, antes do `BlocoTransportador`
  - `DanfeSharp.Test/PagamentoTests.cs` (ou similar) — arquivo novo de testes; ajustar `DanfeSharp.Test.csproj` para incluir
- **APIs públicas**: aditivas. Novas propriedades em `DetalheViewModel` (`Descricao`), comportamento novo de renderização. Sem breaking changes.
- **Backward compatibility**: total. DANFE sem `<pag>` no XML renderiza igual ao estado atual.
- **Dependências externas**: nenhuma nova.
- **Cliente impactado positivamente**: Revenda Mais (escopo direto) e qualquer outro emitente cujas DANFEs sejam recebidas por contrapartes que esperam ver o bloco de forma de pagamento.
- **Não impacta**: geração do XML, outras colunas/blocos da DANFE, DANFE NFCe (modelo 65), DANFE de MDF-e, DANFE de CT-e, Nota de Crédito por Recusa Parcial (essa renderização adicional é independente — `<pag>` pode estar presente ou não em NFCredito).
- **Issue mãe**: nfe/DanfeSharp#37; issues irmãs: nfe/DanfeSharp#38 (já mergeada — change `fix-danfe-csosn-rendering`), nfe/DanfeSharp#40.
