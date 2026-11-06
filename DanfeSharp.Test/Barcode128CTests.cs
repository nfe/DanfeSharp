using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DanfeSharp.Test
{
    /// <summary>
    /// Cobertura do <see cref="Barcode128C"/> para a chave de acesso com CNPJ
    /// alfanumérico (NT Conjunta DFe 2025.001 / NT 2026.004): a chave numérica
    /// segue em Code 128C puro e a alfanumérica usa o híbrido CODE-128A/128C.
    /// </summary>
    [TestClass]
    public class Barcode128CTests
    {
        private const int StartA = 103;
        private const int StartC = 105;
        private const int Stop = 106;
        private const int CodeC = 99;
        private const int CodeA = 101;

        private const string ChaveNumerica = "35260712345678000190550010000000011000000019";
        private const string ChaveAlfanumerica = "35260712ABC34501DE35550010000000011000000019";

        /// <summary>Decodifica a sequência de símbolos de volta para o texto e confere o dígito verificador.</summary>
        private static string Decodificar(List<int> simbolos)
        {
            Assert.AreEqual(Stop, simbolos[simbolos.Count - 1], "último símbolo deve ser o Stop");

            var dados = simbolos.Count - 2; // sem dígito verificador e Stop
            var soma = simbolos[0];
            for (var i = 1; i < dados; i++)
                soma += i * simbolos[i];
            Assert.AreEqual(soma % 103, simbolos[dados], "dígito verificador módulo 103");

            bool emC;
            if (simbolos[0] == StartC) emC = true;
            else if (simbolos[0] == StartA) emC = false;
            else throw new AssertFailedException("símbolo de início inválido: " + simbolos[0]);

            var texto = new StringBuilder();
            for (var i = 1; i < dados; i++)
            {
                var v = simbolos[i];
                if (emC && v == CodeA) { emC = false; continue; }
                if (!emC && v == CodeC) { emC = true; continue; }

                if (emC)
                {
                    Assert.IsTrue(v < 100, "valor inválido no Code C: " + v);
                    texto.Append(v.ToString("00"));
                }
                else
                {
                    Assert.IsTrue(v < 64, "valor inválido no Code A: " + v);
                    texto.Append((char)(v + 32));
                }
            }
            return texto.ToString();
        }

        [TestMethod]
        public void Encode_ChaveNumerica_UsaCode128CPuro()
        {
            var simbolos = Barcode128C.Encode(ChaveNumerica);

            Assert.AreEqual(StartC, simbolos[0]);
            Assert.AreEqual(1 + 22 + 2, simbolos.Count, "início + 22 pares + DV + Stop");
            CollectionAssert.DoesNotContain(simbolos.GetRange(1, 22), CodeA);
            Assert.AreEqual(ChaveNumerica, Decodificar(simbolos));
        }

        [TestMethod]
        public void Encode_ChaveNumerica_MantemOsSimbolosDoAlgoritmoLegado()
        {
            // Algoritmo anterior: Start C + pares de dígitos + DV módulo 103 + Stop
            var esperado = new List<int> { StartC };
            for (var i = 0; i < ChaveNumerica.Length; i += 2)
                esperado.Add(int.Parse(ChaveNumerica.Substring(i, 2)));
            var cd = StartC;
            for (var i = 1; i < esperado.Count; i++)
                cd = (cd + i * esperado[i]) % 103;
            esperado.Add(cd);
            esperado.Add(Stop);

            CollectionAssert.AreEqual(esperado, Barcode128C.Encode(ChaveNumerica));
        }

        [TestMethod]
        public void Encode_NumeroImpar_CompletaComZeroAEsquerda()
        {
            Assert.AreEqual("0123", Decodificar(Barcode128C.Encode("123")));
        }

        [TestMethod]
        public void Encode_ChaveAlfanumerica_UsaHibrido128A128C()
        {
            var simbolos = Barcode128C.Encode(ChaveAlfanumerica);

            Assert.AreEqual(44, ChaveAlfanumerica.Length);
            Assert.AreEqual(StartC, simbolos[0], "começa com 8 dígitos: Code C");
            CollectionAssert.Contains(simbolos, CodeA, "letras exigem troca para Code A");
            CollectionAssert.Contains(simbolos, CodeC, "volta para Code C nos dígitos seguintes");
            Assert.AreEqual(ChaveAlfanumerica, Decodificar(simbolos));
        }

        [DataTestMethod]
        [DataRow("A1B2C3", DisplayName = "começa com letra")]
        [DataRow("AB12345", DisplayName = "letras e dígitos ímpares no fim")]
        [DataRow("123ABC", DisplayName = "poucos dígitos antes das letras")]
        [DataRow("1234A5678", DisplayName = "dígito ímpar antes da letra")]
        public void Encode_Alfanumerico_DecodificaParaOMesmoTexto(string codigo)
        {
            Assert.AreEqual(codigo, Decodificar(Barcode128C.Encode(codigo)));
        }

        [TestMethod]
        public void Draw_ChaveAlfanumerica_NaoLancaExcecao()
        {
            using (var tester = new SimplePdfPageTester())
            {
                var barcode = new Barcode128C("35260712abc34501de35550010000000011000000019", tester.CriarEstilo())
                {
                    X = 10,
                    Y = 10,
                    Width = 100,
                    Height = 15
                };

                barcode.Draw(tester.Gfx);

                Assert.AreEqual(ChaveAlfanumerica, barcode.Code, "normaliza para maiúsculas");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Construtor_CodigoVazio_LancaExcecao()
        {
            using (var tester = new SimplePdfPageTester())
            {
                new Barcode128C(" ", tester.CriarEstilo());
            }
        }
    }
}
