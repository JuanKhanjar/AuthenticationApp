namespace AuthenticationApp.Domain.Events
{
    /// <summary>
    /// حدث يتم إطلاقه عند تسجيل مستخدم جديد
    /// </summary>
    public class UserRegisteredDomainEvent : IDomainEvent
    {
        public string UserId { get; }
        public string Email { get; }
        public string FullName { get; }
        public string ConfirmationToken { get; }

        public UserRegisteredDomainEvent(string userId, string email, string fullName, string confirmationToken)
        {
            UserId = userId;
            Email = email;
            FullName = fullName;
            ConfirmationToken = confirmationToken;
        }
    }
}
