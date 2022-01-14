using System;
using System.Collections.Generic;
using System.Text;

namespace DanfeSharp.Atributos
{
    public enum TipoCampo
    {
        Str = 0,
        Int = 1,
        Long = 17,
        Dat = 2,
        DatHor = 3,
        DatHorTz = 4,
        StrNumber = 5,
        StrNumberFill = 6,
        De2 = 7,
        De3 = 8,
        De4 = 9,
        De10 = 10,
        Hor = 11,
        De6 = 12,
        DatCFe = 13,
        HorCFe = 14,
        Enum = 15,
        Custom = 16
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DFeAttributeAttribute : DFeBaseAttribute
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DFeElementAttribute" /> class.
        /// </summary>
        public DFeAttributeAttribute() { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DFeElementAttribute" /> class.
        /// </summary>
        /// <param name="name">The Name.</param>
        public DFeAttributeAttribute(string name) : this()
        {
            Name = name;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DFeElementAttribute" /> class.
        /// </summary>
        /// <param name="tipo">The tipo.</param>
        /// <param name="name">The name.</param>
        public DFeAttributeAttribute(TipoCampo tipo, string name) : this()
        {
            Tipo = tipo;
            Name = name;
        }

        #endregion Constructors
    }

    public abstract class DFeBaseAttribute : Attribute
    {
        #region Constructors

        protected DFeBaseAttribute()
        {
            Tipo = TipoCampo.Str;
            Id = "";
            Name = string.Empty;
            Min = 0;
            Max = 0;
            Ocorrencia = 0;
            Ordem = 0;
            Descricao = string.Empty;
        }

        #endregion Constructors

        #region Properties

        public TipoCampo Tipo { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Descricao { get; set; }

        public int Ordem { get; set; }

        public int Max { get; set; }

        public int Min { get; set; }

        public Ocorrencia Ocorrencia { get; set; }

        #endregion Properties
    }

    public enum Ocorrencia
    {
        NaoObrigatoria,
        Obrigatoria,
        MaiorQueZero
    }
}
