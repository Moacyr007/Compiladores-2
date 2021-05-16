using System;
using System.Collections.Generic;
using System.Linq;

namespace Compilador
{
    public class AnalisadorSintatico
    {
        public List<Token> Tokens { get; set; }
        public int UltimoIdex { get; set; }
        public int IndexAtual { get; set; }
        public TabelaDeSimbolos TabelaDeSimbolos { get; set; }
        public Stack<string> EscopoStack { get; set; }
        public Stack<string> CallStack { get; set; }

        public List<TipoItemTs> TipoItensExpressao { get; set; }

        public AnalisadorSintatico(List<Token> tokens)
        {
            Tokens = tokens;
            TabelaDeSimbolos = new TabelaDeSimbolos();
            EscopoStack = new Stack<string>();
            CallStack = new Stack<string>();
            TipoItensExpressao = new List<TipoItemTs>();
        }

        private void ThrowCompiladorException(Token token)
        {
            throw new CompiladorException(token.Cadeia, token.Linha, this.GetType().Name);
        }

        public void Analisar()
        {
            try
            {
                Programa();
            }
            catch (IndexOutOfRangeException)
            {
                ThrowCompiladorException(Tokens[UltimoIdex]);
            }
        }

        #region AnalisadorSintatico

        private void Programa()
        {
            CallStack.Push(nameof(Programa));

            UltimoIdex = Tokens.Count - 1;
            IndexAtual = 0;

            if (Tokens[IndexAtual].Tipo != TipoToken.ReservadoProgram)
                ThrowCompiladorException(Tokens[IndexAtual]);
            IndexAtual++;

            if (Tokens[IndexAtual].Tipo != TipoToken.Identificador)
                ThrowCompiladorException(Tokens[IndexAtual]);

            EscopoStack.Push($"Global: {Tokens[IndexAtual].Cadeia}");

            IndexAtual++;

            Corpo();

            CallStack.Pop();
        }

        private void Corpo()
        {
            CallStack.Push(nameof(Corpo));

            Dc();

            if (Tokens[IndexAtual].Tipo != TipoToken.ReservadoBegin)
                ThrowCompiladorException(Tokens[IndexAtual]);
            IndexAtual++;

            Comandos();

            if (Tokens[IndexAtual].Tipo != TipoToken.ReservadoEnd)
                ThrowCompiladorException(Tokens[IndexAtual]);
            IndexAtual++;

            CallStack.Pop();
        }

        private void Dc()
        {
            CallStack.Push(nameof(Dc));

            if (!IndexInRange())
                return;

            switch (Tokens[IndexAtual].Tipo)
            {
                case TipoToken.ReservadoVar:
                    DcV();
                    MaisDc();
                    break;
                case TipoToken.ReservadoProcedure:
                    DcP();
                    MaisDc();
                    break;
            }

            CallStack.Pop();
        }

        private void DcP()
        {
            CallStack.Push(nameof(DcP));

            if (Tokens[IndexAtual].Tipo == TipoToken.ReservadoProcedure)
            {
                IndexAtual++;
                if (Tokens[IndexAtual].Tipo == TipoToken.Identificador)
                {
                    var procedure = new ItemTs
                    {
                        Cadeia = Tokens[IndexAtual].Cadeia,
                        Escopo = EscopoStack.Peek(),
                        Tipo = TipoItemTs.Procedimento,
                        Linha = Tokens[IndexAtual].Linha,
                        Parametros = new List<Parametro>()
                    };

                    EscopoStack.Push(Tokens[IndexAtual].Cadeia);

                    IndexAtual++;
                    Parametros();

                    AddParametrosNaProcedure(procedure);

                    TabelaDeSimbolos.TryAddNewItem(procedure);

                    CorpoP();

                    EscopoStack.Pop();
                }
                else
                    ThrowCompiladorException(Tokens[IndexAtual]);
            }
            else
                ThrowCompiladorException(Tokens[IndexAtual]);

            CallStack.Pop();
        }

       

        private void Parametros()
        {
            CallStack.Push(nameof(Programa));

            if (Tokens[IndexAtual].Tipo == TipoToken.SimboloAbreParenteses)
            {
                IndexAtual++;
                ListaPar();

                if (Tokens[IndexAtual].Tipo == TipoToken.SimboloFechaParenteses)
                    IndexAtual++;
                else
                    ThrowCompiladorException(Tokens[IndexAtual]);
            }

            CallStack.Pop();
        }

