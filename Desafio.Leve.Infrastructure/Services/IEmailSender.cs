using System.Threading.Tasks;

namespace Desafio.Leve.Infrastructure.Services
{
  public interface IEmailSender
  {
    Task SendEmailAsync(string to, string subject, string htmlMessage);
  }
}
