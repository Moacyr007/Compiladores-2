using System;
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
            
            var tokenArray = Entrada.Split(" ");
            var linha = 1;

            foreach (var token in tokenArray)
            {
                if (token == "/n")
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

        public List<Token> AnalisarOld()
        {
            if (Entrada == string.Empty)
                return Tokens;

            var finalEntrada = Entrada.Length - 1;
            var posicaoAtual = 0;
            var inicioToken = 0;
            var estado = EstadoAnalisadorLexico.Inicial;
            var linha = 0;

            while (posicaoAtual < finalEntrada)
            {
                switch (estado)
                {
                    case EstadoAnalisadorLexico.Inicial:
                        if (new List<string> {" ", "\n"}.Contains(Entrada[posicaoAtual].ToString()))
                        {
                            if (Entrada[posicaoAtual] == '\n')
                            {
                                linha++;
                            }

                            posicaoAtual++;
                            break;
                        }

                        if (char.IsLetter(Entrada[posicaoAtual]))
                            estado = EstadoAnalisadorLexico.Identificador;

                        else if (char.IsNumber(Entrada[posicaoAtual]))
                            estado = EstadoAnalisadorLexico.Numero;

                        //else if (SimbolosSimples.Contains(Entrada[posicaoAtual].ToString()))
                        //    estado = EstadoAnalisadorLexico.Simbolo;

                        else
                            estado = EstadoAnalisadorLexico.Invalido;

                        break;
                    case EstadoAnalisadorLexico.Invalido:
                        throw new AnalisadorLexicoException(string.Format("Simbolo inválido: {0} \n Linha: {1}",
                            Entrada[posicaoAtual], linha));

                    case EstadoAnalisadorLexico.Identificador:
                        inicioToken = posicaoAtual;
                        while (posicaoAtual < finalEntrada && (char.IsLetter(Entrada[posicaoAtual]) ||
                                                               char.IsNumber(Entrada[posicaoAtual])))
                        {
                            posicaoAtual++;
                        }

                        var identificador = Entrada.Substring(inicioToken, posicaoAtual - inicioToken + 1);

                        if (IdentificadoresReservados.ContainsKey(identificador))
                        {
                            Tokens.Add(new Token(
                                identificador,
                                IdentificadoresReservados[identificador]));

                            estado = EstadoAnalisadorLexico.Inicial;

                            break;
                        }


                        Tokens.Add(new Token(
                            identificador,
                            TipoToken.Identificador));

                        estado = EstadoAnalisadorLexico.Inicial;
                        break;
                    case EstadoAnalisadorLexico.Numero:
                        inicioToken = posicaoAtual;
                        while (posicaoAtual < finalEntrada && char.IsNumber(Entrada[posicaoAtual]))
                            posicaoAtual++;

                        if (posicaoAtual < finalEntrada && Entrada[posicaoAtual] == '.')
                        {
                            posicaoAtual++;
                            if (posicaoAtual < finalEntrada && char.IsNumber(Entrada[posicaoAtual]))
                            {
                                while (posicaoAtual < finalEntrada && char.IsNumber(Entrada[posicaoAtual]))
                                    posicaoAtual++;

                                Tokens.Add(new Token(
                                    Entrada.Substring(inicioToken, posicaoAtual - inicioToken),
                                    TipoToken.NumeroReal));

                                inicioToken = posicaoAtual;
                                estado = EstadoAnalisadorLexico.Inicial;
                                break;
                            }

                            throw new AnalisadorLexicoException(Entrada[posicaoAtual], linha);
                        }

                        Tokens.Add(new Token(
                            Entrada.Substring(inicioToken, posicaoAtual - inicioToken),
                            TipoToken.NumeroInteiro));

                        inicioToken = posicaoAtual;
                        estado = EstadoAnalisadorLexico.Inicial;
                        break;

                    case EstadoAnalisadorLexico.Desconhecido:
                        break;
                    case EstadoAnalisadorLexico.Simbolo:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return Tokens;
        }
    }
}