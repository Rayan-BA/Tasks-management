using Final_Project.Data;
using Microsoft.AspNetCore.Mvc;
using Final_Project.Models;
using Microsoft.AspNetCore.Identity;
//using Final_Project.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace Final_Project.Controllers
{
  public class DashboardController : Controller
  {

    private readonly ApplicationDbContext _DbContext;
    private readonly IHttpContextAccessor _httpContext;
    private readonly UserManager<User> _userManager;
    //private readonly IdentityContext _identityContext;
    private readonly int _currentUser;

    public DashboardController(ApplicationDbContext context, IHttpContextAccessor httpContext, UserManager<User> userManager)
    {
      _DbContext = context;
      _httpContext = httpContext;
      _userManager = userManager;
      //_identityContext = identityContext;
      //_currentUser = Convert.ToInt32(_userManager.GetUserId(HttpContext.User)); // uncomment later
    }

    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Tasks()
    {
      var users = _DbContext.Users.Where(u => u.ManagerId == this._currentUser);
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

    public IActionResult TaskDetails(int Id)
    {
      var task = _DbContext.Task.Where(t => t.Id == Id).Single();
      var joined = _DbContext.Users.Join(
        _DbContext.Task,
          u => u.Id,
          t => t.UserId,
          (u, t) => new
          {
            Id = u.Id,
            DisplayName = u.DisplayName,
          }
        );
      ViewBag.Users = joined;
      return View(task);
    }

    public IActionResult Users()
    {
      ViewBag.Users = _DbContext.Users.ToList();
      return View();
    }

    public IActionResult Groups()
    {
      return View();
    }
  }
}
