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
            Simbolos = new Dictionary<string, ItemTs>();
        }
        public Dictionary<string, ItemTs> Simbolos { get; set; }


        public ItemTs Find(string cadeia, string escopo, bool? isProcedure= null)
        {
            try
            {
                if (isProcedure.HasValue)
                {
                    var simbolo = Simbolos[escopo+":"+cadeia];

                    if (isProcedure.Value && simbolo.Tipo == TipoItemTs.Procedimento)
                        return simbolo;

                    if (!isProcedure.Value && simbolo.Tipo != TipoItemTs.Procedimento)
                        return simbolo;

                    return null;
                }
                return Simbolos[escopo+":"+cadeia];
            }
            catch (KeyNotFoundException e)
            {
                return null;
            }
        }

        public void TryAddNewItem(ItemTs itemTs)
        {
            //Verifica se a variaveil já está na tabela de simbolos
            if (Simbolos.Any(x => x.Key == itemTs.Cadeia && x.Value.Escopo == itemTs.Escopo))
            {
                throw new CompiladorException($"A variável {itemTs.Cadeia} já foi declarada\nLinha: {itemTs.Linha}");
            }

            Simbolos.Add(itemTs.Escopo+":"+itemTs.Cadeia,itemTs);
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

        public void VerificarSeVariavelJaFoiDeclarada(ItemTs itemTs)
        {
            if (Simbolos.All(x => x.Key != itemTs.Escopo+":"+itemTs.Cadeia))
            {
                throw new CompiladorException($"A variável {itemTs.Cadeia} não foi declarada\nLinha: {itemTs.Linha}");
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
    
    public enum TipoParametro
    {
        Desconhecido,
        NumeroInteiro,
        NumeroReal,
    }

    public class ItemTs
    {
        public string Cadeia { get; set; }
        public string Escopo { get; set; }
        public TipoItemTs Tipo { get; set; }
        public int Linha { get; set; }

        public List<Parametro> Parametros { get; set; }
    }
    
    public class Parametro
    {
        public string Cadeia { get; set; }
        public TipoParametro Tipo { get; set; }
    }
}