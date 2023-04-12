namespace Innoloft.Shared.Dtos;

public class Response<T>
{
    public string Status { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }

    public static Response<T> Success(T data) => new Response<T> { Status = "success", Data = data, Message = null };

    public static Response<T> Failure(string message) => new Response<T> { Status = "failure", Data = default, Message = message };
}
