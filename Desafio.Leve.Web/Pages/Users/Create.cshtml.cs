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

    [BindProperty, Required(ErrorMessage = "A função é obrigatória")]
    public string? SelectedRole { get; set; }

    public List<SelectListItem> Roles { get; set; } = new List<SelectListItem>();

    private async Task LoadRolesAsync()
    {
      // garantindo que as Roles existam
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

    public async Task OnGetAsync()
    {
      await LoadRolesAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
      if (!ModelState.IsValid)
      {
        await LoadRolesAsync();
        return Page();
      }

      var currentUserId = _userManager.GetUserId(User);

      // Validar foto ANTES de criar o usuário
      if (Photo != null && Photo.Length > 0)
      {
        // Validando o tipo de arquivo (apenas imagens)
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        var extension = Path.GetExtension(Photo.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
        {
          ModelState.AddModelError(nameof(Photo), "Apenas arquivos de imagem são permitidos (jpg, jpeg, png, gif, bmp, webp).");
          await LoadRolesAsync();
          return Page();
        }

        // Validando o tamanho do arquivo (max 5MB)
        const int maxFileSizeInBytes = 5 * 1024 * 1024;
        if (Photo.Length > maxFileSizeInBytes)
        {
          ModelState.AddModelError(nameof(Photo), "O tamanho máximo permitido é 5 MB.");
          await LoadRolesAsync();
          return Page();
        }
      }

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
        await LoadRolesAsync();
        return Page();
      }

      if (Photo != null && Photo.Length > 0)
      {
        // Remover caracteres do path e gerar nome único
        var sanitizedFileName = Path.GetFileNameWithoutExtension(Photo.FileName)
            .Replace("..", "")
            .Replace("/", "")
            .Replace("\\", "");
        var extension = Path.GetExtension(Photo.FileName).ToLowerInvariant();
        var uniqueFileName = $"{Guid.NewGuid()}_{sanitizedFileName}{extension}";

        var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", user.Id);
        Directory.CreateDirectory(uploads);
        var filePath = Path.Combine(uploads, uniqueFileName);

        // Certifique-se de que o path final esteja dentro do diretório de uploads (evite a travessia de diretórios).
        var fullUploadsPath = Path.GetFullPath(uploads);
        var fullFilePath = Path.GetFullPath(filePath);
        if (!fullFilePath.StartsWith(fullUploadsPath))
        {
          ModelState.AddModelError(nameof(Photo), "Nome de arquivo inválido.");
          await LoadRolesAsync();
          return Page();
        }

        using (var stream = System.IO.File.Create(filePath))
        {
          await Photo.CopyToAsync(stream);
        }
        user.PhotoPath = $"/uploads/{user.Id}/{uniqueFileName}";
        await _userManager.UpdateAsync(user);
      }

      // Determine a Role a ser atribuída (o padrão é Subordinado).
      var roleToAssign = string.IsNullOrWhiteSpace(SelectedRole) ? "Subordinado" : SelectedRole;

      // Permitir a atribuição de Gestor somente se o usuário atual tiver a função de Gestor.
      if (roleToAssign == "Gestor" && !User.IsInRole("Gestor"))
      {
        roleToAssign = "Subordinado";
      }

      if (!await _roleManager.RoleExistsAsync(roleToAssign))
      {
        await _roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole(roleToAssign));
      }

      await _userManager.AddToRoleAsync(user, roleToAssign);

      // Enviar um e-mail de boas-vindas/notificação (não incluir a senha).
      if (!string.IsNullOrWhiteSpace(user.Email))
      {
        var subject = "Bem-vindo(a) — sua conta foi criada";
        var body = $"<p>Olá {user.FullName ?? user.UserName},</p>" +
                   $"<p>Sua conta foi criada com sucesso com o e-mail <strong>{user.Email}</strong> e a role <strong>{roleToAssign}</strong>.</p>" +
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
