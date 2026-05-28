## ADDED Requirements

> **Base normativa:** o quadro "Forma de Pagamento" **não é literalmente prescrito** pelo [MOC 7.0 — Anexo II](https://www.confaz.fazenda.gov.br/legislacao/arquivo-manuais/moc7-anexo-ii-manual-especificacoes-tecnicas-danfe-codigo-barras.pdf) (Manual de Especificações Técnicas do DANFE para NF-e modelo 55) — verifiquei literalmente o PDF em 2026-05-28: zero menções a `pagamento`/`detPag`/`tPag`/`xPag`. A obrigatoriedade está no XML, via [NT 2016.002 v1.50](https://www.nfe.fazenda.gov.br/portal/exibirArquivo.aspx?conteudo=tQNDQOeNeyI%3D), que tornou o grupo `<pag>` exigido na NF-e (em produção desde 2018). O bloco visual no DANFE NF-e modelo 55 é **convenção universal de mercado** (TOTVS, SAP, SmartGo, eMissor) que receptores fiscais e contadores esperam ver. A spec abaixo formaliza essa convenção para o `nfe/DanfeSharp`.

### Requirement: Schema parsing of `<pag>/<detPag>/xPag` group

O renderer SHALL deserializar o grupo `<pag><detPag>` do XML da NF-e incluindo a tag `<xPag>` (descrição livre da forma de pagamento). A propriedade `detPag.xPag` deve ser opcional — XMLs anteriores à NT 2016.002 ou que não tenham `<xPag>` continuam parseando sem erro.

#### Scenario: XML com `<xPag>` preenchido

- **GIVEN** um XML com `<pag><detPag><tPag>99</tPag><xPag>RECURSOS PROPRIOS</xPag><vPag>48000.00</vPag></detPag></pag>`
- **WHEN** o XML é deserializado pelo `ProcNFeSerializer`
- **THEN** `detPag.xPag == "RECURSOS PROPRIOS"`, `detPag.tPag == FormaPagamento.fpOutros`, `detPag.vPag == 48000.00m`

#### Scenario: XML sem `<xPag>`

- **GIVEN** um XML com `<pag><detPag><tPag>01</tPag><vPag>100.00</vPag></detPag></pag>` (sem `<xPag>`)
- **WHEN** o XML é deserializado
- **THEN** `detPag.xPag == null` (ou string vazia), `detPag.tPag == FormaPagamento.fpDinheiro`, `detPag.vPag == 100.00m`

#### Scenario: XML sem grupo `<pag>` (NF-e antiga ou v4.0+ com `tPag=90`)

- **GIVEN** um XML de NF-e que não contém o elemento `<pag>` (ex.: fixture `v3.10_Retrato.xml`)
- **WHEN** o XML é deserializado
- **THEN** `infNFe.pag == null` (ou lista vazia) — sem exceção lançada

### Requirement: ViewModel exposes `Descricao` mapped from `xPag`

O `DanfeSharp.Modelo.DetalheViewModel` SHALL expor uma propriedade pública `Descricao` (`string`, opcional) que reflete o conteúdo da tag XML `<xPag>`. O `DanfeViewModelCreator` SHALL populá-la quando o XML for processado.

#### Scenario: Producer popula Descricao

- **GIVEN** um XML com `<detPag><tPag>99</tPag><xPag>BOLETO BB</xPag><vPag>500.00</vPag></detPag>`
- **WHEN** `DanfeViewModelCreator.CriarDeStringXml(xml)` é chamado
- **THEN** o `DanfeViewModel.Pagamento[0].DetalhePagamento[0].Descricao == "BOLETO BB"`

#### Scenario: Producer normaliza string vazia para null

- **GIVEN** um XML com `<xPag></xPag>` (elemento presente mas vazio)
- **WHEN** o XML é processado
- **THEN** `DetalheViewModel.Descricao == null` (não string vazia) — para consistência com a regra de "ausência" em outros campos do projeto

### Requirement: Render block "FORMA DE PAGAMENTO" when `Pagamento` is populated

A DANFE de NF-e modelo 55 SHALL renderizar um bloco com cabeçalho "Forma de Pagamento" quando `ViewModel.Pagamento` contém pelo menos um `DetalheViewModel`. O bloco SHALL aparecer entre o "Cálculo do Imposto" e o "Transportador" no fluxo visual da DANFE. Cada `DetalheViewModel` SHALL ocupar uma linha com duas células: **FORMA PAGAMENTO** (descrição) e **VALOR** (numérico em moeda brasileira).

#### Scenario: NF-e com 1 detPag

- **GIVEN** uma NF-e com `Pagamento = [ { DetalhePagamento = [ DetalheViewModel { FormaPagamento=fpOutros, Descricao="RECURSOS PROPRIOS", Valor=48000.00m } ] } ]`
- **WHEN** a DANFE é renderizada
- **THEN** aparece um bloco com cabeçalho "Forma de Pagamento" contendo 1 linha: "RECURSOS PROPRIOS" + "R$ 48.000,00"

#### Scenario: NF-e com múltiplos detPag

- **GIVEN** uma NF-e com 3 itens em `DetalhePagamento`: (Dinheiro, R$ 100), (Cartão de Crédito, R$ 50), (PIX [tPag=17], R$ 25)
- **WHEN** a DANFE é renderizada
- **THEN** o bloco "Forma de Pagamento" exibe 3 linhas, uma para cada `DetalheViewModel`, na ordem do XML

### Requirement: Cell formatting for FORMA PAGAMENTO column

A célula da coluna "FORMA PAGAMENTO" SHALL conter o conteúdo de `DetalheViewModel.Descricao` quando preenchido; caso contrário, a descrição derivada do `[DescriptionAttribute]` do enum `FormaPagamento` (ex.: `fpDinheiro → "Dinheiro"`, `fpCartaoCredito → "Cartão de Crédito"`, `fpOutros → "Outros"`).

#### Scenario: Descricao prevalece sobre descrição do enum

- **GIVEN** `DetalheViewModel { FormaPagamento=fpCartaoCredito, Descricao="VISA 4× 2,5%" }`
- **WHEN** a célula FORMA PAGAMENTO é renderizada
- **THEN** o conteúdo da célula é "VISA 4× 2,5%" (não "Cartão de Crédito")

#### Scenario: Sem Descricao, usa enum

- **GIVEN** `DetalheViewModel { FormaPagamento=fpDinheiro, Descricao=null }`
- **WHEN** a célula FORMA PAGAMENTO é renderizada
- **THEN** o conteúdo é "Dinheiro" (derivado do `[Description]` do enum)

#### Scenario: tPag=99 sempre exige Descricao para ser informativo

- **GIVEN** `DetalheViewModel { FormaPagamento=fpOutros, Descricao="RECURSOS PROPRIOS" }`
- **WHEN** a célula FORMA PAGAMENTO é renderizada
- **THEN** o conteúdo é "RECURSOS PROPRIOS"

#### Scenario: tPag=99 sem Descricao usa fallback genérico

- **GIVEN** `DetalheViewModel { FormaPagamento=fpOutros, Descricao=null }` (XML mal-formado mas possível)
- **WHEN** a célula FORMA PAGAMENTO é renderizada
- **THEN** o conteúdo é "Outros" (descrição do enum) — sem null reference

### Requirement: Cell formatting for VALOR column

A célula da coluna "VALOR" SHALL renderizar `DetalheViewModel.Valor` em formato numérico brasileiro: separador de milhar `.`, separador decimal `,`, duas casas decimais sempre. **Sem prefixo `R$`** — segue a convenção do `DanfeSharp` para campos numéricos dentro de blocos (`BlocoCalculoImposto`, `BlocoCalculoIssqn` também usam apenas o número formatado, sem `R$`). O prefixo `R$` na DANFE aparece apenas em totalizadores específicos (cabeçalho/canhoto), não em blocos de campos.

#### Scenario: Valor inteiro

- **GIVEN** `Valor = 48000.00m`
- **WHEN** a célula VALOR é renderizada
- **THEN** o conteúdo é `48.000,00`

#### Scenario: Valor com decimais

- **GIVEN** `Valor = 123.45m`
- **WHEN** a célula VALOR é renderizada
- **THEN** o conteúdo é `123,45`

#### Scenario: Valor grande

- **GIVEN** `Valor = 1234567.89m`
- **WHEN** a célula VALOR é renderizada
- **THEN** o conteúdo é `1.234.567,89`

### Requirement: Graceful omission when `<pag>` is absent

A DANFE SHALL NOT renderizar o bloco "Forma de Pagamento" quando o XML não contém o grupo `<pag>` ou quando o grupo está presente mas sem nenhum `<detPag>`. A ausência do bloco NÃO SHALL afetar o layout dos demais blocos (Transportador continua aparecendo na posição atual; cálculo de imposto também).

#### Scenario: NF-e antiga sem `<pag>`

- **GIVEN** uma NF-e cujo XML é v3.10 sem grupo `<pag>` (ex.: fixture `v3.10_Retrato.xml`)
- **WHEN** a DANFE é renderizada
- **THEN** o PDF gerado é idêntico ao gerado antes desta change (zero regressão visual)

#### Scenario: NF-e com `<pag>` vazio

- **GIVEN** uma NF-e com `<pag></pag>` (presente mas vazio — não conforme, mas possível por bug do emissor)
- **WHEN** a DANFE é renderizada
- **THEN** o bloco "Forma de Pagamento" é omitido (não desenha cabeçalho nem tabela)

### Requirement: Helper `FormaPagamento.GetDescricao()` reusable across the project

O renderer SHALL expor um helper público (extension method ou static helper) que retorna a descrição legível de um valor `FormaPagamento` lendo o `DescriptionAttribute` do enum por reflexão. Esse helper SHALL retornar uma string não-vazia para todos os valores definidos no enum (`01` a `19`, `90`, `99`).

#### Scenario: Descrição de cada valor do enum

- **GIVEN** o enum `FormaPagamento` com `[Description("Dinheiro")]` em `fpDinheiro`
- **WHEN** `FormaPagamentoExtensions.GetDescricao(FormaPagamento.fpDinheiro)` é chamado
- **THEN** retorna `"Dinheiro"`

#### Scenario: Helper não lança em valor desconhecido

- **GIVEN** um valor inteiro `(FormaPagamento)42` (fora do enum, caso de XML com novo código antes do enum ser atualizado)
- **WHEN** o helper é chamado
- **THEN** retorna `""` (string vazia) ou o código numérico como string — sem exceção
