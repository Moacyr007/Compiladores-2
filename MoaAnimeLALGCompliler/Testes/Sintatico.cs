using System;
using Compilador;
using FluentAssertions;
using Xunit;

namespace Testes
{
    public class Sintatico
    {
        [Fact]
        public void EntradaValida()
        {
            //Arrange
            //var entrada = System.IO.File.ReadAllText(@"Testes/EntradaCorreta.txt");
            //System.IO.File.WriteAllText("entrada.txt", "if numero > 1\n123abc");
            var analisadorLexico = new AnalisadorLexico(@"
                  program teste

                  /* declaracao de variaveis */
                  var a,b,c: integer;
                  var d,e,f: real;
                  var g,h : integer;

                  /* declaracao de procedimentos */
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

                  /*  corpo * principal / */

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
            
            analisadorLexico.Analisar();
            var analisadorSintatico = new AnalisadorSintatico(analisadorLexico.Tokens);

            //Act
            Action analisar = () => analisadorSintatico.Analisar();

            //Assert
            analisar.Should().NotThrow();
        }
    }
}