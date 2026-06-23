namespace MyAPP.API.Models;

public class ApiErrorResponse
{
    public string Error { get; set; } = string.Empty;
    public string? Detail { get; set; }
}
