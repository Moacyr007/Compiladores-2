using System;
using System.Collections.Generic;

namespace Compilador
{
    public class AnalisadorSintatico
    {
        public List<Token> Tokens { get; set; }
        public int UltimoIdex { get; set; }
        public int IndexAtual { get; set; }

        public AnalisadorSintatico(List<Token> tokens)
        {
            Tokens = tokens;
        }

        private void ThrowCompiladorException(Token token)
        {
            throw new CompiladorException(token.Valor, token.Linha, this.GetType().Name);
        }

        private bool IndexInRange()
        {
            return IndexAtual <= UltimoIdex;
        }

        public void Iniciar()
        {
            try
            {
                Programa();
            }
            catch (IndexOutOfRangeException e)
            {
                ThrowCompiladorException(Tokens[UltimoIdex]);
            }
        }
        
        private void Programa()
        {
            UltimoIdex = Tokens.Count - 1;
            IndexAtual = 0;

            if (Tokens[IndexAtual].Tipo != TipoToken.ReservadoProgram)
                ThrowCompiladorException(Tokens[IndexAtual]);

            IndexAtual++;

            if (Tokens[IndexAtual].Tipo != TipoToken.ReservadoIdent)
                ThrowCompiladorException(Tokens[IndexAtual]);

            IndexAtual++;

            Corpo();
        }

        private void Corpo()
        {
            Dc();

            if (Tokens[IndexAtual].Tipo != TipoToken.ReservadoBegin)
                ThrowCompiladorException(Tokens[IndexAtual]);

            IndexAtual++;

            Comandos();

            if (Tokens[IndexAtual].Tipo != TipoToken.ReservadoEnd)
                ThrowCompiladorException(Tokens[IndexAtual]);

            IndexAtual++;
        }

        private void Dc()
        {
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
        }

        private void DcP()
        {
            if (Tokens[IndexAtual].Tipo == TipoToken.ReservadoProcedure)
            {
                IndexAtual++;
                if (Tokens[IndexAtual].Tipo == TipoToken.ReservadoIdent)
                {
                    IndexAtual++;
                    Parametros();
                    CorpoP();
                }
                else
                    ThrowCompiladorException(Tokens[IndexAtual]);
            }
            else
                ThrowCompiladorException(Tokens[IndexAtual]);
        }

        private void Parametros()
        {
            if (Tokens[IndexAtual].Tipo == TipoToken.SimboloAbreParenteses)
            {
                IndexAtual++;
                ListaPar();

                if (Tokens[IndexAtual].Tipo == TipoToken.SimboloFechaParenteses)
                    IndexAtual++;
                else
                    ThrowCompiladorException(Tokens[IndexAtual]);
            }
        }

        private void ListaPar()
        {
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
        }

        private void MaisPar()
        {
            if (Tokens[IndexAtual].Tipo == TipoToken.SimboloPontoEVirgula)
            {
                IndexAtual++;
                ListaPar();
            }
        }

        private void CorpoP()
        {
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
        }

        private void DcLoc()
        {
            DcV();
            MaisDcloc();
        }

        private void MaisDcloc()
        {
            if (Tokens[IndexAtual].Tipo == TipoToken.SimboloPontoEVirgula)
            {
                IndexAtual++;
                DcLoc();
            }
        }

        private void ListaArg()
        {
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
        }

        private void Argumentos()
        {
            if (Tokens[IndexAtual].Tipo == TipoToken.ReservadoIdent)
            {
                IndexAtual++;
                MaisIdent();
            }
            else
                ThrowCompiladorException(Tokens[IndexAtual]);
        }

        private void MaisIdent()
        {
            if (Tokens[IndexAtual].Tipo == TipoToken.SimboloPontoEVirgula)
            {
                IndexAtual++;
                Argumentos();
            }
        }

        private void DcV()
        {
            if (Tokens[IndexAtual].Tipo != TipoToken.ReservadoVar)
            {
                IndexAtual++;
                Variaveis();
            }
            else
            {
                TipoVar();
            }
        }

