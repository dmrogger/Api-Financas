using System.Text.Json.Serialization;

namespace ApiFinancas.Src.Application.DTOs.Common
{
    public class Result<T>
    {
        public bool Success { get; private set; }
        public string? Error { get; private set; }
        public T? Data { get; private set; }

        /// <summary>
        /// Tipo da falha (nulo quando Success = true). Uso interno: o cliente recebe essa informação pelo status HTTP
        /// </summary>
        [JsonIgnore]
        public ETipoErro? TipoErro { get; private set; }

        private Result(bool success, T? data, string? error, ETipoErro? tipoErro)
        {
            Success = success;
            Data = data;
            Error = error;
            TipoErro = tipoErro;
        }

        public static Result<T> Ok(T data)
            => new Result<T>(true, data, null, null);

        public static Result<T> Fail(string error, ETipoErro tipoErro = ETipoErro.Validacao)
            => new Result<T>(false, default, error, tipoErro);
    }
}
