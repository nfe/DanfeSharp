using System;
using System.Collections.Generic;
using System.Linq;

namespace DanfeSharp.Modelo
{
    public class ProdutoViewModel
    {
        /// <summary>
        /// <para>Código do produto ou serviço</para>
        /// <para>Tag cProd</para>
        /// </summary>
        public String Codigo { get; set; }

        /// <summary>
        /// <para>Informações Adicionais do Produto</para>
        /// <para>Tag infAdProd</para>
        /// </summary>
        public String InformacoesAdicionais { get; set; }

        /// <summary>
        /// <para>Descrição do produto ou serviço</para>
        /// <para>Tag xProd</para>
        /// </summary>
        public String Descricao { get; set; }

        /// <summary>
        /// <para>Código NCM com 8 dígitos ou 2 dígitos (gênero)</para>
        /// <para>Tag NCM</para>
        /// </summary>
        public String Ncm { get; set; }

        /// <summary>
        /// <para>Origem da mercadoria.</para>
        /// <para>Tag <c>orig</c> do grupo <c>ICMS*</c>.</para>
        /// </summary>
        public String Origem { get; set; }

        /// <summary>
        /// <para>Código de Tributação do ICMS para emitente do Regime Normal.</para>
        /// <para>Tag <c>CST</c> do grupo <c>ICMS*</c>. Mutuamente exclusivo com <see cref="Csosn"/>.</para>
        /// </summary>
        public String Cst { get; set; }

        /// <summary>
        /// <para>Código de Tributação do ICMS para emitente do Simples Nacional.</para>
        /// <para>Tag <c>CSOSN</c> do grupo <c>ICMSSN*</c>. Mutuamente exclusivo com <see cref="Cst"/>.</para>
        /// </summary>
        public String Csosn { get; set; }

        // Backing para o setter público legacy de OCst. Usado apenas quando o
        // consumer atribui OCst diretamente sem popular Origem/Cst/Csosn — ver
        // setter abaixo.
        private String _ocstLegacy;

        /// <summary>
        /// <para>Composição "Origem/Código" exibida na coluna ICMS da DANFE.</para>
        /// <para>Computado a partir de <see cref="Origem"/> + (<see cref="Cst"/> ?? <see cref="Csosn"/>),
        /// separados por <c>/</c>. Omite componentes vazios — XMLs sem <c>orig</c>
        /// exibem só o código (sem barra solta no início).</para>
        /// <para>O setter é mantido como <c>[Obsolete]</c> para preservar compatibilidade
        /// de ABI/source com consumers que construíam <see cref="ProdutoViewModel"/>
        /// atribuindo <c>OCst</c> diretamente (uso típico antes da introdução de
        /// <see cref="Origem"/>/<see cref="Cst"/>/<see cref="Csosn"/>). O valor atribuído
        /// só é usado quando <see cref="Origem"/>, <see cref="Cst"/> e <see cref="Csosn"/>
        /// estão todos vazios — caso contrário a composição dinâmica prevalece.</para>
        /// </summary>
        public String OCst
        {
            get
            {
                String codigo = !String.IsNullOrEmpty(Cst) ? Cst : Csosn;
                Boolean temOrigem = !String.IsNullOrEmpty(Origem);
                Boolean temCodigo = !String.IsNullOrEmpty(codigo);
                if (temOrigem && temCodigo) return Origem + "/" + codigo;
                if (temOrigem) return Origem;
                if (temCodigo) return codigo;
                return _ocstLegacy ?? String.Empty;
            }
            [Obsolete("Atribua Origem e Cst (ou Csosn) separadamente; OCst agora é derivado. Setter mantido apenas para compatibilidade.")]
            set => _ocstLegacy = value;
        }

        /// <summary>
        /// <para>Código Fiscal de Operações e Prestações</para>
        /// <para>Tag CFOP</para>
        /// </summary>
        public int Cfop { get; set; }

        /// <summary>
        /// <para>Unidade Comercial</para>
        /// <para>Tag uCom</para>
        /// </summary>
        public String Unidade { get; set; }

        /// <summary>
        /// <para>Quantidade Comercial</para>
        /// <para>Tag qCom</para>
        /// </summary>
        public double Quantidade { get; set; }

        /// <summary>
        /// <para>Valor Unitário de Comercialização</para>
        /// <para>Tag vUnCom</para>
        /// </summary>
        public double ValorUnitario { get; set; }

        /// <summary>
        /// <para>Valor Total Bruto dos Produtos ou Serviços</para>
        /// <para>Tag vProd</para>
        /// </summary>
        public double ValorTotal { get; set; }

        /// <summary>
        /// <para>Valor da BC do ICMS</para>
        /// <para>Tag vBC</para>
        /// </summary>
        public double BaseIcms { get; set; }

        ///<summary>
        /// Valor da BC do ICMS ST (vBCST)
        /// </summary>
        public double? BaseIcmsST { get; set; }

