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
            {"ident", TipoToken.DeclaracaoIdentificador}
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
        };

        public Dictionary<string, TipoToken> Simbolos = new Dictionary<string, TipoToken>()
        {
            {"$", TipoToken.SimboloCifrao},
            {"(", TipoToken.AbreParenteses},
            {")", TipoToken.FechaParenteses},
        };

        //Glalg = {N,T,P,S} 
        //write, read, ;, else, begin, end, :, , , }
        // {"ident", "numero_int", "numero_real", "(", ")", "*", "/", "+", "-",  "<", "if", "then", "$", "while", "do",
        public List<Token> Analisar()
        {
            if (Entrada == string.Empty)
                return Tokens;
            
            //var tokenArray = Entrada.Split(new[] {',' , '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var tokenList = moaSplit(Entrada);
            var linha = 1;

            foreach (var token in tokenList)
            {
                if (token == "\n")
                    linha++;
                else if (IdentificadoresReservados.ContainsKey(token))
                {
                    Tokens.Add(new Token(
                        token,
                        IdentificadoresReservados[token]));
                }

                else if (Simbolos.ContainsKey(token))
                {
                    Tokens.Add(new Token(
                        token,
                        IdentificadoresReservados[token]));
                }

                else if (Operadores.ContainsKey(token))
                {
                    Tokens.Add(new Token(
                        token,
                        Operadores[token]));
                }

                else if (int.TryParse(token, out _))
                {
                    Tokens.Add(new Token(
                        token,
                        TipoToken.NumeroInteiro));
                }

                else if (float.TryParse(token, out _))
                {
                    Tokens.Add(new Token(
                        token,
                        TipoToken.NumeroInteiro));
                }

                else if (char.IsLetter(token[0]))
                {
                    var flagIdentificadorEhInvalido = false;
                    foreach (var caractere in token)
                    {
                        if (!char.IsNumber(caractere) && !char.IsLetter(caractere))
                        {
                            flagIdentificadorEhInvalido = true;
                            break;
                        }
                    }

                    if (flagIdentificadorEhInvalido)
                    {
                        throw new AnalisadorLexicoException($"Identificador inválido: {token} \n Linha: {linha}");
                    }

                    Tokens.Add(new Token(
                        token,
                        TipoToken.Identificador));
                }

                else
                {
                    throw new AnalisadorLexicoException($"Identificador inválido: {token} \n Linha: {linha}");
                }
            }

            return Tokens;
        }

        //Retorna uma lista com a entrada separada por ' ', semelhante a string.split mas também separa por '\n' incluindo o '\n' na lista
        public List<string> moaSplit(string entrada)
        {
            List<string> tokenList = new List<string>();
            var tamanhoEntrada = entrada.Length;

            string tokenAtual = "";
            for (int i = 0; i < tamanhoEntrada; i++)
            {
                if (entrada[i] == ' ')
                {
                    if(!string.IsNullOrEmpty(tokenAtual))
                        tokenList.Add(tokenAtual);
                    tokenAtual = "";
                    continue;
                }
                if (entrada[i] == '\n')
                {
                    if(!string.IsNullOrEmpty(tokenAtual))
                        tokenList.Add(tokenAtual);
                    
                    tokenAtual = "";
                    tokenList.Add("\n");
                    
                    continue;
                }
                tokenAtual += entrada[i];
            }
            
            if(!string.IsNullOrEmpty(tokenAtual))
                tokenList.Add(tokenAtual);

            return tokenList;

        }
    }
}