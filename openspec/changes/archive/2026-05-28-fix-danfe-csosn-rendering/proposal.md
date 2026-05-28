## Why

A DANFE gerada pelo `nfe/DanfeSharp` renderiza a coluna **O/CSOSN** do bloco "DADOS DOS PRODUTOS / SERVIÇOS" de forma incorreta para o cliente Revenda Mais e potencialmente para outros emitentes: (1) origem e código tributário aparecem concatenados sem separador (`120` em vez de `1/20`); (2) o cabeçalho usa sempre `O/CSOSN` mesmo quando o emitente é do Regime Normal (deveria mostrar `O/CST`). O XML está íntegro — somente a renderização visual está errada — mas o problema gerou casos de cancelamento de NF-e por receptores que questionaram a conformidade visual com a convenção universal de mercado (TOTVS, SAP, SmartGo, eMissor, etc.). A coluna `CST` é obrigatória no DANFE conforme [MOC 7.0 — Anexo II §3.1.7](https://www.confaz.fazenda.gov.br/legislacao/arquivo-manuais/moc7-anexo-ii-manual-especificacoes-tecnicas-danfe-codigo-barras.pdf); o formato combinado `O/CST` + separador `/` não é prescrito literalmente pelo MOC, mas é convenção de mercado consolidada. A correção bloqueia retrabalho fiscal sem mudar XML nem outras colunas. Issue: [nfe/DanfeSharp#38](https://github.com/nfe/DanfeSharp/issues/38).

## What Changes

- Renderer da coluna ICMS (origem + CST/CSOSN) na tabela de produtos/serviços da DANFE passa a inserir **separador `/`** entre origem e código tributário (ex.: `1/20`, `0/102`).
- Cabeçalho da coluna passa a ser **dinâmico**: `O/CST` quando o emitente é do Regime Normal (tag `<CST>` presente no grupo `<ICMS*>`) ou `O/CSOSN` quando é do Simples Nacional (tag `<CSOSN>` presente). Quando o XML mistura emitentes ou contém ambos os formatos no mesmo documento, prevalece a tag observada no item.
- Comportamento neutro quando `<orig>` está ausente: célula exibe apenas o código (sem barra solta no início).
- Cobertura de testes unitários cobrindo Regime Normal (CST 00/20/40), Simples Nacional (CSOSN 102/500) e ausência de `<orig>`.
- Nenhuma mudança em XML, geração de NF-e, ou outras colunas da DANFE. Não há alteração de API pública do DanfeSharp (`Danfe.GerarPdf` continua com a mesma assinatura).

## Capabilities

### New Capabilities

- `danfe-icms-column`: regras de renderização da coluna de tributação ICMS no bloco de produtos/serviços da DANFE de NF-e modelo 55 — incluindo formato de célula (origem/código com separador), rótulo dinâmico do cabeçalho conforme regime tributário do emitente, e fallback para XML incompleto.

### Modified Capabilities

_Nenhuma — repo ainda não tem specs OpenSpec (este é o primeiro change após `openspec init`)._

## Impact

- **Código afetado** (a confirmar no design):
  - Renderer da tabela de produtos (provavelmente `DanfeSharp/Blocos/BlocoProdutosServicos.cs` ou equivalente)
  - Modelo de dado de produto/imposto (provavelmente `DanfeSharp/Modelo/ProdutoViewModel.cs` ou semelhante) — para expor origem separada do código
  - Definição do cabeçalho da tabela (label dinâmico)
- **APIs públicas**: nenhuma quebra. A propriedade exposta hoje pode ser mantida por compatibilidade, mas o componente de renderização passa a usar uma representação mais rica (origem + código separados).
- **Dependências externas**: nenhuma nova.
- **Backward compatibility**: NF-e que hoje renderizam com `<orig>` ausente continuam renderizando — apenas sem a barra solta inicial.
- **Cliente impactado positivamente**: Revenda Mais (e qualquer outro emitente cujas DANFEs sejam recebidas por contrapartes fiscalmente atentas).
- **Não impacta**: DANFE de NFCe (modelo 65), DANFE de MDF-e, DANFE de CT-e, geração do XML.
