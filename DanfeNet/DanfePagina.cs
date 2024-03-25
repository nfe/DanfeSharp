using System;
using System.Drawing;
using System.Linq;
using DanfeNet.Blocos;
using DanfeNet.Elementos;
using DanfeNet.Graphics;
using org.pdfclown.documents;
using org.pdfclown.documents.contents.colorSpaces;
using org.pdfclown.documents.contents.composition;

namespace DanfeNet;

internal class DanfePagina
{
    public DanfePdf DanfePdf { get; private set; }
    public Page PdfPage { get; private set; }
    public PrimitiveComposer PrimitiveComposer { get; private set; }
    public Gfx Gfx { get; private set; }
    public RectangleF RetanguloNumeroFolhas { get;  set; }
    public RectangleF RetanguloCorpo { get; private set; }
    public RectangleF RetanguloDesenhavel { get; private set; }
    public RectangleF RetanguloCreditos { get; private set; }
    public RectangleF Retangulo { get; private set; }

    public DanfePagina(DanfePdf danfePdf)
    {
        DanfePdf = danfePdf ?? throw new ArgumentNullException(nameof(danfePdf));
        PdfPage = new Page(DanfePdf.PdfDocument);
        DanfePdf.PdfDocument.Pages.Add(PdfPage);
         
        PrimitiveComposer = new PrimitiveComposer(PdfPage);
        Gfx = new Gfx(PrimitiveComposer);

        if (DanfePdf.ViewModel.Orientacao == Orientacao.Retrato)            
            Retangulo = new RectangleF(0, 0, Constantes.A4Largura, Constantes.A4Altura);            
        else            
            Retangulo = new RectangleF(0, 0, Constantes.A4Altura, Constantes.A4Largura);
            
        RetanguloDesenhavel = Retangulo.InflatedRetangle(DanfePdf.ViewModel.Margem);
        RetanguloCreditos = new RectangleF(RetanguloDesenhavel.X, RetanguloDesenhavel.Bottom + DanfePdf.EstiloPadrao.PaddingSuperior, RetanguloDesenhavel.Width, Retangulo.Height - RetanguloDesenhavel.Height - DanfePdf.EstiloPadrao.PaddingSuperior);
        PdfPage.Size = new SizeF(Retangulo.Width.ToPoint(), Retangulo.Height.ToPoint());    
    }

    public void DesenharCreditos(string creditos)
    {
        Gfx.DrawString(creditos, RetanguloCreditos, DanfePdf.EstiloPadrao.CriarFonteItalico(6), AlinhamentoHorizontal.Direita);
    }

    private void DesenharCanhoto()
    {
        if (DanfePdf.ViewModel.QuantidadeCanhotos == 0) return;

        var canhoto = DanfePdf.Canhoto;
        canhoto.SetPosition(RetanguloDesenhavel.Location);

        if (DanfePdf.ViewModel.Orientacao == Orientacao.Retrato)
        {           
            canhoto.Width = RetanguloDesenhavel.Width;

            for (int i = 0; i < DanfePdf.ViewModel.QuantidadeCanhotos; i++)
            {
                canhoto.Draw(Gfx);
                canhoto.Y += canhoto.Height;
            }

            RetanguloDesenhavel = RetanguloDesenhavel.CutTop(canhoto.Height * DanfePdf.ViewModel.QuantidadeCanhotos);
        }
        else
        {
            canhoto.Width = RetanguloDesenhavel.Height;
            Gfx.PrimitiveComposer.BeginLocalState();
            Gfx.PrimitiveComposer.Rotate(90, new PointF(0, canhoto.Width + canhoto.X + canhoto.Y).ToPointMeasure());

            for (int i = 0; i < DanfePdf.ViewModel.QuantidadeCanhotos; i++)
            {
                canhoto.Draw(Gfx);
                canhoto.Y += canhoto.Height;
            }              

            Gfx.PrimitiveComposer.End();
            RetanguloDesenhavel = RetanguloDesenhavel.CutLeft(canhoto.Height * DanfePdf.ViewModel.QuantidadeCanhotos);

        }
    }

    public void DesenhaNumeroPaginas(int n, int total)
    {
        if (n <= 0) throw new ArgumentOutOfRangeException(nameof(n));
        if (total <= 0) throw new ArgumentOutOfRangeException(nameof(n));
        if (n > total) throw new ArgumentOutOfRangeException("O número da página atual deve ser menor que o total.");

        Gfx.DrawString($"Folha {n}/{total}", RetanguloNumeroFolhas, DanfePdf.EstiloPadrao.FonteNumeroFolhas, AlinhamentoHorizontal.Centro);
        Gfx.Flush();
    }

    public void DesenharAvisoHomologacao()
    {
        TextStack ts = new TextStack(RetanguloCorpo) { AlinhamentoVertical = AlinhamentoVertical.Centro, AlinhamentoHorizontal = AlinhamentoHorizontal.Centro, LineHeightScale = 0.9F }
            .AddLine("SEM VALOR FISCAL", DanfePdf.EstiloPadrao.CriarFonteRegular(48))
            .AddLine("AMBIENTE DE HOMOLOGAÇÃO", DanfePdf.EstiloPadrao.CriarFonteRegular(30));

        Gfx.PrimitiveComposer.BeginLocalState();
        Gfx.PrimitiveComposer.SetFillColor(new org.pdfclown.documents.contents.colorSpaces.DeviceRGBColor(0.35, 0.35, 0.35));
        ts.Draw(Gfx);
        Gfx.PrimitiveComposer.End();
    }

    public void DesenharBlocos(bool isPrimeirapagina = false)
    {
        if (isPrimeirapagina && DanfePdf.ViewModel.QuantidadeCanhotos > 0) DesenharCanhoto();

        var blocos = isPrimeirapagina ? DanfePdf._Blocos : DanfePdf._Blocos.Where(x => x.VisivelSomentePrimeiraPagina == false);

        foreach (var bloco in blocos)
        {
            bloco.Width = RetanguloDesenhavel.Width;

            if (bloco.Posicao == PosicaoBloco.Topo)
            {
                bloco.SetPosition(RetanguloDesenhavel.Location);
                RetanguloDesenhavel = RetanguloDesenhavel.CutTop(bloco.Height);
            }
            else
            {
                bloco.SetPosition(RetanguloDesenhavel.X, RetanguloDesenhavel.Bottom - bloco.Height);
                RetanguloDesenhavel = RetanguloDesenhavel.CutBottom(bloco.Height);
            }

            bloco.Draw(Gfx);

            if (bloco is BlocoIdentificacaoEmitente)
            {
                var rf = (bloco as BlocoIdentificacaoEmitente).RetanguloNumeroFolhas;
                RetanguloNumeroFolhas = rf;
            }
        }

        RetanguloCorpo = RetanguloDesenhavel;
        Gfx.Flush();
    }
    
    
    public void DesenharAvisoCancelamento()
    {
        var ts = new TextStack(RetanguloCorpo)
            {
                AlinhamentoVertical = AlinhamentoVertical.Centro,
                AlinhamentoHorizontal = AlinhamentoHorizontal.Centro,
                LineHeightScale = 0.9F
            }
            .AddLine("DOCUMENTO CANCELADO", DanfePdf.EstiloPadrao.CriarFonteRegular(48));

        var colorRed = new DeviceRGBColor(255, 0.35, 0.35);
        Gfx.PrimitiveComposer.BeginLocalState();
        Gfx.PrimitiveComposer.SetFillColor(colorRed);
        ts.Draw(Gfx);
        Gfx.PrimitiveComposer.End();
    }
    
}