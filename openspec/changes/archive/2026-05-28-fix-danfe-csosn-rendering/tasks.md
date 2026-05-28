## 1. Investigação de uso externo (pre-flight)

- [ ] 1.1 Buscar consumidores internos do NFe.io que lêem `ProdutoViewModel.OCst` para confirmar que ninguém depende do formato concatenado antigo (`"120"`). Se algum consumidor faz parsing, listar no PR e propor migração. _(Pendente — requer acesso a outros repos do NFe.io fora deste working tree.)_
- [ ] 1.2 Confirmar com o cliente Revenda Mais (via @Carolina Fagundes) que o separador esperado é `/` e não outro caractere (`-`, espaço, etc.). _(Pendente — comunicação externa; default `/` aplicado conforme convenção universal de mercado. Não é mandato literal do MOC 7.0 Anexo II §3.1.7, que prescreve a coluna `CST` mas não o formato combinado — confirmado por leitura literal do PDF oficial em 2026-05-28.)_

## 2. Modelo de dados — `ProdutoViewModel`

- [x] 2.1 Adicionar propriedade pública `Origem` (`string`, nullable) em `DanfeSharp/Modelo/ProdutoViewModel.cs`, com XML doc `/// <summary>Origem da mercadoria (tag &lt;orig&gt;)</summary>`.
- [x] 2.2 Adicionar propriedade pública `Cst` (`string`, nullable) com XML doc apontando para tag `<CST>` do grupo `<ICMS*>`.
- [x] 2.3 Adicionar propriedade pública `Csosn` (`string`, nullable) com XML doc apontando para tag `<CSOSN>` do grupo `<ICMSSN*>`.
- [x] 2.4 Transformar `OCst` em propriedade calculada (getter-only) que retorna a composição `Origem/codigo` conforme regra da spec `danfe-icms-column` (cell rendering). Implementado em `ProdutoViewModel.cs` com tratamento explícito de origem-only, código-only, e ambos vazios.

## 3. Producer — `DanfeViewModelCreator`

- [x] 3.1 Em `DanfeSharp/Modelo/DanfeViewModelCreator.cs:497`, substituída `produto.OCst = icms.orig + icms.CST + icms.CSOSN;` por três atribuições explícitas a `Origem`/`Cst`/`Csosn`.
- [x] 3.2 Verificadas outras chamadas a `icms.orig`/`icms.CST`/`icms.CSOSN` em todo o repo — só este ponto em `DanfeViewModelCreator.cs` lia esses três campos para produto. Confirmado via `grep`.
- [x] 3.3 Normalização aplicada: `String.IsNullOrEmpty(icms.orig) ? null : icms.orig` (idem CST e CSOSN). Cobre o caso do `XmlSerializer` retornar `""` para tags ausentes; o `OCst` computado também trata `""` como ausente por defesa em profundidade (test `OCst_OrigemEmString_TrataComoAusente`).

## 4. Renderer — `TabelaProdutosServicos`

- [x] 4.1 Em `DanfeSharp/Blocos/TabelaProdutosServicos.cs:25`, substituir a expressão `ViewModel.Emitente.CRT == "3" ? "O/CST" : "O/CSOSN"` por chamada ao helper estático `ProdutoViewModel.CalcularCabecalhoColunaIcms(ViewModel.Produtos)` — lógica extraída para método puro testável.
- [x] 4.2 Verificar que a linha 75 (`p.OCst`) continua renderizando a célula corretamente — `OCst` agora é a propriedade calculada que entrega o valor formatado `"1/20"`. Confirmado: nenhuma mudança necessária neste ponto.
- [ ] 4.3 Verificar visualmente se a coluna tem largura suficiente para `1/20` (4 chars) sem truncar — o layout antigo cabia `120` (3 chars). _(Validação manual pendente: §6 cobre.)_

## 5. Testes unitários

