using System;
using System.Drawing;
using DanfeNet.Elementos;
using DanfeNet.Graphics;
using org.pdfclown.documents;
using org.pdfclown.documents.contents.colorSpaces;
using org.pdfclown.documents.contents.composition;

namespace DanfeNet;

internal class DanfeEventoPagina
{
    #region Constructors

    public DanfeEventoPagina(DanfeCartaCorrecao danfe)
    {
        Danfe = danfe ?? throw new ArgumentNullException(nameof(danfe));
        PdfPage = new Page(Danfe.PdfDocument);
        Danfe.PdfDocument.Pages.Add(PdfPage);

        PrimitiveComposer = new PrimitiveComposer(PdfPage);
        Gfx = new Gfx(PrimitiveComposer);

        if (Danfe.ViewModel.Orientacao == Orientacao.Retrato)
            Retangulo = new RectangleF(0, 0, Constantes.A4Largura, Constantes.A4Altura);
        else
            Retangulo = new RectangleF(0, 0, Constantes.A4Altura, Constantes.A4Largura);

        RetanguloDesenhavel = Retangulo.InflatedRetangle(Danfe.ViewModel.Margem);
        RetanguloCreditos = new RectangleF(RetanguloDesenhavel.X, RetanguloDesenhavel.Bottom + Danfe.EstiloPadrao.PaddingSuperior, RetanguloDesenhavel.Width, Retangulo.Height - RetanguloDesenhavel.Height - Danfe.EstiloPadrao.PaddingSuperior);
        PdfPage.Size = new SizeF(Retangulo.Width.ToPoint(), Retangulo.Height.ToPoint());
    }

    #endregion

    #region Properties

    public DanfeCartaCorrecao Danfe { get; private set; }
    public Page PdfPage { get; private set; }
    public PrimitiveComposer PrimitiveComposer { get; private set; }
    public Gfx Gfx { get; private set; }
    public RectangleF RetanguloNumeroFolhas { get; set; }
    public RectangleF RetanguloCorpo { get; private set; }
    public RectangleF RetanguloDesenhavel { get; private set; }
    public RectangleF RetanguloCreditos { get; private set; }
    public RectangleF Retangulo { get; private set; }

    #endregion

    #region Methods

    public void DesenharCreditos(string creditos)
    {
        Gfx.DrawString(creditos, RetanguloCreditos, Danfe.EstiloPadrao.CriarFonteItalico(6), AlinhamentoHorizontal.Direita);
    }

    public void DesenharAvisoHomologacao()
    {
        var ts = new TextStack(Retangulo) { AlinhamentoVertical = AlinhamentoVertical.Centro, AlinhamentoHorizontal = AlinhamentoHorizontal.Centro, LineHeightScale = 0.9F }
            .AddLine("SEM VALOR FISCAL", Danfe.EstiloPadrao.CriarFonteRegular(30))
            .AddLine("AMBIENTE DE HOMOLOGAÇÃO", Danfe.EstiloPadrao.CriarFonteRegular(30));

        Gfx.PrimitiveComposer.BeginLocalState();
        Gfx.PrimitiveComposer.SetFillColor(new DeviceRGBColor(0.35, 0.35, 0.35));
        ts.Draw(Gfx);
        Gfx.PrimitiveComposer.End();
    }

    public void DesenharBlocos()
    {
        var blocos = Danfe.Blocos;

        foreach (var bloco in blocos)
        {
            bloco.Width = RetanguloDesenhavel.Width;

            if (bloco.Posicao == PosicaoBloco.Topo)
            {
                bloco.SetPosition(RetanguloDesenhavel.Location);
                RetanguloDesenhavel = RetanguloDesenhavel.CutTop(bloco.Height + 3);
            }
            else
            {
                bloco.SetPosition(RetanguloDesenhavel.X, RetanguloDesenhavel.Bottom - bloco.Height);
                RetanguloDesenhavel = RetanguloDesenhavel.CutBottom(bloco.Height);
            }

            bloco.Draw(Gfx);
        }

        RetanguloCreditos = new RectangleF(RetanguloDesenhavel.X, RetanguloDesenhavel.Y - 2, RetanguloDesenhavel.Width, Retangulo.Height - RetanguloDesenhavel.Height - Danfe.EstiloPadrao.PaddingSuperior);
        RetanguloCorpo = RetanguloDesenhavel;
        Gfx.Flush();
    }

    #endregion
}