using System;

namespace Desafio.Leve.Domain.Models
{
  // Domain-level lightweight user representation (no Identity dependency)
  public class ApplicationUser
  {
    public string Id { get; set; }
    public string FullName { get; set; }
  }
}
