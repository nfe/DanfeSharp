using org.pdfclown.documents.contents.composition;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace DanfeSharp
{
    internal static class Extentions
    {
        private const float PointFactor = 72F / 25.4F;

        private static Dictionary<string, string> forma = new Dictionary<string, string>
        {
            {"fpDinheiro", "Dinheiro" },
            {"fpCheque", "Cheque" },
            {"fpCartaoCredito", "Cartão de Crédito" },
            {"fpCartaoDebito", "Cartão de Débito" },
            {"fpCreditoLoja", "Crédito Loja" },
            {"fpValeAlimentacao", "Vale Alimentação" },
            {"fpValeRefeicao", "Vale Refeição" },
            {"fpValePresente", "Vale Presente" },
            {"fpValeCombustivel", "Vale Combustível" },
            {"fpDuplicataMercantil", "Duplicata Mercantil" },
            {"fpBoletoBancario", "Boleto Bancário" },
            {"fpDepositoBancario", "Depósito Bancário" },
            {"fpPagamentoInstantaneoPIX", "Pagamento Instantâneo (PIX)" },
            {"fpTransferenciabancaria", "Transferência bancária, Carteira Digital" },
            {"fpProgramadefidelidade", "Programa de fidelidade, Cashback, Crédito Virtualo" },
            {"fpSemPagamento", "Sem pagamento" },
            {"fpPagamentoEletronicoNaoInformado", "Pagamento eletrônico não informado" },
            {"fpOutro", "Outros" }
        };


        /// <summary>
        /// Converts Millimeters to Point
        /// </summary>
        /// <param name="mm"></param>
        /// <returns></returns>
        public static float ToPoint(this float mm)
        {
            return PointFactor * mm;
        }

        /// <summary>
        /// Converts Point to Millimeters
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static float ToMm(this float point)
        {
            return point / PointFactor;
        }

        /// <summary>
        /// Converts Point to Millimeters
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static SizeF ToMm(this SizeF s)
        {
            return new SizeF(s.Width.ToMm(), s.Height.ToMm());
        }

        /// <summary>
        /// Converts Point to Millimeters
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static SizeF ToPointMeasure(this SizeF s)
        {
            return new SizeF(s.Width.ToPoint(), s.Height.ToPoint());
        }

        /// <summary>
        /// Converts Millimeters to Point
        /// </summary>
        /// <param name="mm"></param>
        /// <returns></returns>
        public static double ToPoint(this double mm)
        {
            return PointFactor * mm;
        }

        /// <summary>
        /// Converts Point to Millimeters
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static double ToMm(this double point)
        {
            return point / PointFactor;
        }

        public static RectangleF InflatedRetangle(this RectangleF rect, float top, float button, float horizontal)
        {
            return new RectangleF(rect.X + horizontal, rect.Y + top, rect.Width - 2 * horizontal, rect.Height - top - button);
        }

        public static RectangleF InflatedRetangle(this RectangleF rect, float value) => rect.InflatedRetangle(value, value, value);

        public static RectangleF ToPointMeasure(this RectangleF r) => new RectangleF(r.X.ToPoint(), r.Y.ToPoint(), r.Width.ToPoint(), r.Height.ToPoint());

        public static RectangleF CutTop(this RectangleF r, float height) => new RectangleF(r.X, r.Y + height, r.Width, r.Height - height);
        public static RectangleF CutBottom(this RectangleF r, float height) => new RectangleF(r.X, r.Y, r.Width, r.Height - height);
        public static RectangleF CutLeft(this RectangleF r, float width) => new RectangleF(r.X + width, r.Y, r.Width - width, r.Height);

        public static PointF ToPointMeasure(this PointF r) => new PointF(r.X.ToPoint(), r.Y.ToPoint());

        public static StringBuilder AppendChaveValor(this StringBuilder sb, String chave, String valor)
        {
            if (sb.Length > 0) sb.Append(' ');
            return sb.Append(chave).Append(": ").Append(valor);
        }

        public static XAlignmentEnum ToPdfClownAlignment(this AlinhamentoHorizontal ah)
        {
            switch (ah)
            {
                case AlinhamentoHorizontal.Esquerda:
                    return XAlignmentEnum.Left;
                case AlinhamentoHorizontal.Centro:
                    return XAlignmentEnum.Center;
                case AlinhamentoHorizontal.Direita:
                    return XAlignmentEnum.Right;
            }

            throw new InvalidOperationException();
        }

        public static YAlignmentEnum ToPdfClownAlignment(this AlinhamentoVertical av)
        {
            switch (av)
            {
                case AlinhamentoVertical.Topo:
                    return YAlignmentEnum.Top;
                case AlinhamentoVertical.Centro:
                    return YAlignmentEnum.Middle;
                case AlinhamentoVertical.Base:
                    return YAlignmentEnum.Bottom;
            }

            throw new InvalidOperationException();
        }

        public static string FormaPagamentoToString(this FormaPagamento value)
        {
            if (forma.TryGetValue(value.ToString(), out string result))
                return result;

            throw new InvalidOperationException();
        }

        public static string SpaceOnAccessKey(this string current)
        {
            if (!string.IsNullOrWhiteSpace(current))
            {
                var modified = string.Empty;
                for (int i = 0; i < current.Length; i++)
                {
                    if (i != 0 && i % 4 == 0)
                        modified += " ";

                    modified += current[i];
                }
                return modified;
            }

            return null;
        }

        public static string UrlNFCeProduction(this string state)
        {
            var stateDictionary = new Dictionary<string, string>
            {
                {"AC", "www.sefaznet.ac.gov.br/nfce/consulta"},
                {"AL", "www.sefaz.al.gov.br/nfce/consulta"},
                {"AP", "www.sefaz.ap.gov.br/nfce/consulta" },
                {"AM", "www.sefaz.am.gov.br/nfce/consulta"},
                {"BA", "www.sefaz.ba.gov.br/nfce/consulta"},
                {"CE", "http://nfce.sefaz.ce.gov.br/pages/ShowNFCe.html"},
                {"DF", "www.fazenda.df.gov.br/nfce/consulta"},
                {"ES", "www.sefaz.es.gov.br/nfce/consulta"},
                {"GO", "https://www.sefaz.go.gov.br/nfce/consulta"},
                {"MA", "www.sefaz.ma.gov.br/nfce/consulta"},
                {"MG", "www.fazenda.mg.gov.br/nfce/consulta"},
                {"MT", "www.sefaz.mt.gov.br/nfce/consultanfce"},
                {"MS", "www.dfe.ms.gov.br/nfce/consulta"},
                {"PA", "www.sefa.pa.gov.br/nfce/consulta"},
                {"PB", "www.receita.pb.gov.br/nfce/consulta"},
                {"PR", "www.fazenda.pr.gov.br/nfce/consulta"},
                {"PE", "nfce.sefaz.pe.gov.br/nfce/consulta"},
                {"PI", "www.sefaz.pi.gov.br/nfce/consulta"},
                {"RJ", "www.fazenda.rj.gov.br/nfce/consulta"},
                {"RN", "www.set.rn.gov.br/nfce/consulta"},
                {"RS", "www.sefaz.rs.gov.br/nfce/consulta"},
                {"RO", "www.sefin.ro.gov.br/nfce/consulta"},
                {"RR", "www.sefaz.rr.gov.br/nfce/consulta"},
                {"SP", "www.nfce.fazenda.sp.gov.br/consulta"},
                {"SE", "www.nfce.se.gov.br/nfce/consulta"},
                {"TO", "www.sefaz.to.gov.br/nfce/consulta"},
                {"SC", ""}
            }.TryGetValue(state, out string value);

            return value;
        }

        public static string UrlNFCeTest(this string state)
        {
            var stateDictionary = new Dictionary<string, string>
            {
                {"AC", "www.sefaznet.ac.gov.br/nfce/consulta"},
                {"AL", "www.sefaz.al.gov.br/nfce/consulta"},
                {"AP", "www.sefaz.ap.gov.br/nfce/consulta" },
                {"AM", "www.sefaz.am.gov.br/nfce/consulta"},
                {"BA", "hinternet.sefaz.ba.gov.br/nfce/consulta"},
                {"CE", "http://nfceh.sefaz.ce.gov.br/pages/ShowNFCe.html"},
                {"DF", "www.fazenda.df.gov.br/nfce/consulta"},
                {"ES", "www.sefaz.es.gov.br/nfce/consulta"},
                {"GO", "https://www.sefaz.go.gov.br/nfce/consulta"},
                {"MA", "www.sefaz.ma.gov.br/nfce/consulta"},
                {"MG", "www.fazenda.mg.gov.br/nfce/consulta"},
                {"MT", "homologacao.sefaz.mt.gov.br/nfce/consultanfce"},
                {"MS", "www.dfe.ms.gov.br/nfce/consulta"},
                {"PA", "www.sefa.pa.gov.br/nfce/consulta"},
                {"PB", "www.receita.pb.gov.br/nfcehom"},
                {"PR", "www.fazenda.pr.gov.br/nfce/consulta"},
                {"PE", "nfce.sefaz.pe.gov.br/nfce/consulta"},
                {"PI", "www.sefaz.pi.gov.br/nfce/consulta"},
                {"RJ", "www.fazenda.rj.gov.br/nfce/consulta"},
                {"RN", "www.set.rn.gov.br/nfce/consulta"},
                {"RS", "www.sefaz.rs.gov.br/nfce/consulta"},
                {"RO", "www.sefin.ro.gov.br/nfce/consulta"},
                {"RR", "www.sefaz.rr.gov.br/nfce/consulta"},
                {"SP", "www.homologacao.nfce.fazenda.sp.gov.br/consulta"},
                {"SE", "www.hom.nfe.se.gov.br/nfce/consulta"},
                {"TO", "www.sefaz.to.gov.br/nfce/consulta"},
                {"SC", ""}
            }.TryGetValue(state, out string value);

            return value;
        }
    }
}
