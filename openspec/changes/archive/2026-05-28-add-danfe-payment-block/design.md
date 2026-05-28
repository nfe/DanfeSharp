## Context

DanfeSharp é um renderer de DANFE em C# (.NET netstandard2.0) que carrega o XML da NF-e via `XmlSerializer` (`DanfeSharp.Esquemas.ProcNFe`) e o traduz para um modelo de apresentação (`DanfeSharp.Modelo.DanfeViewModel`) consumido por blocos de desenho. A DANFE atual termina visualmente em "DADOS DOS PRODUTOS / SERVIÇOS" + cálculo de impostos + transportador + dados adicionais — sem qualquer representação de forma de pagamento.

**Estado atual do código (auditado)**

- `DanfeSharp/Esquemas/ProcNFe.cs:386-414` — classes `pag` e `detPag` já existem para deserializar o XML. **Falta a tag `xPag`** (descrição livre da forma de pagamento, especialmente quando `tPag=99`).
- `DanfeSharp/Esquemas/ProcNFe.cs:1020+` — enum `FormaPagamento` já completo (01–19, 90, 99) com `[Description("...")]` em cada valor (`fpDinheiro="Dinheiro"`, `fpCartaoCredito="Cartão de Crédito"`, etc.). Mapeamento `tPag → descrição` já está pronto para uso.
- `DanfeSharp/Modelo/PagamentoViewModel.cs` — classes `PagamentoViewModel`, `DetalheViewModel`, `CartaoViewModel` já existem. **Falta a propriedade `Descricao`** em `DetalheViewModel` para refletir `xPag`.
- `DanfeSharp/Modelo/DanfeViewModel.cs:49-51, 314` — propriedade `Pagamento` (`List<PagamentoViewModel>`) declarada e inicializada.
- `DanfeSharp/Modelo/DanfeViewModelCreator.cs:318-332` — já popula `model.Pagamento` lendo `pag.detPag`. **Falta** popular `detalhe.Descricao` (que ainda não existe).
- `DanfeSharp/Blocos/` — pasta com renderers (`BlocoCalculoImposto`, `BlocoTransportador`, etc.). **Não existe** `BlocoFormaPagamento`.
- `DanfeSharp/Danfe.cs:58-75` — sequência de inserção dos blocos no construtor da `Danfe`. **Nenhuma chamada** a um bloco de pagamento.

**Base normativa**

