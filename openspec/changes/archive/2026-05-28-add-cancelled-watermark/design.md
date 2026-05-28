## Context

DanfeSharp renderiza DANFE em PDF a partir de XML de NF-e. Para ambiente de homologação (NF-e emitida em ambiente de testes da SEFAZ), o renderer já desenha uma marca d'água "SEM VALOR FISCAL — AMBIENTE DE HOMOLOGAÇÃO" centralizada em cada página da DANFE — pattern em `DanfePagina.DesenharAvisoHomologacao()` (linha 97), chamado por `Danfe.CriarPagina()` quando `ViewModel.TipoAmbiente != 1`.

**Realidade do cancelamento em NF-e modelo 55** (input crítico do usuário durante o review): o cancelamento NÃO acontece via alteração da própria NF-e — ele é registrado via **evento separado** (`NFeProcEvento` com `tpEvento=110111` — Cancelamento), que retorna `cStat=135` (Evento registrado e vinculado a NF-e). O XML original da NF-e (`<NFe><protNFe><infProt>`) mantém `cStat=100` (Autorizado) mesmo após o cancelamento — quem sabe que a nota foi cancelada é o sistema do emissor, consultando o histórico de eventos da chave de acesso.

O `cStat=101` aparece apenas em:
- Retornos de autorização rejeitada com motivo "NF-e já cancelada" (raro);
- Alguns sistemas que **modificam o XML carregado** para refletir o status atual (não padrão, mas existe);
- O próprio XML de evento de cancelamento, em campos diferentes do `infProt.cStat`.

