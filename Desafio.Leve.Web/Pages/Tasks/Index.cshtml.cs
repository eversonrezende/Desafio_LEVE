using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Desafio.Leve.Domain.Models;
using Desafio.Leve.Infrastructure;
using Desafio.Leve.Infrastructure.Services;
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

    public List<TaskItem> Tasks { get; set; } = new List<TaskItem>();

    public async Task OnGetAsync()
    {
      var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
      if (User.IsInRole("Gestor"))
      {
        // show tasks created by this manager or assigned to them
        Tasks = await _db.Tasks
          .Where(t => t.CreatedByUserId == userId || t.AssignedToUserId == userId)
          .OrderByDescending(t => t.CreatedAt)
          .ToListAsync();
      }
      else
      {
        // subordinate: only tasks assigned to them
        Tasks = await _db.Tasks
          .Where(t => t.AssignedToUserId == userId)
          .OrderByDescending(t => t.CreatedAt)
          .ToListAsync();
      }
    }

    public async Task<IActionResult> OnPostCompleteAsync(int id)
    {
      var task = await _db.Tasks.FindAsync(id);
      if (task == null) return NotFound();

      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      if (task.AssignedToUserId != userId)
      {
        return Forbid();
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
  }
}