- O **MOC 7.0 — Anexo II — Manual de Especificações Técnicas do DANFE** ([CONFAZ, Outubro/2020](https://www.confaz.fazenda.gov.br/legislacao/arquivo-manuais/moc7-anexo-ii-manual-especificacoes-tecnicas-danfe-codigo-barras.pdf)) **não tem seção dedicada a forma de pagamento** para DANFE de NF-e modelo 55. Verifiquei literalmente o PDF oficial em 2026-05-28 (extraído via `pdftotext -enc UTF-8`): zero ocorrências de `pagamento`, `forma de pag`, `detPag`, `tPag`, `xPag`, `vPag`. As seções 3.1.1 a 3.1.10 cobrem Chave de Acesso, Dados da NF-e, Emitente, Local de Retirada/Entrega, Fatura/Duplicatas, Produtos/Serviços, Inf. Complementares, Reservado ao Fisco, Transportador — nenhuma para pagamento.
- A **[NT 2016.002](https://www.nfe.fazenda.gov.br/portal/exibirArquivo.aspx?conteudo=tQNDQOeNeyI%3D)** (v1.50, em produção desde 2018) tornou o grupo `<pag>` obrigatório no XML da NF-e. O **Manual do DANFE NFC-e** (MOC 7.0 Anexo III, separado) mandou exibir o bloco no cupom da NFCe — mas isso é NFCe (modelo 65), não NF-e (modelo 55).
- Para **DANFE de NF-e modelo 55**, o bloco "Forma de Pagamento" é **convenção universal de mercado** (TOTVS, SAP, SmartGo, eMissor, demais renderers da indústria), não mandato literal do MOC. Receptores fiscais e contadores esperam ver esse bloco; a ausência gera dúvida sobre a integridade da NF-e (foi o que motivou a demanda do cliente Revenda Mais).

**Stakeholders**

- Cliente **Revenda Mais** (relator, via @Carolina Fagundes em Microsoft Teams 2026-05-27, invoice de referência `9175b02ac0cf4ed898025c4bad09e2fe`)
- Consumidores externos do `nfe/DanfeSharp` (Library NuGet/fork interno do NFe.io)
- Receptores das DANFEs (varejistas, contadores, fiscais)

**Constraints**

- Manter compatibilidade total: DANFE sem `<pag>` no XML deve renderizar idêntica ao estado atual (zero regressão visual em emitentes que ainda não usam o grupo `<pag>` ou que emitem `tPag=90` "Sem Pagamento").
- Nada de breaking change na API pública (`Danfe.GerarPdf`, `DanfeViewModel.Pagamento`).
- Mudanças confinadas ao renderer + ao schema + ao viewmodel; XML/parser/emissão não mudam.
- Sem dependências externas novas.
- Schema XML deve aceitar `<pag>` ausente (caso de NF-e antiga, anterior à NT 2016.002, ainda parseável).

## Goals / Non-Goals

**Goals:**

- Bloco visual "FORMA DE PAGAMENTO" passa a aparecer na DANFE quando o XML traz `<pag><detPag>` preenchido, com tabela contendo cabeçalho "FORMA PAGAMENTO" + "VALOR" e uma linha por `detPag`.
- Mapeamento confiável de `tPag` numérico para descrição legível (`01 → "Dinheiro"`, `03 → "Cartão de Crédito"`, ..., `99 → "Outros"`) usando o `DescriptionAttribute` já presente no enum.
- `tPag=99` (Outros) mostra o conteúdo de `xPag` na coluna FORMA PAGAMENTO (ex.: "RECURSOS PROPRIOS" do caso Revenda Mais).
- Quando `xPag` está preenchido mesmo com `tPag` conhecido (caso atípico mas permitido pelo schema), prevalece o `xPag` (mais específico que a descrição genérica do `tPag`).
- Valor formatado em moeda brasileira (`R$ 48.000,00`) com separador de milhar e duas casas decimais.
- DANFE sem `<pag>` no XML continua renderizando idêntica (bloco omitido).
- Cobertura por testes unitários nos cenários acima.

**Non-Goals:**

- Não renderizar subgrupos `<card>` (CNPJ adquirente, bandeira, autorização) — só FORMA + VALOR nesta change.
- Não cobrir DANFE NFCe (modelo 65) — esse já tem manual próprio (MOC 7.0 Anexo III) e renderer separado (`DanfeNFC` em DanfeSharp).
- Não mudar o XML schema da NF-e nem o parser.
- Não tratar o campo `<vTroco>` na primeira iteração (mais relevante para NFCe).
- Não criar UI para configurar layout (cores, fontes) — segue o estilo dos blocos existentes.

## Decisions

### Decision 1: Adicionar `xPag` ao schema, ao ViewModel e popular no Creator

**Escolha:** adicionar a tag `xPag` (string opcional) à classe `detPag` em `ProcNFe.cs`, adicionar propriedade `Descricao` (string opcional) ao `DetalheViewModel` em `PagamentoViewModel.cs`, e ajustar `DanfeViewModelCreator.cs:328` para `detalhe.Descricao = detPag.xPag`.

**Alternativa rejeitada:** ignorar `xPag` e renderizar sempre a descrição genérica do `[Description]` do enum `FormaPagamento`. Rejeitada porque perderíamos o caso `tPag=99` ("Outros") — o XML diz "RECURSOS PROPRIOS" mas o DANFE mostraria só "Outros", esvaziando a informação. A própria spec da NF-e (campo YA03 — `xPag`) existe exatamente para esse caso.

### Decision 2: Renderer como `BlocoBase` (não `ElementoBase`) seguindo o padrão dos blocos do DANFE

**Escolha:** criar `DanfeSharp/Blocos/BlocoFormaPagamento.cs` herdando de `BlocoBase`, com `Cabecalho = "Forma de Pagamento"`, `Posicao = PosicaoBloco.Topo` (alinhado pra esquerda como blocos finais), e usando `AdicionarLinhaCampos()` para cada `DetalheViewModel`. Cada linha tem 2 campos: `FORMA PAGAMENTO` (descrição) + `VALOR` (numérico, com helpers de moeda).

**Alternativa considerada e rejeitada:** usar `ElementoBase` + objeto `Tabela` (padrão de `TabelaProdutosServicos`). Rejeitada porque:
- `TabelaProdutosServicos` é caso especial: tem 15 colunas, layout horizontal complexo, e ainda é inserido fora do `AdicionarBloco<>` em `Danfe.cs:141`. Fugir desse molde rico em hooks não simplifica o caso simples de pagamento.
- Os outros blocos (`BlocoCalculoImposto`, `BlocoDuplicataFatura`, `BlocoTransportador`) usam `BlocoBase` + `AdicionarLinhaCampos()` e o resultado visual é consistente com o resto da DANFE.
- `BlocoBase` já integra com o `AdicionarBloco<T>()` em `Danfe.cs` — basta uma linha pra inserir no fluxo.

### Decision 3: Posicionar o bloco entre `BlocoCalculoImposto` e `BlocoTransportador`

**Escolha:** inserir `AdicionarBloco<BlocoFormaPagamento>()` em `Danfe.cs:71` (entre a linha do `BlocoCalculoImposto` e a do `BlocoTransportador`). Ordem resultante:

1. IdentificacaoEmitente
2. DestinatarioRemetente
3. LocalRetirada / LocalEntrega (opcionais)
4. DuplicataFatura
5. CalculoImposto
6. **FormaPagamento ← novo**
7. Transportador
8. DadosAdicionais
9. (CalculoIssqn, condicional)

**Alternativa considerada:** inserir depois de `BlocoTransportador`. Rejeitada porque visualmente, na maioria das DANFEs comerciais que vi referenciadas, a forma de pagamento aparece próxima dos blocos de cálculo (antes do transportador) — segue a lógica de "tudo relacionado a valores fiscais junto, transportador é logística".

**Alternativa considerada:** flag de configuração para escolher a posição. Rejeitada por overengineering — uma posição razoável + sólida é melhor que duas opções configuráveis.

### Decision 4: Inserir o bloco APENAS quando o `ViewModel.Pagamento` tiver itens

**Escolha:** o construtor do `BlocoFormaPagamento` valida `ViewModel.Pagamento?.Any(p => p.DetalhePagamento?.Any() == true) == true`; se falso, nenhuma linha é adicionada e o bloco renderiza vazio (height efetivo ≈ 0). Alternativamente, condicionar o `AdicionarBloco<BlocoFormaPagamento>()` em `Danfe.cs` a `ViewModel.Pagamento?.Any() == true`. Ambas estratégias são aceitáveis; a primeira é mais robusta (o bloco se auto-omitir não corrompe o fluxo se for chamado em um caso de edge), a segunda é mais econômica (não cria objeto desnecessariamente).

**Decisão final:** auto-omitir dentro do construtor (estratégia 1) + condicional em `Danfe.cs` adicionalmente (estratégia 2) — defense in depth.

### Decision 5: Mapeamento `tPag → descrição` via `DescriptionAttribute` do enum

**Escolha:** criar helper estático `FormaPagamentoExtensions.GetDescricao(FormaPagamento)` (ou método em outro lugar pertinente) que retorna o conteúdo de `[Description]` por reflexão. O cabeçalho da célula "FORMA PAGAMENTO" usa `xPag` se presente, senão chama esse helper.

**Alternativa rejeitada:** switch literal mapeando código → string. Rejeitada porque duplica info já no enum, e o `DescriptionAttribute` é a convenção C# para descrições. Também: se a NT adicionar `fp20` (novo meio de pagamento), basta acrescentar uma linha no enum com `[Description]`; o helper continua funcionando.

### Decision 6: Sem feature flag

A correção é puramente aditiva (sem mudança em comportamento existente). Não há cenário onde um consumidor queira a DANFE sem o bloco quando o XML tem `<pag>`. Logo, sem flag de configuração para isso. Em algum momento, se aparecer demanda do tipo "quero a DANFE minimalista sem pagamento", podemos adicionar via `DanfeViewModel.ExibirPagamento` (default `true`).

## Risks / Trade-offs

- **[Risk]** A coluna VALOR do novo bloco pode quebrar o layout em folha com muito pouco espaço vertical disponível (NF-e com muitos itens de produto + muitos detPag). → **Mitigation:** o `BlocoBase` lida com paginação no engine PDF existente; testar com fixtures que tenham 10+ detPag pra confirmar o comportamento. Adicionar fixture XML se necessário.
- **[Risk]** Schema `detPag.xPag` adicionado pode quebrar parse de XMLs antigos sem `<xPag>` — improvável (XmlSerializer trata elementos faltantes como null por padrão), mas testar com XML pre-NT-2016.002 (provavelmente `v3.10_Retrato.xml` ou `v1.xml` em `DanfeSharp.Test/Xml/NFe/`). → **Mitigation:** `xPag` declarado como `string?`, sem `[XmlElement(IsRequired = true)]`. Teste de regressão garante que XMLs sem `<pag>` continuam parseando.
- **[Trade-off]** Inserir o bloco SEMPRE no fluxo (mesmo quando vazio) gasta nanossegundos extras na construção do `Danfe`. → Aceito; o gain de robustez supera.
- **[Trade-off]** Usando `DescriptionAttribute` via reflexão para `tPag → descrição` tem custo (mínimo) por linha. → Aceito; é a forma idiomática em C# e o número de detPag por NF-e é tipicamente ≤ 5.
- **[Risk]** Tabela `tPag` pode ser atualizada por NT futura (NT 2020.001 já adicionou códigos 16-19). → **Mitigation:** o enum em `ProcNFe.cs:1020` já está atualizado até NT 2020.001; mudanças futuras pedirão acréscimo simples (1 valor + 1 `[Description]`).

## Migration Plan

1. Adicionar `xPag` (string opcional) à classe `detPag` em `ProcNFe.cs`.
2. Adicionar propriedade `Descricao` (string opcional) ao `DetalheViewModel` em `PagamentoViewModel.cs`.
3. Atualizar `DanfeViewModelCreator.cs:328` para popular `detalhe.Descricao = detPag.xPag`.
4. Criar helper `FormaPagamentoExtensions.GetDescricao(FormaPagamento)` (provavelmente em `DanfeSharp/Esquemas/ProcNFe.cs` perto do enum, ou em `Modelo/` como classe de extensão).
5. Criar `DanfeSharp/Blocos/BlocoFormaPagamento.cs` herdando `BlocoBase`, com `Cabecalho = "Forma de Pagamento"` e iteração sobre `ViewModel.Pagamento[].DetalhePagamento`.
6. Inserir `AdicionarBloco<BlocoFormaPagamento>()` em `Danfe.cs` entre `BlocoCalculoImposto` e `BlocoTransportador`.
7. Adicionar testes unitários cobrindo os 5 cenários da spec.
8. Atualizar snapshots/fixtures de PDF em `DanfeSharp.Test/bin/Debug/Output/DeXml/` (revisar diff visual: `v4_ComLocalEntrega.pdf` provavelmente passa a exibir o bloco se a fixture XML tiver `<pag>`).
9. PR em draft → review → merge → release minor do pacote (se aplicável).

**Rollback:** se houver regressão, revert do PR. Não há migração de dados nem state persistido.

## Open Questions

- **xPag prevalece sobre descrição do enum?** Tenho que confirmar com Carolina/cliente Revenda Mais se quando ambos estão preenchidos (`tPag=01` "Dinheiro" + `xPag="Pagamento à vista"`) o renderer deve mostrar `xPag` ou ignorar. Default da spec: prevalece `xPag` (mais específico). Reabrir se a Revenda Mais quiser o oposto.
- **Trato `<vTroco>` nesta change?** Atualmente `Troco` está no `PagamentoViewModel`. Não há mandato literal para mostrar troco no DANFE NF-e (MOC silencia; é convenção mais forte em NFCe). Decisão atual: **não exibir** troco nesta change (out of scope); reavaliar se cliente pedir.
- **Largura da coluna VALOR é suficiente para R$ XX.XXX.XXX,XX?** Validar visualmente com fixtures com valores grandes (R$ 1.000.000,00 e acima) na fase de testes.
- **DANFE em paisagem (`Orientacao.Paisagem`) acomoda o novo bloco?** Validar visualmente; ajustar `Posicao` ou `Estilo` se necessário.
- **Helper `GetDescricao` deve ficar em `DanfeSharp.Esquemas` ou `DanfeSharp.Modelo`?** Decisão tátil — resolver na implementação. Provavelmente `Modelo` (não polui o schema layer).
