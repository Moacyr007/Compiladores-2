using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Compilador
{
    public class AnalisadorLexico
    {
        //palavraReservada = ('var', 'integer', 'real', 'if', 'then', 'while', 'do', 'write', 'read', 'else', 'begin', 'end', 'program', 'procedure')
        //simbolo = ('*','+', '-', '=', '<', '>', '(', ')', ',', ':', ';', '$', ':=', '<>', '<=', '>=', '/')
        //comentario = ('/', '*', '{', '}')

        public string Entrada { get; set; }
        public List<Token> Tokens { get; set; }

        public AnalisadorLexico(string entrada)
        {
            Entrada = entrada;
            Tokens = new List<Token>();
        }

        public Dictionary<string, TipoToken> IdentificadoresReservados = new Dictionary<string, TipoToken>()
        {
            {"if", TipoToken.ReservadoIf},
            {"else", TipoToken.ReservadoElse},
            {"then", TipoToken.ReservadoThen},
            {"while", TipoToken.ReservadoWhile},
            {"do", TipoToken.ReservadoDo},
            {"numero_int", TipoToken.ReservadoNumeroInt},
            {"numero_real", TipoToken.ReservadoNumeroReal},
            {"ident", TipoToken.ReservadoIdent},
            {"program", TipoToken.ReservadoProgram},
            {"begin", TipoToken.ReservadoBegin},
            {"end", TipoToken.ReservadoEnd},
            {"var", TipoToken.ReservadoVar},
            {"procedure", TipoToken.ReservadoProcedure},
            {"real", TipoToken.ReservadoReal},
            {"integer", TipoToken.ReservadoInteger},
            {"read", TipoToken.ReservadoRead},
            {"white", TipoToken.ReservadoWrite},
        };

        public Dictionary<string, TipoToken> Operadores = new Dictionary<string, TipoToken>()
        {
            {"*", TipoToken.OperadorMultiplicacao},
            {"/", TipoToken.OperadorDivisao},
            {"+", TipoToken.OperadorSoma},
            {"-", TipoToken.OperadorSubtracao},
            {">", TipoToken.OperadorComparacao},
            {"<", TipoToken.OperadorComparacao},
            {"<>", TipoToken.OperadorComparacao},
            {">=", TipoToken.OperadorComparacao},
            {"=", TipoToken.OperadorIgual},
        };

        public Dictionary<string, TipoToken> Simbolos = new Dictionary<string, TipoToken>()
        {
            {"$", TipoToken.SimboloCifrao},
            {"(", TipoToken.SimboloAbreParenteses},
            {")", TipoToken.SimboloFechaParenteses},
            {",", TipoToken.SimboloVirgula},
            {";", TipoToken.SimboloPontoEVirgula},
            {":", TipoToken.SimboloDoisPontos},
            {":=", TipoToken.SimboloAtribuicao},
        };

        //Glalg = {N,T,P,S} 
        //write, read, ;, else, begin, end, :, , , }
        // {"ident", "numero_int", "numero_real", "(", ")", "*", "/", "+", "-",  "<", "if", "then", "$", "while", "do",
        public List<Token> Analisar()
        {
            if (Entrada == string.Empty)
                return Tokens;
            
            //var tokenArray = Entrada.Split(new[] {',' , '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var tokenList = SepararTokens(Entrada);

            foreach (var token in tokenList)
            {
                if (IdentificadoresReservados.ContainsKey(token.Valor))
                {
                    Tokens.Add(new Token(
                        token.Valor,
                        IdentificadoresReservados[token.Valor], 
                        token.Linha));
                }

                else if (Simbolos.ContainsKey(token.Valor))
                {
                    Tokens.Add(new Token(
                        token.Valor,
                        IdentificadoresReservados[token.Valor],
                        token.Linha));
                }

                else if (Operadores.ContainsKey(token.Valor))
                {
                    Tokens.Add(new Token(
                        token.Valor,
                        Operadores[token.Valor],
                        token.Linha));
                }

                else if (int.TryParse(token.Valor, out _))
                {
                    Tokens.Add(new Token(
                        token.Valor,
                        TipoToken.NumeroInteiro,
                        token.Linha));
                }

                else if (float.TryParse(token.Valor, out _))
                {
                    Tokens.Add(new Token(
                        token.Valor,
                        TipoToken.NumeroInteiro,
                        token.Linha));
                }

                else if (char.IsLetter(token.Valor[0]))
                {
                    var flagIdentificadorEhInvalido = false;
                    foreach (var caractere in token.Valor)
                    {
                        if (!char.IsNumber(caractere) && !char.IsLetter(caractere))
                        {
                            flagIdentificadorEhInvalido = true;
                            break;
                        }
                    }

                    if (flagIdentificadorEhInvalido)
                    {
                        throw new CompiladorException($"Identificador inválido: {token.Valor} \n Linha: {token.Linha}");
                    }

                    Tokens.Add(new Token(
                        token.Valor,
                        TipoToken.Identificador,
                        token.Linha));
                }

                else
                {
                    throw new CompiladorException($"Identificador inválido: {token.Valor} \n Linha: {token.Linha}");
                }
            }

            return Tokens;
        }

        //Retorna uma lista com a entrada separada por ' ', semelhante a string.split mas também separa por '\n' incluindo o '\n' na lista
        public static List<Token> SepararTokens(string entrada)
        {
            List<Token> tokenList = new List<Token>();
            var tamanhoEntrada = entrada.Length;
            int linha = 1;
            string tokenAtualValor = "";
            
            for (int i = 0; i < tamanhoEntrada; i++)
            {
                if (entrada[i] == ' ')
                {
                    if(!string.IsNullOrEmpty(tokenAtualValor))
                        tokenList.Add(new Token(tokenAtualValor, TipoToken.Desconhecido, linha));
                    tokenAtualValor = "";
                    continue;
                }
                if (entrada[i] == '\n')
                {
                    if(!string.IsNullOrEmpty(tokenAtualValor))
                        tokenList.Add(new Token(tokenAtualValor, TipoToken.Desconhecido, linha));

                    linha++;
                    tokenAtualValor = "";
                    
                    continue;
                }
                tokenAtualValor += entrada[i];
            }
            
            if(!string.IsNullOrEmpty(tokenAtualValor))
                tokenList.Add(new Token(tokenAtualValor, TipoToken.Desconhecido, linha));

            return tokenList;

        }
    }
}