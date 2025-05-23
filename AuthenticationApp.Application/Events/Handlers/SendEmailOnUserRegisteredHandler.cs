using AuthenticationApp.Application.Interfaces;
using AuthenticationApp.Domain.Events;
using AuthenticationApp.Domain.Interfaces;
using System.Net;

namespace AuthenticationApp.Application.Events.Handlers
{
    public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
    {
        Task Handle(TEvent domainEvent, CancellationToken cancellationToken);
    }


    /// <summary>
    /// يعالج حدث UserRegisteredDomainEvent ويقوم بإرسال بريد تأكيد
    /// </summary>
    public class SendEmailOnUserRegisteredHandler : IDomainEventHandler<UserRegisteredDomainEvent>
    {
        private readonly IEmailSender _emailSender;
        private readonly IJwtTokenGenerator _tokenGenerator;

        public SendEmailOnUserRegisteredHandler(IEmailSender emailSender, IJwtTokenGenerator tokenGenerator)
        {
            _emailSender = emailSender;
            _tokenGenerator = tokenGenerator;
        }

        public async Task Handle(UserRegisteredDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var confirmLink = $"https://localhost:7242/api/auth/confirm-email?userId={domainEvent.UserId}&token={WebUtility.UrlEncode(domainEvent.ConfirmationToken)}";

            var htmlBody = $"""
                <h2>Welcome, {domainEvent.FullName}</h2>
                <p>Thanks for registering. Please confirm your email:</p>
                <a href='{confirmLink}' style='color: white; background-color: #4CAF50; padding: 10px 15px; text-decoration: none;'>Confirm Email</a>
            """;

            await _emailSender.SendEmailAsync(domainEvent.Email, "Confirm your email", htmlBody);
        }
    }
}
