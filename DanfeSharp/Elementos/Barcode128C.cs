using System;
using System.Collections.Generic;
using System.Drawing;
using DanfeSharp.Graphics;

namespace DanfeSharp
{
    /// <summary>
    /// Desenha o Código de Barras Code 128 da chave de acesso.
    /// Chave 100% numérica é codificada em Code 128C puro (comportamento legado);
    /// chave alfanumérica (CNPJ alfanumérico — NT Conjunta DFe 2025.001 / NT 2026.004)
    /// usa o esquema híbrido CODE-128A + CODE-128C, alternando os conjuntos via código
    /// de controle (99 = muda p/ C, 101 = muda p/ A).
    /// </summary>
    internal class Barcode128C : ElementoBase
    {
        private static byte[][] Dic;

        public static readonly float MargemVertical = 2;

        /// <summary>
        /// Código a ser codificado em barras.
        /// </summary>
        public String Code { get; private set; }

        /// <summary>
        /// Largura do código de barras.
        /// </summary>
        public float Largura { get; set; }

        static Barcode128C()
        {

            Dic = new byte[][]
            {
                new byte[] { 2,1,2,2,2,2},
                new byte[] { 2,2,2,1,2,2},
                new byte[] { 2,2,2,2,2,1},
                new byte[] { 1,2,1,2,2,3},
                new byte[] { 1,2,1,3,2,2},
                new byte[] { 1,3,1,2,2,2},
                new byte[] { 1,2,2,2,1,3},
                new byte[] { 1,2,2,3,1,2},
                new byte[] { 1,3,2,2,1,2},
                new byte[] { 2,2,1,2,1,3},
                new byte[] { 2,2,1,3,1,2},
                new byte[] { 2,3,1,2,1,2},
                new byte[] { 1,1,2,2,3,2},
                new byte[] { 1,2,2,1,3,2},
                new byte[] { 1,2,2,2,3,1},
                new byte[] { 1,1,3,2,2,2},
                new byte[] { 1,2,3,1,2,2},
                new byte[] { 1,2,3,2,2,1},
                new byte[] { 2,2,3,2,1,1},
                new byte[] { 2,2,1,1,3,2},
                new byte[] { 2,2,1,2,3,1},
                new byte[] { 2,1,3,2,1,2},
                new byte[] { 2,2,3,1,1,2},
                new byte[] { 3,1,2,1,3,1},
                new byte[] { 3,1,1,2,2,2},
                new byte[] { 3,2,1,1,2,2},
                new byte[] { 3,2,1,2,2,1},
                new byte[] { 3,1,2,2,1,2},
                new byte[] { 3,2,2,1,1,2},
                new byte[] { 3,2,2,2,1,1},
                new byte[] { 2,1,2,1,2,3},
                new byte[] { 2,1,2,3,2,1},
                new byte[] { 2,3,2,1,2,1},
                new byte[] { 1,1,1,3,2,3},
                new byte[] { 1,3,1,1,2,3},
                new byte[] { 1,3,1,3,2,1},
                new byte[] { 1,1,2,3,1,3},
                new byte[] { 1,3,2,1,1,3},
                new byte[] { 1,3,2,3,1,1},
                new byte[] { 2,1,1,3,1,3},
                new byte[] { 2,3,1,1,1,3},
                new byte[] { 2,3,1,3,1,1},
                new byte[] { 1,1,2,1,3,3},
                new byte[] { 1,1,2,3,3,1},
                new byte[] { 1,3,2,1,3,1},
                new byte[] { 1,1,3,1,2,3},
                new byte[] { 1,1,3,3,2,1},
                new byte[] { 1,3,3,1,2,1},
                new byte[] { 3,1,3,1,2,1},
                new byte[] { 2,1,1,3,3,1},
                new byte[] { 2,3,1,1,3,1},
                new byte[] { 2,1,3,1,1,3},
                new byte[] { 2,1,3,3,1,1},
                new byte[] { 2,1,3,1,3,1},
                new byte[] { 3,1,1,1,2,3},
                new byte[] { 3,1,1,3,2,1},
                new byte[] { 3,3,1,1,2,1},
                new byte[] { 3,1,2,1,1,3},
                new byte[] { 3,1,2,3,1,1},
                new byte[] { 3,3,2,1,1,1},
                new byte[] { 3,1,4,1,1,1},
                new byte[] { 2,2,1,4,1,1},
                new byte[] { 4,3,1,1,1,1},
                new byte[] { 1,1,1,2,2,4},
                new byte[] { 1,1,1,4,2,2},
                new byte[] { 1,2,1,1,2,4},
                new byte[] { 1,2,1,4,2,1},
                new byte[] { 1,4,1,1,2,2},
                new byte[] { 1,4,1,2,2,1},
                new byte[] { 1,1,2,2,1,4},
                new byte[] { 1,1,2,4,1,2},
                new byte[] { 1,2,2,1,1,4},
                new byte[] { 1,2,2,4,1,1},
                new byte[] { 1,4,2,1,1,2},
                new byte[] { 1,4,2,2,1,1},
                new byte[] { 2,4,1,2,1,1},
                new byte[] { 2,2,1,1,1,4},
                new byte[] { 4,1,3,1,1,1},
                new byte[] { 2,4,1,1,1,2},
                new byte[] { 1,3,4,1,1,1},
                new byte[] { 1,1,1,2,4,2},
                new byte[] { 1,2,1,1,4,2},
                new byte[] { 1,2,1,2,4,1},
                new byte[] { 1,1,4,2,1,2},
                new byte[] { 1,2,4,1,1,2},
                new byte[] { 1,2,4,2,1,1},
                new byte[] { 4,1,1,2,1,2},
                new byte[] { 4,2,1,1,1,2},
                new byte[] { 4,2,1,2,1,1},
                new byte[] { 2,1,2,1,4,1},
                new byte[] { 2,1,4,1,2,1},
                new byte[] { 4,1,2,1,2,1},
                new byte[] { 1,1,1,1,4,3},
                new byte[] { 1,1,1,3,4,1},
                new byte[] { 1,3,1,1,4,1},
                new byte[] { 1,1,4,1,1,3},
                new byte[] { 1,1,4,3,1,1},
                new byte[] { 4,1,1,1,1,3},
                new byte[] { 4,1,1,3,1,1},
                new byte[] { 1,1,3,1,4,1},
                new byte[] { 1,1,4,1,3,1},
                new byte[] { 3,1,1,1,4,1},
                new byte[] { 4,1,1,1,3,1},
                new byte[] { 2,1,1,4,1,2},
                new byte[] { 2,1,1,2,1,4},
                new byte[] { 2,1,1,2,3,2},
                new byte[] { 2,3,3,1,1,1,2}

            };

        }

