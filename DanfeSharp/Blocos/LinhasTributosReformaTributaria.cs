using System;
using System.Collections.Generic;
using System.Linq;
using DanfeSharp.Modelo;

namespace DanfeSharp.Blocos
{
    /// <summary>
    /// Conteúdo das células do quadro de produtos no leiaute da Reforma Tributária (NT 2026.010, item 4.3, modelo de
    /// referência). Formatação do quadro, separada do dado do item (<see cref="ProdutoViewModel" />).
    /// <para>
    /// Só entra o tributo com informação no XML — a NT (4.4) veda imprimir informação inexistente — e os tributos
    /// vão aos pares por linha, como no modelo ("ICMS…% / CBS…%"), para a nota com muitos itens não ganhar páginas
    /// (item 3 da NT). O modelo não tem ICMS ST por item, então a base de ST não entra.
    /// </para>
    /// </summary>
    public static class LinhasTributosReformaTributaria
    {
        /// <summary>Descrição completa seguida de NCM e cClassTrib, como no modelo de referência.</summary>
        public static String Descricao(ProdutoViewModel p)
        {
            var marcadores = new List<String>();
            if (!String.IsNullOrWhiteSpace(p.Ncm)) marcadores.Add($"[NCM {p.Ncm}]");
            if (!String.IsNullOrWhiteSpace(p.ClassificacaoTributaria)) marcadores.Add($"[cClassTrib {p.ClassificacaoTributaria}]");

            return marcadores.Count == 0 ? p.DescricaoCompleta : p.DescricaoCompleta + "\r\n" + String.Join(" ", marcadores);
        }

        public static IEnumerable<String> Bases(ProdutoViewModel p) => EmPares(
            p.PossuiIcms ? Linha("ICMS", p.BaseIcms) : null,
            p.BaseIbsCbs.HasValue ? Linha("IBS/CBS", p.BaseIbsCbs) : null,
            p.BaseIS.HasValue ? Linha("IS", p.BaseIS) : null,
            p.BaseIpi.HasValue ? Linha("IPI", p.BaseIpi) : null);

        public static IEnumerable<String> Aliquotas(ProdutoViewModel p) => EmPares(
            p.PossuiIcms ? Linha("ICMS", p.AliquotaIcms, "%") : null,
            p.AliquotaCbs.HasValue ? Linha("CBS", p.AliquotaCbs, "%") : null,
            p.AliquotaIbsUF.HasValue ? Linha("IBS UF", p.AliquotaIbsUF, "%") : null,
            p.AliquotaIpi.HasValue ? Linha("IPI", p.AliquotaIpi, "%") : null,
            p.AliquotaIbsMun.HasValue ? Linha("IBS MUN", p.AliquotaIbsMun, "%") : null,
            p.AliquotaIS.HasValue ? Linha("IS", p.AliquotaIS, "%") : null);

        public static IEnumerable<String> Valores(ProdutoViewModel p) => EmPares(
            p.PossuiIcms ? Linha("ICMS", p.ValorIcms) : null,
            p.ValorCbs.HasValue ? Linha("CBS", p.ValorCbs) : null,
            p.ValorIbsUF.HasValue ? Linha("IBS UF", p.ValorIbsUF) : null,
            p.ValorIpi.HasValue ? Linha("IPI", p.ValorIpi) : null,
            p.ValorIbsMun.HasValue ? Linha("IBS MUN", p.ValorIbsMun) : null,
            p.ValorIS.HasValue ? Linha("IS", p.ValorIS) : null);

        // Ordem do modelo da NT: (ICMS, CBS), (IBS UF, IPI), (IBS MUN, IS); os ausentes saem e os pares se fecham.
        private static IEnumerable<String> EmPares(params String[] itens)
        {
            var presentes = itens.Where(i => i != null).ToList();
            for (var i = 0; i < presentes.Count; i += 2)
                yield return i + 1 < presentes.Count ? presentes[i] + " / " + presentes[i + 1] : presentes[i];
        }

        private static String Linha(String tributo, double? valor, String sufixo = "") => $"{tributo} {valor.Formatar()}{sufixo}";
    }
}
