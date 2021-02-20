using System;

namespace Compilador
{
    public struct Token
    {
        public Token(string valor, TipoToken tipo)
        {
            Valor = valor;
            Tipo = tipo;
        }

        public string Valor { get; set; }
        public TipoToken Tipo { get; set; }
    }
}