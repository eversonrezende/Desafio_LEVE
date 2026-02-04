using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Desafio.Leve.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
  }
}
