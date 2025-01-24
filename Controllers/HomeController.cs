using System.Diagnostics;
using System.IO.Compression;

//using Final_Project.Areas.Identity.Data;
using Final_Project.Data;
using Final_Project.FormModels;
using Final_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Final_Project.Controllers
{
  public class HomeController : Controller
  {

    private readonly ApplicationDbContext _DbContext;
    private readonly IHttpContextAccessor _httpContext;
    private readonly UserManager<User> _userManager;
    private readonly int _currentUserId;

    public HomeController(ApplicationDbContext context, IHttpContextAccessor httpContext, UserManager<User> userManager)
    {
      _DbContext = context;
      _httpContext = httpContext;
      _userManager = userManager;
      _currentUserId = Convert.ToInt32(_userManager.GetUserId(httpContext.HttpContext.User));
    }

    public IActionResult Index()
    {
      var tasks = _DbContext.Task.Where(t => t.UserId == _currentUserId);
      ViewBag.DueToday = tasks.Where(t => t.DueAt.Date.Equals(DateTime.Today.Date)).Count();
      ViewBag.Overdue = tasks.Where(t => !t.IsCompleted && DateTime.Now.Date > t.DueAt).Count();
      ViewBag.Uncompleted = tasks.Where(t => !t.IsCompleted).Count();
      ViewBag.Important = tasks.Where(t => !t.IsCompleted && t.IsImportant).Count();
      ViewBag.Tasks = tasks.ToList();
      return View();
    }

    public IActionResult Tasks()
    {
      var tasks = _DbContext.Task.Where(t => t.UserId == _currentUserId);
      ViewBag.Important = tasks.Where(t => t.IsImportant).ToList();
      ViewBag.Uncompleted = tasks.Where(t => !t.IsCompleted).ToList();
      ViewBag.Completed = tasks.Where(t => t.IsCompleted).ToList();
      return View();
    }

    public IActionResult ToggleTask(int Id, bool checkbox)
    {
      var task = _DbContext.Task.Find(Id);
      task.IsCompleted = checkbox;
      _DbContext.Task.Update(task);
      _DbContext.SaveChanges();
      return RedirectToAction("Tasks");
    }

    public IActionResult ClaimTask(int Id, string Url)
    {
      var groupId = Convert.ToInt32(Url.Split("=")[1]);
      var task = _DbContext.Task.Find(Id);
      if (task.UserId == _currentUserId)
      {
        task.UserId = null;
      }
      else if (task.UserId == null)
      {
        task.UserId = _currentUserId;
      }
      _DbContext.Task.Update(task);
      _DbContext.SaveChanges();
      return RedirectToAction("Groups");
    }

    public IActionResult Groups(int Id)
    {
      if (Id != 0)
      {
        var tasks = _DbContext.Task.Where(t => t.GroupId == Id).ToList();
        var memberships = _DbContext.GroupMemberships.Where(gm => gm.GroupId == Id).ToList();
        var users = new List<User>();
        foreach (var m in memberships)
        {
          var u = _DbContext.Users.Where(u => u.Id == m.UserId).SingleOrDefault();
          if (u != null)
          {
            users.Add(u);
          }
        }
        ViewBag.Tasks = tasks;
        ViewBag.Users = users;
        return PartialView("~/Views/Home/_Partials/_GroupPartial.cshtml");
      }
      var gm = _DbContext.GroupMemberships.Where(gm => gm.UserId == _currentUserId);
      var groupIds = gm.Select(gm => gm.GroupId).ToList();
      var groups = new List<Group>();
      foreach (var gId in groupIds)
      {
        groups.Add(_DbContext.Group.Where(g => g.Id == gId).SingleOrDefault());
      }
      return View(groups);
    }

    public IActionResult Profile()
    {
      var user = _DbContext.Users.Find(_currentUserId);
      //if (user.Image != null)
      //{
      //  var b64 = Convert.ToBase64String(user.Image);
      //  ViewBag.Image = string.Format("data:image/png,base64," + b64 + ");");
      //}
      ViewBag.ManagerEmail = _DbContext.Users.Where(m => m.Id == user.ManagerId).SingleOrDefault();
      var pendingReq = _DbContext.ManagerRequests.Where(mr => mr.IsPending && mr.UserId == user.Id).SingleOrDefault();
      if (pendingReq != null)
      {
        var manager = _DbContext.Users.Where(m => m.Id == pendingReq.ManagerId).SingleOrDefault();
        ViewBag.PendingManagerEmail = manager.Email;
      }
      return View(user);
    }

    public IActionResult EditProfile()
    {
      var user = _DbContext.Users.Find(_currentUserId);
      ViewBag.DisplayName = user.DisplayName;
      ViewBag.Id = user.Id;
      ViewBag.ManagerEmail = _DbContext.Users.Where(m => m.Id == user.ManagerId).SingleOrDefault();
      return View();
    }

    public IActionResult UpdateProfile(ProfileEditModel pem, IFormFile img)
    {
      var user = _DbContext.Users.Find(_currentUserId);

      //using (var memoryStream = new MemoryStream())
      //{
      //  img.CopyToAsync(memoryStream);

      //  // Upload the file if less than 2 MB
      //  if (memoryStream.Length < 2097152 && memoryStream.Length> 0 && (img.ContentType == "image/png" || img.ContentType == "image/jpeg"))
      //  {
      //    user.Image = memoryStream.ToArray();
      //    _DbContext.Users.Update(user);
      //    _DbContext.SaveChanges();
      //  }
      //}
      //_DbContext.Users.Where(u => u.NormalizedEmail == user.Mana)
      //_DbContext.Users.Update(user);
      user.DisplayName = pem.DisplayName;
      if (pem.ManagerEmail != null)
      {
        var manager = _DbContext.Users.Where(m => m.NormalizedEmail == pem.ManagerEmail.ToUpper()).SingleOrDefault();
        if (user.ManagerId != manager.Id)
        {
          _DbContext.ManagerRequests.Add(new ManagerRequests
          {
            UserId = user.Id,
            ManagerId = manager.Id,
            IsAccepted = false,
            IsPending = true,
            CreatedAt = DateTime.Now
          });
        }
        //user.ManagerId = manager.Id;
      }
      _DbContext.Users.Update(user);
      _DbContext.SaveChanges();
      return RedirectToAction("Profile");
    }

    [Route("/Home/ChartData")]
    public JsonResult ChartDate()
    {
      var labels = new List<string>();
      var data = new List<int>();
      labels.Add("You");
      var gm = _DbContext.GroupMemberships.Where(gm => gm.UserId == _currentUserId).ToList();
      var groups = new List<Group>();
      foreach (var g in gm)
      {
        groups.Add(_DbContext.Group.Find(g.GroupId));
      }
      foreach (var g in groups)
      {
        labels.Add(g.Name);
      }
      data.Add(_DbContext.Task.Where(t => t.UserId == _currentUserId).Count());
      foreach (var g in groups)
      {
        data.Add(_DbContext.Task.Where(t => t.GroupId == g.Id).Count());
      }
      return new JsonResult(new { labels, data });
    }
  }
}