        private void ListaPar()
        {
            CallStack.Push(nameof(ListaPar));

            Variaveis();
            if (Tokens[IndexAtual].Tipo == TipoToken.SimboloDoisPontos)
            {
                IndexAtual++;
                TipoVar();
                MaisPar();
            }
            else
            {
                ThrowCompiladorException(Tokens[IndexAtual]);
            }

            CallStack.Pop();
        }

        private void MaisPar()
        {
            CallStack.Push(nameof(MaisPar));

            if (Tokens[IndexAtual].Tipo == TipoToken.SimboloPontoEVirgula)
            {
                IndexAtual++;
                ListaPar();
            }

            CallStack.Pop();
        }

        private void CorpoP()
        {
            CallStack.Push(nameof(CorpoP));

            DcLoc();
            if (Tokens[IndexAtual].Tipo == TipoToken.ReservadoBegin)
            {
                IndexAtual++;
                Comandos();
                if (Tokens[IndexAtual].Tipo == TipoToken.ReservadoEnd)
                {
                    IndexAtual++;
                }
                else
                    ThrowCompiladorException(Tokens[IndexAtual]);
            }
            else
                ThrowCompiladorException(Tokens[IndexAtual]);

            CallStack.Pop();
        }

        private void DcLoc()
        {
            CallStack.Push(nameof(DcLoc));

            DcV();
            MaisDcloc();

            CallStack.Pop();
        }

        private void MaisDcloc()
        {
            CallStack.Push(nameof(MaisDcloc));

            if (Tokens[IndexAtual].Tipo == TipoToken.SimboloPontoEVirgula)
            {
                IndexAtual++;
                DcLoc();
            }

            CallStack.Pop();
        }

        private void ListaArg()
        {
            CallStack.Push(nameof(ListaArg));

            if (Tokens[IndexAtual].Tipo == TipoToken.SimboloAbreParenteses)
            {
                IndexAtual++;
                Argumentos();
                if (Tokens[IndexAtual].Tipo == TipoToken.SimboloFechaParenteses)
                {
                    IndexAtual++;
                }
                else
                    ThrowCompiladorException(Tokens[IndexAtual]);
            }

            CallStack.Pop();
        }

        private void Argumentos()
        {
            CallStack.Push(nameof(Argumentos));

            if (Tokens[IndexAtual].Tipo == TipoToken.Identificador)
            {
                IndexAtual++;
                MaisIdent();
            }
            else
                ThrowCompiladorException(Tokens[IndexAtual]);

            CallStack.Pop();
        }

        private void MaisIdent()
        {
            CallStack.Push(nameof(MaisIdent));

            if (Tokens[IndexAtual].Tipo == TipoToken.SimboloPontoEVirgula)
            {
                IndexAtual++;
                Argumentos();
            }

            CallStack.Pop();
        }

        private void DcV()
        {
            CallStack.Push(nameof(DcV));

            if (Tokens[IndexAtual].Tipo == TipoToken.ReservadoVar)
            {
                IndexAtual++;
                Variaveis();
                if (Tokens[IndexAtual].Tipo == TipoToken.SimboloDoisPontos)
                {
                    IndexAtual++;
                    TipoVar();
                }
                else
                    ThrowCompiladorException(Tokens[IndexAtual]);
            }
            else
                ThrowCompiladorException(Tokens[IndexAtual]);

            CallStack.Pop();
        }

        private void Variaveis()
        {
            CallStack.Push(nameof(Variaveis));

            if (Tokens[IndexAtual].Tipo == TipoToken.Identificador)
            {
                ValidarVariavel();
                IndexAtual++;
                MaisVar();
            }
            else
            {
                ThrowCompiladorException(Tokens[IndexAtual]);
            }

            CallStack.Pop();
        }

        private void MaisVar()
        {
            CallStack.Push(nameof(MaisVar));

            if (Tokens[IndexAtual].Tipo == TipoToken.SimboloVirgula)
            {
                IndexAtual++;
                Variaveis();
            }

            CallStack.Pop();
        }

        private void TipoVar() 
        {
            CallStack.Push(nameof(TipoVar));

            if (Tokens[IndexAtual].Tipo == TipoToken.ReservadoReal ||
                Tokens[IndexAtual].Tipo == TipoToken.ReservadoInteger)
            {
                var tipoItemTs = Tokens[IndexAtual].Tipo == TipoToken.ReservadoReal
                    ? TipoItemTs.NumeroReal
                    : TipoItemTs.NumeroInteiro;

                TabelaDeSimbolos.SetTipoVariaveis(tipoItemTs);

                IndexAtual++;
            }
            else
                ThrowCompiladorException(Tokens[IndexAtual]);

            CallStack.Pop();
        }

