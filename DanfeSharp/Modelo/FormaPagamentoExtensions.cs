using System;
using System.ComponentModel;
using System.Reflection;
using DanfeSharp.Esquemas.NFe;

namespace DanfeSharp.Modelo
{
    /// <summary>
    /// Helpers para o enum <see cref="FormaPagamento"/>.
    /// </summary>
    /// <remarks>
    /// Referência: openspec/specs/danfe-payment-block/spec.md (Requirement
    /// "Helper FormaPagamento.GetDescricao() reusable across the project").
    /// </remarks>
    public static class FormaPagamentoExtensions
    {
        /// <summary>
        /// Retorna a descrição legível do valor (lida do <see cref="DescriptionAttribute"/>
        /// declarado no enum). Para valor não definido no enum, retorna string vazia
        /// — nunca null nem exceção, conforme spec.
        /// </summary>
        public static string GetDescricao(this FormaPagamento formaPagamento)
        {
            Type type = typeof(FormaPagamento);
            string name = Enum.GetName(type, formaPagamento);
            if (string.IsNullOrEmpty(name)) return string.Empty;

            FieldInfo field = type.GetField(name);
            if (field == null) return string.Empty;

            DescriptionAttribute attr = field.GetCustomAttribute<DescriptionAttribute>();
            return attr?.Description ?? string.Empty;
        }
    }
}