        /// <summary>
        /// <para>Valor do ICMS</para>
        /// <para>Tag vICMS</para>
        /// </summary>
        public double ValorIcms { get; set; }

        /// <summary>
        /// <para>Alíquota do imposto</para>
        /// <para>Tag pICMS</para>
        /// </summary>
        public double AliquotaIcms { get; set; }

        /// <summary>
        /// <para>Valor do IPI</para>
        /// <para>Tag vIPI</para>
        /// </summary>
        public double? ValorIpi { get; set; }

        /// <summary>
        /// <para>Alíquota do IPI</para>
        /// <para>Tag pIPI</para>
        /// </summary>
        public double? AliquotaIpi { get; set; }

        /// <summary>
        /// <para>Valor aproximado total de tributos federais, estaduais e municipais. [NT2013.003]</para>
        /// <para>Tag vTotTrib</para>
        /// </summary>
        public double? ValorAproximadoTributos { get; set; }

        /// <summary>
        /// <para>Chave de acesso (44 dígitos) da NF-e original referenciada por este item.</para>
        /// <para>Aplicável apenas em Notas de Crédito por Recusa Parcial
        /// (<c>finNFe=5</c> + <c>tpNFCredito=06</c>, Ajuste SINIEF 8/26).
        /// Mapeia para <c>det/DFeReferenciado/chaveAcesso</c>.</para>
        /// </summary>
        public String ChaveAcessoReferenciada { get; set; }

        /// <summary>
        /// <para>Número do item específico no documento referenciado.</para>
        /// <para>Aplicável apenas em Notas de Crédito por Recusa Parcial.
        /// Mapeia para <c>det/DFeReferenciado/nItem</c>.</para>
        /// </summary>
        public int? NumeroItemReferenciado { get; set; }

        /// <summary>
        /// <para>Base de cálculo do IPI</para>
        /// <para>Tag IPITrib/vBC</para>
        /// </summary>
        public double? BaseIpi { get; set; }

        // ---- Reforma Tributária (NT 2026.010, item 4.3) ----

        /// <summary>Código de Classificação Tributária do IBS/CBS (cClassTrib, UB14).</summary>
        public String ClassificacaoTributaria { get; set; }

        /// <summary>Base de cálculo do IBS/CBS (gIBSCBS/vBC, UB16).</summary>
        public double? BaseIbsCbs { get; set; }

        /// <summary>Alíquota do IBS UF impressa: a efetiva (gRed/pAliqEfet) quando houver redução; senão pIBSUF.</summary>
        public double? AliquotaIbsUf { get; set; }

        /// <summary>Valor do IBS UF (vIBSUF, UB35).</summary>
        public double? ValorIbsUf { get; set; }

        /// <summary>Alíquota do IBS Município impressa: a efetiva quando houver redução; senão pIBSMun.</summary>
        public double? AliquotaIbsMun { get; set; }

        /// <summary>Valor do IBS Município (vIBSMun, UB54).</summary>
        public double? ValorIbsMun { get; set; }

        /// <summary>Alíquota da CBS impressa: a efetiva quando houver redução; senão pCBS.</summary>
        public double? AliquotaCbs { get; set; }

        /// <summary>Valor da CBS (vCBS, UB67).</summary>
        public double? ValorCbs { get; set; }

        /// <summary>Base de cálculo do Imposto Seletivo (vBCIS, UB05).</summary>
        public double? BaseIs { get; set; }

        /// <summary>Alíquota do Imposto Seletivo (pIS, UB06).</summary>
        public double? AliquotaIs { get; set; }

        /// <summary>Valor do Imposto Seletivo (vIS, UB11).</summary>
        public double? ValorIs { get; set; }

        /// <summary>O item tem informação da Reforma Tributária para o DANFE (IBS/CBS ou IS).</summary>
        public Boolean PossuiReformaTributaria =>
            !String.IsNullOrWhiteSpace(ClassificacaoTributaria) || BaseIbsCbs.HasValue || BaseIs.HasValue || ValorIs.HasValue;

        /// <summary>
        /// Descrição do item no leiaute da Reforma Tributária: a descrição completa seguida de NCM e
        /// cClassTrib (modelo de referência da NT 2026.010).
        /// </summary>
        public String DescricaoReformaTributaria
        {
            get
            {
                var marcadores = new List<String>();
                if (!String.IsNullOrWhiteSpace(Ncm)) marcadores.Add($"[NCM {Ncm}]");
                if (!String.IsNullOrWhiteSpace(ClassificacaoTributaria)) marcadores.Add($"[cClassTrib {ClassificacaoTributaria}]");

                return marcadores.Count == 0 ? DescricaoCompleta : DescricaoCompleta + "\r\n" + String.Join(" ", marcadores);
            }
        }

