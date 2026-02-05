using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Desafio.Leve.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Desafio.Leve.Web.Pages.Users
{
  [Authorize(Roles = "Gestor")]
  public class IndexModel : PageModel
  {
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(UserManager<ApplicationUser> userManager) => _userManager = userManager;

    public IList<ApplicationUser> Users { get; set; }

    public async Task OnGetAsync()
    {
      var currentUserId = _userManager.GetUserId(User);

      // Mostra apenas usuários criados pelo gestor atual
      Users = await _userManager.Users
        .Where(u => u.CreatedById == currentUserId)
        .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeactivateAsync(string id)
    {
      var user = await _userManager.FindByIdAsync(id);
      if (user == null) return NotFound();

      var currentUserId = _userManager.GetUserId(User);
      if (user.CreatedById != currentUserId)
        return Forbid();

      user.IsActive = false;
      await _userManager.UpdateAsync(user);

      return RedirectToPage();
    }

    public async Task<IActionResult> OnPostActivateAsync(string id)
    {
      var user = await _userManager.FindByIdAsync(id);
      if (user == null) return NotFound();

      var currentUserId = _userManager.GetUserId(User);
      if (user.CreatedById != currentUserId)
        return Forbid();

      user.IsActive = true;
      await _userManager.UpdateAsync(user);

      return RedirectToPage();
    }
  }
}
