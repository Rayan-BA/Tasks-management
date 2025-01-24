using Final_Project.Data;
using Microsoft.AspNetCore.Mvc;
using Final_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Final_Project.Controllers
{
  public class DashboardController : Controller
  {

    private readonly ApplicationDbContext _DbContext;
    private readonly IHttpContextAccessor _httpContext;
    private readonly UserManager<User> _userManager;
    private readonly int _currentUserId;

    private readonly IUserStore<User> _userStore;

    public DashboardController(ApplicationDbContext context, IHttpContextAccessor httpContext, UserManager<User> userManager, IUserStore<User> userStore)
    {
      _DbContext = context;
      _httpContext = httpContext;
      _userManager = userManager;
      _currentUserId = Convert.ToInt32(_userManager.GetUserId(httpContext.HttpContext.User));
      _userStore = userStore;
    }

    public async Task<IActionResult> TestUsers()
    {
      for (int i = 0; i < 10; i++)
      {
        var user = Activator.CreateInstance<User>();
        user.DisplayName = "user" + DateTime.Now.Microsecond + DateTime.Now.Microsecond;
        await _userStore.SetUserNameAsync(user, user.DisplayName, CancellationToken.None);
        await _userManager.CreateAsync(user, "Password1!");
        _DbContext.ManagerRequests.Add(new ManagerRequests { 
          UserId = user.Id, ManagerId = _currentUserId, IsAccepted = false,  IsPending = true
        });
        _DbContext.SaveChanges();
      }
      return RedirectToAction("Index");
    }

    public IActionResult Index()
    {
      var dt = DateTime.Now;
      var tasks = _DbContext.Task.Where(t => t.AssignerId == _currentUserId);
      ViewBag.PendingReq = _DbContext.ManagerRequests.Where(r => r.ManagerId == _currentUserId && r.IsPending).Count();
      ViewBag.Uncompleted = tasks.Where(t => !t.IsCompleted).Count();
      var overdue = tasks.Where(t => !t.IsCompleted && dt.Date > t.DueAt);
      ViewBag.Overdue = overdue.ToList();
      ViewBag.OverdueCount = overdue.Count();
      ViewBag.CloseDeadline = tasks.Where(t => t.DueAt.Day - dt.Date.Day <= 1 && t.DueAt.Day - dt.Date.Day > 0).ToList();
      var total = tasks.Count();
      var completed = tasks.Where(t => t.IsCompleted).Count();

      float ratio = (completed / (float)total) * 100;
      if (total == 0)
      {
        ViewBag.Progress = 0;
      }
      else
      {
        ViewBag.Progress = ratio.ToString("0.00");
      }
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
      if (ModelState.IsValid)
      {
        task.AssignerId = Convert.ToInt32(_userManager.GetUserId(HttpContext.User));
        task.CreatedAt = DateTime.Now;
        _DbContext.Task.Add(task);
        _DbContext.SaveChanges();
      }
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
      requests.ForEach(req =>
      {
        var u = _DbContext.Users.Where(u => u.Id == req.UserId).SingleOrDefault();
        if (u != null) requestingUsers.Add(u);
      });
      ViewBag.ReqUsers = requestingUsers;
      return View(users);
    }

    public IActionResult UserDetails(int Id)
    {
      var user = _DbContext.Users.Find(Id);
      var groupIds = _DbContext.GroupMemberships.Where(gm => gm.UserId == Id).Select(gm => gm.GroupId).ToList();
      var groups = new List<Group>();
      groupIds.ForEach(groupId =>
          groups.Add(_DbContext.Group.Where(g => g.Id == groupId && g.ManagerId == _currentUserId).Single())
      );
      ViewBag.Groups = groups;
      ViewBag.Tasks = _DbContext.Task.Where(t => t.UserId == Id).ToList();
      return View(user);
    }

    public IActionResult RemoveUser(int Id)
    {
      var user = _DbContext.Users.Find(Id);
      user.ManagerId = null;
      _DbContext.Users.Update(user);
      _DbContext.SaveChanges();
      return RedirectToAction("Users");
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
      if (ModelState.IsValid)
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
      }
      return RedirectToAction("Groups");
    }

    public IActionResult GroupDetails(int Id)
    {
      var group = _DbContext.Group.Find(Id);
      var userIds = _DbContext.GroupMemberships.Where(gm => gm.GroupId == Id).Select(gm => gm.UserId).ToList();
      var users = new List<User>();
      userIds.ForEach(Id =>
          users.Add(_DbContext.Users.Where(u => u.Id == Id && u.ManagerId == _currentUserId).SingleOrDefault())
      );
      ViewBag.Users = users;
      ViewBag.Tasks = _DbContext.Task.Where(t => t.GroupId == Id).ToList();
      return View(group);
    }

  }
}
