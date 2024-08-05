namespace SpotifyAPI.Models.Errors;

public class ErrorResponse
{
    public string Message { get; set; } = null!;
    public int StatusCode { get; set; }
}