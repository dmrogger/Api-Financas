using ApiFinancas.Src.Application.DTOs.Requests;
using System.ComponentModel.DataAnnotations;

namespace ApiFinancas.Src.Application.DTOs.Requests.Movimentacoes
{
    public class ObterOperacoesRequest : BaseRequest
    {
        public DateTime DataInicial { get; set; }

        public DateTime DataFinal { get; set; }

    }
}
