## Context

DanfeSharp é um renderer de DANFE em C# (.NET) que carrega o XML da NF-e via `XmlSerializer` (`DanfeSharp.Esquemas.ProcNFe`) e o traduz para um modelo de apresentação (`DanfeSharp.Modelo.DanfeViewModel`) consumido por blocos de desenho que usam o PDF builder embutido. A coluna "O/CSOSN" do bloco `TabelaProdutosServicos` mostra **origem da mercadoria + código de tributação ICMS** (CST para Regime Normal, CSOSN para Simples Nacional).

**Estado atual (bugs)**

1. **`DanfeSharp/Modelo/DanfeViewModelCreator.cs:497`** — a montagem do valor da coluna usa concatenação de strings sem separador:
   ```csharp
   produto.OCst = icms.orig + icms.CST + icms.CSOSN;
   ```
   Como CST e CSOSN são mutuamente exclusivos (apenas um é preenchido por item), o resultado para um produto com `<orig>1</orig><CST>20</CST>` vira `"120"` em vez de `"1/20"`.

2. **`DanfeSharp/Blocos/TabelaProdutosServicos.cs:25`** — o cabeçalho é decidido apenas pelo CRT do emitente:
   ```csharp
   String cabecalho4 = ViewModel.Emitente.CRT == "3" ? "O/CST" : "O/CSOSN";
   ```
   Se o XML do emitente não traz `<CRT>` (campo opcional em emissores antigos ou XMLs mal-formados), `Emitente.CRT` fica `null`, a comparação falha, e o cabeçalho cai em `"O/CSOSN"` mesmo quando os produtos do XML têm `<CST>` (Regime Normal). É o que provavelmente ocorreu nas DANFEs do cliente Revenda Mais.

**Stakeholders**

- Cliente **Revenda Mais** (relator, via @Carolina Fagundes em Microsoft Teams 2026-05-27)
- Consumidores externos do `nfe/DanfeSharp` (Library NuGet/fork interno usado em outros produtos NFe.io)
- Receptores das DANFEs (varejistas, contadores) que conferem código tributário visualmente

**Constraints**

- Manter compatibilidade de API pública (`Danfe.GerarPdf` e variantes); nada de breaking change para consumidores
- Manter cobertura visual idêntica para DANFEs hoje renderizadas corretamente (regressão zero)
- Mudanças confinadas ao renderer/viewmodel — XML schema e parser não mudam
- Sem novas dependências; código C# .NET conforme padrão do projeto

## Goals / Non-Goals

**Goals:**
- Coluna mostra origem e código tributário separados por `/` (ex.: `1/20`, `0/102`)
- Cabeçalho mostra `O/CST` quando os itens do XML usam `<CST>` (Regime Normal) e `O/CSOSN` quando usam `<CSOSN>` (Simples Nacional)
- Robustez contra `Emitente.CRT` ausente/inconsistente — derivar do conteúdo real dos itens é mais confiável que confiar só no campo do emitente
- Quando `<orig>` está ausente: célula mostra só o código, sem barra solta no início
- Backward compatibility do `ProdutoViewModel.OCst` (consumidores externos podem ler essa propriedade)