        private void MaisDc() 
        {
            CallStack.Push(nameof(MaisDc));

            if (Tokens[IndexAtual].Tipo == TipoToken.SimboloPontoEVirgula)
            {
                IndexAtual++;
                Dc();
            }

            CallStack.Pop();
        }

        private void Pfalsa()
        {
            CallStack.Push(nameof(Pfalsa));

            if (Tokens[IndexAtual].Tipo == TipoToken.ReservadoElse)
            {
                IndexAtual++;
                Comandos();
            }

            CallStack.Pop();
        }

        private void Comandos()
        {
            CallStack.Push(nameof(Comandos));

            Comando();
            MaisComandos();

            CallStack.Pop();
        }

        private void MaisComandos()
        {
            CallStack.Push(nameof(MaisComandos));

            if (Tokens[IndexAtual].Tipo == TipoToken.SimboloPontoEVirgula)
            {
                IndexAtual++;
                Comandos();
            }

            CallStack.Pop();
        }

        private void Comando()
        {
            CallStack.Push(nameof(Comando));

            switch (Tokens[IndexAtual].Tipo)
            {
                case TipoToken.ReservadoRead:
                    IndexAtual++;
                    if (Tokens[IndexAtual].Tipo == TipoToken.SimboloAbreParenteses)
                    {
                        IndexAtual++;
                        Variaveis();
                        if (Tokens[IndexAtual].Tipo == TipoToken.SimboloFechaParenteses)
                            IndexAtual++;
                        else
                            ThrowCompiladorException(Tokens[IndexAtual]);
                    }
                    else
                        ThrowCompiladorException(Tokens[IndexAtual]);

                    break;

                case TipoToken.ReservadoWrite:
                    IndexAtual++;
                    if (Tokens[IndexAtual].Tipo == TipoToken.SimboloAbreParenteses)
                    {
                        IndexAtual++;
                        Variaveis();
                        if (Tokens[IndexAtual].Tipo == TipoToken.SimboloFechaParenteses)
                            IndexAtual++;
                        else
                            ThrowCompiladorException(Tokens[IndexAtual]);
                    }
                    else
                        ThrowCompiladorException(Tokens[IndexAtual]);

                    break;

                case TipoToken.ReservadoWhile:
                    IndexAtual++;
                    Condicao();
                    if (Tokens[IndexAtual].Tipo == TipoToken.ReservadoDo)
                    {
                        IndexAtual++;
                        Comandos();
                        if (Tokens[IndexAtual].Tipo == TipoToken.SimboloCifrao)
                        {
                            IndexAtual++;
                        }
                        else
                            ThrowCompiladorException(Tokens[IndexAtual]);
                    }
                    else
                        ThrowCompiladorException(Tokens[IndexAtual]);

                    break;

                case TipoToken.ReservadoIf:
                    IndexAtual++;

                    Condicao();
                    if (Tokens[IndexAtual].Tipo == TipoToken.ReservadoThen)
                    {
                        IndexAtual++;
                        Comandos();
                        Pfalsa();
                        if (Tokens[IndexAtual].Tipo == TipoToken.SimboloCifrao)
                        {
                            IndexAtual++;
                        }
                        else
                            ThrowCompiladorException(Tokens[IndexAtual]);
                    }
                    else
                        ThrowCompiladorException(Tokens[IndexAtual]);

                    break;

                case TipoToken.Identificador:

                    var procedure = TabelaDeSimbolos.Find(Tokens[IndexAtual].Cadeia, EscopoStack.Peek(), true);
                    bool isProcedure = procedure != null;

                    IndexAtual++;
                    RestoIdent();

                    if (isProcedure)
                    {
                        ValidarParametros(procedure);
                    }
                    break;

                default:
                    ThrowCompiladorException(Tokens[IndexAtual]);
                    break;
            }

            CallStack.Pop();
        }

        
        private void RestoIdent()
        {
            CallStack.Push(nameof(RestoIdent));

            if (Tokens[IndexAtual].Tipo == TipoToken.SimboloAtribuicao)
            {
                IndexAtual++;
                Expressao();
                ValidarTiposAtribuicao();
            }
            else
            {
                ListaArg();
            }

            CallStack.Pop();
        }

