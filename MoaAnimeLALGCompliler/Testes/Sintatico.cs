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
            var analisadorLexico = new AnalisadorLexico(Helper.GetExemploCorretoLalg());
                  
            analisadorLexico.Analisar();
            var analisadorSintatico = new AnalisadorSintatico(analisadorLexico.Tokens);

            //Act
            Action analisar = () => analisadorSintatico.Analisar();

            //Assert
            analisar.Should().NotThrow();
        }
        
        [Fact]
        public void EntradaInvalida()
        {
            //Arrange
            var analisadorLexico = new AnalisadorLexico(Helper.GetExemploIncorretoLalg());
                  
            analisadorLexico.Analisar();
            var analisadorSintatico = new AnalisadorSintatico(analisadorLexico.Tokens);

            //Act
            Action analisar = () => analisadorSintatico.Analisar();

            //Assert
            analisar.Should().Throw<CompiladorException>();
        }
    }
}