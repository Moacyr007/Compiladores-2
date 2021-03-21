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
            tokens.Should().Equal(new List<Token>(){new Token("teste", Identificador)});
        }
        
        [Fact]
        public void Entrada_Identificadores_Validos() 
        {
            //Arrange
            var analisadorLexico = new AnalisadorLexico("if teste");

            //Act
            var tokens = analisadorLexico.Analisar();

            //Assert
            tokens.Should().Equal(new List<Token>(){new Token("if", ReservadoIf), new Token("teste", Identificador)});
        }
        
        [Fact]
        public void Entrada_Identificador_Invalido()
        {
            //Arrange
            var analisadorLexico = new AnalisadorLexico("13212dasd");

            //Act
            Action analisar = () => analisadorLexico.Analisar();

            //Assert
            analisar.Should().Throw<AnalisadorLexicoException>().WithMessage("Identificador Inválido: 13212dasd \n Linha: 1");
        }
        
    }
}