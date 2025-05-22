namespace Packbuilder.Exceptions
{
    public class NotFoundException<T>() : BaseException($"{nameof(T)} not found");
}