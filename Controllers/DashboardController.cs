using Final_Project.Data;
using Microsoft.AspNetCore.Mvc;
using Final_Project.Models;
using Microsoft.AspNetCore.Identity;
using Final_Project.Areas.Identity.Data;

namespace Final_Project.Controllers
{
  public class DashboardController : Controller
  {

    private readonly ApplicationDbContext _DbContext;
    private readonly IHttpContextAccessor _httpContext;
    private readonly UserManager<User> _userManager;
    private readonly IdentityContext _identityContext;
    private readonly int _currentUser;

    public DashboardController(ApplicationDbContext context, IHttpContextAccessor httpContext, UserManager<User> userManager, IdentityContext identityContext)
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
      var users = _identityContext.Users.Where(u => u.ManagerId == this._currentUser);
      ViewBag.Users = users;
      ViewBag.Tasks = _DbContext.Task.ToList();
      return View();
    }

    [HttpPost]
    public IActionResult CreateTask(TaskModel task)
    {
      task.AssignerId = Convert.ToInt32(_userManager.GetUserId(HttpContext.User));
      task.CreatedAt = DateTime.Now;
      _DbContext.Task.Add(task);
      _DbContext.SaveChanges();
      return RedirectToAction("Tasks");
    }

    public IActionResult Users()
    {
      return View();
    }

    public IActionResult Groups()
    {
      return View();
    }
  }
}
