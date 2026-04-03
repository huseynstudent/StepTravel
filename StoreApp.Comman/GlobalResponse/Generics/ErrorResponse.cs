namespace StoreApp.Comman.GlobalResponse.Generics;

public class ErrorResponse
{

    public int Code { get; set; }
    public string Message { get; set; }
    public string ErrorType { get; set; }
    public ErrorResponse(string message, ErrorTypes errorType)
    {
        Code = (int)errorType;
        Message = message;
        ErrorType = errorType.ToString();
    }

    public ErrorResponse()
    {

    }

    public enum ErrorTypes
    {
        BadRequest = 1,
        NotFound = 2,
        ValidationError = 3
    }
}