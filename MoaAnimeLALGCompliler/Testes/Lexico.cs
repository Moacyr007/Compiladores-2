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
            var analisadorLexico = new AnalisadorLexico("aasd13212dasd");

            //Act
            var tokens = analisadorLexico.Analisar();

            //Assert
            tokens.Should().Equal(new List<Token>(){new Token("aasd13212dasd", Identificador)});
        }
        
        [Fact]
        public void Entrada_Identificador_Invalido()
        {
            //Arrange
            var analisadorLexico = new AnalisadorLexico("13212dasd");

            //Act
            Action analisar = () => analisadorLexico.Analisar();

            //Assert
            analisar.Should().Throw<Exception>();
        }
        
    }
}