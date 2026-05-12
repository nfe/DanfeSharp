using DanfeSharp.Graphics;
using DanfeSharp.Modelo;
using System;
using System.Drawing;

namespace DanfeSharp
{
    class NumeroNfSerie2 : ElementoBase
    {
        public RectangleF RetanguloNumeroFolhas { get; private set; }
        public DanfeViewModel ViewModel { get; private set; }

        public NumeroNfSerie2(Estilo estilo, DanfeViewModel viewModel) : base(estilo)
        {
            ViewModel = viewModel;
        }

        public override void Draw(Gfx gfx)
        {
            base.Draw(gfx);

            // 7.7.4 Conteúdo do Bloco de Campos de Identificação do Documento
            // O conteúdo dos campos “DANFE”, “entrada ou saída”, “número”, “série” e “folhas do
            // documento” deverá ser impresso em caixa alta(maiúsculas). Além disto:
            //         a descrição “DANFE” deverá estar impressa em negrito e ter tamanho mínimo de
            //         doze(12) pontos, ou 10 CPP;
            //
            //         a série e número da NF-e, o número de ordem da folha, o total de folhas do
            //         DANFE e o número identificador do tipo de operação(se “ENTRADA” ou
            //         “SAÍDA”, conforme tag “tpNF”) deverão estar impressos em negrito e ter
            //          tamanho mínimo de dez(10) pontos, ou 10 CPP;
            //
            //         a identificação “DOCUMENTO AUXILIAR DA NOTA FISCAL ELETRÔNICA” e as
            //         descrições do tipo de operação, “ENTRADA” ou “SAÍDA” deverão ter tamanho
            //         mínimo de oito(8) pontos, ou 17 CPP.

            float paddingHorizontal = ViewModel.Orientacao == Orientacao.Retrato ? 2.5F : 5F;

            var rp1 = BoundingBox.InflatedRetangle(1F, 0.5F, paddingHorizontal);
            var rp2 = rp1;

            // Cabeçalho: "DANFE" para NF-e regular, "NOTA DE CRÉDITO" quando
            // finNFe=5 (Ajustes SINIEF 49/25 + 8/26), "NOTA DE DÉBITO" quando
            // finNFe=6 (Ajuste SINIEF 49/25). Tamanho de fonte ajustado para
            // acomodar o título mais longo no mesmo retângulo.
            bool isNotaCredito = ViewModel.Finalidade == 5;
            bool isNotaDebito = ViewModel.Finalidade == 6;
            string tituloCabecalho;
            if (isNotaCredito) tituloCabecalho = "NOTA DE CRÉDITO";
            else if (isNotaDebito) tituloCabecalho = "NOTA DE DÉBITO";
            else tituloCabecalho = "DANFE";
            var f1 = Estilo.CriarFonteNegrito(isNotaCredito || isNotaDebito ? 10 : 12);
            var f1h = f1.AlturaLinha;
            gfx.DrawString(tituloCabecalho, rp2, f1, AlinhamentoHorizontal.Centro);

            rp2 = rp2.CutTop(f1h + 0.5F);

            var f2 = Estilo.CriarFonteRegular(8F);
            var f2h = (float)f2.AlturaLinha;

            var ts = new TextStack(rp2)
            {
                AlinhamentoVertical = AlinhamentoVertical.Topo
            }
            .AddLine("Documento Auxiliar da", f2)
            .AddLine("Nota Fiscal Eletrônica", f2);

            // Para Nota de Crédito por Recusa / Nota de Débito, exibe o subtipo
            // logo abaixo do bloco descritivo. Quando presente, o TextStack
            // ganha uma 3ª linha — o cálculo do CutTop precisa refletir isso
            // para não sobrepor o bloco "ENTRADA/SAÍDA" abaixo.
            float subtipoLineHeight = 0F;
            if (isNotaCredito && ViewModel.TipoNotaCredito.HasValue)
            {
                string subtipo = ViewModel.TipoNotaCredito.Value switch
                {
                    3 => "(Recusa Total / Não Localização)",
                    6 => "(Recusa Parcial)",
                    _ => $"(tpNFCredito={ViewModel.TipoNotaCredito.Value:D2})"
                };
                var fSubtipo = Estilo.CriarFonteNegrito(7F);
                ts.AddLine(subtipo, fSubtipo);
                subtipoLineHeight = (float)fSubtipo.AlturaLinha * TextStack.DefaultLineHeightScale;
            }
            else if (isNotaDebito && ViewModel.TipoNotaDebito.HasValue)
            {
                string subtipo = ViewModel.TipoNotaDebito.Value switch
                {
                    1 => "(Transferência de Créditos p/ Cooperativa)",
                    2 => "(Cancelamento de Créditos)",
                    3 => "(Débitos Não Processados)",
                    4 => "(Multas / Juros)",
                    5 => "(Sucessão)",
                    6 => "(Pagamento Antecipado)",
                    7 => "(Perda de Estoque)",
                    8 => "(Desenquadramento Simples Nacional)",
                    _ => $"(tpNFDebito={ViewModel.TipoNotaDebito.Value:D2})"
                };
                var fSubtipo = Estilo.CriarFonteNegrito(7F);
                ts.AddLine(subtipo, fSubtipo);
                subtipoLineHeight = (float)fSubtipo.AlturaLinha * TextStack.DefaultLineHeightScale;
            }

            ts.Draw(gfx);

            rp2 = rp2.CutTop(2F * f2h + subtipoLineHeight + 1.5F);


            ts = new TextStack(rp2)
            {
                AlinhamentoVertical = AlinhamentoVertical.Topo,
                AlinhamentoHorizontal = AlinhamentoHorizontal.Esquerda
            }
            .AddLine("0 - ENTRADA", f2)
            .AddLine("1 - SAÍDA", f2);
            ts.Draw(gfx);

            float rectEsSize = 1.75F * f2h;
            var rectEs = new RectangleF(rp2.Right - rectEsSize, rp2.Y + (2F * f2h - rectEsSize) / 2F, rectEsSize, rectEsSize);

            gfx.StrokeRectangle(rectEs, 0.25F);

            gfx.DrawString(ViewModel.TipoNF.ToString(), rectEs, Estilo.FonteNumeroFolhas, AlinhamentoHorizontal.Centro, AlinhamentoVertical.Centro);


            var f4 = Estilo.FonteNumeroFolhas;
            var f4h = Estilo.FonteNumeroFolhas.AlturaLinha;

            rp2.Height = 2F * f4h * TextStack.DefaultLineHeightScale + f2h;
            rp2.Y = rp1.Bottom - rp2.Height;

            ts = new TextStack(rp2)
            {
                AlinhamentoVertical = AlinhamentoVertical.Topo,
                AlinhamentoHorizontal = AlinhamentoHorizontal.Centro
            }
            .AddLine("Nº.: " + ViewModel.NfNumero.ToString(Formatador.FormatoNumeroNF), f4)
            .AddLine($"Série: {ViewModel.NfSerie}", f4);

            ts.Draw(gfx);

            RetanguloNumeroFolhas = new RectangleF(rp1.Left, rp1.Bottom - Estilo.FonteNumeroFolhas.AlturaLinha, rp1.Width, Estilo.FonteNumeroFolhas.AlturaLinha);
        }
    }
}