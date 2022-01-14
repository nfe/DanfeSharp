using System;
using System.Collections.Generic;
using System.Text;

namespace DanfeSharp.Atributos
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DFeRootAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DFeRootAttribute" /> class.
        /// </summary>
        public DFeRootAttribute()
        {
            Name = string.Empty;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DFeRootAttribute" /> class.
        /// </summary>
        /// <param name="root">The Namespace.</param>
        public DFeRootAttribute(string root)
        {
            Name = root;
        }

        #endregion Constructors

        #region Properties

        public string Name { get; set; }

        public string Namespace { get; set; }

        #endregion Properties
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DFeIgnoreAttribute : Attribute { }
}
