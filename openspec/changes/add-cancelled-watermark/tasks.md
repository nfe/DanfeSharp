## 1. ViewModel — propriedade IsCancelled

- [ ] 1.1 Adicionar `public bool IsCancelled { get; set; }` ao `DanfeSharp/Modelo/DanfeViewModel.cs`, com XML doc explicando: "Sinaliza que a NF-e foi cancelada via evento (tpEvento=110111). Quando true, renderiza marca d'água 'DOCUMENTO CANCELADO'. Como o cancelamento em NF-e modelo 55 acontece via evento separado, o consumer deve setar essa flag com base no estado do banco de dados — não vem do XML da NF-e."

## 2. Renderer — método DesenharAvisoCancelamento

- [ ] 2.1 Adicionar método `public void DesenharAvisoCancelamento()` em `DanfeSharp/DanfePagina.cs`, espelhando o padrão de `DesenharAvisoHomologacao` (linha ~97). Texto fixo `"DOCUMENTO CANCELADO"`, fonte `Danfe.EstiloPadrao.CriarFonteRegular(48)`, cor `DeviceRGBColor(0.35, 0.35, 0.35)`, centralizado no `RetanguloCorpo`. Copiado/derivado da PR #8 do @mateuszanini com créditos no commit message.

## 3. Trigger — Danfe.CriarPagina

- [ ] 3.1 Em `DanfeSharp/Danfe.cs`, dentro do método `CriarPagina`, após a chamada existente `if (ViewModel.TipoAmbiente != 1) p.DesenharAvisoHomologacao();`, adicionar `if (ViewModel.IsCancelled || ViewModel.CodigoStatusReposta == 101) p.DesenharAvisoCancelamento();`. Ordem importa: homologação primeiro (se aplicável), cancelamento depois (sobreposição centralizada quando ambos true).

## 4. Testes unitários

- [ ] 4.1 Criar `DanfeSharp.Test/DanfeCanceladaTests.cs` com testes:
  - (a) `DanfeViewModel_DefaultIsCancelledFalse` — recém-criado, default false.
  - (b) `Render_IsCancelledTrue_GeraMarca` — carrega fixture XML, seta IsCancelled=true, renderiza, extrai texto via pdftotext, confirma "DOCUMENTO CANCELADO" presente.
  - (c) `Render_cStat101_GeraMarca` — fixture com cStat=101 no XML, renderiza sem flag, confirma marca presente.
  - (d) `Render_AmbosTrigger_GeraUmaMarcaPorPagina` — IsCancelled=true E cStat=101, confirma marca aparece uma vez por página (não duplicada).
  - (e) `Render_NaoCancelada_SemMarca` — XML default + IsCancelled=false, confirma PDF não contém "DOCUMENTO CANCELADO".
  - (f) `Render_MultiPagina_MarcaEmTodasPaginas` — usa fixture v4_DanfeCompleto (20 itens, multi-página), seta IsCancelled=true, conta ocorrências de "DOCUMENTO CANCELADO" no texto extraído (deve ser >= número de páginas).
- [ ] 4.2 Atualizar `DanfeSharp.Test/DanfeSharp.Test.csproj` com `<Compile Include="DanfeCanceladaTests.cs" />`.

## 5. Fixture demo

- [ ] 5.1 Não criar XML novo — o estado de cancelamento vem via flag, não via XML. Reaproveitar `v4_DanfeIntermediario.xml` como base. Test method `v4_DanfeIntermediario_Cancelada()` carrega, seta `IsCancelled=true`, gera PDF `v4_DanfeIntermediario_Cancelada.pdf` em `bin/Debug/Output/DeXml/`.
- [ ] 5.2 Test method `v4_DanfeCompleto_Cancelada()` análogo, para visualizar marca em multi-página (20 itens).
- [ ] 5.3 Test method `v4_DanfeMinimo_Cancelada()` análogo, para visualizar marca em DANFE simples.

## 6. Build + validação visual

- [ ] 6.1 `dotnet build` → 0 erros.
- [ ] 6.2 `dotnet vstest --TestCaseFilter:"FullyQualifiedName~DanfeCancelada"` → todos verdes.
- [ ] 6.3 `dotnet vstest --TestCaseFilter:"FullyQualifiedName~DanfeXmlTests"` → todos os anteriores continuam verdes (zero regressão).
- [ ] 6.4 Gerar 3 PDFs demo (Mínimo + Intermediário + Completo, todos cancelados) e copiar para `../danfe-payment-validation/exemplos-cancelada/`.

## 7. Documentação no PR

- [ ] 7.1 Body do PR cita PR #8 do @mateuszanini como referência prévia. Reconhece a contribuição dele (visual correto, gatilho equivocado).
- [ ] 7.2 Body documenta a decisão de detecção primária via flag (vs cStat=101 only) e o motivo (cancelamento via evento na realidade do NF-e modelo 55).
- [ ] 7.3 Body inclui exemplos visuais (antes/depois) ou link para os PDFs em `../danfe-payment-validation/exemplos-cancelada/`.

## 8. PR + review

- [ ] 8.1 Garantir link `Fixes #40` no body para fechamento automático no merge.
- [ ] 8.2 Solicitar review do @joaokita (co-assignee automático) + alguém fiscal-aware.
- [ ] 8.3 Após approval, retirar do draft e mergear.

## 9. Comunicação pós-merge

- [ ] 9.1 Alinhar com time da API NFe.io: setar `model.IsCancelled = true` quando consultarem NF-e cancelada no banco. _(Manual, externo.)_
- [ ] 9.2 Comentar em PR #8 do @mateuszanini referenciando esta change como continuação/finalização do trabalho dele.

## 10. Arquivamento OpenSpec

- [ ] 10.1 Após merge, `openspec validate add-cancelled-watermark` para confirmar consistência.
- [ ] 10.2 `openspec archive add-cancelled-watermark -y` para mover a change e promover specs para `openspec/specs/danfe-cancelled-watermark/spec.md`. Preencher o `Purpose` com a base normativa (MOC §3.10.1 autoriza, NT do evento de cancelamento, convenção de mercado).
