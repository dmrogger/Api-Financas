using ApiFinaças.Src.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ApiFinaças.Src.Application.DTOs.Requests.Movimentacoes
{
    public class ExcluirOperacoesRequest : BaseRequest
    {
        public ExcluirOperacoesRequest()
        {

        }

        [Required(ErrorMessage = "Id da operação necessário")]
        public Guid IdOperacao { get; set; }

        [Required(ErrorMessage = "Tipo da operação necessário")]
        public ETipoOperacao TipoOperacao { get; set; }
    }
}
