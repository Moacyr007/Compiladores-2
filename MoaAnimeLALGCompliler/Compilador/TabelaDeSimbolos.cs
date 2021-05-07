using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Compilador
{
    public class TabelaDeSimbolos
    {
        public TabelaDeSimbolos()
        {
            Simbolos = new Dictionary<string, Simbolo>();
        }
        public Dictionary<string, Simbolo> Simbolos { get; set; }


        public Simbolo Find(string cadeia)
        {
            return Simbolos[cadeia];
        }

        public bool TryAdd(Simbolo simbolo)
        {
            if (Simbolos.Any(x => x.Key == simbolo.Cadeia))
            {
                //throw new CompiladorException($"O simbolo {simbolo.Cadeia} já está na tabela de simbolos");
                return false;
            }

            Simbolos.Add(simbolo.Cadeia,simbolo);

            return true;
        }
        
        //TODO
        /*1- determinar se um dado nome está na tabela,
            2- adicionar um novo nome à tabela,
            3- acessar as informações associadas com um dado nome,
            4- adicionar novas informações para um dado nome,
        5- deletar um nome ou um grupo de nomes da tabela. */

    }

    public enum TipoItemTs
    {
        NumeroInteiro,
        NumeroReal,
        Procedimento
    }

    public class Simbolo
    {
        public Simbolo(string cadeia, int escopo, TipoToken tipoToken)
        {
            Cadeia = cadeia;
            Escopo = escopo;
            
            switch(tipoToken)
            {
               
                case TipoToken.NumeroInteiro:
                    break;
                case TipoToken.NumeroReal:
                    break;
               
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(tipoToken), tipoToken, null);
            }
        }
        public string Cadeia { get; set; }
        public int Escopo { get; set; }
        public TipoItemTs Tipo { get; set; }
        
    }
}