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

        private List<string> SimbolosDuplos = new List<string>() {":=", "<>", "<=", ">=", "*/", "/*"};
        
        public Dictionary<string, TipoToken> IdentificadoresReservados = new Dictionary<string, TipoToken>()
        {
            {"if", TipoToken.ReservadoIf},
            {"else", TipoToken.ReservadoElse},
            {"then", TipoToken.ReservadoThen},
            {"while", TipoToken.ReservadoWhile},
            {"do", TipoToken.ReservadoDo},
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
            {"<=", TipoToken.OperadorComparacao},
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
            {"{", TipoToken.SimboloAbreChaves},
            {"}", TipoToken.SimboloFechaChaves},
            {".", TipoToken.SimboloPonto},
        };

        public List<Token> Analisar()
        {
            if (Entrada == string.Empty)
                return Tokens;
            
            var tokenList = SepararTokens(Entrada);

            foreach (var token in tokenList)
            {
                if (IdentificadoresReservados.ContainsKey(token.Cadeia))
                {
                    Tokens.Add(new Token(
                        token.Cadeia,
                        IdentificadoresReservados[token.Cadeia], 
                        token.Linha));
                }

                else if (Simbolos.ContainsKey(token.Cadeia))
                {
                    Tokens.Add(new Token(
                        token.Cadeia,
                        Simbolos[token.Cadeia],
                        token.Linha));
                }

                else if (Operadores.ContainsKey(token.Cadeia))
                {
                    Tokens.Add(new Token(
                        token.Cadeia,
                        Operadores[token.Cadeia],
                        token.Linha));
                }

                else if (int.TryParse(token.Cadeia, out _))
                {
                    Tokens.Add(new Token(
                        token.Cadeia,
                        TipoToken.NumeroInteiro,
                        token.Linha));
                }

                else if (float.TryParse(token.Cadeia, out _))
                {
                    Tokens.Add(new Token(
                        token.Cadeia,
                        TipoToken.NumeroReal,
                        token.Linha));
                }

                else if (char.IsLetter(token.Cadeia[0]))
                {
                    var flagIdentificadorEhInvalido = false;
                    foreach (var caractere in token.Cadeia)
                    {
                        if (!char.IsNumber(caractere) && !char.IsLetter(caractere))
                        {
                            flagIdentificadorEhInvalido = true;
                            break;
                        }
                    }

                    if (flagIdentificadorEhInvalido)
                    {
                        throw new CompiladorException($"Identificador inválido: {token.Cadeia} \n Linha: {token.Linha}");
                    }

                    Tokens.Add(new Token(
                        token.Cadeia,
                        TipoToken.Identificador,
                        token.Linha));
                }

                else
                {
                    throw new CompiladorException($"Identificador inválido: {token.Cadeia} \n Linha: {token.Linha}");
                }
            }

            return Tokens;
        }

        //Retorna uma lista com a entrada separada por ' ', semelhante a string.split mas também separa por '\n' incluindo o '\n' na lista
        public List<Token> SepararTokens(string entrada)
        {
            List<Token> tokenList = new List<Token>();
            var tamanhoEntrada = entrada.Length -1;
            int linha = 1;
            string tokenAtualValor = "";
            bool lendoComentario = false;
            bool lendoComentarioChaves = false;
            var i = 0;
            while(i < tamanhoEntrada)
            {
                if (i+1 < tamanhoEntrada && entrada[i] == '*' && entrada[i+1] == '/')
                {
                    i+=2;
                    lendoComentario = false;
                    
                    continue;
                }
                
                if (entrada[i] == '}')
                {
                    lendoComentarioChaves = false;
                    i++;
                    continue;
                }
                
                if (lendoComentario || lendoComentarioChaves)
                {
                    i++;
                    continue;
                }
                
                if (i+1 < tamanhoEntrada && entrada[i] == '/' && entrada[i+1] == '*')
                {
                    i+=2;
                    lendoComentario = true;   
                    
                    continue;
                }
                
                if (entrada[i] == '{')
                {
                    i++;
                    lendoComentarioChaves = true;
                    continue;
                }

                if (i+1 < tamanhoEntrada && SimbolosDuplos.Contains(string.Concat(entrada[i], entrada[i+1])))
                {
                    if(!string.IsNullOrEmpty(tokenAtualValor))
                        tokenList.Add(new Token(tokenAtualValor, TipoToken.Desconhecido, linha));
                    
                    tokenAtualValor = string.Concat(entrada[i], entrada[i+1]);
                    tokenList.Add(new Token(tokenAtualValor, TipoToken.Desconhecido, linha));
                    
                    tokenAtualValor = "";
                    i+=2;
                    continue;
                }
                
                if (entrada[i] == ' ' || entrada[i] == '\r')
                {
                    if(!string.IsNullOrEmpty(tokenAtualValor))
                        tokenList.Add(new Token(tokenAtualValor, TipoToken.Desconhecido, linha));
                    tokenAtualValor = "";
                    i++;
                    continue;
                }
                
                if (entrada[i] == '\n')
                {
                    if(!string.IsNullOrEmpty(tokenAtualValor))
                        tokenList.Add(new Token(tokenAtualValor, TipoToken.Desconhecido, linha));

                    linha++;
                    tokenAtualValor = "";
                    i++;
                    continue;
                }

                //Adicionar identificador
                if (char.IsLetter(entrada[i]))
                {
                    tokenAtualValor += entrada[i];
                    i++;
                    while (char.IsLetter(entrada[i]) || char.IsNumber(entrada[i]))
                    {
                        tokenAtualValor += entrada[i];
                        i++;
                    }
                    tokenList.Add(new Token(tokenAtualValor, TipoToken.Desconhecido, linha));
                    tokenAtualValor = "";
                    continue;
                }
                
                //Adicionar numero
                if (char.IsNumber(entrada[i]))
                {
                    tokenAtualValor += entrada[i];
                    i++;
                    
                    while (char.IsNumber(entrada[i]))
                    {
                        tokenAtualValor += entrada[i];
                        i++;
                    }

                    if (entrada[i].ToString() == ".")
                    {
                        tokenAtualValor += entrada[i];
                        i++;
                        while (char.IsNumber(entrada[i]))
                        {
                            tokenAtualValor += entrada[i];
                            i++;
                        }
                    }
                    
                    tokenList.Add(new Token(tokenAtualValor, TipoToken.Desconhecido, linha));
                    tokenAtualValor = "";
                    continue;
                }
                
                if (!char.IsDigit(entrada[i]) && !char.IsLetter(entrada[i]))
                {
                    if(!string.IsNullOrEmpty(tokenAtualValor))
                        tokenList.Add(new Token(tokenAtualValor, TipoToken.Desconhecido, linha));

                    tokenAtualValor = entrada[i].ToString();
                    tokenList.Add(new Token(tokenAtualValor, TipoToken.Desconhecido, linha));
                    tokenAtualValor = "";
                    i++;
                    continue;
                }
                
                tokenAtualValor += entrada[i];
                i++;
            }
            
            if(!string.IsNullOrEmpty(tokenAtualValor))
                tokenList.Add(new Token(tokenAtualValor, TipoToken.Desconhecido, linha));

            return tokenList;

        }
    }
}