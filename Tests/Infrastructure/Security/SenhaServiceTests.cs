using ApiFinancas.Src.Infrastructure.Security;

namespace ApiFinancas.Tests.Infrastructure.Security
{
    public class SenhaServiceTests
    {
        private readonly SenhaService _senhaService = new SenhaService();

        [Fact(DisplayName = "Deve gerar hash diferente da senha")]
        public void DeveGerarHashDiferenteDaSenha()
        {
            var hash = _senhaService.HashSenha("Senha@123");

            Assert.False(string.IsNullOrWhiteSpace(hash));
            Assert.NotEqual("Senha@123", hash);
        }

        [Fact(DisplayName = "Deve gerar hashes diferentes para a mesma senha")]
        public void DeveGerarHashesDiferentesParaMesmaSenha()
        {
            var primeiro = _senhaService.HashSenha("Senha@123");
            var segundo = _senhaService.HashSenha("Senha@123");

            Assert.NotEqual(primeiro, segundo);
        }

        [Fact(DisplayName = "Deve validar a senha correta")]
        public void DeveValidarSenhaCorreta()
        {
            var hash = _senhaService.HashSenha("Senha@123");

            Assert.True(_senhaService.ValidaSenha("Senha@123", hash));
        }

        [Fact(DisplayName = "Não deve validar a senha incorreta")]
        public void NaoDeveValidarSenhaIncorreta()
        {
            var hash = _senhaService.HashSenha("Senha@123");

            Assert.False(_senhaService.ValidaSenha("SenhaErrada@123", hash));
        }
    }
}
