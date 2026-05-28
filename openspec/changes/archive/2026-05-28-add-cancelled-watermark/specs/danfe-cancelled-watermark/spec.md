## ADDED Requirements

> **Base normativa:** [MOC 7.0 — Anexo II §3.10.1](https://www.confaz.fazenda.gov.br/legislacao/arquivo-manuais/moc7-anexo-ii-manual-especificacoes-tecnicas-danfe-codigo-barras.pdf) (Marca d'Água) autoriza literalmente: *"O formulário poderá conter marca d'água desde que não prejudique a legibilidade dos dados impressos."* — não obriga nem especifica texto, posição ou opacidade para NF-e cancelada. Verifiquei o PDF do CONFAZ em 2026-05-28. O texto "DOCUMENTO CANCELADO" e o estilo visual (fonte 48pt, cinza RGB 0.35, centralizada) são **convenção universal de mercado** (TOTVS, SAP, SmartGo, eMissor) alinhada ao padrão do aviso "SEM VALOR FISCAL — AMBIENTE DE HOMOLOGAÇÃO" já implementado no DanfeSharp.
>
> **Realidade do cancelamento em NF-e modelo 55:** o cancelamento NÃO é refletido no `<cStat>` do XML protocolo da NF-e original — ele é registrado via **evento separado** (`NFeProcEvento`, `tpEvento=110111`). O consumer (sistema do emissor) é quem sabe que a nota foi cancelada e precisa sinalizar ao renderer via flag explícita. Detecção via `cStat=101` cobre cenário marginal (rejeição "NF-e já cancelada" ou XML modificado pelo consumer).

### Requirement: Detect cancelled invoice via `IsCancelled` flag

O `DanfeSharp.Modelo.DanfeViewModel` SHALL expor propriedade pública `IsCancelled` (`bool`, default `false`). Quando setada como `true`, a DANFE renderiza marca d'água "DOCUMENTO CANCELADO" em todas as páginas. Quando `false` (default), nenhuma marca de cancelamento é desenhada.

#### Scenario: Default false — DANFE renderiza sem marca

- **GIVEN** um `DanfeViewModel` recém-criado (sem setar `IsCancelled`)
- **WHEN** a DANFE é gerada
- **THEN** a propriedade `IsCancelled == false` E o PDF gerado não contém marca "DOCUMENTO CANCELADO"

#### Scenario: Consumer seta IsCancelled = true

- **GIVEN** um `DanfeViewModel` carregado de XML de NF-e ativa (`infProt.cStat == 100`)
- **WHEN** o consumer seta `model.IsCancelled = true` antes de gerar a DANFE
- **THEN** o PDF gerado contém marca "DOCUMENTO CANCELADO" centralizada em todas as páginas

### Requirement: Detect cancelled invoice via legacy `cStat=101` fallback

Como mecanismo OR adicional (fallback de compatibilidade), a DANFE SHALL renderizar a marca de cancelamento quando `CodigoStatusReposta == 101`, mesmo se `IsCancelled == false`. Cobre o cenário (atípico mas existente) onde o XML protocolo carregado já reflete o status de cancelamento via `cStat=101`.

#### Scenario: XML com infProt.cStat=101

- **GIVEN** um XML de NF-e cujo `<protNFe><infProt><cStat>101</cStat>` está presente e o `DanfeViewModel.CodigoStatusReposta == 101`
- **WHEN** a DANFE é gerada (sem flag explícita)
- **THEN** o PDF gerado contém marca "DOCUMENTO CANCELADO" em todas as páginas

#### Scenario: Ambos os mecanismos ativados — marca aparece uma única vez

- **GIVEN** `IsCancelled == true` E `CodigoStatusReposta == 101`
- **WHEN** a DANFE é gerada
- **THEN** o PDF contém uma única instância da marca "DOCUMENTO CANCELADO" por página (não duplicada)

### Requirement: Render "DOCUMENTO CANCELADO" watermark on every page

A marca d'água SHALL ser desenhada em **todas** as páginas geradas da DANFE (não só a primeira), garantindo que receptores fiscais identifiquem o status mesmo se receberem apenas folhas individuais de uma DANFE multi-página.

O estilo visual SHALL seguir o padrão do `DesenharAvisoHomologacao` existente: `TextStack` centralizado horizontal e verticalmente no `RetanguloCorpo` (área de produtos da página), fonte `Estilo.CriarFonteRegular(48)`, cor `DeviceRGBColor(0.35, 0.35, 0.35)` cinza médio, `LineHeightScale=0.9F`.

#### Scenario: Single-page DANFE — marca na página única

- **GIVEN** uma DANFE de NF-e cancelada que cabe em 1 página
- **WHEN** a DANFE é gerada
- **THEN** a página única do PDF contém a marca

#### Scenario: Multi-page DANFE — marca em todas as páginas

- **GIVEN** uma DANFE de NF-e cancelada com 20 itens (gera ≥ 2 páginas)
- **WHEN** a DANFE é gerada
- **THEN** cada página do PDF gerado contém a marca "DOCUMENTO CANCELADO" centralizada

#### Scenario: Visual style spec

- **GIVEN** uma DANFE de NF-e cancelada
- **WHEN** o PDF é gerado e a marca é extraída visualmente
- **THEN** o texto "DOCUMENTO CANCELADO" aparece com fonte regular grande (~48pt), cor cinza médio (RGB 0.35,0.35,0.35), centralizado na área principal da página, sem obscurecer dados estruturados (números, descrição de itens) — a cor cinza é leve o suficiente para o conteúdo continuar legível por trás

### Requirement: Co-existence with homologação watermark

Quando uma NF-e está **em homologação E cancelada** simultaneamente (cenário raro, válido em testes), AMBOS os avisos SHALL aparecer na mesma página — "SEM VALOR FISCAL / AMBIENTE DE HOMOLOGAÇÃO" (homologação) **e** "DOCUMENTO CANCELADO" (cancelada). A sobreposição é aceitável porque sinaliza claramente os dois estados, ambos relevantes para o receptor.

#### Scenario: NF-e em homologação cancelada — dois avisos sobrepostos

- **GIVEN** uma NF-e cuja `TipoAmbiente != 1` (homologação) E `IsCancelled == true`
- **WHEN** a DANFE é gerada
- **THEN** o PDF contém AMBOS os avisos: "SEM VALOR FISCAL / AMBIENTE DE HOMOLOGAÇÃO" e "DOCUMENTO CANCELADO", centralizados no mesmo retângulo (visualmente sobrepostos)

### Requirement: Zero regression for non-cancelled DANFE

DANFE de NF-e **não cancelada** (`IsCancelled == false` E `CodigoStatusReposta != 101`) SHALL renderizar **idêntica** ao estado anterior a esta change — sem qualquer marca de cancelamento, sem alteração de layout ou tamanho do PDF resultante além de variação atribuível a metadados (alguns bytes).

#### Scenario: DANFE não-cancelada renderiza sem marca

- **GIVEN** uma NF-e ativa (`IsCancelled == false`, `CodigoStatusReposta == 100`)
- **WHEN** a DANFE é gerada
- **THEN** o PDF não contém texto "DOCUMENTO CANCELADO" em nenhuma página

#### Scenario: Backward compat — código existente não setou IsCancelled

- **GIVEN** código consumidor que não conhece a nova propriedade `IsCancelled` (escrito antes desta change)
- **WHEN** chama `DanfeViewModelCreator.CriarDeArquivoXml(path)` e gera a DANFE
- **THEN** o PDF gerado é idêntico ao gerado antes desta change (zero regressão)
