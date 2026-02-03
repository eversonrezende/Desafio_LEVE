using System.Collections.Generic;
using System.Threading.Tasks;
using Desafio.Leve.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
      Users = await Task.FromResult(_userManager.Users as IList<ApplicationUser> ?? new List<ApplicationUser>());
    }
  }
}
