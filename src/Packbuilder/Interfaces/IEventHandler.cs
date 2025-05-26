namespace Packbuilder.Interfaces; 

public interface IEventHandler<T>
{
    public Task Handle(T @event);
}