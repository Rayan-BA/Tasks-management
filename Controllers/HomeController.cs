using System.Diagnostics;
using System.IO.Compression;

//using Final_Project.Areas.Identity.Data;
using Final_Project.Data;
using Final_Project.FormModels;
using Final_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
      ViewBag.DueToday = _DbContext.Task.Where(t => t.DueDate.Equals(DateOnly.FromDateTime(DateTime.Today))).Count();
      //ViewBag.DueThisWeek
      ViewBag.TotalTasks = _DbContext.Task.Where(t => !t.IsCompleted).Count();
      ViewBag.ImportantTasks = _DbContext.Task.Where(t => !t.IsCompleted && t.IsImportant).Count();
      ViewBag.Tasks = _DbContext.Task.ToList();
      return View();
    }

    public IActionResult Tasks()
    {
      ViewBag.Tasks = _DbContext.Task.ToList();
      return View();
    }

    public IActionResult Groups(int Id)
    {
      if (Id != 0)
      {
        var tasks = _DbContext.Task.Where(t => t.GroupId == Id).ToList();
        var memberships = _DbContext.GroupMemberships.Where(gm => gm.GroupId == Id).ToList();
        ViewBag.Tasks = tasks;
        ViewBag.Memberships = memberships;
        return PartialView("~/Views/Home/_Partials/_GroupPartial.cshtml");
      }

      var groups = _DbContext.Group.ToList();
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
          _DbContext.ManagerRequests.Add(new ManagerRequests { 
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
  }
}

