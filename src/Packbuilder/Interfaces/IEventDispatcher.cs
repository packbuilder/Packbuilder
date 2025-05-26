namespace Packbuilder.Interfaces;

public interface IEventDispatcher
{
    Task PublishAsync<T>(T @event);
}