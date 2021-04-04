using System;

namespace Compilador
{
    public struct Token
    {
        public Token(string valor, TipoToken tipo, int linha)
        {
            Valor = valor;
            Tipo = tipo;
            Linha = linha;
        }

        public string Valor { get; set; }
        public int Linha { get; set; }
        public TipoToken Tipo { get; set; }
    }
}