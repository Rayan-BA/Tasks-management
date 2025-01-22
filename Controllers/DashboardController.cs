using Final_Project.Data;
using Microsoft.AspNetCore.Mvc;
using Final_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Collections.Generic;

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
      ViewBag.PendingReq = _DbContext.ManagerRequests.Where(r => r.ManagerId == _currentUserId && r.IsPending).Count();
      ViewBag.Uncompleted = _DbContext.Task.Where(t => t.AssignerId == _currentUserId && !t.IsCompleted).Count();
      ViewBag.Overdue = _DbContext.Task.Where(t => 
          t.AssignerId == _currentUserId && !t.IsCompleted).Count();
      var completed = _DbContext.Task.Where(t => t.AssignerId == _currentUserId && t.IsCompleted).Count();
      var total = _DbContext.Task.Where(t => t.AssignerId == _currentUserId).Count();
      ViewBag.WeekProgress = Convert.ToInt32(completed / total);
      return View();
    }

    public JsonResult ChartData()
    {
      var users = _DbContext.Users.Where(u => u.ManagerId == _currentUserId).ToList();
      List<string> labels = _DbContext.Users.Where(u => u.ManagerId == _currentUserId).Select(u => u.DisplayName).ToList();
      List<int> dataTotal = new List<int>();
      List<int> dataCompleted = new List<int>();
      foreach (var user in users)
      {
        dataTotal.Add(_DbContext.Task.Where(t => t.UserId == user.Id).Count());
        dataCompleted.Add(_DbContext.Task.Where(t => t.UserId == user.Id && t.IsCompleted).Count());
      }
      return new JsonResult(new
      {
        total = dataTotal,
        completed = dataCompleted,
        labels = labels
      });
    }

    public IActionResult Tasks()
    {
      //var users = _DbContext.Users.Where(u => u.ManagerId == _currentUserId);
      ViewBag.Users = _DbContext.Users.Where(u => u.ManagerId == _currentUserId).ToList();
      ViewBag.Groups = _DbContext.Group.Where(u => u.ManagerId == _currentUserId).ToList();
      ViewBag.Tasks = _DbContext.Task.OrderByDescending(t => t.CreatedAt).ToList();
      return View();
    }

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
      ViewBag.Users = _DbContext.Users.Where(u => u.ManagerId == _currentUserId).ToList();
      ViewBag.Groups = _DbContext.Group.Where(u => u.ManagerId == _currentUserId).ToList();
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
      var users = _DbContext.Users.Where(u => u.ManagerId == _currentUserId).ToList();
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
      ViewBag.Groups = _DbContext.Group.Where(g => g.ManagerId == _currentUserId).ToList();
      ViewBag.Users = _DbContext.Users.Where(u => u.ManagerId == _currentUserId).ToList();
      return View();
    }

    public IActionResult CreateGroup(Group group, List<string> users)
    {
      group.CreatedAt = DateTime.Now;
      group.ManagerId = _currentUserId;
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
