namespace StoreApp.Comman.Exceptions;

public class ValidationErrorException : System.Exception
{
    public ValidationErrorException(string message) : base(message)
    {
    }
}
