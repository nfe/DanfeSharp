﻿using DanfeSharp.Modelo;
using org.pdfclown.documents.contents.composition;
using System.Drawing;

namespace DanfeSharp.Blocos.NFC
{
    internal class BlocoIdentificacaoContingencia : ElementoBase
    {
        public BlocoIdentificacaoContingencia(DanfeViewModel viewModel, Estilo estilo, PrimitiveComposer primitiveComposer, float y) : base(estilo)
        {
            primitiveComposer.BeginLocalState();

            primitiveComposer.SetFont(estilo.FonteCampoTituloNegrito.FonteInterna, estilo.FonteCampoTituloNegrito.Tamanho);
            primitiveComposer.ShowText("DANFE-NFC-e impresso em contingência - Offline", new PointF(140, y + 10), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);

            primitiveComposer.SetFont(estilo.FonteCampoConteudoNegrito.FonteInterna, estilo.FonteCampoConteudoNegrito.Tamanho);
            primitiveComposer.ShowText($"Regularmente recebido pela administração tributária autorizadora", new PointF(140, y + 20), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);

            primitiveComposer.SetFont(estilo.FonteCampoConteudo.FonteInterna, estilo.FonteCampoConteudo.Tamanho);
            primitiveComposer.ShowText($"Data entrada contingência: {viewModel.ContingenciaDataHora}", new PointF(140, y + 30), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);

            primitiveComposer.SetFont(estilo.FonteCampoConteudo.FonteInterna, estilo.FonteCampoConteudo.Tamanho);
            primitiveComposer.ShowText($"Justificativa contingência: {viewModel.ContingenciaJustificativa}", new PointF(140, y + 40), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);

            Y_NFC = y + 50;

            primitiveComposer.DrawLine(new PointF(15, Y_NFC), new PointF(265, Y_NFC));
            primitiveComposer.SetLineDash(new org.pdfclown.documents.contents.LineDash(new double[] { 3, 2 }));

            primitiveComposer.Stroke();
            primitiveComposer.End();
        }
    }
}