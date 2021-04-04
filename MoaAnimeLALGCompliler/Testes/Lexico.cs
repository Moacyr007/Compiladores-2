using System;
using System.Collections.Generic;
using Compilador;
using FluentAssertions;
using Xunit;
using Xunit.Sdk;
using static Compilador.TipoToken;

namespace Testes
{
    public class Lexico
    {
        [Fact]
        public void Entrada_vazia_retorna_nenhum_token()
        {
            //Arrange
            var analisadorLexico = new AnalisadorLexico(string.Empty);

            //Act
            var tokens = analisadorLexico.Analisar();

            //Assert
            tokens.Should().Equal(new List<Token>());
        }

        [Fact]
        public void Entrada_Identificador_Valido()
        {
            //Arrange
            var analisadorLexico = new AnalisadorLexico("teste");

            //Act
            var tokens = analisadorLexico.Analisar();

            //Assert
            tokens.Should().Equal(new List<Token>() {new Token("teste", Identificador)});
        }

        [Fact]
        public void Entrada_Identificadores_Validos()
        {
            //Arrange
            var analisadorLexico = new AnalisadorLexico("if teste");

            //Act
            var tokens = analisadorLexico.Analisar();

            //Assert
            tokens.Should().Equal(new List<Token>() {new Token("if", ReservadoIf), new Token("teste", Identificador)});
        }

        [Fact]
        public void Entrada_Identificador_Invalido()
        {
            //Arrange
            var analisadorLexico = new AnalisadorLexico("13212dasd");

            //Act
            Action analisar = () => analisadorLexico.Analisar();

            //Assert
            analisar.Should().Throw<AnalisadorLexicoException>()
                .WithMessage("Identificador Inválido: 13212dasd \n Linha: 1");
        }
        
        [Fact]
        public void Entrada_Identificador_Invalido_Linha2()
        {
            //Arrange
            System.IO.File.WriteAllText("entrada.txt", "if numero > 1\n123abc");
            var textoArquivo = System.IO.File.ReadAllText("entrada.txt");
            var analisadorLexico = new AnalisadorLexico(textoArquivo);
            
            //Act
            Action analisar = () => analisadorLexico.Analisar();

            //Assert
            analisar.Should().Throw<AnalisadorLexicoException>()
                .WithMessage("Identificador Inválido: 123abc \n Linha: 2");
        }

        /*[Fact]
        public void Entrada_Lslg_Correto()
        {
            //Arrange
            var analisadorLexico = new AnalisadorLexico(@"program teste

                /* declaracao de variaveis #1#
                var a,b,c: integer;
                var d,e,f: real;
                var g,h : integer;

                /* declaracao de procedimentos #1#
                procedure um (a, g: real; d, c: integer)
                  var h, i, j: real;
                  var l: integer
                begin
                  h := 2.0;
                  a := g + 3.4 / h;
                  l := c - d * 2;
                  if (c+d)>=5 then
                    write(a)
                  else
                    write(l)
                  $
                end;

                procedure dois (j: integer; k: real; l: integer)
                  var cont,quant: integer
                begin
                  read(quant);
                  while cont <= quant do
                     write(cont)
                  $;
                  l := l + j + cont;
                  write(k);
                  write(l)
                end

                /*  corpo * principal / #1#

                begin
                read(e); {real}
                read(f); {real}
                read(g); {inteiro}
                read(h); { inteiro} 
                d := e/f; {real}
                dois(h;d;h);
                um(f;e;g;h)         {real,real,inteiro,inteiro}
                {aqui termina o programa}
                end.");

            //Act
            Action analisar = () => analisadorLexico.Analisar();

            //Assert
            //TODO
            analisar.Should().BeNull();
        }*/
    }
}