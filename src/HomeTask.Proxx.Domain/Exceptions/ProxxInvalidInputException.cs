namespace HomeTask.Proxx.Domain.Exceptions;

public class ProxxInvalidInputException : Exception
{
    public ProxxInvalidInputException(string? message) : base(message)
    {
    }
}