        private void Condicao()
        {
            CallStack.Push(nameof(Condicao));

            Expressao();
            var tipoExpressao1 = TipoItensExpressao.FirstOrDefault();

            Relacao();
            Expressao();
            var tipoExpressao2 = TipoItensExpressao.FirstOrDefault();

            if (tipoExpressao1 != tipoExpressao2)
            {
                throw new CompiladorException(
                    $"Não é possível comparar expressões de tipos diferentes\nLinha: {Tokens[IndexAtual].Linha}");
            }

            CallStack.Pop();
        }

        private void Relacao()
        {
            CallStack.Push(nameof(Relacao));

            if (Tokens[IndexAtual].Tipo == TipoToken.OperadorIgual ||
                Tokens[IndexAtual].Tipo == TipoToken.OperadorComparacao)
            {
                IndexAtual++;
            }

            CallStack.Pop();
        }


        private void Expressao()
        {
            CallStack.Push(nameof(Expressao));
            TipoItensExpressao = new List<TipoItemTs>();

            Termo();
            OutrosTermos();

            ValidarTiposExpressao();

            CallStack.Pop();
        }

        private void OpUn()
        {
            CallStack.Push(nameof(OpUn));

            if (Tokens[IndexAtual].Tipo == TipoToken.OperadorSoma ||
                Tokens[IndexAtual].Tipo == TipoToken.OperadorSubtracao)
                IndexAtual++;

            CallStack.Pop();
        }

        private void OutrosTermos()
        {
            CallStack.Push(nameof(OutrosTermos));

            if (Tokens[IndexAtual].Tipo == TipoToken.OperadorSoma ||
                Tokens[IndexAtual].Tipo == TipoToken.OperadorSubtracao)
            {
                OpAd();
                Termo();
                OutrosTermos();
            }

            CallStack.Pop();
        }

        private void OpAd()
        {
            CallStack.Push(nameof(OpAd));

            if (Tokens[IndexAtual].Tipo == TipoToken.OperadorSoma ||
                Tokens[IndexAtual].Tipo == TipoToken.OperadorSubtracao)
            {
                IndexAtual++;
            }

            CallStack.Pop();
        }

        private void Termo()
        {
            CallStack.Push(nameof(Termo));

            OpUn();
            Fator();
            MaisFatores();

            CallStack.Pop();
        }

        private void MaisFatores()
        {
            CallStack.Push(nameof(MaisFatores));

            if (Tokens[IndexAtual].Tipo == TipoToken.OperadorMultiplicacao ||
                Tokens[IndexAtual].Tipo == TipoToken.OperadorDivisao)
            {
                OpMul();
                Fator();
                MaisFatores();
            }

            CallStack.Pop();
        }

        private void OpMul()
        {
            CallStack.Push(nameof(OpMul));

            if (Tokens[IndexAtual].Tipo == TipoToken.OperadorMultiplicacao ||
                Tokens[IndexAtual].Tipo == TipoToken.OperadorDivisao)
            {
                IndexAtual++;
            }

            CallStack.Pop();
        }

        private void Fator()
        {
            CallStack.Push(nameof(Fator));

            switch (Tokens[IndexAtual].Tipo)
            {
                case TipoToken.Identificador:
                    ValidarVariavel();
                    var simboloTs = TabelaDeSimbolos.Find(Tokens[IndexAtual].Cadeia, EscopoStack.Peek());
                    TipoItensExpressao.Add(simboloTs.Tipo);
                    IndexAtual++;
                    break;
                case TipoToken.NumeroInteiro:
                    IndexAtual++;
                    TipoItensExpressao.Add(TipoItemTs.NumeroInteiro);
                    break;
                case TipoToken.NumeroReal:
                    IndexAtual++;
                    TipoItensExpressao.Add(TipoItemTs.NumeroReal);
                    break;
                case TipoToken.SimboloAbreParenteses:
                    IndexAtual++;

                    Expressao();

                    if (Tokens[IndexAtual].Tipo == TipoToken.SimboloFechaParenteses)
                        IndexAtual++;
                    else
                        ThrowCompiladorException(Tokens[IndexAtual]);
                    break;
            }

            CallStack.Pop();
        }

        #endregion

        #region Helpers

        private bool IndexInRange()
        {
            return IndexAtual <= UltimoIdex;
        }

