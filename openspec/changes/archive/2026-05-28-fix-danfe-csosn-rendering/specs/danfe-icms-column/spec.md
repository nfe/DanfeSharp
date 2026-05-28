## ADDED Requirements

> **Base normativa:** o quadro "Dados dos Produtos/Serviços" do DANFE e a obrigatoriedade da coluna `CST` estão definidos no [MOC 7.0 — Anexo II — Manual de Especificações Técnicas do DANFE e Código de Barras](https://www.confaz.fazenda.gov.br/legislacao/arquivo-manuais/moc7-anexo-ii-manual-especificacoes-tecnicas-danfe-codigo-barras.pdf), §3.1.7 (página 11). O campo `<orig>` (origem da mercadoria, domínio 0–8) faz parte do grupo `<ICMS>` no schema da NF-e ([MOC 7.0 — Anexo I](https://www.confaz.fazenda.gov.br/legislacao/arquivo-manuais/moc7-anexo-i-leiaute-e-rv.pdf)). O **formato combinado `O/CST` com separador `/` é convenção de mercado** (TOTVS, SAP, SmartGo, eMissor, etc.) — não está literalmente prescrito no MOC, mas é o que receptores fiscais esperam visualmente e o que outros renderers de DANFE da indústria adotam.

### Requirement: Cell rendering of `Origem/CST` or `Origem/CSOSN` in product table

A coluna ICMS da tabela de produtos da DANFE de NF-e modelo 55 SHALL renderizar **origem da mercadoria** e **código de tributação** (CST ou CSOSN) separados pelo caractere `/`, lidos do grupo `<ICMS*>` do item.

A regra de composição da célula é:

- Se `<orig>` está presente E (`<CST>` ou `<CSOSN>`) está presente → célula = `"<orig>/<código>"`
- Se `<orig>` está ausente E (`<CST>` ou `<CSOSN>`) está presente → célula = `"<código>"` (sem barra)
- Se `<orig>` está presente E código está ausente → célula = `"<orig>"`
- Se ambos ausentes → célula = `""` (vazio)

`CST` e `CSOSN` são mutuamente exclusivos por item (apenas um é preenchido), conforme MOC NF-e.

#### Scenario: Regime Normal — item com origem e CST

- **GIVEN** um item com `<orig>1</orig><CST>20</CST>` no grupo `<ICMS20>`
- **WHEN** a DANFE é renderizada
- **THEN** a célula da coluna ICMS para esse item exibe `"1/20"`

#### Scenario: Simples Nacional — item com origem e CSOSN

- **GIVEN** um item com `<orig>0</orig><CSOSN>102</CSOSN>` no grupo `<ICMSSN102>`
- **WHEN** a DANFE é renderizada
- **THEN** a célula da coluna ICMS para esse item exibe `"0/102"`

#### Scenario: Origem ausente — não exibir barra solta

- **GIVEN** um item com `<CST>40</CST>` e sem tag `<orig>` (ou `<orig>` vazia)
- **WHEN** a DANFE é renderizada
- **THEN** a célula da coluna ICMS exibe `"40"` (sem `"/40"`)

#### Scenario: Código ausente, apenas origem

- **GIVEN** um item com `<orig>2</orig>` e sem `<CST>` nem `<CSOSN>` (XML mal-formado mas presente em base legada)
- **WHEN** a DANFE é renderizada
- **THEN** a célula da coluna ICMS exibe `"2"` (sem `"2/"`)

#### Scenario: Ambos ausentes

- **GIVEN** um item sem `<orig>`, `<CST>` nem `<CSOSN>`
- **WHEN** a DANFE é renderizada
- **THEN** a célula da coluna ICMS exibe string vazia

### Requirement: Dynamic column header `O/CST` or `O/CSOSN` derived from items

O cabeçalho da coluna ICMS na tabela de produtos SHALL ser determinado pelo conteúdo dos itens da nota, NÃO apenas pelo CRT do emitente.

A regra é:

- Se qualquer item da nota tem `<CST>` não-vazio → cabeçalho = `"O/CST"`
- Senão se qualquer item da nota tem `<CSOSN>` não-vazio → cabeçalho = `"O/CSOSN"`
- Senão (nenhum item tem código tributário estruturado) → cabeçalho = `"O/CST"` (fallback determinístico)

A regra precedência (`<CST>` antes de `<CSOSN>`) trata o caso teoricamente impossível de coluna mista — se aparecer, prevalece `"O/CST"` porque Regime Normal é o caso majoritário no mercado brasileiro.

#### Scenario: Todos os itens com CST — cabeçalho CST

- **GIVEN** uma NF-e onde todos os itens têm `<CST>` preenchido
- **WHEN** a DANFE é renderizada
- **THEN** o cabeçalho da coluna ICMS exibe `"O/CST"`

#### Scenario: Todos os itens com CSOSN — cabeçalho CSOSN

- **GIVEN** uma NF-e onde todos os itens têm `<CSOSN>` preenchido
- **WHEN** a DANFE é renderizada
- **THEN** o cabeçalho da coluna ICMS exibe `"O/CSOSN"`

#### Scenario: `<emit><CRT>` ausente mas itens têm CST — cabeçalho CST

- **GIVEN** uma NF-e onde `<emit>` não tem o elemento `<CRT>` (ou está vazio) e todos os itens têm `<CST>` preenchido
- **WHEN** a DANFE é renderizada
- **THEN** o cabeçalho da coluna ICMS exibe `"O/CST"` (deriva dos itens, não do CRT)

#### Scenario: Nenhum item com CST nem CSOSN — fallback

- **GIVEN** uma NF-e onde nenhum item tem `<CST>` nem `<CSOSN>` (cenário improvável mas possível em XMLs antigos)
- **WHEN** a DANFE é renderizada
- **THEN** o cabeçalho da coluna ICMS exibe `"O/CST"`

### Requirement: Backward compatibility of `ProdutoViewModel.OCst`

A propriedade pública `ProdutoViewModel.OCst` SHALL continuar existindo e SHALL retornar a string formatada conforme as regras de célula descritas em "Cell rendering of `Origem/CST` or `Origem/CSOSN`" — preservando compatibilidade com consumidores externos que lêem essa propriedade.

A propriedade SHALL ser calculada (computed/read-only externamente) a partir das propriedades brutas `Origem`, `Cst`, `Csosn`.

A propriedade SHALL NOT retornar a forma concatenada anterior (`"120"` para origem=1+CST=20) — a correção é intencional e considerada bug fix, não breaking change.

#### Scenario: OCst calculado a partir de Origem + Cst

- **GIVEN** um `ProdutoViewModel` com `Origem="1"`, `Cst="20"`, `Csosn=null`
- **WHEN** `produto.OCst` é lido
- **THEN** retorna `"1/20"`

#### Scenario: OCst calculado a partir de Origem + Csosn

- **GIVEN** um `ProdutoViewModel` com `Origem="0"`, `Cst=null`, `Csosn="102"`
- **WHEN** `produto.OCst` é lido
- **THEN** retorna `"0/102"`

#### Scenario: OCst sem origem

- **GIVEN** um `ProdutoViewModel` com `Origem=null`, `Cst="40"`, `Csosn=null`
- **WHEN** `produto.OCst` é lido
- **THEN** retorna `"40"`
