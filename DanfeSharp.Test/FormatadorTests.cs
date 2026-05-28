using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DanfeSharp.Test
{
    /// <summary>
    /// Cobertura do <see cref="Formatador"/> para CNPJ alfanumérico (Reforma
    /// Tributária — LC 214/2025, vigência 2026-07-01) e regressão dos formatos
    /// numéricos legados.
    /// </summary>
    [TestClass]
    public class FormatadorTests
    {
        // CNPJ alfanumérico: regex agora aceita [0-9A-Z]{12}\d{2}. Máscara
        // visual `XX.XXX.XXX/XXXX-XX` aplicada com letras nas posições corretas.

        [DataTestMethod]
        [DataRow("12345678000190", "12.345.678/0001-90", DisplayName = "CNPJ numérico legado")]
        [DataRow("11222333000181", "11.222.333/0001-81", DisplayName = "CNPJ numérico válido")]
        [DataRow("12ABC345000188", "12.ABC.345/0001-88", DisplayName = "CNPJ alfanumérico letras no meio")]
        [DataRow("98XYZ123000158", "98.XYZ.123/0001-58", DisplayName = "CNPJ alfanumérico outro vetor")]
        public void FormatarCnpj_FormatosValidos_AplicaMascara(string input, string expected)
        {
            Assert.AreEqual(expected, Formatador.FormatarCnpj(input));
        }

        [DataTestMethod]
        [DataRow("12.345.678/0001-90", "12.345.678/0001-90", DisplayName = "CNPJ já formatado (idempotência)")]
        [DataRow("12.ABC.345/0001-88", "12.ABC.345/0001-88", DisplayName = "CNPJ alfanumérico já formatado")]
        public void FormatarCnpj_JaFormatado_PreservaMascara(string input, string expected)
        {
            Assert.AreEqual(expected, Formatador.FormatarCnpj(input));
        }

        [DataTestMethod]
        [DataRow("12345678", DisplayName = "Muito curto")]
        [DataRow("123456789012345", DisplayName = "Muito longo")]
        [DataRow("12abc345000188", DisplayName = "Letras minúsculas — rejeitadas")]
        [DataRow("12ABC345000ABCD", DisplayName = "DV alfanumérico — rejeitado")]
        public void FormatarCnpj_ComprimentoOuFormatoInvalido_RetornaInputSemMascara(string input)
        {
            // Comportamento histórico: quando a regex não bate, o input é devolvido
            // sem máscara, sem exception.
            Assert.AreEqual(input, Formatador.FormatarCnpj(input));
        }

        [DataTestMethod]
        [DataRow("12345678901", "123.456.789-01")]
        [DataRow("11144477735", "111.444.777-35")]
        [DataRow("123.456.789-01", "123.456.789-01")]
        public void FormatarCpf_FormatosValidos_AplicaMascara(string input, string expected)
        {
            Assert.AreEqual(expected, Formatador.FormatarCpf(input));
        }

        [DataTestMethod]
        [DataRow("1234567890A", DisplayName = "CPF com letra — rejeitado (CPF nunca aceita alfanumérico)")]
        [DataRow("123", DisplayName = "Muito curto")]
        [DataRow("123456789012", DisplayName = "Muito longo")]
        public void FormatarCpf_FormatoInvalido_RetornaInputSemMascara(string input)
        {
            Assert.AreEqual(input, Formatador.FormatarCpf(input));
        }

        // FormatarCpfCnpj decide por comprimento da string limpa (sem máscara)
        // antes da regex — eliminando ambiguidade quando um CNPJ alfanumérico
        // pudesse acidentalmente bater na regex CPF (improvável mas defensivo).

        [DataTestMethod]
        [DataRow("12345678901", "123.456.789-01", DisplayName = "CPF numérico")]
        [DataRow("11144477735", "111.444.777-35", DisplayName = "CPF numérico vetor 2")]
        [DataRow("12345678000190", "12.345.678/0001-90", DisplayName = "CNPJ numérico legado")]
        [DataRow("12ABC345000188", "12.ABC.345/0001-88", DisplayName = "CNPJ alfanumérico")]
        [DataRow("98XYZ123000158", "98.XYZ.123/0001-58", DisplayName = "CNPJ alfanumérico vetor 2")]
        public void FormatarCpfCnpj_FormatosValidos_AplicaMascaraCorrespondente(string input, string expected)
        {
            Assert.AreEqual(expected, Formatador.FormatarCpfCnpj(input));
        }

        [DataTestMethod]
        [DataRow("123.456.789-01", "123.456.789-01", DisplayName = "CPF com máscara — limpa e refaz")]
        [DataRow("12.345.678/0001-90", "12.345.678/0001-90", DisplayName = "CNPJ com máscara — limpa e refaz")]
        [DataRow("12.ABC.345/0001-88", "12.ABC.345/0001-88", DisplayName = "CNPJ alfanumérico com máscara — limpa e refaz")]
        public void FormatarCpfCnpj_ComMascara_NormalizaEReaplica(string input, string expected)
        {
            Assert.AreEqual(expected, Formatador.FormatarCpfCnpj(input));
        }

        [DataTestMethod]
        [DataRow("123", DisplayName = "3 chars — não é CPF nem CNPJ")]
        [DataRow("123456789012", DisplayName = "12 chars — comprimento inválido")]
        [DataRow("1234567890123", DisplayName = "13 chars — comprimento inválido")]
        public void FormatarCpfCnpj_ComprimentoInvalido_RetornaInputTrimadoSemMascara(string input)
        {
            // Não joga exception (comportamento histórico). Apenas devolve trimado.
            Assert.AreEqual(input, Formatador.FormatarCpfCnpj(input));
        }

        [TestMethod]
        public void FormatarCpfCnpj_StringVazia_RetornaStringVazia()
        {
            Assert.AreEqual(string.Empty, Formatador.FormatarCpfCnpj(string.Empty));
        }

        [TestMethod]
        public void FormatarCpfCnpj_Null_RetornaStringVazia()
        {
            Assert.AreEqual(string.Empty, Formatador.FormatarCpfCnpj(null));
        }

        [TestMethod]
        public void FormatarCpfCnpj_StringComEspacos_TrimaAntesDeFormatar()
        {
            Assert.AreEqual("12.ABC.345/0001-88", Formatador.FormatarCpfCnpj("  12ABC345000188  "));
        }
    }
}
