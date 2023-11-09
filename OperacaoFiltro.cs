using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum OperacaoFiltro
    {
        Igual,
        Diferente,
        Contem,
        NaoContem,
        MaiorOuIgual,
        Maior,
        MenorOuIgual,
        Menor,
        ListaContem,
        ListaNaoContem,
    }

    public static class OperacaoFiltroExtensions
    {
        private const string IGUAL = "==";
        private const string DIFERENTE = "!=";
        private const string STRING_CONTEM = "Contains";
        private const string STRING_NAO_CONTEM = "NotContains";
        private const string MAIOR_OU_IGUAL = ">=";
        private const string MAIOR = ">";
        private const string MENOR_OU_IGUAL = "<=";
        private const string MENOR = "<";
        private const string LISTA_CONTEM = "In";
        private const string LISTA_NAO_CONTEM = "NotIn";

        public static OperacaoFiltro FromString(string value)
        {
            if (value.Equals(IGUAL)) return OperacaoFiltro.Igual;
            if (value.Equals(DIFERENTE)) return OperacaoFiltro.Diferente;
            if (value.Equals(STRING_CONTEM)) return OperacaoFiltro.Contem;
            if (value.Equals(STRING_NAO_CONTEM)) return OperacaoFiltro.NaoContem;
            if (value.Equals(MAIOR_OU_IGUAL)) return OperacaoFiltro.MaiorOuIgual;
            if (value.Equals(MAIOR)) return OperacaoFiltro.Maior;
            if (value.Equals(MENOR_OU_IGUAL)) return OperacaoFiltro.MenorOuIgual;
            if (value.Equals(MENOR)) return OperacaoFiltro.Menor;
            if (value.Equals(LISTA_CONTEM)) return OperacaoFiltro.ListaContem;
            if (value.Equals(LISTA_NAO_CONTEM)) return OperacaoFiltro.ListaNaoContem;
            throw new InvalidEnumArgumentException($"Valor inválido para enum OperacaoFiltro: {value}");
        }

        public static string ToString(OperacaoFiltro value)
        {
            if (value == OperacaoFiltro.Diferente) return DIFERENTE;
            if (value == OperacaoFiltro.Contem) return STRING_CONTEM;
            if (value == OperacaoFiltro.NaoContem) return STRING_NAO_CONTEM;
            if (value == OperacaoFiltro.MaiorOuIgual) return MAIOR_OU_IGUAL;
            if (value == OperacaoFiltro.Maior) return MAIOR;
            if (value == OperacaoFiltro.MenorOuIgual) return MENOR_OU_IGUAL;
            if (value == OperacaoFiltro.Menor) return MENOR;
            if (value == OperacaoFiltro.ListaContem) return LISTA_CONTEM;
            if (value == OperacaoFiltro.ListaNaoContem) return LISTA_NAO_CONTEM;
            return IGUAL;
        }
    }
}
