using System.Collections;
using System.Collections.Generic;
using System.Drawing; // só RectangleF/SizeF (value types, sem GDI+)
using QRCoder;
using org.pdfclown.documents;
using org.pdfclown.documents.contents.colorSpaces;
using org.pdfclown.documents.contents.composition;
using org.pdfclown.documents.contents.xObjects;

namespace DanfeSharp
{
    /// <summary>
    /// QR Code do DANFE desenhado como vetor no próprio PDF (um Form XObject com um retângulo por
    /// sequência de módulos escuros).
    /// <para>
    /// Antes o QR Code virava <c>Bitmap</c> pelo <c>QRCode.GetGraphic</c> e depois JPEG pelo GDI+
    /// (<c>System.Drawing</c>). Em produção isso roda em Linux com <c>libgdiplus</c>, que não é
    /// thread-safe, e o <c>DanfeService</c> é singleton: o DANFE da NFC-e falhava com
    /// "A generic error occurred in GDI+" todos os dias (nfe/dfetech-product-invoice-api#421).
    /// O caminho vetorial não usa GDI+, não depende de estado compartilhado e produz um QR Code
    /// nítido em qualquer escala, sem os artefatos do JPEG.
    /// </para>
    /// </summary>
    public static class GerarQrCode
    {
        /// <summary>
        /// Matriz de módulos do QR Code, com a quiet zone de 4 módulos que o QRCoder já inclui.
        /// <c>true</c> = módulo escuro. Linha 0 é o topo.
        /// </summary>
        internal static bool[][] GerarModulos(string text)
        {
            var generator = new QRCodeGenerator();
            QRCodeData data = generator.CreateQrCode(plainText: text, eccLevel: QRCodeGenerator.ECCLevel.M, eciMode: QRCodeGenerator.EciMode.Iso8859_1);

            List<BitArray> matrix = data.ModuleMatrix;
            var modulos = new bool[matrix.Count][];
            for (int y = 0; y < matrix.Count; y++)
            {
                modulos[y] = new bool[matrix[y].Length];
                for (int x = 0; x < matrix[y].Length; x++)
                {
                    modulos[y][x] = matrix[y][x];
                }
            }

            return modulos;
        }

        /// <summary>
        /// QR Code como Form XObject de 1 unidade por módulo. Quem desenha escala para o tamanho
        /// final em <c>ShowXObject</c>, como já fazia com a imagem.
        /// </summary>
        internal static XObject GerarXObject(string text, Document document)
        {
            bool[][] modulos = GerarModulos(text);
            int tamanho = modulos.Length;

            var form = new FormXObject(document, new SizeF(tamanho, tamanho));
            var composer = new PrimitiveComposer(form);

            // O Form XObject herda o estado gráfico de quem o desenha: a cor é explícita para o QR
            // Code não sair com a cor de preenchimento da página.
            composer.SetFillColor(DeviceRGBColor.Black);

            for (int y = 0; y < tamanho; y++)
            {
                int x = 0;
                while (x < tamanho)
                {
                    if (!modulos[y][x])
                    {
                        x++;
                        continue;
                    }

                    // Um retângulo por sequência de módulos escuros na linha: menos operadores e
                    // nenhuma emenda visível entre módulos vizinhos.
                    int inicio = x;
                    while (x < tamanho && modulos[y][x])
                    {
                        x++;
                    }

                    composer.DrawRectangle(new RectangleF(inicio, y, x - inicio, 1));
                }
            }

            composer.Fill();
            composer.Flush();

            return form;
        }
    }
}
