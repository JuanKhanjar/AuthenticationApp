using AuthenticationApp.Domain.Events;

namespace AuthenticationApp.Application.Common
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
    }
}
