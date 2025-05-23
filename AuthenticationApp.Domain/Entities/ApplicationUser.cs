
using AuthenticationApp.Domain.Events;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationApp.Domain.Entities
{
    /// <summary>
    /// الكيان الذي يمثل المستخدم في النظام
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // أحداث نطاق مرتبطة بالمستخدم
        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void RegisterUser(string confirmationToken)
        {
            var @event = new UserRegisteredDomainEvent(this.Id, this.Email!, this.FullName, confirmationToken);
            _domainEvents.Add(@event);
        }
    }
}
