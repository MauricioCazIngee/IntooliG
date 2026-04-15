namespace IntooliG.Application.Common.Models;

/// <summary>
/// Envelope estándar para respuestas JSON (success, data, message, status).
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string Message { get; init; } = string.Empty;
    public int Status { get; init; }

    public static ApiResponse<T> Ok(T data, string message = "OK", int status = 200) =>
        new() { Success = true, Data = data, Message = message, Status = status };

    public static ApiResponse<T> Fail(string message, int status = 400, T? data = default) =>
        new() { Success = false, Data = data, Message = message, Status = status };
}
