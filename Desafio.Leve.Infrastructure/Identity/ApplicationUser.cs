using System;
using Microsoft.AspNetCore.Identity;

namespace Desafio.Leve.Infrastructure.Identity
{
  public class ApplicationUser : IdentityUser
  {
    public string? FullName { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? PhoneFixed { get; set; }
    public string? PhoneMobile { get; set; }
    public string? Address { get; set; }
    public string? PhotoPath { get; set; }

    // ID do gestor que criou este usuário
    public string? CreatedById { get; set; }

    // Status do usuário (ativo/inativo)
    public bool IsActive { get; set; } = true;
  }
}
