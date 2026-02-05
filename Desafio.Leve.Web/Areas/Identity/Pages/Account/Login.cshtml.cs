using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Desafio.Leve.Infrastructure.Identity;

namespace Desafio.Leve.Web.Areas.Identity.Pages.Account;

public class LoginModel : PageModel
{
  private readonly SignInManager<ApplicationUser> _signInManager;
  private readonly ILogger<LoginModel> _logger;

  public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger)
  {
    _signInManager = signInManager;
    _logger = logger;
  }

  [BindProperty]
  public InputModel Input { get; set; } = new();

  public string? ReturnUrl { get; set; }

  public class InputModel
  {
    [Required(ErrorMessage = "O e-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha")]
    public string Password { get; set; } = string.Empty;
  }

  public async Task OnGetAsync(string? returnUrl = null)
  {
    // Limpa o cookie externo existente para garantir um processo de login limpo.
    await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

    ReturnUrl = returnUrl;
  }

  public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
  {
    returnUrl ??= Url.Content("~/");

    if (ModelState.IsValid)
    {
      // Isso não contabiliza falhas de login para o bloqueio da conta.
      // Para permitir que erros de senha acionem o bloqueio da conta, defina lockoutOnFailure: true
      var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, isPersistent: false, lockoutOnFailure: false);

      if (result.Succeeded)
      {
        _logger.LogInformation("Usuário logado.");
        return LocalRedirect(returnUrl);
      }
      if (result.IsLockedOut)
      {
        _logger.LogWarning("Conta bloqueada.");
        return RedirectToPage("./Lockout");
      }
      else
      {
        ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
        return Page();
      }
    }

    return Page();
  }
}
