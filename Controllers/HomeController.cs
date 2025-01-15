using System.Diagnostics;
using Final_Project.Areas.Identity.Data;
using Final_Project.Data;
using Final_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Final_Project.Controllers
{
  public class HomeController : Controller
  {

    private readonly ApplicationDbContext _DbContext;
    private readonly IHttpContextAccessor _httpContext;
    private readonly UserManager<User> _userManager;
    private readonly IdentityContext _identityContext;
    private readonly int _currentUser;

    public HomeController(ApplicationDbContext context, IHttpContextAccessor httpContext, UserManager<User> userManager, IdentityContext identityContext)
    {
      _DbContext = context;
      _httpContext = httpContext;
      _userManager = userManager;
      _identityContext = identityContext;
      //_currentUser = Convert.ToInt32(_userManager.GetUserId(HttpContext.User)); // uncomment later
    }

    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Tasks()
    {
      ViewBag.Tasks = _DbContext.Task.ToList();
      return View();
    }

    public IActionResult Groups()
    {
      return View();
    }
  }
}
