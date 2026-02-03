using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;

namespace Desafio.Leve.Infrastructure.Services
{
  public class MailKitEmailSender : IEmailSender
  {
    private readonly EmailOptions _options;

    public MailKitEmailSender(IOptions<EmailOptions> options)
    {
      _options = options.Value;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlMessage)
    {
      var msg = new MimeMessage();
      msg.From.Add(new MailboxAddress(_options.FromName, _options.From));
      msg.To.Add(MailboxAddress.Parse(to));
      msg.Subject = subject;

      var body = new BodyBuilder { HtmlBody = htmlMessage };
      msg.Body = body.ToMessageBody();

      using var client = new SmtpClient();
      var secure = _options.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;
      await client.ConnectAsync(_options.Host, _options.Port, secure);
      if (!string.IsNullOrEmpty(_options.User))
        await client.AuthenticateAsync(_options.User, _options.Password);
      await client.SendAsync(msg);
      await client.DisconnectAsync(true);
    }
  }
}
