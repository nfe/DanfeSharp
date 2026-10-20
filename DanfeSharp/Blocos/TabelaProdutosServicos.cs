using System;
using System.Collections.Generic;
using System.Drawing;
using DanfeSharp.Graphics;
using DanfeSharp.Modelo;

namespace DanfeSharp.Blocos
{
    internal class TabelaProdutosServicos : ElementoBase
    {
        public CabecalhoBloco CabecalhoBloco { get; private set; }
        public Tabela Tabela { get; private set; }
        public DanfeViewModel ViewModel { get; private set; }

        public TabelaProdutosServicos(DanfeViewModel viewModel, Estilo estilo) : base(estilo)
        {
            ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            CabecalhoBloco = new CabecalhoBloco(estilo, "DADOS DOS PRODUTOS / SERVIÇOS");

            var ad = AlinhamentoHorizontal.Direita;
            var ac = AlinhamentoHorizontal.Centro;
            var ae = AlinhamentoHorizontal.Esquerda;

            Tabela = new Tabela(Estilo);
            String cabecalho4 = ProdutoViewModel.CalcularCabecalhoColunaIcms(ViewModel.Produtos);

            if (ViewModel.ExibirReformaTributaria)
            {
                AdicionarColunasReformaTributaria(cabecalho4, ac, ad, ae);
                return;
            }

            if (ViewModel.IsRetrato)
            { 
                Tabela
                .ComColuna(8.5f, ac, "CÓDIGO", "PRODUTO")
                .ComColuna(0, ae, "DESCRIÇÃO DO PRODUTO / SERVIÇO")
                .ComColuna(5F, ac, "NCM/SH")
                .ComColuna(3.9F, ac, cabecalho4)
                .ComColuna(3.5F, ac, "CFOP")
                .ComColuna(3.25F, ac, "UN")
                .ComColuna(4F, ad, "QUANT.")
                .ComColuna(6F, ad, "VALOR", "UNIT.")
                .ComColuna(6F, ad, "VALOR", "TOTAL")
                .ComColuna(6F, ad, "B CÁLC", "ICMS")
                .ComColuna(6F, ad, "B CÁLC", "ICMS ST")
                .ComColuna(5, ad, "VALOR", "ICMS")
                .ComColuna(5, ad, "VALOR", "IPI")
                .ComColuna(3.5F, ad, "ALIQ.", "ICMS")
                .ComColuna(3.5F, ad, "ALIQ.", "IPI");
            }
            else
            {
                Tabela
                .ComColuna(8.1f, ac, "CÓDIGO PRODUTO")
                .ComColuna(0, ae, "DESCRIÇÃO DO PRODUTO / SERVIÇO")
                .ComColuna(5F, ac, "NCM/SH")
                .ComColuna(3.1F, ac, cabecalho4)
                .ComColuna(3.1F, ac, "CFOP")
                .ComColuna(3F, ac, "UN")
                .ComColuna(4.00F, ad, "QUANT.")
                .ComColuna(5.6F, ad, "VALOR UNIT.")
                .ComColuna(5.6F, ad, "VALOR TOTAL")
                .ComColuna(5.6F, ad, "B CÁLC ICMS")
                .ComColuna(5.6F, ad, "B CÁLC ICMS ST")
                .ComColuna(5.6F, ad, "VALOR ICMS")
                .ComColuna(5.6F, ad, "VALOR IPI")
                .ComColuna(3F, ad, "ALIQ.", "ICMS")
                .ComColuna(3F, ad, "ALIQ.", "IPI");
            }

            Tabela.AjustarLarguraColunas();

            foreach (var p in ViewModel.Produtos)
            {
                var linha = new List<String>
                {
                    p.Codigo,
                    p.DescricaoCompleta,
                    p.Ncm,
                    p.OCst,
                    p.Cfop.Formatar("N0"),
                    p.Unidade,
                    p.Quantidade.Formatar(),
                    p.ValorUnitario.Formatar(),
                    p.ValorTotal.Formatar(),
                    p.BaseIcms.Formatar(),
                    p.BaseIcmsST?.Formatar(),
                    p.ValorIcms.Formatar(),
                    p.ValorIpi.Formatar(),
                    p.AliquotaIcms.Formatar(),
                    p.AliquotaIpi.Formatar()
                };

                Tabela.AdicionarLinha(linha);
            }
        }

        /// <summary>
        /// Quadro "Dados dos Produtos/Serviços" no leiaute da Reforma Tributária (NT 2026.010, item 4.3, modelo de
        /// referência): NCM e cClassTrib na descrição, CST e CFOP numa célula, e as colunas de bases, alíquotas e
        /// valores com uma linha por tributo informado no XML (ICMS, IBS UF, IBS Município, CBS, IS, IPI).
        /// </summary>
        private void AdicionarColunasReformaTributaria(String cabecalhoCst, AlinhamentoHorizontal ac, AlinhamentoHorizontal ad, AlinhamentoHorizontal ae)
        {
            var retrato = ViewModel.IsRetrato;

            Tabela
                .ComColuna(retrato ? 7F : 6F, ac, "CÓDIGO", "PRODUTO")
                .ComColuna(0, ae, "DESCRIÇÃO DO PRODUTO / SERVIÇO")
                .ComColuna(retrato ? 5.5F : 4.5F, ac, cabecalhoCst, "CFOP")
                .ComColuna(retrato ? 6F : 5F, ad, "QTD.", "UN")
                .ComColuna(retrato ? 7F : 6F, ad, "VALOR", "UNIT.")
                .ComColuna(retrato ? 7F : 6F, ad, "VALOR", "TOTAL")
                .ComColuna(retrato ? 13F : 11F, ad, "BASES DE", "CÁLCULO")
                .ComColuna(retrato ? 11F : 9F, ad, "ALÍQUOTAS")
                .ComColuna(retrato ? 13F : 11F, ad, "VALOR DOS", "TRIBUTOS");

            Tabela.AjustarLarguraColunas();

            foreach (var p in ViewModel.Produtos)
            {
                Tabela.AdicionarLinha(new List<String>
                {
                    p.Codigo,
                    p.DescricaoReformaTributaria,
                    p.OCst + "\r\n" + p.Cfop.Formatar("N0"),
                    p.Quantidade.Formatar() + "\r\n" + p.Unidade,
                    p.ValorUnitario.Formatar(),
                    p.ValorTotal.Formatar(),
                    String.Join("\r\n", p.LinhasBases()),
                    String.Join("\r\n", p.LinhasAliquotas()),
                    String.Join("\r\n", p.LinhasValores())
                });
            }
        }

        public override void Draw(Gfx gfx)
        {
            base.Draw(gfx);

            Tabela.SetPosition(RetanguloTabela.Location);
            Tabela.SetSize(RetanguloTabela.Size);
            Tabela.Draw(gfx);

            CabecalhoBloco.SetPosition(X, Y);
            CabecalhoBloco.Width = Width;
            CabecalhoBloco.Draw(gfx);    
        }


        public RectangleF RetanguloTabela => BoundingBox.CutTop(CabecalhoBloco.Height);
        public Boolean CompletamenteDesenhada => Tabela.LinhaAtual == ViewModel.Produtos.Count;
        public override bool PossuiContono => false;
    }
}
