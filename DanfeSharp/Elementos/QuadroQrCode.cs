using DanfeSharp.Graphics;
using org.pdfclown.documents.contents.xObjects;
using System;
using System.Drawing;

namespace DanfeSharp
{
    /// <summary>
    /// Quadro "QR Code" do leiaute da Reforma Tributária (NT 2026.010, modelo de referência): cabeçalho e o QR Code
    /// quadrado, centralizado no espaço que sobra.
    /// </summary>
    internal class QuadroQrCode : ElementoBase
    {
        public XObject QrCode { get; set; }

        public QuadroQrCode(Estilo estilo) : base(estilo)
        {
        }

        public override void Draw(Gfx gfx)
        {
            base.Draw(gfx);

            var r = BoundingBox.InflatedRetangle(0.75F);
            gfx.DrawString("QR CODE", r, Estilo.FonteCampoCabecalho, AlinhamentoHorizontal.Centro, AlinhamentoVertical.Topo);

            if (QrCode == null)
                return;

            r = r.CutTop(Estilo.FonteCampoCabecalho.AlturaLinha);
            var lado = Math.Min(r.Width, r.Height);
            gfx.ShowXObject(QrCode, new RectangleF(r.X + (r.Width - lado) / 2, r.Y + (r.Height - lado) / 2, lado, lado));
        }
    }
}
