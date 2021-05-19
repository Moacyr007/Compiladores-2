using System.Collections.Generic;

namespace Compilador
{
    public class GeradorCodigoHipo
    {
        public InstrucoesMaquinaHipo[] AreaDeCodigo { get; set; }
        public Stack<string> AreaDeDados { get; set; } //Também chamada de pinha D
    }

    public enum InstrucoesMaquinaHipo
    {
        INPP, //inicia programa – será sempre a 1ª instrução}
        PARA, //termina a execução do programa

        CRCT, //carrega constante k no topo da pilha D
        CRVL, //carrega valor de endereço n no topo da pilha D

        SOMA, //soma o elemento antecessor com o topo da pilha; desempilha os dois e empilha o resultado
        SUBT, //subtrai o antecessor pelo elemento do topo
        MULT, //multiplica elemento antecessor pelo elemento do topo
        DIVI, //opcionalmente, podemos ter um DIVR – divisão real

        INVE, //inverte sinal do topo

        CONJ, //AND   |conjunção de valores lógicos
        DISJ, //OR    |disjunção de valores lógicos
        NEGA, //NOT   |negação lógica
        CPME, // <    |comparação de menor entre o antecessor e o topo
        CPMA, // >    |comparação de maior
        CPIG, // =    |comparação de igualdade
        CDES, // !=   |comparação de desigualdade
        CPMI, // <=   |comparação menor-igual
        CMAI, // >=   |comparação maior-igual

        ARMZ, //armazena o topo da pilha no endereço n de D

        DSVI, //desvio incondicional para a instrução de endereço p

        DSVF /*desvio condicional para a instrução de endereço p; 
                - o desvio será executado caso a condição resultante seja falsa; 
                - o valor da condição estará no topo*/
    }
}