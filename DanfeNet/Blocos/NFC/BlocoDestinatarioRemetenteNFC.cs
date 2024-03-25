using System.Drawing;
using DanfeNet.Elementos;
using DanfeNet.Models;
using org.pdfclown.documents.contents.composition;

namespace DanfeNet.Blocos.NFC;

internal class BlocoDestinatarioRemetenteNFC : BlocoBase
{
    public BlocoDestinatarioRemetenteNFC(Danfe viewModel, Estilo estilo, PrimitiveComposer primitiveComposer, float y) : base(viewModel, estilo)
    {
        primitiveComposer.BeginLocalState();
        primitiveComposer.SetFont(estilo.FonteCampoConteudo.FonteInterna, estilo.FonteCampoConteudo.Tamanho);

        if (viewModel.Destinatario != null && !string.IsNullOrWhiteSpace(viewModel?.Destinatario?.CnpjCpf))
        {
            var dest = viewModel?.Destinatario;

            primitiveComposer.ShowText("CONSUMIDOR", new PointF(140, y + 10), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);

            if (dest?.RazaoSocial?.Length > 30)
            {
                if(dest?.RazaoSocial?.Length >= 39)
                {
                    primitiveComposer.ShowText(dest.RazaoSocial.Substring(0, 39), new PointF(140, y + 20), XAlignmentEnum.Center, YAlignmentEnum.Top, 0);
                        
                    if (dest.RazaoSocial.Length >= 60)
                        primitiveComposer.ShowText(dest.RazaoSocial.Substring(39, 60), new PointF(140, y + 30), XAlignmentEnum.Center, YAlignmentEnum.Top, 0);
                    else
                        primitiveComposer.ShowText(dest.RazaoSocial.Substring(39), new PointF(140, y + 30), XAlignmentEnum.Center, YAlignmentEnum.Top, 0);
                }
                else
                    primitiveComposer.ShowText(dest.RazaoSocial, new PointF(140, y + 20), XAlignmentEnum.Center, YAlignmentEnum.Top, 0);

                    
                primitiveComposer.ShowText($"CNPJ/CPF: {dest.CnpjCpf}", new PointF(140, y + 40), XAlignmentEnum.Center, YAlignmentEnum.Top, 0);

                y = y + 10;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(dest.RazaoSocial))
                    primitiveComposer.ShowText($"NOME: {dest.RazaoSocial}", new PointF(140, y + 20), XAlignmentEnum.Center, YAlignmentEnum.Top, 0);

                primitiveComposer.ShowText($"CNPJ/CPF: {dest.CnpjCpf}", new PointF(140, y + 30), XAlignmentEnum.Center, YAlignmentEnum.Top, 0);
            }

            if (!string.IsNullOrWhiteSpace(dest.EnderecoLogadrouro) &&
                !string.IsNullOrWhiteSpace(dest.EnderecoNumero) &&
                !string.IsNullOrWhiteSpace(dest.EnderecoBairro) &&
                !string.IsNullOrWhiteSpace(dest.EnderecoUf) &&
                !string.IsNullOrWhiteSpace(dest.Municipio))
            {
                if (dest.EnderecoLogadrouro.Length <= 20)
                    primitiveComposer.ShowText($"{dest.EnderecoLogadrouro}, {dest.EnderecoNumero}, {dest.EnderecoBairro}, {dest.Municipio} - {dest.EnderecoUf}", new PointF(140, y + 40), XAlignmentEnum.Center, YAlignmentEnum.Top, 0);
            }

            Y_NFC = y + 50;

            primitiveComposer.DrawLine(new PointF(15, Y_NFC), new PointF(265, Y_NFC));
            primitiveComposer.SetLineDash(new org.pdfclown.documents.contents.LineDash(new double[] { 3, 2 }));

            primitiveComposer.Stroke();
            primitiveComposer.End();
        }
        else
        {
            primitiveComposer.ShowText("CONSUMIDOR NÃO IDENTIFICADO", new PointF(140, y + 10), XAlignmentEnum.Center, YAlignmentEnum.Middle, 0);

            Y_NFC = y + 20;

            primitiveComposer.DrawLine(new PointF(15, Y_NFC), new PointF(265, Y_NFC));
            primitiveComposer.SetLineDash(new org.pdfclown.documents.contents.LineDash(new double[] { 3, 2 }));

            primitiveComposer.Stroke();
            primitiveComposer.End();
        }
    }

    public override string Cabecalho => "Destinatário / Remetente";
    public override PosicaoBloco Posicao => PosicaoBloco.Topo;
}