**Non-Goals:**
- Não tratar marca de cancelada (cabe em #40 / PR #8)
- Não tratar bloco "FORMA DE PAGAMENTO" (cabe em #39)
- Não mudar o XML schema ou parser do `ProcNFe.cs`
- Não criar UI para configurar o separador (`/` é fixo, padrão MOC NF-e)
- Não cobrir DANFE NFCe (modelo 65) — só modelo 55

## Decisions

### Decision 1: Derivar o cabeçalho do conteúdo dos itens, não do `Emitente.CRT`

**Escolha:** o cabeçalho `O/CST` ou `O/CSOSN` é decidido inspecionando os itens da nota. Se qualquer item tem `Cst` não-vazio → `"O/CST"`; senão se qualquer item tem `Csosn` não-vazio → `"O/CSOSN"`; senão (XML sem ICMS estruturado) → `"O/CST"` como fallback determinístico (regime normal é a maioria).

**Alternativa rejeitada:** continuar usando `Emitente.CRT == "3"`. Rejeitada porque `CRT` é vulnerável a:
- XMLs antigos sem o campo
- Erros de cadastro do emitente
- Casos limítrofes (excesso de sublimite, CRT=2)

A presença concreta de `<CST>` vs `<CSOSN>` nos itens **é o ground-truth** do regime tributário em vigor — o que o receptor da DANFE consegue conferir contra o XML.

**Alternativa rejeitada:** usar ambos os campos `Cst` e `Csosn` numa lógica composta com `Emitente.CRT`. Rejeitada por complexidade desproporcional ao ganho.

### Decision 2: Modelar `Origem`, `Cst` e `Csosn` como propriedades separadas em `ProdutoViewModel`, mantendo `OCst` por compat

**Escolha:** adicionar três propriedades novas:
```csharp
public string Origem { get; set; }   // "0", "1", ..., "8" — pode ser null/empty
public string Cst { get; set; }      // ex. "00", "10", "20", "40" — null se item é Simples
public string Csosn { get; set; }    // ex. "102", "500" — null se item é Regime Normal
```

A propriedade `OCst` continua existindo (backward compat) mas passa a ser **calculada** com a regra: `Origem/Cst` ou `Origem/Csosn` separados por `/`, omitindo elementos vazios. Setter privado/interno; o producer (DanfeViewModelCreator) preenche as três propriedades brutas.

**Alternativa rejeitada:** sobrescrever a fórmula de `OCst` direto sem expor `Origem`/`Cst`/`Csosn` separados. Rejeitada porque o renderer de cabeçalho (Decision 1) precisa olhar item-a-item para `Cst` vs `Csosn` — fica feio extrair isso de uma string concatenada.

**Alternativa rejeitada:** deprecar `OCst` e usar só os três campos brutos. Rejeitada porque rompe compat de consumidores externos que talvez já leiam `OCst`.

### Decision 3: Formato exato da célula e do cabeçalho

**Base normativa**

A coluna `CST` é **obrigatória** no quadro "Dados dos Produtos/Serviços" do DANFE, conforme [MOC 7.0 — Anexo II, §3.1.7](https://www.confaz.fazenda.gov.br/legislacao/arquivo-manuais/moc7-anexo-ii-manual-especificacoes-tecnicas-danfe-codigo-barras.pdf) (página 11). O domínio do campo `<orig>` (origem da mercadoria: 0–8) está definido no schema do XML, em [MOC 7.0 — Anexo I](https://www.confaz.fazenda.gov.br/legislacao/arquivo-manuais/moc7-anexo-i-leiaute-e-rv.pdf), grupo `<ICMS>`. **O MOC não manda literalmente o formato combinado `O/CST` com separador `/`** — esse é convenção de mercado universalmente adotada (TOTVS, SAP, SmartGo, eMissor, etc.) por compactação visual e familiaridade dos receptores fiscais. A correção deste change alinha o `nfe/DanfeSharp` com essa convenção, restaurando o comportamento que receptores fiscais esperam ver e mantendo origem + CST/CSOSN visualmente distinguíveis.

**Célula** (na linha de cada produto):

| `Origem` | `Cst` | `Csosn` | Resultado |
|---|---|---|---|
| `"1"` | `"20"` | `null` | `"1/20"` |
| `"0"` | `null` | `"102"` | `"0/102"` |
| `null` | `"20"` | `null` | `"20"` (sem barra solta) |
| `null` | `null` | `"500"` | `"500"` |
| `"1"` | `null` | `null` | `"1"` (raro, mas possível em XML inconsistente) |
| `null` | `null` | `null` | `""` (vazio) |

Regra: `string.Join("/", [Origem, Cst ?? Csosn].Where(s => !string.IsNullOrEmpty(s)))`.

**Cabeçalho** (uma vez por tabela):
- `"O/CST"` se qualquer item tem `Cst` não-vazio
- senão `"O/CSOSN"` se qualquer item tem `Csosn` não-vazio
- senão `"O/CST"` (fallback determinístico)

A coluna mista (alguns itens com CST e outros com CSOSN) é considerada inviável pelo CRT do emitente (regime tributário é por emitente, não por item) — Regime Normal usa apenas CST, Simples Nacional usa apenas CSOSN. Se aparecer no XML por inconsistência, prevalece a primeira regra (existe pelo menos um CST → `"O/CST"`).

### Decision 4: Não introduzir feature flag ou config

A correção é pura — restitui comportamento conforme MOC NF-e. Não há cenário onde o consumidor queira manter o bug. Logo, sem flag.

## Risks / Trade-offs

- **[Risk]** Consumidores externos do `DanfeSharp` que dependem do formato exato concatenado `"120"` de `ProdutoViewModel.OCst` quebrariam. → **Mitigation:** documentar a mudança no CHANGELOG/release notes (semver minor, pois é breaking de output mas a propriedade continua existindo). Avaliar se algum consumidor interno do NFe.io lê `OCst` parseando — buscar referências antes de mergear (grep em outros repos).
- **[Risk]** Algum XML de teste do `DanfeSharp.Test` ter snapshot do PDF gerado com `"120"` no lugar errado. → **Mitigation:** rodar suite e atualizar snapshots conscientemente; documentar diff visual no PR.
- **[Risk]** Decisão 1 (header derivado dos itens) muda comportamento em casos de NF-e onde `Emitente.CRT="3"` mas itens têm `<CSOSN>` por algum motivo (raro/malformado). → **Mitigation:** o ground-truth visual continua o do item; renderizar `"O/CSOSN"` nesse caso é mais coerente com o conteúdo das células do que mentir e mostrar `"O/CST"`.
- **[Trade-off]** Adicionar 3 propriedades novas em `ProdutoViewModel` aumenta a superfície da classe. → Aceito porque a alternativa (raciocinar via string parsing) é pior arquitetura.

## Migration Plan

1. Adicionar propriedades `Origem`, `Cst`, `Csosn` ao `ProdutoViewModel` (sem remover `OCst`).
2. Atualizar `DanfeViewModelCreator.cs:497` para popular as três propriedades; eliminar a concatenação direta de `OCst`.
3. Transformar `OCst` em propriedade calculada a partir das três novas (com regra do separador).
4. Atualizar `TabelaProdutosServicos.cs:25` (header) para inspecionar os itens em vez de checar `Emitente.CRT`.
5. Adicionar testes unitários e atualizar snapshots existentes.
6. Validar visualmente com a invoice de referência `86fed76fd38f42e1833375e33a94567c` (cliente Revenda Mais) — comparar PDF antes/depois.
7. PR em draft → review → merge para `main` → release minor do pacote (se aplicável).

**Rollback:** se houver regressão pós-merge, reverter o PR — não há migração de dados nem state persistido.

## Open Questions

- Algum consumidor interno do NFe.io (outros repos) lê `ProdutoViewModel.OCst` esperando o formato concatenado `"120"`? — verificar antes do merge via grep cross-repo (`OCst` em outros consumidores que linkam o DanfeSharp).
- ~~O Manual de Orientação ao Contribuinte (MOC) NF-e especifica o caractere separador?~~ **Resolvido (2026-05-28):** o MOC 7.0 Anexo II §3.1.7 não cita literalmente o separador `/` nem o cabeçalho combinado `O/CST` — manda apenas que a coluna `CST` seja preenchida. O formato combinado é convenção de mercado (TOTVS, SAP, SmartGo, eMissor). Adotamos `/` por ser o universalmente esperado pelos receptores; se cliente Revenda Mais apontar outro padrão, reabrir.
- A library tem outros pontos onde origem/CST/CSOSN são compostos como string? Buscar `icms.orig` e similares para garantir cobertura.
