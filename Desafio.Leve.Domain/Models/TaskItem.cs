using System;

namespace Desafio.Leve.Domain.Models
{
  public class TaskItem
  {
    public int Id { get; set; }

    [System.ComponentModel.DataAnnotations.Required]
    public string Title { get; set; }

    [System.ComponentModel.DataAnnotations.Required]
    public string Description { get; set; }

    [System.ComponentModel.DataAnnotations.Required]
    public DateTime DueDate { get; set; }

    public bool IsCompleted { get; set; }

    public string? AssignedToUserId { get; set; }

    public string? CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  }
}
