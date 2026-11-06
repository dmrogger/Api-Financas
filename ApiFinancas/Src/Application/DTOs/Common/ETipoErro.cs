namespace ApiFinancas.Src.Application.DTOs.Common
{
    /// <summary>
    /// Classifica a falha de um Result para o controller escolher o status HTTP adequado
    /// </summary>
    public enum ETipoErro
    {
        /// <summary>Dados inválidos (400)</summary>
        Validacao = 1,

        /// <summary>Recurso inexistente ou de outro usuário (404)</summary>
        NaoEncontrado = 2,

        /// <summary>Recurso visível, mas que o usuário não pode alterar (403)</summary>
        Proibido = 3,

        /// <summary>Conflito com dado já existente, como nome duplicado (409)</summary>
        Conflito = 4
    }
}
