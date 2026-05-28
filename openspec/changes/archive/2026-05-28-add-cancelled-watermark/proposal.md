## Why

O `nfe/DanfeSharp` não tem indicação visual quando o DANFE é gerado para uma **NF-e cancelada**. Receptores fiscais e contadores que recebem o PDF podem confundir o documento com uma NF-e ativa, causando erros operacionais (contabilização indevida, dupla escrita, etc.). Cliente Revenda Mais reportou esse cenário (issue [nfe/DanfeSharp#40](https://github.com/nfe/DanfeSharp/issues/40), umbrella [#37](https://github.com/nfe/DanfeSharp/issues/37)) — eles emitem NF-e que ocasionalmente são canceladas e querem distribuir o PDF cancelado para registro histórico, mas precisam que a marca "CANCELADO" seja evidente.

**Observação técnica crítica:** o cancelamento de NF-e modelo 55 é registrado via **evento separado** (`NFeProcEvento` com `tpEvento=110111`, evento de Cancelamento, retornando `cStat=135` quando autorizado). O XML original da NF-e (`<protNFe><infProt><cStat>`) **permanece com 100 (autorizado)** mesmo após o cancelamento — quem sabe que ela foi cancelada é o sistema do emissor, que consulta o histórico de eventos. Portanto, a detecção via `infProt.cStat == 101` (presente na PR #8 antiga do @mateuszanini, 2023) **não funciona na prática** para a maioria dos casos reais — `cStat=101` é o status de **rejeição** "NF-e já cancelada" quando o emissor tenta processar uma chave já cancelada, não o status normal de uma nota cancelada com sucesso.

## What Changes

- Adicionar propriedade `DanfeViewModel.IsCancelled` (`bool`, default `false`) — flag explícita que o consumidor seta quando sabe que a NF-e está cancelada (informação vinda do banco de dados / sistema do emissor, não do XML da nota).
- Manter detecção legacy via `CodigoStatusReposta == 101` como fallback OR (zero custo, cobre o cenário raro de XML com infProt já refletindo o status de cancelamento).
- Criar método `DanfePagina.DesenharAvisoCancelamento()` espelhando o padrão existente de `DesenharAvisoHomologacao()` (mesmo `TextStack` centralizado, fonte 48pt, cor RGB(0.35, 0.35, 0.35) cinza médio).
- Adicionar chamada condicional em `Danfe.cs:CriarPagina` para desenhar o aviso em **todas as páginas** quando a NF-e está cancelada — independentemente da orientação (retrato/paisagem) e independentemente do ambiente (produção/homologação; numa NF-e cancelada em homologação, ambos os avisos aparecem).
- Texto da marca: **"DOCUMENTO CANCELADO"** — concorda gramaticalmente com "DANFE" (Documento Auxiliar), é a convenção de mercado mais comum (TOTVS/SAP/SmartGo) e era a escolha do @mateuszanini na PR #8.
- Renderização funciona para DANFE multi-página: cada página recebe a marca, garantindo que mesmo se receptor olhar só uma folha veja o status.
- Fixture demo e teste cobrindo: NF-e não-cancelada (zero regressão), NF-e cancelada via flag, NF-e cancelada via cStat=101.

## Capabilities

### New Capabilities

- `danfe-cancelled-watermark`: detecção do estado cancelado da NF-e (via flag `IsCancelled` ou via `cStat=101` legacy) + renderização da marca d'água "DOCUMENTO CANCELADO" centralizada em todas as páginas da DANFE de NF-e modelo 55.

### Modified Capabilities

_Nenhuma — não toca `danfe-icms-column` (#38) nem `danfe-payment-block` (#39)._

## Impact

- **Código afetado** (escopo bem confinado, ~25 linhas no total):
  - `DanfeSharp/Modelo/DanfeViewModel.cs` — nova propriedade `IsCancelled` (bool, default false)
  - `DanfeSharp/DanfePagina.cs` — novo método `DesenharAvisoCancelamento()` (espelha `DesenharAvisoHomologacao`)
  - `DanfeSharp/Danfe.cs:CriarPagina` — adiciona condicional `if (ViewModel.IsCancelled || ViewModel.CodigoStatusReposta == 101) p.DesenharAvisoCancelamento();`
- **APIs públicas**: aditivas. Nova propriedade `IsCancelled` (default false → comportamento prévio preservado). Nenhuma quebra.
- **Backward compatibility**: total. Consumidores que não setam `IsCancelled` continuam gerando DANFE sem a marca; comportamento idêntico ao anterior.
- **Cliente impactado positivamente**: Revenda Mais (escopo direto) + qualquer emissor que precise distribuir DANFE de notas canceladas.
- **Não impacta**: XML da NF-e, fluxo de cancelamento na SEFAZ, geração da NF-e em si, NFCe (modelo 65, já tem manual próprio), outros eventos (CC-e/Carta de Correção, EPEC, etc.).
- **Base normativa**: [MOC 7.0 — Anexo II §3.10.1](https://www.confaz.fazenda.gov.br/legislacao/arquivo-manuais/moc7-anexo-ii-manual-especificacoes-tecnicas-danfe-codigo-barras.pdf) (Marca d'Água) autoriza literalmente o uso: *"O formulário poderá conter marca d'água desde que não prejudique a legibilidade dos dados impressos."* — só isso. NÃO obriga nem especifica texto/posição/opacidade para nota cancelada. O texto "DOCUMENTO CANCELADO" e o estilo visual (fonte 48pt cinza centralizada) são **convenção universal de mercado** alinhada com a renderização de "SEM VALOR FISCAL" em ambiente de homologação (que o DanfeSharp já implementa). Verifiquei literalmente o PDF do CONFAZ em 2026-05-28.
- **Issue mãe**: nfe/DanfeSharp#37; irmãs: nfe/DanfeSharp#38 (mergeado) + nfe/DanfeSharp#39 (PR #42 draft).
- **Referência prévia**: PR #8 do @mateuszanini (2023, branch `feature/aviso-cancelamento`) — implementação parcialmente correta no commit `f658489` (visual perfeito), mas com detecção via `cStat=101` apenas, que não funciona na prática para o cenário real (cancelamento via evento). Esta change reaproveita o método `DesenharAvisoCancelamento()` dele mas substitui o trigger por `IsCancelled` flag (primário) + `cStat=101` (fallback).
