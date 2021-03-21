namespace Compilador
{
    public enum EstadoAnalisadorLexico
    {
        Desconhecido = 0,
        Inicial,
        Invalido,
        Identificador,
        Numero,
        Simbolo
    }
}