- [x] 5.1 Regime Normal: `Origem="1"`, `Cst="20"` → `OCst == "1/20"` e cabeçalho `"O/CST"`. Cobertura em `ProdutoIcmsColumnTests.OCst_RegimeNormal_OrigemComCst_RetornaOrigemBarraCst` + `Cabecalho_TodosItensComCst_RetornaOCST`.
- [x] 5.2 Simples Nacional: `Origem="0"`, `Csosn="102"` → `OCst == "0/102"` e cabeçalho `"O/CSOSN"`. Cobertura em `OCst_SimplesNacional_OrigemComCsosn_RetornaOrigemBarraCsosn` + `Cabecalho_TodosItensComCsosn_RetornaOCSOSN`.
- [x] 5.3 Ausência de origem: apenas `Cst="40"` → `OCst == "40"` (sem barra solta). Cobertura em `OCst_SemOrigem_ApenasCst_RetornaCodigoSemBarra` + `OCst_SemOrigem_ApenasCsosn_RetornaCodigoSemBarra`.
- [x] 5.4 Cenário do bug reportado (CRT ausente mas itens com CST): cabeçalho derivado dos itens é `"O/CST"`. Cobertura em `Cabecalho_RegressaoBug38_RevendaMaisComCstESemEmitenteCRT`.
- [x] 5.5 Fallback total (nada estruturado no XML): célula vazia, cabeçalho `"O/CST"`. Cobertura em `OCst_TodosNull_RetornaStringVazia` + `Cabecalho_NenhumItemComCodigo_RetornaOCSTFallback` + `Cabecalho_ProdutosNull_RetornaOCSTFallback` + `Cabecalho_ListaVazia_RetornaOCSTFallback`.
- [x] 5.6 `dotnet vstest DanfeSharp.Test.dll --TestCaseFilter:"FullyQualifiedName~ProdutoIcmsColumnTests"` → **15/15 aprovados, 0 falhas, 297 ms**. As 21 falhas restantes na suite completa (LogoTests, demais DanfeTest) são pré-existentes em `DanfeViewModel.TextoReservadoFisco():355` lançando `NotImplementedException` — fora do escopo de #38.

## 6. Validação visual (manual)

- [ ] 6.1 Gerar PDF da DANFE para a invoice de referência `86fed76fd38f42e1833375e33a94567c` (company `c7c78c0f701964d36adcb0d2263e3d1b4` do cliente Revenda Mais). Confirmar visualmente que a célula da coluna ICMS exibe `1/20` e o cabeçalho exibe `O/CST` (ou `O/CSOSN` conforme regime real).
- [ ] 6.2 Gerar DANFE de um XML genérico do diretório `DanfeSharp.Test/Xml/NFe/` em regime normal e em Simples para confirmar visualmente que ambos os casos renderizam corretamente, com zero regressão estética nas demais colunas.
- [ ] 6.3 Tirar prints antes/depois e anexar no PR como evidência.

## 7. Documentação e changelog

- [x] 7.1 Repo não mantém CHANGELOG.md — alterações são documentadas em commit message + body da PR. Esta task fica satisfeita pelo body de PR #41.
- [x] 7.2 XML doc do `OCst` atualizado em `ProdutoViewModel.cs` explicando o formato `"<origem>/<código>"` e a regra de omissão de componentes vazios.

## 8. PR e revisão

- [ ] 8.1 Garantir que o body do PR linka a issue `Fixes #38` para fechamento automático no merge.
- [ ] 8.2 Solicitar review do @joaokita (co-assignee automático no repo) e de algum reviewer fiscal-aware do time NFe.io.
- [ ] 8.3 Após approval, retirar o draft e mergear (squash) em `main`.

## 9. Arquivamento OpenSpec

- [ ] 9.1 Após merge, rodar `openspec validate` para confirmar consistência.
- [ ] 9.2 Rodar `openspec archive fix-danfe-csosn-rendering` para mover a change para `openspec/changes/archive/` e atualizar `openspec/specs/danfe-icms-column/spec.md` com os requirements ADDED desta change.
