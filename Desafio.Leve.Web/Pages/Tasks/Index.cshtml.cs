using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Desafio.Leve.Domain.Models;
using Desafio.Leve.Infrastructure;
using Desafio.Leve.Infrastructure.Services;
using Desafio.Leve.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Desafio.Leve.Web.Pages.Tasks
{
  [Authorize]
  public class IndexModel : PageModel
  {
    private readonly AppDbContext _db;
    private readonly IEmailSender _emailSender;
    private readonly Microsoft.Extensions.Logging.ILogger<IndexModel> _logger;

    public IndexModel(AppDbContext db, IEmailSender emailSender, Microsoft.Extensions.Logging.ILogger<IndexModel> logger) => (_db, _emailSender, _logger) = (db, emailSender, logger);

    public List<TaskItemViewModel> Tasks { get; set; } = new List<TaskItemViewModel>();

    public async Task OnGetAsync()
    {
      var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);

      List<TaskItem> tasks;
      if (User.IsInRole("Gestor"))
      {
        tasks = await _db.Tasks
          .Where(t => t.CreatedByUserId == userId || t.AssignedToUserId == userId)
          .OrderByDescending(t => t.CreatedAt)
          .ToListAsync();
      }
      else
      {
        tasks = await _db.Tasks
          .Where(t => t.AssignedToUserId == userId)
          .OrderByDescending(t => t.CreatedAt)
          .ToListAsync();
      }

      // Load user names for assigned users
      var userIds = tasks.Where(t => t.AssignedToUserId != null).Select(t => t.AssignedToUserId).Distinct().ToList();
      var users = await _db.Users
        .Where(u => userIds.Contains(u.Id))
        .ToDictionaryAsync(u => u.Id, u => u.FullName);

      Tasks = tasks.Select(t => TaskItemViewModel.FromTaskItem(
        t,
        t.AssignedToUserId != null && users.ContainsKey(t.AssignedToUserId) ? users[t.AssignedToUserId] : null
      )).ToList();
    }

    public async Task<IActionResult> OnPostCompleteAsync(int id)
    {
      var task = await _db.Tasks.FindAsync(id);
      if (task == null) return NotFound();

      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      if (task.AssignedToUserId != userId)
      {
        TempData["ErrorMessage"] = "Acesso negado. Apenas o responsável pela tarefa pode marcá-la como concluída.";
        return RedirectToPage();
      }

      if (task.IsCompleted) return RedirectToPage();

      task.IsCompleted = true;
      await _db.SaveChangesAsync();

      // Notify the manager/creator
      var manager = await _db.Users.FindAsync(task.CreatedByUserId);
      if (manager != null && !string.IsNullOrWhiteSpace(manager.Email))
      {
        var subject = $"Tarefa concluída: {task.Title}";
        var body = $"<p>A tarefa '<strong>{task.Title}</strong>' atribuída a <strong>{userId}</strong> foi marcada como concluída.</p>" +
                   $"<p><strong>Descrição:</strong> {task.Description}</p>" +
                   $"<p><strong>Data limite:</strong> {task.DueDate:O}</p>";

        try
        {
          await _emailSender.SendEmailAsync(manager.Email, subject, body);
        }
        catch
        {
          // swallow send errors for now; consider logging
        }
      }

      return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
      var task = await _db.Tasks.FindAsync(id);
      if (task == null) return NotFound();

      // Apenas gestores podem deletar tarefas
      if (!User.IsInRole("Gestor"))
      {
        TempData["ErrorMessage"] = "Acesso negado. Apenas gestores podem excluir tarefas.";
        return RedirectToPage();
      }

      // Apenas tarefas não concluídas podem ser deletadas
      if (task.IsCompleted)
      {
        TempData["ErrorMessage"] = "Não é possível excluir tarefas concluídas.";
        return RedirectToPage();
      }

      // Apenas o criador da tarefa pode deletá-la
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      if (task.CreatedByUserId != userId)
      {
        TempData["ErrorMessage"] = "Acesso negado. Apenas o criador da tarefa pode excluí-la.";
        return RedirectToPage();
      }

      _db.Tasks.Remove(task);
      await _db.SaveChangesAsync();

      TempData["SuccessMessage"] = "Tarefa excluída com sucesso.";
      return RedirectToPage();
    }
  }
}
