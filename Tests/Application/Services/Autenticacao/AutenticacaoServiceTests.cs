using ApiFinancas.Src.Application.DTOs.Autenticacao;
using ApiFinancas.Src.Application.Services.Autenticacao;
using ApiFinancas.Src.Domain.Entities;
using ApiFinancas.Src.Domain.Interfaces;
using BCrypt.Net;
using Castle.Core.Configuration;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiFinancas.Tests.Application.Services.Autenticacao
{
    public class AutenticacaoServiceTests
    {
        private readonly Mock<IUsuarioRepository> _repositoryMock;
        private readonly AutenticacaoService _autenticaoservice;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        public AutenticacaoServiceTests()
        {
            _repositoryMock = new Mock<IUsuarioRepository>();
            var settings = new Dictionary<string, string>
            {
                { "Jwt:Key", "uma-chave-super-secreta-para-testes" },
                { "Jwt:Issuer", "ApiFinancas" },
                { "Jwt:Audience", "ApiFinancasUsers" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            _autenticaoservice = new AutenticacaoService(_repositoryMock.Object, _configuration);
        }

        [Fact(DisplayName = "Deve retornar login com usuário válido")]
        public async Task LoginDeveRetornarUsuarioValido()
        {
            var senhaHash = BCrypt.Net.BCrypt.HashPassword("123456");
            var usuario = new Usuario("Teste", "teste@gmail.com", senhaHash);

            _repositoryMock.Setup(x => x.ObterPorEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(usuario);

            var login = new LoginRequest
            {
                Email = "teste@gmail.com",
                Senha = "123456"
            };

            var result = await _autenticaoservice.LoginAsync(login);


            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(result.Data.Email, login.Email);
        }

        [Fact(DisplayName = "Deve retornar um login inválido")]
        public async Task LoginDeveRetornarInvalido()
        {
            var senhaHash = BCrypt.Net.BCrypt.HashPassword("654321");
            var usuario = new Usuario("Teste", "teste@gmail.com", senhaHash);

            _repositoryMock.Setup(x => x.ObterPorEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(usuario);

            var login = new LoginRequest
            {
                Email = "teste@gmail.com",
                Senha = "123456" //Senha Incorreta para verificação 
            };

            var result = await _autenticaoservice.LoginAsync(login);

            var esperado = "Usuário ou senha inválidos";

            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(result.Error, esperado);
        }
    }
}
