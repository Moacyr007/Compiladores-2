using System;

namespace Compilador
{
    public class AnalisadorLexicoException : Exception
    {
        public AnalisadorLexicoException()
        {
        }

        public AnalisadorLexicoException(char simbolo, int linha) : base(
            string.Format("Simbolo inválido : {0} \n Linha: {1}", simbolo, linha))
        {
        }

        public AnalisadorLexicoException(string message) : base(message)
        {
        }

        public AnalisadorLexicoException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}