﻿using DanfeSharp.Esquemas.NFe;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DanfeSharp.Modelo
{
    /// <summary>
    /// Modelo de dados utilizado para o DANFE.
    /// </summary>
    public class DanfeViewModel
    {
        private int _QuantidadeCanhoto;

        /// <summary>
        /// Quantidade de canhotos a serem impressos.
        /// </summary>
        public int QuantidadeCanhotos
        {
            get => _QuantidadeCanhoto;
            set
            {
                if (value >= 0 && value <= 2)
                    _QuantidadeCanhoto = value;
                else
                    throw new ArgumentOutOfRangeException("A quantidade de canhotos deve ser de 0 a 2.");
            }
        }

        private float _Margem;

        /// <summary>
        /// Magens horizontais e verticais do DANFE.
        /// </summary>
        public float Margem
        {
            get => _Margem;
            set
            {
                if (value >= 2 && value <= 5)
                    _Margem = value;
                else
                    throw new ArgumentOutOfRangeException("A margem deve ser entre 2 e 5.");
            }
        }

        /// <summary>
        ///  Grupo de Formas de Pagamento (pag)
        /// </summary>
        public List<PagamentoViewModel> Pagamento { get; set; }

        /// <summary>
        /// QrCode NFC-e
        /// </summary>
        public string QrCode { get; set; }

        /// <summary>
        /// Identificação da Danfe NFC-e
        /// </summary>
        public string EndConsulta { get; set; }

        /// <summary>
        /// <para>Número do Documento Fiscal</para>
        /// <para>Tag nNF</para>
        /// </summary>
        public int NfNumero { get; set; }

        /// <summary>
        /// <para>Série do Documento Fiscal</para>
        /// <para>Tag serie</para>
        /// </summary>
        public int NfSerie { get; set; }

        public Orientacao Orientacao { get; set; }

        /// <summary>
        /// Chave de Acesso
        /// </summary>
        public String ChaveAcesso { get; set; }

        /// <summary>
        /// <para>Descrição da Natureza da Operação</para>
        /// <para>Tag natOp</para>
        /// </summary>
        public String NaturezaOperacao { get; set; }

        /// <summary>
        /// <para>Informações Complementares de interesse do Contribuinte</para>
        /// <para>Tag infCpl</para>
        /// </summary>
        public String InformacoesComplementares { get; set; }

        /// <summary>
        /// <para>Informações adicionais de interesse do Fisco</para>
        /// <para>Tag infAdFisco</para>
        /// </summary>
        public String InformacoesAdicionaisFisco { get; set; }

        /// <summary>
        /// <para>Data e Hora de emissão do Documento Fiscal</para>
        /// <para>Tag dhEmi ou dEmi</para>
        /// </summary>
        public DateTime? DataHoraEmissao { get; set; }

        /// <summary>
        /// <para>Data de Saída ou da Entrada da Mercadoria/Produto</para>
        /// <para>Tag dSaiEnt e dhSaiEnt</para>
        /// </summary>
        public DateTime? DataSaidaEntrada { get; set; }

        /// <summary>
        /// <para>Hora de Saída ou da Entrada da Mercadoria/Produto</para>
        /// <para>Tag dSaiEnt e hSaiEnt</para>
        /// </summary>
        public TimeSpan? HoraSaidaEntrada { get; set; }

        /// <summary>
        /// Dados do Emitente
        /// </summary>
        public EmpresaViewModel Emitente { get; set; }

        /// <summary>
        /// Dados do Destinatário
        /// </summary>
        public EmpresaViewModel Destinatario { get; set; }

        /// <summary>
        /// <para>Tipo de Emissão da NF-e
        /// 1=Emissão normal (não em contingência);
        /// 2=Contingência FS-IA, com impressão do DANFE em formulário de segurança;
        /// 3=Contingência SCAN(Sistema de Contingência do Ambiente Nacional);
        /// 4=Contingência DPEC(Declaração Prévia da Emissão em Contingência);
        /// 5=Contingência FS-DA, com impressão do DANFE em formulário de segurança;
        /// 6=Contingência SVC-AN(SEFAZ Virtual de Contingência do AN);
        /// 7=Contingência SVC-RS(SEFAZ Virtual de Contingência do RS);
        /// 9=Contingência off-line da NFC-e (as demais opções de contingência são válidas também para a NFC-e).
        // Para a NFC-e somente estão disponíveis e são válidas as opções de contingência 5 e 9.
        /// <para>Tag tpNF</para>
        /// </summary>
        public FormaEmissao TipoEmissao { get; set; }

        /// <summary>
        /// <para>Tipo Emissao
        /// <para>Tag tpNF</para>
        /// </summary>
        public int TipoNF { get; set; }

        /// <summary>
        /// Numero do protocolo com sua data e hora
        /// </summary>
        public String ProtocoloAutorizacao { get; set; }

        /// <summary>
        /// Faturas da Nota Fiscal
        /// </summary>
        public List<DuplicataViewModel> Duplicatas { get; set; }

        /// <summary>
        /// Dados da Transportadora
        /// </summary>
        public TransportadoraViewModel Transportadora { get; set; }

        /// <summary>
        /// View Model do bloco Cálculo do Imposto
        /// </summary>
        public CalculoImpostoViewModel CalculoImposto { get; set; }

        /// <summary>
        /// Produtos da Nota Fiscal
        /// </summary>
        public List<ProdutoViewModel> Produtos { get; set; }

        /// <summary>
        /// View Model do Bloco Cálculo do Issqn
        /// </summary>
        public CalculoIssqnViewModel CalculoIssqn { get; set; }

        /// <summary>
        /// Tipo de Ambiente
        /// </summary>
        public int TipoAmbiente { get; set; }

        /// <summary>
        /// Código do status da resposta, cStat, do elemento infProt.
        /// </summary>
        public int? CodigoStatusReposta { get; set; }

        /// <summary>
        /// Descrição do status da resposta, xMotivo, do elemento infProt.
        /// </summary>
        public String DescricaoStatusReposta { get; set; }

        /// <summary>
        /// Informações de Notas Fiscais referenciadas que serão levadas no texto adicional.
        /// </summary>
        public List<String> NotasFiscaisReferenciadas { get; set; }

        #region Local Retirada e Entrega

        public LocalEntregaRetiradaViewModel LocalRetirada { get; set; }

        public LocalEntregaRetiradaViewModel LocalEntrega { get; set; }

        #endregion

        #region Informações adicionais de compra

        /// <summary>
        /// Tag xNEmp
        /// </summary>
        public String NotaEmpenho { get; set; }

        /// <summary>
        /// Tag xPed
        /// </summary>
        public String Pedido { get; set; }

        /// <summary>
        /// Tag xCont
        /// </summary>
        public String Contrato { get; set; }

        #endregion

        #region Opções de exibição

        /// <summary>
        /// Exibi os valores do ICMS Interestadual e Valor Total dos Impostos no bloco Cálculos do Imposto.
        /// </summary>
        public bool ExibirIcmsInterestadual { get; set; } = true;

        /// <summary>
        /// Exibi os valores do PIS e COFINS no bloco Cálculos do Imposto.
        /// </summary>
        public bool ExibirPisConfins { get; set; } = true;

        /// <summary>
        /// Exibi o bloco "Informações do local de entrega" quando o elemento "entrega" estiver disponível.
        /// </summary>
        public bool ExibirBlocoLocalEntrega { get; set; } = true;

        /// <summary>
        /// Exibi o bloco "Informações do local de retirada" quando o elemento "retirada" estiver disponível.
        /// </summary>
        public bool ExibirBlocoLocalRetirada { get; set; } = true;

        
        /// <summary>
        /// Exibe o Nome Fantasia, caso disponível, ao invés da Razão Social no quadro identificação do emitente.
        /// </summary>
        public bool PreferirEmitenteNomeFantasia { get; set; } = true;


        #endregion

        #region Contingencia
        /// <summary>
        /// <para>Data e Hora da entrada em contingência - dhCont
        /// </summary>
        public DateTimeOffset? ContingenciaDataHora { get; set; }

        /// <summary>
        /// <para> Justificativa da entrada em contingência - xJust
        /// </summary>
        public string ContingenciaJustificativa { get; set; }
        #endregion

        public DanfeViewModel()
        {
            QuantidadeCanhotos = 1;
            Margem = 4;
            Orientacao = Orientacao.Retrato;
            CalculoImposto = new CalculoImpostoViewModel();
            Emitente = new EmpresaViewModel();
            Destinatario = new EmpresaViewModel();
            Duplicatas = new List<DuplicataViewModel>();
            Produtos = new List<ProdutoViewModel>();
            Transportadora = new TransportadoraViewModel();
            CalculoIssqn = new CalculoIssqnViewModel();
            Pagamento = new List<PagamentoViewModel>();
            NotasFiscaisReferenciadas = new List<string>();
        }

        public Boolean MostrarCalculoIssqn { get; set; }

        /// <summary>
        /// Substitui o ponto e vírgula (;) por uma quebra de linha.
        /// </summary>
        private String BreakLines(String str)
        {
            return str == null ? String.Empty : str.Replace(';', '\n');
        }

        public static DanfeViewModel CreateFromXmlString(String xml)
        {
            return DanfeViewModelCreator.CreateFromXmlString(xml);
        }

        public virtual String TextoRecebimento
        {
            get
            {
                return $"Recebemos de {Emitente.RazaoSocial} os produtos e/ou serviços constantes na Nota Fiscal Eletrônica indicada {(Orientacao == Orientacao.Retrato ? "abaixo" : "ao lado")}. Emissão: {DataHoraEmissao.Formatar()} Valor Total: R$ {CalculoImposto.ValorTotalNota.Formatar()} Destinatário: {Destinatario.RazaoSocial}";
            }
        }

        public virtual String TextoReservadoFisco()
        {
            if (TipoEmissao is FormaEmissao.Normal)
            {
                return string.Empty;
            }

            var contingencyType = TipoEmissao switch             {
                FormaEmissao.ContingenciaDPEC => "DPEC",
                FormaEmissao.ContingenciaFSDA => "FSDA",
                FormaEmissao.ContingenciaFS => "FSIA",
                FormaEmissao.ContingenciaSVCAN => "SVC-AN",
                FormaEmissao.ContingenciaSVCRS => "SVC-RS",
                FormaEmissao.ContingenciaSCAN => "SCAN",
                _ => throw new NotImplementedException()
            };

            var contingencyDateTime = ContingenciaDataHora?.ToString("yyyy-MM-ddTHH:mm:sszzz");

            return $"""
                CONTINGÊNCIA {contingencyType}
                Início: {contingencyDateTime}
                Justificativa: {ContingenciaJustificativa}
                """;
        }

        public virtual String TextoAdicional()
        {
            StringBuilder sb = new StringBuilder();

            if (!String.IsNullOrEmpty(InformacoesComplementares))
            {
                sb.Append("Inf. Contribuinte: ");
                sb.Append(InformacoesComplementares);
                sb.Replace(";", "\r\n").Replace("||", "\r\n");
            }
                


            if (!String.IsNullOrEmpty(Destinatario.Email))
            {
                // Adiciona um espaço após a virgula caso necessário, isso facilita a quebra de linha.
                var destEmail = Regex.Replace(Destinatario.Email, @"(?<=\S)([,;])(?=\S)", "$1 ").Trim(new char[] { ' ', ',', ';' });
                sb.AppendChaveValor("Email do Destinatário", destEmail);
            }

            if (!String.IsNullOrEmpty(InformacoesAdicionaisFisco))
                sb.AppendChaveValor("Inf. fisco", InformacoesAdicionaisFisco);

            if (!String.IsNullOrEmpty(Pedido) && !Utils.StringContemChaveValor(InformacoesComplementares, "Pedido", Pedido))
                sb.AppendChaveValor("Pedido", Pedido);

            if (!String.IsNullOrEmpty(Contrato) && !Utils.StringContemChaveValor(InformacoesComplementares, "Contrato", Contrato))
                sb.AppendChaveValor("Contrato", Contrato);

            if (!String.IsNullOrEmpty(NotaEmpenho))
                sb.AppendChaveValor("Nota de Empenho", NotaEmpenho);

            foreach (var nfref in NotasFiscaisReferenciadas)
            {
                if (sb.Length > 0) sb.Append(" ");
                sb.Append(nfref);
            }

            #region NT 2013.003 Lei da Transparência

            if (CalculoImposto.ValorAproximadoTributos.HasValue && (String.IsNullOrEmpty(InformacoesComplementares) ||
               !Regex.IsMatch(InformacoesComplementares, @"((valor|vlr?\.?)\s+(aprox\.?|aproximado)\s+(dos\s+)?(trib\.?|tributos))|((trib\.?|tributos)\s+(aprox\.?|aproximado))", RegexOptions.IgnoreCase)))
            {
                if (sb.Length > 0) sb.Append("\r\n");
                sb.Append("Valor Aproximado dos Tributos: ");
                sb.Append(CalculoImposto.ValorAproximadoTributos.FormatarMoeda());
            }

            #endregion

            return sb.ToString();
        }

        public Boolean IsRetrato => Orientacao == Orientacao.Retrato;
        public Boolean IsPaisagem => Orientacao == Orientacao.Paisagem;
    }
}