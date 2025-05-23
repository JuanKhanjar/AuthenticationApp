namespace AuthenticationApp.Domain.Interfaces
{
    /// <summary>
    /// واجهة لإرسال البريد الإلكتروني
    /// </summary>
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
