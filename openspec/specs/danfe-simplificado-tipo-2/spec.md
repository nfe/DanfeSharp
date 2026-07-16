# danfe-simplificado-tipo-2 Specification

## Purpose

Define como o `nfe/DanfeSharp` renderiza o **DANFE Simplificado Tipo 2** — documento auxiliar em formato cupom para NF-e **modelo 55** emitida com `tpImp=6`, voltado ao varejo (operações que hoje seriam NFC-e). Renderização via classe dedicada `DanfeSimplificadoTipo2` (mesmo padrão de `DanfeNFC`: página estreita de altura dinâmica, blocos empilhados), consumindo o `DanfeViewModel` compartilhado.

**Base normativa:** NT 2026.003 v1.00 (especificações técnicas do DANFE Simplificado Tipo 2), em atendimento à NT 2026.002 v1.00 e aos Ajustes SINIEF 32/2025 e 13/2026. Produção: 03/08/2026. A NT define **9 divisões** com conteúdo mínimo, papel de largura ≥ 56 mm, impressora comum e fidelidade ao XML (proibido imprimir dado que não conste no XML, exceto protocolo, cMsg/xMsg).

## Requirements

### Requirement: Divisão I — Cabeçalho

O cupom SHALL iniciar com a identificação do emitente (razão social, CNPJ, endereço) e o título destacado **"DANFE Simplificado Tipo 2"**. Logotipo é opcional.

#### Scenario: Cabeçalho com emitente e título

- **GIVEN** um `DanfeViewModel` com emitente preenchido
- **WHEN** o PDF é gerado
- **THEN** o texto extraído contém a razão social, o CNPJ formatado e "DANFE Simplificado Tipo 2"

### Requirement: Divisão II — Produtos e Divisão III — Totais

A tabela de produtos SHALL exibir código, descrição, quantidade, unidade, valor unitário e valor total por item. Os totais SHALL exibir quantidade total de itens, valor dos produtos, desconto/acréscimos, valor total, as formas de pagamento com valores, a linha "VALOR PAGO" e a linha "TROCO" — o troco é **sempre impresso**, ainda que 0,00.

#### Scenario: Troco sempre presente

- **GIVEN** um `DanfeViewModel` sem `Pagamento[].Troco` informado
- **WHEN** o PDF é gerado
- **THEN** o texto extraído contém "TROCO"

### Requirement: Divisão III-A — IBS/CBS/IS

Quando o ViewModel tiver `TributosIbsCbs` preenchido, o cupom SHALL discriminar CBS, IBS (com parcelas UF/Município quando presentes) e IS (quando houver). Quando `TributosIbsCbs == null`, a divisão SHALL ser **omitida** (fidelidade ao XML).

#### Scenario: Com grupo IBS/CBS

- **GIVEN** `TributosIbsCbs` com valores de IBS e CBS
- **WHEN** o PDF é gerado
- **THEN** o texto contém os rótulos de IBS e CBS com os valores

#### Scenario: Sem grupo IBS/CBS

- **GIVEN** `TributosIbsCbs == null`
- **WHEN** o PDF é gerado
- **THEN** o texto NÃO contém rótulos de IBS/CBS

### Requirement: Divisão IV — Consulta via chave de acesso

O cupom SHALL exibir a URL de consulta da SEFAZ e a chave de acesso de 44 dígitos formatada em **11 blocos de 4 dígitos** separados por espaço.

#### Scenario: Chave em 11 blocos

- **GIVEN** uma chave de acesso de 44 dígitos
- **WHEN** o PDF é gerado
- **THEN** o texto contém a chave com espaços a cada 4 dígitos

### Requirement: Divisão V — QR Code

Quando `DanfeViewModel.QrCode` estiver preenchido, o cupom SHALL renderizar o QR Code com no mínimo 25×25 mm e quiet zone de 3 mm. Quando ausente, a divisão SHALL ser omitida sem erro (a geração do conteúdo do QR v3 para mod. 55 é responsabilidade do consumer).

