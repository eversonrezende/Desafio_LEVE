using Desafio.Leve.Domain.Models;

namespace Desafio.Leve.Web.Models
{
  public class TaskItemViewModel
  {
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public bool IsCompleted { get; set; }
    public string? AssignedToUserId { get; set; }
    public string? AssignedToUserName { get; set; }
    public string? CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public static TaskItemViewModel FromTaskItem(TaskItem task, string? assignedUserName)
    {
      return new TaskItemViewModel
      {
        Id = task.Id,
        Title = task.Title,
        Description = task.Description,
        DueDate = task.DueDate,
        IsCompleted = task.IsCompleted,
        AssignedToUserId = task.AssignedToUserId,
        AssignedToUserName = assignedUserName,
        CreatedByUserId = task.CreatedByUserId,
        CreatedAt = task.CreatedAt
      };
    }
  }
}
