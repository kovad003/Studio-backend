namespace Application.Core;

public class AppException
{
    public AppException(int statusCode, string message, string info = null)
    {
        StatusCode = statusCode;
        Message = message;
        Info = info;
    }

    public int StatusCode { get; set; }
    public string Message { get; set; }
    public string Info { get; set; }
}