        /// <summary>O item tem o grupo ICMS (Nota de Crédito/Débito do RTC pode vir só com IBS/CBS).</summary>
        public Boolean PossuiIcms => !String.IsNullOrEmpty(Cst) || !String.IsNullOrEmpty(Csosn);

        /// <summary>
        /// Linhas das colunas de base, alíquota e valor do leiaute da Reforma Tributária. Só entra o tributo com
        /// informação no XML — a NT 2026.010 (4.4) veda imprimir informação inexistente — e os tributos vão aos
        /// pares por linha, como no modelo de referência ("ICMS…% / CBS…%"), para a nota com muitos itens não
        /// ganhar páginas (item 3 da NT).
        /// </summary>
        public IEnumerable<String> LinhasBases() => EmPares(
            PossuiIcms ? Linha("ICMS", BaseIcms) : null,
            BaseIcmsST > 0 ? Linha("ICMS ST", BaseIcmsST) : null,
            BaseIbsCbs.HasValue ? Linha("IBS/CBS", BaseIbsCbs) : null,
            BaseIs.HasValue ? Linha("IS", BaseIs) : null,
            BaseIpi.HasValue ? Linha("IPI", BaseIpi) : null);

        public IEnumerable<String> LinhasAliquotas() => EmPares(
            PossuiIcms ? Linha("ICMS", AliquotaIcms, "%") : null,
            AliquotaCbs.HasValue ? Linha("CBS", AliquotaCbs, "%") : null,
            AliquotaIbsUf.HasValue ? Linha("IBS UF", AliquotaIbsUf, "%") : null,
            AliquotaIpi.HasValue ? Linha("IPI", AliquotaIpi, "%") : null,
            AliquotaIbsMun.HasValue ? Linha("IBS MUN", AliquotaIbsMun, "%") : null,
            AliquotaIs.HasValue ? Linha("IS", AliquotaIs, "%") : null);

        public IEnumerable<String> LinhasValores() => EmPares(
            PossuiIcms ? Linha("ICMS", ValorIcms) : null,
            ValorCbs.HasValue ? Linha("CBS", ValorCbs) : null,
            ValorIbsUf.HasValue ? Linha("IBS UF", ValorIbsUf) : null,
            ValorIpi.HasValue ? Linha("IPI", ValorIpi) : null,
            ValorIbsMun.HasValue ? Linha("IBS MUN", ValorIbsMun) : null,
            ValorIs.HasValue ? Linha("IS", ValorIs) : null);

        // Ordem do modelo da NT: (ICMS, CBS), (IBS UF, IPI), (IBS MUN, IS); os ausentes saem e os pares se fecham.
        private static IEnumerable<String> EmPares(params String[] itens)
        {
            var presentes = itens.Where(i => i != null).ToList();
            for (var i = 0; i < presentes.Count; i += 2)
                yield return i + 1 < presentes.Count ? presentes[i] + " / " + presentes[i + 1] : presentes[i];
        }

        private static String Linha(String tributo, double? valor, String sufixo = "") => $"{tributo} {valor.Formatar()}{sufixo}";

        public ProdutoViewModel()
        {
            AliquotaIpi = null;
            ValorIpi = null;
        }

        /// <summary>
        /// Calcula o cabeçalho da coluna ICMS na tabela de produtos da DANFE
        /// (<c>"O/CST"</c> ou <c>"O/CSOSN"</c>) a partir do conteúdo dos itens
        /// — abordagem mais robusta que confiar em <c>Emitente.CRT</c>.
        /// Ver <c>openspec/changes/fix-danfe-csosn-rendering/design.md</c> (Decision 1).
        /// </summary>
        public static String CalcularCabecalhoColunaIcms(IEnumerable<ProdutoViewModel> produtos)
        {
            if (produtos == null) return "O/CST";
            Boolean temCst = produtos.Any(p => !String.IsNullOrEmpty(p.Cst));
            Boolean temCsosn = produtos.Any(p => !String.IsNullOrEmpty(p.Csosn));
            return temCst ? "O/CST" : (temCsosn ? "O/CSOSN" : "O/CST");
        }

        public String DescricaoCompleta
        {
            get
            {
                String descriCaoCompleta = Descricao;

                if (!String.IsNullOrWhiteSpace(InformacoesAdicionais))
                {
                    descriCaoCompleta += "\r\n" + InformacoesAdicionais;
                }

                // Para Nota de Crédito por Recusa Parcial: anexa a referência
                // ao item recusado da NF-e original na descrição (sem coluna
                // dedicada, mantendo o layout do DANFE clássico).
                if (!String.IsNullOrWhiteSpace(ChaveAcessoReferenciada))
                {
                    descriCaoCompleta += "\r\nNF-e ref.: " + ChaveAcessoReferenciada;
                    if (NumeroItemReferenciado.HasValue)
                    {
                        descriCaoCompleta += " — item " + NumeroItemReferenciado.Value;
                    }
                }

                return descriCaoCompleta;
            }
        }
    }
}