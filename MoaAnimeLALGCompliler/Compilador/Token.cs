using System;

namespace Compilador
{
    public struct Token
    {
        public Token(string cadeia, TipoToken tipo, int linha)
        {
            Cadeia = cadeia;
            Tipo = tipo;
            Linha = linha;
        }

        public string Cadeia { get; set; }
        public int Linha { get; set; }
        public TipoToken Tipo { get; set; }
    }
}