#### Scenario: QR presente

- **GIVEN** `QrCode` preenchido
- **WHEN** o PDF é gerado
- **THEN** a imagem do QR é renderizada e o rótulo de consulta via QR Code aparece

#### Scenario: QR ausente

- **GIVEN** `QrCode` nulo ou vazio
- **WHEN** o PDF é gerado
- **THEN** a geração conclui sem exceção e sem o rótulo de QR Code

### Requirement: Divisão VI — Consumidor

Quando o destinatário estiver identificado, o cupom SHALL exibir "CONSUMIDOR" com CNPJ/CPF, nome e endereço (quando presentes no XML). Quando não identificado, SHALL exibir "CONSUMIDOR NÃO IDENTIFICADO". A obrigatoriedade de identificação em operação não presencial é validada na emissão (domínio), não no renderer.

#### Scenario: Consumidor identificado

- **GIVEN** destinatário com CPF e nome
- **WHEN** o PDF é gerado
- **THEN** o texto contém "CONSUMIDOR" e o CPF

### Requirement: Divisão VII — Identificação da NF-e

O cupom SHALL exibir número, série, data/hora de emissão em **horário local** (não UTC — conversão é responsabilidade do consumer ao popular `DataHoraEmissao`), o rótulo da via e o protocolo de autorização com data/hora.

#### Scenario: Identificação completa

- **GIVEN** NF-e autorizada com protocolo
- **WHEN** o PDF é gerado
- **THEN** o texto contém número, série, "PROTOCOLO DE AUTORIZAÇÃO" e o protocolo

### Requirement: Divisão VIII — Mensagem fiscal, contingência e homologação

Em ambiente de homologação (`TipoAmbiente == 2`), o cupom SHALL exibir "EMITIDA EM AMBIENTE DE HOMOLOGAÇÃO - SEM VALOR FISCAL". Em contingência pendente de autorização (`TipoEmissao != Normal` e sem protocolo), o cupom SHALL exibir "EMITIDA EM CONTINGÊNCIA" / "Pendente de autorização" (caixa alta, centralizado, 2 linhas) em **dois locais** — abaixo do cabeçalho e abaixo da identificação — e SHALL gerar **segunda via** com rótulo "Via do Estabelecimento". `infAdFisco` SHALL ser impresso quando presente.

#### Scenario: Contingência pendente

- **GIVEN** `TipoEmissao = ContingenciaFSDA` e `ProtocoloAutorizacao` vazio
- **WHEN** o PDF é gerado
- **THEN** "EMITIDA EM CONTINGÊNCIA" aparece 4 vezes no total (2 locais × 2 vias) e a segunda página contém "Via do Estabelecimento"

#### Scenario: Emissão normal autorizada

- **GIVEN** `TipoEmissao = Normal` com protocolo
- **WHEN** o PDF é gerado
- **THEN** o PDF tem 1 página e não contém "EMITIDA EM CONTINGÊNCIA"

### Requirement: Divisão IX — Mensagem do contribuinte

O cupom SHALL imprimir `infCpl` quando presente e a demonstração dos tributos aproximados (Lei 12.741/2012) quando `CalculoImposto.ValorAproximadoTributos > 0`.

#### Scenario: Tributos aproximados

- **GIVEN** `ValorAproximadoTributos > 0`
- **WHEN** o PDF é gerado
- **THEN** o texto contém a referência à Lei 12.741

### Requirement: Zero regression nos leiautes existentes

A introdução do `DanfeSimplificadoTipo2` SHALL NOT alterar a renderização de `Danfe` (A4) nem de `DanfeNFC` (NFC-e). Os blocos novos vivem em `Blocos/SimplificadoTipo2/` e não modificam os blocos NFC existentes (exceção: reuso direto sem alteração de `BlocoInformacaoFiscal`).

#### Scenario: Suíte existente permanece verde

- **GIVEN** a suíte de testes existente do DanfeSharp
- **WHEN** os testes rodam após a change
- **THEN** todos os testes pré-existentes continuam passando
