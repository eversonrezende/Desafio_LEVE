using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Desafio.Leve.Infrastructure.Identity;

namespace Desafio.Leve.Web.Pages.Users
{
  [Authorize(Roles = "Gestor")]
  public class CreateModel : PageModel
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<Microsoft.AspNetCore.Identity.IdentityRole> _roleManager;
    private readonly Desafio.Leve.Infrastructure.Services.IEmailSender _emailSender;
    private readonly Microsoft.Extensions.Logging.ILogger<CreateModel> _logger;

    public CreateModel(UserManager<ApplicationUser> userManager, RoleManager<Microsoft.AspNetCore.Identity.IdentityRole> roleManager, Desafio.Leve.Infrastructure.Services.IEmailSender emailSender, Microsoft.Extensions.Logging.ILogger<CreateModel> logger)
      => (_userManager, _roleManager, _emailSender, _logger) = (userManager, roleManager, emailSender, logger);


    [BindProperty, Required, EmailAddress]
    public string Email { get; set; } = default!;

    [BindProperty, Required]
    public string FullName { get; set; } = default!;

    [BindProperty]
    public System.DateTime? BirthDate { get; set; }

    [BindProperty]
    public string? PhoneFixed { get; set; }

    [BindProperty]
    public string? PhoneMobile { get; set; }

    [BindProperty]
    public string? Address { get; set; }

    [BindProperty, Required, DataType(DataType.Password)]
    public string Password { get; set; } = default!;

    [BindProperty]
    public IFormFile? Photo { get; set; }

    [BindProperty]
    public string? SelectedRole { get; set; }

    public List<SelectListItem> Roles { get; set; } = new List<SelectListItem>();

    public async Task OnGetAsync()
    {
      // ensure roles exist
      var subordinateRole = "Subordinado";
      var gestorRole = "Gestor";
      if (!await _roleManager.RoleExistsAsync(subordinateRole))
      {
        await _roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole(subordinateRole));
      }
      if (!await _roleManager.RoleExistsAsync(gestorRole))
      {
        await _roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole(gestorRole));
      }

      Roles = new List<SelectListItem>
      {
        new SelectListItem { Text = subordinateRole, Value = subordinateRole },
        new SelectListItem { Text = gestorRole, Value = gestorRole }
      };
    }

    public async Task<IActionResult> OnPostAsync()
    {
      if (!ModelState.IsValid)
        return Page();

      var currentUserId = _userManager.GetUserId(User);

      var user = new ApplicationUser
      {
        UserName = Email,
        Email = Email,
        FullName = FullName,
        BirthDate = BirthDate,
        PhoneFixed = PhoneFixed,
        PhoneMobile = PhoneMobile,
        Address = Address,
        CreatedById = currentUserId
      };
      var result = await _userManager.CreateAsync(user, Password);
      if (!result.Succeeded)
      {
        foreach (var e in result.Errors)
          ModelState.AddModelError(string.Empty, e.Description);
        return Page();
      }

      if (Photo != null && Photo.Length > 0)
      {
        // Validate file type (only images)
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        var extension = Path.GetExtension(Photo.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
        {
          ModelState.AddModelError(nameof(Photo), "Apenas arquivos de imagem são permitidos (jpg, jpeg, png, gif, bmp, webp).");
          return Page();
        }

        // Validate file size (max 5MB)
        const int maxFileSizeInBytes = 5 * 1024 * 1024;
        if (Photo.Length > maxFileSizeInBytes)
        {
          ModelState.AddModelError(nameof(Photo), "O tamanho máximo permitido é 5 MB.");
          return Page();
        }

        // Sanitize filename: remove path characters and generate unique name
        var sanitizedFileName = Path.GetFileNameWithoutExtension(Photo.FileName)
            .Replace("..", "")
            .Replace("/", "")
            .Replace("\\", "");
        var uniqueFileName = $"{Guid.NewGuid()}_{sanitizedFileName}{extension}";

        var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", user.Id);
        Directory.CreateDirectory(uploads);
        var filePath = Path.Combine(uploads, uniqueFileName);

        // Ensure the final path is within the uploads directory (prevent path traversal)
        var fullUploadsPath = Path.GetFullPath(uploads);
        var fullFilePath = Path.GetFullPath(filePath);
        if (!fullFilePath.StartsWith(fullUploadsPath))
        {
          ModelState.AddModelError(nameof(Photo), "Nome de arquivo inválido.");
          return Page();
        }

        using (var stream = System.IO.File.Create(filePath))
        {
          await Photo.CopyToAsync(stream);
        }
        user.PhotoPath = $"/uploads/{user.Id}/{uniqueFileName}";
        await _userManager.UpdateAsync(user);
      }

      // Determine role to assign (default to Subordinado)
      var roleToAssign = string.IsNullOrWhiteSpace(SelectedRole) ? "Subordinado" : SelectedRole;

      // Only allow assigning Gestor if the current user is in the Gestor role
      if (roleToAssign == "Gestor" && !User.IsInRole("Gestor"))
      {
        roleToAssign = "Subordinado";
      }

      if (!await _roleManager.RoleExistsAsync(roleToAssign))
      {
        await _roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole(roleToAssign));
      }

      await _userManager.AddToRoleAsync(user, roleToAssign);

      // Send a welcome/notification email (do not include the password)
      if (!string.IsNullOrWhiteSpace(user.Email))
      {
        var subject = "Bem-vindo(a) — sua conta foi criada";
        var body = $"<p>Olá {user.FullName ?? user.UserName},</p>" +
                   $"<p>Sua conta foi criada com sucesso com o e-mail <strong>{user.Email}</strong> e a role <strong>{roleToAssign}</strong>.</p>" +
                   "<p>Use suas credenciais para fazer login em: <a href=\"/Identity/Account/Login\">entrar</a>.</p>" +
                   "<p>Se você não solicitou esta conta, contate o administrador.</p>";
        try
        {
          await _emailSender.SendEmailAsync(user.Email, subject, body);
        }
        catch (System.Exception ex)
        {
          _logger.LogError(ex, "Falha ao enviar email de boas-vindas para {Email}", user.Email);
        }
      }
      return RedirectToPage("Index");
    }
  }
}