A consequência: **detectar cancelamento via `infProt.cStat == 101`** (como a [PR #8](https://github.com/nfe/DanfeSharp/pull/8) do @mateuszanini fez em 2023) **funciona em casos limítrofes**, mas não cobre o cenário comum de produção. O emissor do sistema é quem sabe pelo banco de dados que a NF-e foi cancelada e precisa **informar isso ao renderer** explicitamente.

**Estado atual do código (auditado)**

- `DanfeSharp/Modelo/DanfeViewModel.cs:220` — propriedade `CodigoStatusReposta` (int?) já existe, populada por `DanfeViewModelCreator.cs:414` lendo `infProt.cStat` do XML protocolo.
- `DanfeSharp/DanfePagina.cs:97` — método `DesenharAvisoHomologacao()` usa `TextStack` centralizado com `Danfe.EstiloPadrao.CriarFonteRegular(48)` (texto "SEM VALOR FISCAL") + `CriarFonteRegular(30)` (texto "AMBIENTE DE HOMOLOGAÇÃO"), cor `RGB(0.35, 0.35, 0.35)`. Padrão visual robusto e estabelecido.
- `DanfeSharp/Danfe.cs:CriarPagina` (linha ~167-195) — onde a página é construída; já tem condicional `if (ViewModel.TipoAmbiente != 1) p.DesenharAvisoHomologacao();` (homologação).
- `Paginas` é `List<DanfePagina>` — o renderer já é multi-página (graças ao fix de paginação aplicado em `feature/danfe-forma-pagamento`).
- PR #8 do @mateuszanini contém commit `f658489` com o método `DesenharAvisoCancelamento()` já escrito — texto "DOCUMENTO CANCELADO", mesma estrutura de `DesenharAvisoHomologacao`. Vou reaproveitar.

**Base normativa**

[MOC 7.0 — Anexo II §3.10.1](https://www.confaz.fazenda.gov.br/legislacao/arquivo-manuais/moc7-anexo-ii-manual-especificacoes-tecnicas-danfe-codigo-barras.pdf) (Marca d'Água), literal:

> "O formulário poderá conter marca d'água desde que não prejudique a legibilidade dos dados impressos."

Apenas isso. Verifiquei literalmente o PDF do CONFAZ em 2026-05-28 (via `pdftotext -enc UTF-8`). O MOC **autoriza** o uso, **não obriga** marca para NF-e cancelada, **não especifica** texto/posição/opacidade. O texto "DOCUMENTO CANCELADO" e o estilo (fonte 48pt cinza centralizada) são **convenção universal de mercado** alinhada ao padrão existente do `DesenharAvisoHomologacao`. Como nos casos #38 e #39 desta sprint, esta change segue a convenção sem reivindicar conformidade normativa específica.

**Stakeholders**

- Cliente **Revenda Mais** (relator)
- Consumidores externos do `nfe/DanfeSharp` (API NFe.io que precisa renderizar DANFE de notas canceladas)
- Receptores das DANFEs (contadores, fiscais)

**Constraints**

- Manter compatibilidade total: DANFE de NF-e não-cancelada renderiza idêntico (zero regressão).
- Sem breaking changes na API pública.
- Funcionar em retrato + paisagem + multi-página.
- Funcionar em ambiente de produção E homologação simultaneamente (NF-e de homologação que foi cancelada → ambos os avisos aparecem).

## Goals / Non-Goals

**Goals:**

- Marca d'água "DOCUMENTO CANCELADO" centralizada visível em cada página da DANFE quando a NF-e está cancelada.
- Mecanismo primário de detecção: flag explícita `DanfeViewModel.IsCancelled` (que o consumer NFe.io seta com base no estado real do banco de dados, não do XML).
- Mecanismo fallback: `CodigoStatusReposta == 101` continua acionando a marca, para o cenário (raro mas existente) de XML modificado pelo consumer para refletir cancelamento.
- Zero impacto em DANFE de NF-e ativa.
- Suporte para todas as orientações (retrato, paisagem) e em multi-página.
- Co-existência com aviso de homologação (uma NF-e cancelada em homologação mostra os dois avisos sobrepostos — comportamento esperado, sinaliza claramente o estado).

**Non-Goals:**

- Não tratar outros estados (denegada — `cStat=110`; inutilizada — `cStat=102` quando inutilização; EPEC). Cada um teria texto distinto e fluxo próprio — fora do escopo desta change.
- Não tratar DANFE NFCe (modelo 65) — manual e renderer separados.
- Não parsear o XML do **evento de cancelamento** (`procEventoNFe`) automaticamente. O consumer fornece o `IsCancelled` flag baseado em conhecimento próprio. Parsear o evento exigiria expandir o `DanfeViewModelCreator` para aceitar 2 XMLs (NF-e + evento), o que é uma refatoração maior — pode ser nova change futura se a demanda aparecer.
- Não permitir customização do texto da marca via API (sempre "DOCUMENTO CANCELADO"). Se aparecer demanda específica de cliente, vira nova change.
- Não suportar transparência via PDF alpha channel — fica com cor cinza sólida (mesmo padrão do "SEM VALOR FISCAL"). Transparência real exigiria investigação da PDFClown.NetStandard API — não vale o trade-off no momento.

## Decisions

### Decision 1: Detecção via flag `IsCancelled` (primário) + `cStat=101` (fallback OR)

**Escolha:** adicionar `public bool IsCancelled { get; set; }` ao `DanfeViewModel` (default `false`). A condicional em `Danfe.CriarPagina` checa `ViewModel.IsCancelled || ViewModel.CodigoStatusReposta == 101`. Qualquer um dos dois trigger desenha a marca.

**Por que ambos:**
- Flag explícita é a forma **correta** para o cenário real (cancelamento via evento) — o consumer NFe.io define com base no banco de dados.
- `cStat=101` mantido por **defense in depth** + compatibilidade com a abordagem original da PR #8 (caso algum consumer já dependa). É OR, então não conflita com a flag.
- Zero ambiguidade: se algum dos dois é true, marca aparece. Se ambos false (caso comum), não aparece.

**Alternativa rejeitada:** apenas `cStat=101`. Não funciona na prática para cancelamento via evento (cenário comum). PR #8 fez essa escolha em 2023 — seria errado repetir sem ajuste.

**Alternativa rejeitada:** apenas `IsCancelled` flag. Funciona conceitualmente, mas obrigaria toda configuração via código. Manter o fallback de cStat=101 é zero custo e cobre alguns casos atípicos.

**Alternativa rejeitada:** parsear automaticamente o evento de cancelamento se o XML embedded contiver `procEventoNFe` com `tpEvento=110111` e `cStat=135`. Mais "completo" tecnicamente, mas:
1. Exigiria expandir o `DanfeViewModelCreator` para aceitar/processar a estrutura do evento;
2. Na prática, sistemas como a API NFe.io não embedam o evento no XML principal — o consumer carrega ambos separadamente do storage;
3. Aumenta superfície de bugs em troca de elegância marginal;
4. Pode ser nova change futura (`load-cancellation-event-xml`) se a demanda aparecer.

### Decision 2: Reaproveitar o método `DesenharAvisoCancelamento` da PR #8

**Escolha:** o método criado por @mateuszanini em 2023 (`DanfePagina.cs:DesenharAvisoCancelamento`, +16 linhas no commit `f658489`) é tecnicamente correto — espelha exatamente o `DesenharAvisoHomologacao`, mesmo `TextStack`, mesma cor, mesma fonte 48pt. O problema da PR #8 era o **gatilho** (cStat=101 only), não a renderização. Vou copiar o método dele literalmente (com créditos no commit message).

**Por que reaproveitar:**
- Trabalho de qualidade — não precisa reescrever.
- Reconhece a contribuição do @mateuszanini pública (ele tentou em 2023, ficou parado; agora finaliza com mecanismo correto).
- Reduz risco: padrão visual já validado contra o renderer PDFClown nos avisos de homologação.

### Decision 3: Texto fixo "DOCUMENTO CANCELADO"

**Escolha:** texto invariável `"DOCUMENTO CANCELADO"`, sem configuração via API.

**Por que esse texto:**
- Concorda gramaticalmente com "DANFE" = "Documento Auxiliar da Nota Fiscal Eletrônica" (masculino → "cancelado").
- É a convenção visual mais comum em DANFEs comerciais (vi em TOTVS, SAP, SmartGo, eMissor — padrão de mercado).
- Era a escolha do @mateuszanini na PR #8.

**Alternativa rejeitada:** "CANCELADA" (texto da issue #40, alinhando com a fala da Carolina). Concorda com "Nota Fiscal Eletrônica" (feminino), mas é ambíguo quando vem na DANFE (que é masculino). Decisão favorável a clareza gramatical sobre fidelidade ao texto do solicitante.

**Alternativa rejeitada:** texto configurável via API. Overengineering — não há demanda real para variar texto.

### Decision 4: Sobreposição com aviso de homologação (NF-e cancelada em homologação)

**Escolha:** quando uma NF-e está em homologação **E** está cancelada (cenário atípico de testes), AMBOS os avisos são desenhados — "SEM VALOR FISCAL" (homologação) + "DOCUMENTO CANCELADO" (cancelada), centralizados no mesmo `RetanguloCorpo`. Sobreposição visual aceita.

**Por que aceitar:**
- Sinaliza explicitamente os dois estados (que são ambos válidos e relevantes).
- Implementar separação (priorizar um dos avisos) adicionaria complexidade sem ganho real — em produção, NF-e canceladas em homologação são casos de teste, não impressão fiscal real.
- Receptor que ver os dois avisos sobrepostos entende claramente que é um documento sem valor.

### Decision 5: Cor cinza sólida em vez de transparência real

**Escolha:** usar `RGB(0.35, 0.35, 0.35)` cinza médio sólido (mesmo da PR #8 e do aviso de homologação). Não tentar PDF alpha channel.

**Por que:**
- Efeito visual idêntico ao "transparente" para o olho humano — texto cinza médio sobre fundo branco fica visualmente "translúcido" sem precisar de transparency object no PDF.
- Padrão já validado no `DesenharAvisoHomologacao` — visto em milhares de DANFEs em produção sem reclamação de legibilidade.
- PDFClown.NetStandard pode suportar alpha mas exigiria investigação adicional — trade-off de tempo não vale.

## Risks / Trade-offs

- **[Risk]** Consumer NFe.io pode esquecer de setar `IsCancelled = true` ao gerar DANFE de uma NF-e cancelada → DANFE sairia sem marca. → **Mitigation:** documentar claramente na API (XML doc da propriedade); adicionar exemplo na fixture de testes; coordenar com time da API NFe.io para garantir que setem.
- **[Risk]** Sobreposição com aviso de homologação pode ficar visualmente confuso. → **Mitigation:** cenário raríssimo (NF-e cancelada em homologação); a sobreposição na verdade reforça "não tem valor fiscal" + "está cancelada", que são informações compatíveis e ambas verdadeiras. Não tratar.
- **[Trade-off]** Manter detecção via `cStat=101` como fallback adiciona ~5 caracteres na condicional. → Aceito; é defense in depth.
- **[Risk]** Texto invariável pode não atender cliente futuro com necessidade específica (ex.: "INUTILIZADA", "DENEGADA"). → **Mitigation:** se aparecer, abrir nova change. Premature generalization seria pior.

## Migration Plan

1. Adicionar `IsCancelled` ao `DanfeViewModel.cs` (propriedade pública, default false, com XML doc explicando o uso).
2. Copiar método `DesenharAvisoCancelamento` da PR #8 para `DanfePagina.cs` (mesmo padrão de `DesenharAvisoHomologacao`).
3. Adicionar chamada condicional em `Danfe.cs:CriarPagina` após `DesenharAvisoHomologacao`: `if (ViewModel.IsCancelled || ViewModel.CodigoStatusReposta == 101) p.DesenharAvisoCancelamento();`.
4. Adicionar testes unitários: (a) DANFE sem flag e sem cStat=101 → nenhuma marca; (b) DANFE com `IsCancelled=true` → marca aparece; (c) DANFE com `CodigoStatusReposta=101` → marca aparece; (d) DANFE com ambos → marca aparece uma vez; (e) DANFE multi-página com `IsCancelled=true` → marca em todas as páginas.
5. Criar fixture demo nova: `v4_DanfeCancelada.xml` (NF-e modelo 55 com infProt.cStat=100 normal) + teste que carrega o XML, seta `model.IsCancelled = true`, gera PDF.
6. PR draft → review → merge → release.

**Rollback:** se houver regressão visual em produção, revert do PR. Não há migração de dados nem state persistido — DANFE é regenerado a cada request.

## Open Questions

- **Quando o consumer NFe.io vai integrar?** A flag `IsCancelled` é aditiva e default false, então DanfeSharp pode liberar sem coordenação. Mas pra render DANFE cancelada na prática, o time da API precisa setar o flag — alinhar timing com eles. _(Manual, post-merge.)_
- **Vale criar API tipo `Danfe.FromCancelledInvoice(xml, eventXml)` para parsing automático do evento?** Não nesta change — fica como follow-up se demanda aparecer (ver Non-Goals).
- **Sobreposição com aviso de homologação fica ruim?** Validar visualmente com fixture específica antes de mergear.
