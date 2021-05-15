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


        public Simbolo Find(string key)
        {
            return Simbolos[key];
        }

        public void TryAddNovaVariavel(Simbolo simbolo)
        {
            //Verifica se a variaveil já está na tabela de simbolos
            if (Simbolos.Any(x => x.Key == simbolo.Cadeia && x.Value.Escopo == simbolo.Escopo))
            {
                throw new CompiladorException($"A variável {simbolo.Cadeia} já foi declarada\nLinha: {simbolo.Linha}");
            }

            Simbolos.Add(simbolo.Escopo+":"+simbolo.Cadeia,simbolo);
        }


        public void SetTipoVariaveis(TipoItemTs tipoItemTs)
        {
            foreach (var simboloTs in Simbolos)
            {
                if (simboloTs.Value.Tipo == TipoItemTs.Desconhecido)
                {
                    simboloTs.Value.Tipo = tipoItemTs;
                }
            }

        }

        public void VerificarSeVariavelJaFoiDeclarada(Simbolo simbolo)
        {
            if (Simbolos.All(x => x.Key != simbolo.Escopo+":"+simbolo.Cadeia))
            {
                throw new CompiladorException($"A variável {simbolo.Cadeia} não foi declarada\nLinha: {simbolo.Linha}");
            }
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
        Desconhecido,
        NumeroInteiro,
        NumeroReal,
        Procedimento
    }

    public class Simbolo
    {
        /*public Simbolo(string cadeia, int escopo, TipoItemTs tipoItemTs)
        {
            Cadeia = cadeia;
            Escopo = escopo;
            Tipo = tipoItemTs;
        }*/
        public string Cadeia { get; set; }
        public string Escopo { get; set; }
        public TipoItemTs Tipo { get; set; }
        
        public int Linha { get; set; }

        
    }
}