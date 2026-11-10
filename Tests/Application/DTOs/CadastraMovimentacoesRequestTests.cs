using ApiFinancas.Src.Application.DTOs.Requests.Movimentacoes;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace ApiFinancas.Tests.Application.DTOs
{
    /// <summary>
    /// Garante que a validação do Valor funciona independente da cultura do servidor.
    /// Em pt-BR o separador decimal é vírgula, e o limite "0.01" do [Range] não podia ser convertido
    /// ("0.01 is not a valid value for Decimal"), quebrando todo cadastro de movimentação.
    /// </summary>
    public class CadastraMovimentacoesRequestTests
    {
        /// <summary>
        /// GetCustomAttribute devolve uma instância nova a cada chamada. Isso importa porque o RangeAttribute
        /// converte os limites só na primeira validação e guarda o resultado: reaproveitar a instância
        /// esconderia o problema se outra cultura tivesse validado antes.
        /// </summary>
        private static RangeAttribute NovoAtributoDoValor() =>
            typeof(CadastraMovimentacoesRequest)
                .GetProperty(nameof(CadastraMovimentacoesRequest.Valor))!
                .GetCustomAttribute<RangeAttribute>()!;

        private static bool ValidarNaCultura(decimal valor, string cultura)
        {
            var culturaOriginal = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo(cultura);
                return NovoAtributoDoValor().IsValid(valor);
            }
            finally
            {
                CultureInfo.CurrentCulture = culturaOriginal;
            }
        }

        [Theory]
        [InlineData("pt-BR")]
        [InlineData("en-US")]
        [InlineData("")] // cultura invariante (Docker)
        public void Valor_Positivo_DevePassarEmQualquerCultura(string cultura)
        {
            Assert.True(ValidarNaCultura(150.50m, cultura));
            Assert.True(ValidarNaCultura(0.01m, cultura));
        }

        [Theory]
        [InlineData("pt-BR")]
        [InlineData("en-US")]
        [InlineData("")]
        public void Valor_MenorQueUmCentavo_DeveSerInvalidoEmQualquerCultura(string cultura)
        {
            Assert.False(ValidarNaCultura(0m, cultura));
            Assert.False(ValidarNaCultura(0.009m, cultura));
        }
    }
}
