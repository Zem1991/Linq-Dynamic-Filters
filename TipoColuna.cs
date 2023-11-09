using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum TipoColuna
    {
        String,
        Boolean,
        PickList,
        Number,
    }

    public static class TipoColunaExtensions
    {
        private const string STRING = "String";
        private const string BOOLEAN = "Boolean";
        private const string PICKLIST = "PickList";
        private const string NUMBER = "Number";

        public static TipoColuna FromString(string value)
        {
            if (value.Equals(STRING)) return TipoColuna.String;
            if (value.Equals(BOOLEAN)) return TipoColuna.Boolean;
            if (value.Equals(PICKLIST)) return TipoColuna.PickList;
            if (value.Equals(NUMBER)) return TipoColuna.Number;
            throw new InvalidEnumArgumentException($"Valor inválido para enum TipoColuna: {value}");
        }

        public static string ToString(TipoColuna value)
        {
            if (value == TipoColuna.Boolean) return BOOLEAN;
            if (value == TipoColuna.PickList) return PICKLIST;
            if (value == TipoColuna.Number) return NUMBER;
            return STRING;
        }
    }
}
