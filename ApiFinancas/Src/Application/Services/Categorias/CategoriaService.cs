using ApiFinancas.Src.Application.DTOs.Common;
using ApiFinancas.Src.Application.DTOs.Requests.Categorias;
using ApiFinancas.Src.Application.DTOs.Responses.Categorias;
using ApiFinancas.Src.Application.Interfaces.Categorias;
using ApiFinancas.Src.Domain.Entities;
using ApiFinancas.Src.Domain.Interfaces;

namespace ApiFinancas.Src.Application.Services.Categorias
{
    /// <summary>
    /// Serviço de categorias. Regras:
    /// - o usuário vê as categorias padrão e as próprias;
    /// - só altera ou exclui as próprias (padrão → 403; de outro usuário → 404, para não revelar que existe);
    /// - o nome não pode repetir o de outra categoria visível ao usuário.
    /// </summary>
    public class CategoriaService : ICategoriaService
    {
        public const string MensagemNaoEncontrada = "Categoria não encontrada.";
        public const string MensagemCategoriaPadrao = "Categorias padrão não podem ser alteradas nem excluídas.";
        public const string MensagemNomeDuplicado = "Já existe uma categoria com esse nome.";

        private readonly ICategoriaRepository _categoriaRepository;

        public CategoriaService(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        public async Task<Result<List<CategoriaResponse>>> ListarAsync(Guid idUsuario)
        {
            var categorias = await _categoriaRepository.ListarVisiveisParaUsuarioAsync(idUsuario);

            return Result<List<CategoriaResponse>>.Ok(categorias
                .Select(c => new CategoriaResponse(c))
                .ToList());
        }

        public async Task<Result<CategoriaResponse>> CriarAsync(CategoriaRequest request, Guid idUsuario)
        {
            var nome = request.Nome.Trim();

            if (await _categoriaRepository.ExisteNomeVisivelParaUsuarioAsync(idUsuario, nome))
                return Result<CategoriaResponse>.Fail(MensagemNomeDuplicado, ETipoErro.Conflito);

            var categoria = new Categoria(nome, idUsuario);
            await _categoriaRepository.AdicionarAsync(categoria);

            return Result<CategoriaResponse>.Ok(new CategoriaResponse(categoria));
        }

        public async Task<Result<CategoriaResponse>> EditarAsync(Guid idCategoria, CategoriaRequest request, Guid idUsuario)
        {
            var busca = await ObterCategoriaEditavelAsync(idCategoria, idUsuario);
            if (!busca.Success)
                return Result<CategoriaResponse>.Fail(busca.Error!, busca.TipoErro!.Value);

            var categoria = busca.Data!;
            var nome = request.Nome.Trim();

            if (await _categoriaRepository.ExisteNomeVisivelParaUsuarioAsync(idUsuario, nome, idCategoria))
                return Result<CategoriaResponse>.Fail(MensagemNomeDuplicado, ETipoErro.Conflito);

            categoria.Renomear(nome);
            await _categoriaRepository.AtualizarAsync(categoria);

            return Result<CategoriaResponse>.Ok(new CategoriaResponse(categoria));
        }

        public async Task<Result<string>> DeletarAsync(Guid idCategoria, Guid idUsuario)
        {
            var busca = await ObterCategoriaEditavelAsync(idCategoria, idUsuario);
            if (!busca.Success)
                return Result<string>.Fail(busca.Error!, busca.TipoErro!.Value);

            // As movimentações dessa categoria ficam "Sem categoria" (FK com ON DELETE SET NULL)
            await _categoriaRepository.DeletarAsync(busca.Data!);

            return Result<string>.Ok("Categoria excluída com sucesso!");
        }

        /// <summary>
        /// Busca uma categoria que o usuário pode alterar, ou o motivo de não poder
        /// </summary>
        private async Task<Result<Categoria>> ObterCategoriaEditavelAsync(Guid idCategoria, Guid idUsuario)
        {
            var categoria = await _categoriaRepository.ObterPorIdAsync(idCategoria);

            if (categoria == null || !categoria.VisivelPara(idUsuario))
                return Result<Categoria>.Fail(MensagemNaoEncontrada, ETipoErro.NaoEncontrado);

            if (categoria.Padrao)
                return Result<Categoria>.Fail(MensagemCategoriaPadrao, ETipoErro.Proibido);

            return Result<Categoria>.Ok(categoria);
        }
    }
}