        private void ValidarVariavel()
        {
            var i = 0;

            while (new List<string>() {nameof(MaisVar), nameof(Variaveis)}.Contains(CallStack.ElementAt(i)))
            {
                i++;
            }

            var callerName = CallStack.ElementAt(i);

            if (callerName == nameof(DcV) || callerName == nameof(ListaPar))
            {
                TabelaDeSimbolos.TryAddNewItem(new ItemTs
                {
                    Cadeia = Tokens[IndexAtual].Cadeia, Escopo = EscopoStack.Peek(), Tipo = TipoItemTs.Desconhecido,
                    Linha = Tokens[IndexAtual].Linha
                });
            }
            else
            {
                TabelaDeSimbolos.VerificarSeVariavelJaFoiDeclarada(new ItemTs
                    {Cadeia = Tokens[IndexAtual].Cadeia, Escopo = EscopoStack.Peek(), Tipo = TipoItemTs.Desconhecido});
            }
        }

        private void ValidarTiposAtribuicao()
        {
            TipoItemTs tipoVar;

            var indexAtribuicao = IndexAtual;
            while (Tokens[indexAtribuicao].Tipo != TipoToken.SimboloAtribuicao)
            {
                indexAtribuicao--;
            }

            indexAtribuicao--;
            
            switch (Tokens[indexAtribuicao].Tipo)
            {
                case TipoToken.Identificador:
                    tipoVar = TabelaDeSimbolos.Find(Tokens[indexAtribuicao].Cadeia, EscopoStack.Peek()).Tipo;
                    break;
                case TipoToken.NumeroReal:
                    tipoVar = TipoItemTs.NumeroReal;
                    break;
                default:
                    tipoVar = TipoItemTs.NumeroInteiro;
                    break;
            }

            if (TipoItensExpressao.Any(x => x != tipoVar))
            {
                throw new CompiladorException(
                    $"Atribuição inválida, era esperado um valor do tipo {tipoVar.ToString()}\nLinha: {Tokens[indexAtribuicao].Linha}");
            }
        }

        private void ValidarTiposExpressao()
        {
            var firstTipoExpressao = TipoItensExpressao.FirstOrDefault();
            if (TipoItensExpressao.Any(x => x != firstTipoExpressao))
            {
                throw new CompiladorException(
                    $"Não é possível ter  tipos diferentes em uma expressão\nLinha: {Tokens[IndexAtual].Linha}");
            }
        }
        
        private void AddParametrosNaProcedure(ItemTs procedure)
        {
            var i = IndexAtual-4;
           
            
            while (Tokens[i].Tipo != TipoToken.SimboloAbreParenteses)
            {
                if (Tokens[i].Tipo == TipoToken.Identificador)
                {
                    var indexTipoParametro = i;
                    
                    while (Tokens[indexTipoParametro].Tipo != TipoToken.ReservadoInteger && Tokens[indexTipoParametro].Tipo != TipoToken.ReservadoReal)
                    {
                        indexTipoParametro++;
                    }
                    
                    procedure.Parametros.Add(new Parametro
                    {
                        Cadeia = Tokens[i].Cadeia,
                        Tipo = Tokens[indexTipoParametro].Tipo == TipoToken.ReservadoInteger
                            ? TipoParametro.NumeroInteiro
                            : TipoParametro.NumeroReal
                    });
                }

                i--;
            }
        }
        
        private void ValidarParametros(ItemTs procedure)
        {
            //Validar argumentos procedure

            var i = IndexAtual;

            var tipoParametros = new List<TipoParametro>();

            while (Tokens[i].Tipo != TipoToken.SimboloAbreParenteses)
            {
                if (Tokens[i].Tipo == TipoToken.Identificador)
                {
                    var parametro = TabelaDeSimbolos.Find(Tokens[i].Cadeia, EscopoStack.Peek());
                    tipoParametros.Add(parametro.Tipo == TipoItemTs.NumeroInteiro
                        ? TipoParametro.NumeroInteiro
                        : TipoParametro.NumeroReal);
                }

                i--;
            }

            if (tipoParametros.Count != procedure.Parametros.Count)
            {
                throw new CompiladorException($"Quantidade errada de parametros\nLinha: {Tokens[i].Linha}");
            }

            for (int j = 0; j < tipoParametros.Count - 1; j++)
            {
                if (tipoParametros[j] != procedure.Parametros[j].Tipo)
                {
                    throw new CompiladorException(
                        $"Parametro com tipo diferente do esperado, encontrou {tipoParametros[j].ToString()} mas esperava {procedure.Parametros[j].Tipo.ToString()}\nLinha: {procedure.Linha}");
                }
            }
        }

    }
    
    

    #endregion
}