## 1. Investigação de uso externo (pre-flight)

- [ ] 1.1 Buscar consumidores internos do NFe.io que lêem `ProdutoViewModel.OCst` para confirmar que ninguém depende do formato concatenado antigo (`"120"`). Se algum consumidor faz parsing, listar no PR e propor migração.
- [ ] 1.2 Confirmar com o cliente Revenda Mais (via @Carolina Fagundes) que o separador esperado é `/` e não outro caractere (`-`, espaço, etc.).

## 2. Modelo de dados — `ProdutoViewModel`

- [ ] 2.1 Adicionar propriedade pública `Origem` (`string`, nullable) em `DanfeSharp/Modelo/ProdutoViewModel.cs`, com XML doc `/// <summary>Origem da mercadoria (tag &lt;orig&gt;)</summary>`.
- [ ] 2.2 Adicionar propriedade pública `Cst` (`string`, nullable) com XML doc apontando para tag `<CST>` do grupo `<ICMS*>`.
- [ ] 2.3 Adicionar propriedade pública `Csosn` (`string`, nullable) com XML doc apontando para tag `<CSOSN>` do grupo `<ICMSSN*>`.
- [ ] 2.4 Transformar `OCst` em propriedade calculada (getter-only ou setter privado) que retorna `string.Join("/", new[] { Origem, Cst ?? Csosn }.Where(s => !string.IsNullOrEmpty(s)))`, conforme regra da spec `danfe-icms-column` (cell rendering).

## 3. Producer — `DanfeViewModelCreator`

- [ ] 3.1 Em `DanfeSharp/Modelo/DanfeViewModelCreator.cs:497`, substituir `produto.OCst = icms.orig + icms.CST + icms.CSOSN;` por três atribuições: `produto.Origem = icms.orig; produto.Cst = icms.CST; produto.Csosn = icms.CSOSN;`.
- [ ] 3.2 Verificar se há outras chamadas a `icms.orig`, `icms.CST` ou `icms.CSOSN` em `DanfeViewModelCreator.cs` que precisam ser revisadas (busca local por `icms.orig`, `\.CST\b`, `\.CSOSN\b`).
- [ ] 3.3 Garantir que valores nulos vindos do XML deserializer (`<orig>` ausente etc.) viram `null` (não `""`), para que a regra de composição do `OCst` funcione corretamente. Se o deserializer retorna `""` por padrão, normalizar para `null` na atribuição.

## 4. Renderer — `TabelaProdutosServicos`

- [ ] 4.1 Em `DanfeSharp/Blocos/TabelaProdutosServicos.cs:25`, substituir `String cabecalho4 = ViewModel.Emitente.CRT == "3" ? "O/CST" : "O/CSOSN";` por lógica que inspeciona os itens do `ViewModel.Produtos`:
   ```csharp
   bool hasCst = ViewModel.Produtos.Any(p => !string.IsNullOrEmpty(p.Cst));
   bool hasCsosn = ViewModel.Produtos.Any(p => !string.IsNullOrEmpty(p.Csosn));
   String cabecalho4 = hasCst ? "O/CST" : (hasCsosn ? "O/CSOSN" : "O/CST");
   ```
   Adicionar `using System.Linq;` se ainda não estiver no arquivo.
- [ ] 4.2 Confirmar que a linha 75 (`p.OCst`) continua renderizando a célula corretamente — `OCst` agora é a propriedade calculada que entrega o valor formatado `"1/20"`. Nenhuma mudança necessária neste ponto.
- [ ] 4.3 Verificar se a coluna tem largura suficiente para `1/20` (4 chars) sem truncar — o layout antigo cabia `120` (3 chars). Se necessário, ajustar largura no objeto `Tabela` (provavelmente em `BlocoBase`/Setup).

## 5. Testes unitários

- [ ] 5.1 Em `DanfeSharp.Test/`, adicionar teste cobrindo Regime Normal: XML com `<orig>1</orig><CST>20</CST>` → `ProdutoViewModel.OCst == "1/20"` e cabeçalho da tabela `"O/CST"`.
- [ ] 5.2 Adicionar teste para Simples Nacional: XML com `<orig>0</orig><CSOSN>102</CSOSN>` → `OCst == "0/102"` e cabeçalho `"O/CSOSN"`.
- [ ] 5.3 Adicionar teste para ausência de `<orig>`: XML com apenas `<CST>40</CST>` → `OCst == "40"` (sem barra solta).
- [ ] 5.4 Adicionar teste para `<emit><CRT>` ausente: XML sem CRT no emitente mas com `<CST>` nos itens → cabeçalho derivado dos itens é `"O/CST"`.
- [ ] 5.5 Adicionar teste de regressão garantindo que XML sem `<orig>` `<CST>` `<CSOSN>` não quebra (fallback determinístico de cabeçalho `"O/CST"`, célula vazia).
- [ ] 5.6 Rodar `dotnet test` na solution e confirmar 0 falhas.

## 6. Validação visual (manual)

- [ ] 6.1 Gerar PDF da DANFE para a invoice de referência `86fed76fd38f42e1833375e33a94567c` (company `c7c78c0f701964d36adcb0d2263e3d1b4` do cliente Revenda Mais). Confirmar visualmente que a célula da coluna ICMS exibe `1/20` e o cabeçalho exibe `O/CST` (ou `O/CSOSN` conforme regime real).
- [ ] 6.2 Gerar DANFE de um XML genérico do diretório `DanfeSharp.Test/Xml/NFe/` em regime normal e em Simples para confirmar visualmente que ambos os casos renderizam corretamente, com zero regressão estética nas demais colunas.
- [ ] 6.3 Tirar prints antes/depois e anexar no PR como evidência.

## 7. Documentação e changelog

- [ ] 7.1 Adicionar entrada no README/CHANGELOG (se o repo mantém um) explicando o bug fix, o impacto em `OCst`, e a nova lógica do cabeçalho.
- [ ] 7.2 Documentar publicamente em comentário XML doc do `OCst` que o formato é agora `"<origem>/<código>"`, separador `/`.

## 8. PR e revisão

- [ ] 8.1 Garantir que o body do PR linka a issue `Fixes #38` para fechamento automático no merge.
- [ ] 8.2 Solicitar review do @joaokita (co-assignee automático no repo) e de algum reviewer fiscal-aware do time NFe.io.
- [ ] 8.3 Após approval, retirar o draft e mergear (squash) em `main`.

## 9. Arquivamento OpenSpec

- [ ] 9.1 Após merge, rodar `openspec validate` para confirmar consistência.
- [ ] 9.2 Rodar `openspec archive fix-danfe-csosn-rendering` para mover a change para `openspec/changes/archive/` e atualizar `openspec/specs/danfe-icms-column/spec.md` com os requirements ADDED desta change.
