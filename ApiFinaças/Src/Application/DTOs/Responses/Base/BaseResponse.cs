namespace ApiFinancas.Src.Application.DTOs.Responses.Base
{
    public class BaseResponse<T>
    {
        public bool Sucess {  get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
        public string TraceId { get; set; } = Guid.NewGuid().ToString();
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;


        public static BaseResponse<T> Ok(T data, string? message = null) 
            => new() { Sucess = true, Message = message };

        public static BaseResponse<T> Fail(List<string> errors, string? message = null)
            => new() { Sucess =  false, Errors = errors, Message = message };

        public static BaseResponse<T> Fail(string error, string? message = null)
            => new() { Sucess = false, Errors = new List<string> { error }, Message = message };

    }
}
