using System;
using System.Collections;
using System.Collections.Generic;

namespace Compilador
{
    public class AnalisadorLexico
    {
        public string Entrada { get; set; }
        public List<Token> Tokens { get; set; }
        public AnalisadorLexico(string entrada)
        {
            Entrada = entrada;
            Tokens = new List<Token>();
        }

        //Glalg = {N,T,P,S} 
        //T = {ident, numero_int, numero_real, (, ), *, /, +, -, <>, >=, >, <, if, then, $, while, do,
        //write, read, ;, else, begin, end, :, , , }
       
        public List<Token> Analisar()
        {
            if (Entrada == string.Empty)
            {
                return Tokens;
            }

            var posicao = 0;
            
            /*while (posicao <= Entrada.Length)
            {
                
            }*/
            return Tokens;
        }
    }
}