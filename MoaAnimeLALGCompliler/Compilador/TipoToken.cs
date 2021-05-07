namespace Compilador
{
    public enum TipoToken
    {
        Desconhecido = 0,
        
        Identificador = 1,
        SimboloAbreParenteses = 4,
        SimboloFechaParenteses = 5,
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
        ReservadoIdent = 21,
        ReservadoProgram = 22,
        ReservadoBegin = 23,
        ReservadoEnd = 23,
        ReservadoVar = 24,
        ReservadoProcedure = 25,
        ReservadoReal = 26,
        ReservadoInteger = 27,
        SimboloVirgula = 28,
        SimboloPontoEVirgula = 29,
        SimboloDoisPontos = 30,
        OperadorIgual = 31,
        ReservadoRead = 32,
        ReservadoWrite = 33,
        SimboloAtribuicao = 34,
        SimboloAbreChaves = 35,
        SimboloFechaChaves = 36,
        SimboloPonto = 37
    }
}