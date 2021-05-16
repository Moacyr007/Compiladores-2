using System;
using System.IO;

namespace Compilador
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Inicio\nA eentrada será lida do aquivo Entrada.txt");
            var analisadorLexico = new AnalisadorLexico(File.ReadAllText("Entrada.txt"));
            analisadorLexico.Analisar();
            Console.WriteLine("Analise léxica completa...");
            var analisadorSintatico = new AnalisadorSintatico(analisadorLexico.Tokens);
            analisadorSintatico.Analisar();
            Console.WriteLine("Analise sintática completa...");
            Console.WriteLine("Analise semantica completa...");
            Console.WriteLine("Acabou");
        }
    }
}