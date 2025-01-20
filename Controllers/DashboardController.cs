using Final_Project.Data;
using Microsoft.AspNetCore.Mvc;
using Final_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace Final_Project.Controllers
{
  public class DashboardController : Controller
  {

    private readonly ApplicationDbContext _DbContext;
    private readonly IHttpContextAccessor _httpContext;
    private readonly UserManager<User> _userManager;
    private readonly int _currentUserId;

    public DashboardController(ApplicationDbContext context, IHttpContextAccessor httpContext, UserManager<User> userManager)
    {
      _DbContext = context;
      _httpContext = httpContext;
      _userManager = userManager;
      _currentUserId = Convert.ToInt32(_userManager.GetUserId(httpContext.HttpContext.User)); // uncomment later
    }

    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Tasks()
    {
      //var users = _DbContext.Users.Where(u => u.ManagerId == _currentUserId);
      ViewBag.Users = _DbContext.Users.ToList();
      ViewBag.Groups = _DbContext.Group.ToList();
      ViewBag.Tasks = _DbContext.Task.OrderByDescending(t => t.CreatedAt).ToList();
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
      var task = _DbContext.Task.Where(t => t.Id == Id).SingleOrDefault();
      var user = _DbContext.Users.Where(u => u.Id == task.UserId).SingleOrDefault();
      var group = _DbContext.Group.Where(u => u.Id == task.GroupId).SingleOrDefault();
      if (user != null)
      {
        ViewBag.User = user.DisplayName;
      }
      if (group != null)
      {
        ViewBag.Group = group.Name;
      }
      return View(task);
    }

    public IActionResult EditTask(int Id)
    {
      ViewBag.Users = _DbContext.Users.ToList();
      ViewBag.Groups = _DbContext.Group.ToList();
      var task = _DbContext.Task.SingleOrDefault(t => t.Id == Id);
      return View(task);
    }

    public IActionResult UpdateTask(TaskModel task)
    {
      _DbContext.Task.Update(task);
      _DbContext.SaveChanges();
      return RedirectToAction("TaskDetails", new { Id = task.Id });
    }

    public IActionResult DeleteTask(int Id)
    {
      var task = _DbContext.Task.SingleOrDefault(t => t.Id == Id);
      if (task != null)
      {
        _DbContext.Task.Remove(task);
        _DbContext.SaveChanges();
      }
      return RedirectToAction("Tasks");
    }

    public IActionResult Users()
    {
      var users = _DbContext.Users.ToList();
      var requests = _DbContext.ManagerRequests.Where(r => r.ManagerId == _currentUserId && r.IsPending).ToList();
      var requestingUsers = new List<User>();
      requests.ForEach(req => {
        var u = _DbContext.Users.Where(u => u.Id == req.UserId).SingleOrDefault();
        if (u != null) requestingUsers.Add(u);
      });
      ViewBag.ReqUsers = requestingUsers;
      return View(users);
    }

    public IActionResult AcceptRequest(int Id)
    {
      var requests = _DbContext.ManagerRequests.Where(r => r.UserId == Id).SingleOrDefault();
      requests.IsPending = false;
      requests.IsAccepted = true;
      _DbContext.ManagerRequests.Update(requests);
      var user = _DbContext.Users.Find(Id);
      user.ManagerId = requests.ManagerId;
      _DbContext.SaveChanges();
      return RedirectToAction("Users");
    }

    public IActionResult RejectRequest(int Id)
    {
      var requests = _DbContext.ManagerRequests.Where(r => r.UserId == Id).SingleOrDefault();
      requests.IsPending = false;
      requests.IsAccepted = false;
      _DbContext.ManagerRequests.Update(requests);
      _DbContext.SaveChanges();
      return RedirectToAction("Users");
    }

    public IActionResult Groups()
    {
      ViewBag.Groups = _DbContext.Group.ToList();
      var ms = _DbContext.GroupMemberships.ToList();
      ViewBag.Users = _DbContext.Users.ToList();
      return View(ms);
    }

    public IActionResult CreateGroup(Group group, List<string> users)
    {
      group.CreatedAt = DateTime.Now;
      _DbContext.Group.Add(group);
      _DbContext.SaveChanges();
      foreach (var userId in users)
      {
        _DbContext.GroupMemberships.Add(new GroupMemberships() { GroupId = group.Id, UserId = Convert.ToInt32(userId) });
      }
      _DbContext.SaveChanges();
      return RedirectToAction("Groups");
    }
  }
}
