namespace com.epam.rp.core.Models;

public class ErrorResponse
{
    public int Status { get; set; }
    public string? Error { get; set; }
    public string? Message { get; set; }
}