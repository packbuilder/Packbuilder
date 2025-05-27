namespace Packbuilder.Exceptions
{
    public class NotFoundException<T>() : ExceptionBase($"{nameof(T)} not found");
}