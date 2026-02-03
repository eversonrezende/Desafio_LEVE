namespace Desafio.Leve.Infrastructure.Services
{
  public class EmailOptions
  {
    public string Host { get; set; } = "";
    public int Port { get; set; } = 25;
    public bool UseSsl { get; set; } = true;
    public string User { get; set; } = "";
    public string Password { get; set; } = "";
    public string From { get; set; } = "no-reply@localhost";
    public string FromName { get; set; } = "Desafio LEVE";
  }
}
