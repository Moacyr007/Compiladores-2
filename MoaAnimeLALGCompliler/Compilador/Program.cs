using System;
using System.IO;

namespace Compilador
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Inicio");
            var analisadorLexico = new AnalisadorLexico(File.ReadAllText("Entrada.txt"));
            analisadorLexico.Analisar();
            var analisadorSintatico = new AnalisadorSintatico(analisadorLexico.Tokens);
            analisadorSintatico.Analisar();
            Console.WriteLine("Acabou");
        }
    }
}