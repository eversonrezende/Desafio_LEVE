using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Desafio.Leve.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Desafio.Leve.Web.Pages.Users
{
  [Authorize(Roles = "Gestor")]
  public class EditModel : PageModel
  {
    private readonly UserManager<ApplicationUser> _userManager;

    public EditModel(UserManager<ApplicationUser> userManager)
    {
      _userManager = userManager;
    }

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string? FullName { get; set; }

    [BindProperty]
    public DateTime? BirthDate { get; set; }

    [BindProperty]
    public string? PhoneFixed { get; set; }

    [BindProperty]
    public string? PhoneMobile { get; set; }

    [BindProperty]
    public string? Address { get; set; }

    [BindProperty]
    public IFormFile? Photo { get; set; }

    public string? CurrentPhotoPath { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
      if (string.IsNullOrEmpty(id))
        return NotFound();

      var user = await _userManager.FindByIdAsync(id);
      if (user == null)
        return NotFound();

      // Verifica se o usuário foi criado pelo gestor atual
      var currentUserId = _userManager.GetUserId(User);
      if (user.CreatedById != currentUserId)
        return Forbid();

      Email = user.Email ?? string.Empty;
      FullName = user.FullName;
      BirthDate = user.BirthDate;
      PhoneFixed = user.PhoneFixed;
      PhoneMobile = user.PhoneMobile;
      Address = user.Address;
      CurrentPhotoPath = user.PhotoPath;

      return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
      if (string.IsNullOrEmpty(id))
        return NotFound();

      var user = await _userManager.FindByIdAsync(id);
      if (user == null)
        return NotFound();

      // Verifica se o usuário foi criado pelo gestor atual
      var currentUserId = _userManager.GetUserId(User);
      if (user.CreatedById != currentUserId)
        return Forbid();

      if (!ModelState.IsValid)
      {
        CurrentPhotoPath = user.PhotoPath;
        return Page();
      }

      // Atualiza os campos
      user.FullName = FullName;
      user.BirthDate = BirthDate;
      user.PhoneFixed = PhoneFixed;
      user.PhoneMobile = PhoneMobile;
      user.Address = Address;

      // Processa upload de foto, se fornecida
      if (Photo != null && Photo.Length > 0)
      {
        // Validar tipo de arquivo (somente imagens)
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        var extension = Path.GetExtension(Photo.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
        {
          ModelState.AddModelError(nameof(Photo), "Apenas arquivos de imagem são permitidos (jpg, jpeg, png, gif, bmp, webp).");
          CurrentPhotoPath = user.PhotoPath;
          return Page();
        }

        // Validar tamanho do arquivo (máximo 5 MB)
        const int maxFileSizeInBytes = 5 * 1024 * 1024;
        if (Photo.Length > maxFileSizeInBytes)
        {
          ModelState.AddModelError(nameof(Photo), "O tamanho máximo permitido é 5 MB.");
          CurrentPhotoPath = user.PhotoPath;
          return Page();
        }

        // Remover caracteres do caminho e gerar nome único
        var sanitizedFileName = Path.GetFileNameWithoutExtension(Photo.FileName)
            .Replace("..", "")
            .Replace("/", "")
            .Replace("\\", "");
        var uniqueFileName = $"{Guid.NewGuid()}_{sanitizedFileName}{extension}";

        var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", user.Id);
        Directory.CreateDirectory(uploads);
        var filePath = Path.Combine(uploads, uniqueFileName);

        // Certifique-se de que o caminho final esteja dentro do diretório de uploads (evite a travessia de diretórios).
        var fullUploadsPath = Path.GetFullPath(uploads);
        var fullFilePath = Path.GetFullPath(filePath);
        if (!fullFilePath.StartsWith(fullUploadsPath))
        {
          ModelState.AddModelError(nameof(Photo), "Caminho de arquivo inválido.");
          CurrentPhotoPath = user.PhotoPath;
          return Page();
        }

        // Apague a foto antiga, se existir.
        if (!string.IsNullOrEmpty(user.PhotoPath))
        {
          var oldPhotoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.PhotoPath.TrimStart('/'));
          if (System.IO.File.Exists(oldPhotoPath))
          {
            System.IO.File.Delete(oldPhotoPath);
          }
        }

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
          await Photo.CopyToAsync(stream);
        }
        user.PhotoPath = $"/uploads/{user.Id}/{uniqueFileName}";
      }

      var result = await _userManager.UpdateAsync(user);
      if (!result.Succeeded)
      {
        foreach (var error in result.Errors)
          ModelState.AddModelError(string.Empty, error.Description);
        CurrentPhotoPath = user.PhotoPath;
        return Page();
      }

      return RedirectToPage("Index");
    }
  }
}
