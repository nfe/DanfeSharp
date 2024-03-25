using System.ComponentModel;
using System.Xml.Serialization;

namespace DanfeNet;

/// <summary>
/// Bandeira da operadora de cartão de crédito e/ou débito
/// </summary>
public enum BandeiraCartao
{
    [Description("Visa")] [XmlEnum("01")] bcVisa = 01,

    [Description("Mastercard")] [XmlEnum("02")] bcMasterCard = 02,

    [Description("American Express")] [XmlEnum("03")] bcAmericanExpress = 03,

    [Description("Sorocred")] [XmlEnum("04")] bcSorocred = 04,

    [Description("Diners Club")] [XmlEnum("05")] bcDinersClub = 05,

    [Description("Elo")] [XmlEnum("06")] Elo = 06,

    [Description("Hipercard")] [XmlEnum("07")] Hipercard = 07,

    [Description("Aura")] [XmlEnum("08")] Aura = 08,

    [Description("Cabal")] [XmlEnum("09")] Cabal = 09,

    [Description("Alelo")]  [XmlEnum("10")] Alelo = 10,

    [Description("BanesCard")] [XmlEnum("11")] BanesCard = 11,

    [Description("CalCard")] [XmlEnum("12")] CalCard = 12,

    [Description("Credz")] [XmlEnum("13")] Credz = 13,

    [Description("Discover")] [XmlEnum("14")] Discover = 14,

    [Description("GoodCard")] [XmlEnum("15")] GoodCard = 15,

    [Description("GreenCard")] [XmlEnum("16")] GreenCard = 16,

    [Description("Hiper")] [XmlEnum("17")] Hiper = 17,

    [Description("JCB")] [XmlEnum("18")] JCB = 18,

    [Description("Mais")] [XmlEnum("19")] Mais = 19,

    [Description("MaxVan")] [XmlEnum("20")] MaxVan = 20,

    [Description("Policard")] [XmlEnum("21")] Policard = 21,

    [Description("RedeCompras")] [XmlEnum("22")] RedeCompras = 22,

    [Description("Sodexo")] [XmlEnum("23")] Sodexo = 23,

    [Description("ValeCard")] [XmlEnum("24")] ValeCard = 24,

    [Description("Verocheque")] [XmlEnum("25")] Verocheque = 25,

    [Description("VR")] [XmlEnum("26")] VR = 26,

    [Description("Ticket")] [XmlEnum("27")] Ticket = 27,

    [Description("Outros")][XmlEnum("99")] bcOutros = 99,
}