        private void Variaveis()
        {
            if (Tokens[IndexAtual].Tipo == TipoToken.ReservadoIdent)
            {
                IndexAtual++;
                MaisVar();
            }
            else
            {
                ThrowCompiladorException(Tokens[IndexAtual]);
            }
        }

        private void MaisVar()
        {
            if (Tokens[IndexAtual].Tipo == TipoToken.SimboloVirgula)
            {
                IndexAtual++;
                Variaveis();
            }
        }

        private void TipoVar()
        {
            if (Tokens[IndexAtual].Tipo == TipoToken.ReservadoNumeroReal)
                IndexAtual++;
            else
                ThrowCompiladorException(Tokens[IndexAtual]);
        }

        private void MaisDc()
        {
            switch (Tokens[IndexAtual].Tipo)
            {
                case TipoToken.ReservadoVar:
                    Dc();
                    break;
                case TipoToken.ReservadoProcedure:
                    Dc();
                    break;
            }
        }

        private void Pfalsa()
        {
            if (Tokens[IndexAtual].Tipo == TipoToken.ReservadoElse)
            {
                IndexAtual++;
                Comandos();
            }
        }

        private void Comandos()
        {
            Comando();
            MaisComandos();
        }

        private void MaisComandos()
        {
            if (Tokens[IndexAtual].Tipo == TipoToken.SimboloPontoEVirgula)
            {
                IndexAtual++;
                Comandos();
            }
        }

        private void Comando()
        {
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

                case TipoToken.ReservadoIdent:
                    IndexAtual++;
                    RestoIdent();
                    break;

                default:
                    ThrowCompiladorException(Tokens[IndexAtual]);
                    break;
            }
        }

        private void RestoIdent()
        {
            if (Tokens[IndexAtual].Tipo == TipoToken.SimboloAtribuicao)
            {
                IndexAtual++;
                Expressao();
            }
            else
            {
                ListaArg();
            }
        }

        private void Condicao()
        {
            Expressao();
            Relacao();
            Expressao();
        }

        private void Relacao()
        {
            if (Tokens[IndexAtual].Tipo == TipoToken.OperadorIgual ||
                Tokens[IndexAtual].Tipo == TipoToken.OperadorComparacao)
            {
                IndexAtual++;
            }
        }


        private void Expressao()
        {
            Termo();
            OutrosTermos();
        }

        private void OpUn()
        {
            if (Tokens[IndexAtual].Tipo == TipoToken.OperadorSoma ||
                Tokens[IndexAtual].Tipo == TipoToken.OperadorSubtracao)
                IndexAtual++;
        }

        private void OutrosTermos()
        {
            if (Tokens[IndexAtual].Tipo == TipoToken.OperadorSoma ||
                Tokens[IndexAtual].Tipo == TipoToken.OperadorSubtracao)
            {
                OpAd();
                Termo();
                OutrosTermos();
            }
        }

        private void OpAd()
        {
            if (Tokens[IndexAtual].Tipo == TipoToken.OperadorSoma ||
                Tokens[IndexAtual].Tipo == TipoToken.OperadorSubtracao)
            {
                IndexAtual++;
            }
        }

        private void Termo()
        {
            OpUn();
            Fator();
            MaisFatores();
        }

        private void MaisFatores()
        {
            if (Tokens[IndexAtual].Tipo == TipoToken.OperadorMultiplicacao ||
                Tokens[IndexAtual].Tipo == TipoToken.OperadorDivisao)
            {
                OpMul();
                Fator();
                MaisFatores();
            }
        }

        private void OpMul()
        {
            if (Tokens[IndexAtual].Tipo == TipoToken.OperadorMultiplicacao ||
                Tokens[IndexAtual].Tipo == TipoToken.OperadorDivisao)
            {
                IndexAtual++;
            }
        }

        private void Fator()
        {
            switch (Tokens[IndexAtual].Tipo)
            {
                case TipoToken.ReservadoIdent:
                    break;
                case TipoToken.ReservadoNumeroInt:
                    break;
                case TipoToken.ReservadoNumeroReal:
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
        }
    }
}