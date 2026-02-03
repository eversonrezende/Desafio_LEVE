using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Desafio.Leve.Domain.Models;
using Desafio.Leve.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Desafio.Leve.Web.Pages.Tasks
{
  [Authorize]
  public class CreateModel : PageModel
  {
    private readonly AppDbContext _db;
    private readonly Desafio.Leve.Infrastructure.Services.IEmailSender _emailSender;
    private readonly Microsoft.Extensions.Logging.ILogger<CreateModel> _logger;

    public CreateModel(AppDbContext db, Desafio.Leve.Infrastructure.Services.IEmailSender emailSender, Microsoft.Extensions.Logging.ILogger<CreateModel> logger)
      => (_db, _emailSender, _logger) = (db, emailSender, logger);

    [BindProperty]
    public TaskItem TaskItem { get; set; } = new TaskItem();

    public Microsoft.AspNetCore.Mvc.Rendering.SelectList? UsersSelect { get; set; }

    public async Task OnGetAsync()
    {
      TaskItem.DueDate = DateTime.UtcNow.AddDays(7);
      await LoadUsersAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
      if (!ModelState.IsValid)
      {
        _logger.LogWarning("ModelState inválido ao criar tarefa: {Errors}", string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        await LoadUsersAsync();
        return Page();
      }

      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
      TaskItem.CreatedAt = DateTime.UtcNow;
      TaskItem.IsCompleted = false;
      TaskItem.CreatedByUserId = userId;
      if (string.IsNullOrWhiteSpace(TaskItem.AssignedToUserId))
        TaskItem.AssignedToUserId = userId;

      _db.Tasks.Add(TaskItem);
      await _db.SaveChangesAsync();

      // send notification email to assigned user (if email available)
      if (!string.IsNullOrWhiteSpace(TaskItem.AssignedToUserId))
      {
        var assigned = await _db.Users.FindAsync(TaskItem.AssignedToUserId);
        if (assigned != null && !string.IsNullOrWhiteSpace(assigned.Email))
        {
          var subject = $"Nova tarefa atribuída: {TaskItem.Title}";
          var body = $"<p>Olá {(string.IsNullOrEmpty(assigned.FullName) ? assigned.Email : assigned.FullName)},</p>" +
                     $"<p>Uma nova tarefa foi atribuída a você:</p>" +
                     $"<ul><li><strong>Título:</strong> {TaskItem.Title}</li><li><strong>Descrição:</strong> {TaskItem.Description}</li><li><strong>Data limite:</strong> {TaskItem.DueDate:O}</li></ul>" +
                     $"<p>Acesse o sistema para mais detalhes.</p>";
          try
          {
            await _emailSender.SendEmailAsync(assigned.Email, subject, body);
          }
          catch (System.Exception ex)
          {
            _logger.LogError(ex, "Falha ao enviar email de atribuição para {Email}", assigned.Email);
          }
        }
      }
      return RedirectToPage("/Tasks/Index");
    }

    private async Task LoadUsersAsync()
    {
      var users = await _db.Users
        .Select(u => new { u.Id, Display = (u.FullName == null || u.FullName == "") ? u.Email : u.FullName })
        .ToListAsync();
      UsersSelect = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(users, "Id", "Display");
    }
  }
}
