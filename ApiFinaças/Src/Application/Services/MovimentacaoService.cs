using ApiFinaças.Src.Application.DTOs.Requests;
using ApiFinaças.Src.Application.DTOs.Responses;
using ApiFinaças.Src.Domain.Entities;
using ApiFinaças.Src.Infrastructure.Repositories.Interfaces;

namespace ApiFinaças.Src.Application.Services
{
    public class MovimentacaoService
    {
        private readonly IMovimentacaoRepository _movimentacaoRepository;

        public MovimentacaoService(IMovimentacaoRepository movimentacaoRepository)
        {
            _movimentacaoRepository = movimentacaoRepository;
        }

        public Task<AdicionarEntradaResponse>CriaMovimentacaoEntrada(AdicionarEntradaRequest request)
        {
            {
                var movimentacao = new Movimentacao(request.Valor,request.DataRequisicao,Guid.NewGuid(),request.idUsuario);
               
                var response = _movimentacaoRepository.CriarMovimentacao(movimentacao);

                return null;
            }
        }
    }
}
