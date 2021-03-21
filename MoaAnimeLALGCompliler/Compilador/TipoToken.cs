namespace Compilador
{
    public enum TipoToken
    {
        //T = {ident, numero_int, numero_real, (, ), *, /, +, -, <>, >=, >, <, if, then, $, while, do,
        //write, read, ;, else, begin, end, :, , , }

        Desconhecido = 0,
        
        Identificador = 1,
        AbreParenteses = 4,
        FechaParenteses = 5,
        OperadorMultiplicacao = 6,
        OperadorDivisao = 7,
        OperadorSoma = 8,
        OperadorSubtracao = 9,
        OperadorComparacao = 10,
        ReservadoIf = 11,
        ReservadoElse = 12,
        NumeroInteiro = 13,
        NumeroReal = 14,
        ReservadoThen = 15,
        ReservadoWhile = 16,
        ReservadoDo = 17,
        ReservadoNumeroInt = 18,
        ReservadoNumeroReal = 19,
        SimboloCifrao = 20,
        DeclaracaoIdentificador = 21
    }
}