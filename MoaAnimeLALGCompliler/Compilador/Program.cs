using System;
using System.IO;

namespace Compilador
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Inicio\nA eentrada será lida do aquivo Entrada.txt\n");
            var analisadorLexico = new AnalisadorLexico(File.ReadAllText("Entrada.txt"));
            analisadorLexico.Analisar();
            Console.WriteLine("Analise léxica completa...");
            var analisadorSintatico = new AnalisadorSintatico(analisadorLexico.Tokens);
            analisadorSintatico.Analisar();
            Console.WriteLine("Analise sintática completa...");
            Console.WriteLine("Analise semantica completa...");
            Console.WriteLine($"\nTabela de Simbolos:\n");
            foreach (var simboloTs in analisadorSintatico.TabelaDeSimbolos.Simbolos.Values)
            {
                Console.WriteLine(
                    $"Cadeia : {simboloTs.Cadeia} |Escopo: {simboloTs.Escopo} | Linha: {simboloTs.Linha} | Tipo: {simboloTs.Tipo}");
                if (simboloTs.Parametros != null)
                {
                    Console.Write(" |Parametros: ");

                    foreach (var parametro in simboloTs.Parametros)
                    {
                        Console.Write($" Cadea: {parametro.Cadeia} Tipo: {parametro.Tipo}");
                    }
                    Console.Write("\n");
                }
            }

            Console.WriteLine("Código HIpo");
            foreach (var item in analisadorSintatico.GeradorCodigoHipo.AreaDeCodigo)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine("\nAcabou");
        }
    }
}