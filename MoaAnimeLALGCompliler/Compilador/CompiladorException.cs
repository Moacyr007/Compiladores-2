using System;

namespace Compilador
{
    public class CompiladorException : Exception
    {
        public CompiladorException()
        {
        }

        public CompiladorException(string simbolo, int linha, string etapa) : base(
            string.Format("{2}Simbolo inválido : {0} \n Linha: {1}", simbolo, linha, etapa))
        {
        }

        public CompiladorException(string message) : base(message)
        {
        }

        public CompiladorException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}