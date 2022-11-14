using System.Threading.Tasks;

namespace Szpek.Infrastructure.Email
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