        public Barcode128C(String code, Estilo estilo, float largura = 75F) : base(estilo)
        {
            if (String.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException("O código não pode ser vazio.", "code");
            }

            Code = code.Trim().ToUpperInvariant();
            Largura = largura;
        }

        /// <summary>
        /// Codifica o valor na sequência de símbolos Code 128 (início, dados, dígito verificador e parada).
        /// Numérico puro usa Code 128C; alfanumérico usa o híbrido CODE-128A/128C.
        /// </summary>
        internal static List<int> Encode(string code)
        {
            var allDigits = true;
            foreach (var ch in code)
            {
                if (!Char.IsDigit(ch)) { allDigits = false; break; }
            }

            var values = allDigits ? EncodeCodeC(code) : EncodeHybrid(code);

            // Dígito verificador (módulo 103) sobre o símbolo de início + dados
            int cd = values[0];
            for (int i = 1; i < values.Count; i++)
            {
                cd += i * values[i];
                cd %= 103;
            }
            values.Add(cd);

            values.Add(106); // Stop
            return values;
        }

        private static List<int> EncodeCodeC(string code)
        {
            if (code.Length % 2 != 0)
            {
                code = "0" + code; // Code C codifica pares de dígitos
            }

            var values = new List<int> { 105 }; // Start C
            for (int i = 0; i < code.Length; i += 2)
            {
                values.Add(Int32.Parse(code.Substring(i, 2)));
            }
            return values;
        }

        private static List<int> EncodeHybrid(string code)
        {
            var values = new List<int>();
            var n = code.Length;
            var i = 0;

            int Digits(int from)
            {
                var c = 0;
                while (from + c < n && Char.IsDigit(code[from + c])) c++;
                return c;
            }

            // Conjunto inicial: Code C se começa com >= 4 dígitos (ou tudo dígito par); senão Code A.
            bool inC;
            var lead = Digits(0);
            if (lead >= 4 || (lead == n && lead % 2 == 0))
            {
                values.Add(105); // Start C
                inC = true;
            }
            else
            {
                values.Add(103); // Start A
                inC = false;
            }

            while (i < n)
            {
                if (inC)
                {
                    var d = Digits(i);
                    if (d >= 2)
                    {
                        var pairs = d / 2;
                        for (var k = 0; k < pairs; k++)
                        {
                            values.Add(Int32.Parse(code.Substring(i, 2)));
                            i += 2;
                        }
                        if (i < n)
                        {
                            values.Add(101); // muda p/ Code A (dígito ímpar restante ou letra)
                            inC = false;
                        }
                    }
                    else
                    {
                        values.Add(101); // muda p/ Code A
                        inC = false;
                    }
                }
                else // Code A
                {
                    var d = Digits(i);
                    if (d >= 4 || (d >= 2 && i + d == n && d % 2 == 0))
                    {
                        values.Add(99); // muda p/ Code C
                        inC = true;
                    }
                    else
                    {
                        // Code A: espaço.._ (ASCII 32-95) => valor 0-63; '0'-'9' => 16-25; 'A'-'Z' => 33-58
                        values.Add(code[i] - 32);
                        i++;
                    }
                }
            }

            return values;
        }

        private void DrawBarcode(RectangleF rect, Gfx gfx)
        {
            var codeBytes = Encode(this.Code);

            float n = codeBytes.Count * 11 + 2;
            float w = rect.Width / n;

            float x = 0;

            for (int i = 0; i < codeBytes.Count; i++)
            {
                byte[] pt = Barcode128C.Dic[codeBytes[i]];

                for (int i2 = 0; i2 < pt.Length; i2++)
                {
                    if (i2 % 2 == 0)
                    {
                        gfx.DrawRectangle(rect.X + x, rect.Y, w * pt[i2], rect.Height);
                    }

                    x += w * pt[i2];
                }
            }

            gfx.Fill();
        }

        public override void Draw(Gfx gfx)
        {
            base.Draw(gfx);

            float w2 = (Width - Largura) / 2F;
            DrawBarcode(new RectangleF( X + w2 , Y + MargemVertical, Largura, Height - 2* MargemVertical), gfx);


        }

    }
}