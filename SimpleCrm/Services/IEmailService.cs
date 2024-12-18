namespace SimpleCrm.Services
{
    public interface IEmailService
    {

        Task SendEmailAsync(List<string> to, string subject, string html, string from = null);

    }
}
