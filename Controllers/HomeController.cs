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
    private readonly IHttpContextAccessor _httpContextAccerssor;
    private readonly UserManager<User> _userManager;

    public HomeController(ApplicationDbContext DbContext, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
    {
      _DbContext = DbContext;
      _httpContextAccerssor = httpContextAccessor;
      _userManager = userManager;
    }

    private bool IsCurrentUser(int? Id)
    {
      return GetCurrentUser().Id == Id;
    }

    private async Task<User?> GetCurrentUser()
    {
      var httpContext = _httpContextAccerssor.HttpContext;
      if (httpContext == null) return null;
      return await _userManager.GetUserAsync(httpContext.User);
    }

    public async Task<IActionResult> Index()
    {
      var tasks = _DbContext.Task.Where(t => IsCurrentUser(t.UserId));
      ViewBag.DueToday = await tasks.Where(t => t.DueAt.Date.Equals(DateTime.Today.Date)).CountAsync();
      ViewBag.Overdue = await tasks.Where(t => !t.IsCompleted && DateTime.Now.Date > t.DueAt).CountAsync();
      ViewBag.Uncompleted = await tasks.Where(t => !t.IsCompleted).CountAsync();
      ViewBag.Important = tasks.Where(t => !t.IsCompleted && t.IsImportant).CountAsync();
      ViewBag.Tasks = await tasks.ToListAsync();
      return View();
    }

    public async Task<IActionResult> Tasks()
    {
      var tasks = _DbContext.Task.Where(t => IsCurrentUser(t.UserId));
      ViewBag.Important = await tasks.Where(t => t.IsImportant).ToListAsync();
      ViewBag.Uncompleted = await tasks.Where(t => !t.IsCompleted).ToListAsync();
      ViewBag.Completed = await tasks.Where(t => t.IsCompleted).ToListAsync();
      return View();
    }

    public async Task<IActionResult> ToggleTask(int Id, bool checkbox)
    {
      var task = await _DbContext.Task.FindAsync(Id);
      if (task != null)
      {
        task.IsCompleted = checkbox;
        _DbContext.Task.Update(task);
        _DbContext.SaveChanges();
      }
      return RedirectToAction("Tasks");
    }

    public async Task<IActionResult> ClaimTask(int Id, string Url)
    {
      var groupId = Convert.ToInt32(Url.Split("=")[1]);
      var task = await _DbContext.Task.FindAsync(Id);
      if (task != null)
      {
        if (IsCurrentUser(task.UserId))
        {
          task.UserId = null;
        }
        else if (task.UserId == null)
        {
          task.UserId = GetCurrentUser().Id;
        }
        //_DbContext.Task.Update(task);
        await _DbContext.SaveChangesAsync();
      }
      return RedirectToAction("Groups");
    }

    public async Task<IActionResult> Groups(int Id)
    {
      if (Id != 0)
      {
        var tasks = await _DbContext.Task.Where(t => t.GroupId == Id).ToListAsync();
        var memberships = await _DbContext.GroupMemberships.Where(gm => gm.GroupId == Id).ToListAsync();
        var users = new List<User>();
        foreach (var m in memberships)
        {
          var u = await _DbContext.Users.Where(u => u.Id == m.UserId).SingleOrDefaultAsync();
          if (u != null)
          {
            users.Add(u);
          }
        }
        ViewBag.Tasks = tasks;
        ViewBag.Users = users;
        return PartialView("~/Views/Home/_Partials/_GroupPartial.cshtml");
      }
      var gm = _DbContext.GroupMemberships.Where(gm => IsCurrentUser(gm.UserId));
      var groupIds = await gm.Select(gm => gm.GroupId).ToListAsync();
      var groups = new List<Group>();
      foreach (var gId in groupIds)
      {
        var g = await _DbContext.Group.Where(g => g.Id == gId).SingleOrDefaultAsync();
        if (g != null) groups.Add(g);
      }
      return View(groups);
    }

    public async Task<IActionResult> Profile()
    {
      var user = await GetCurrentUser();
      if (user != null)
      {
        if (user.Image != null)
        {
          var b64 = Convert.ToBase64String(user.Image);
          ViewBag.Image = string.Format("data:image/png;base64," + b64);
        }
        ViewBag.ManagerEmail = await _DbContext.Users.Where(m => m.Id == user.ManagerId).SingleOrDefaultAsync();
        var pendingReq = await _DbContext.ManagerRequests.Where(mr => mr.IsPending && mr.UserId == user.Id).SingleOrDefaultAsync();
        if (pendingReq != null)
        {
          var manager = await _DbContext.Users.Where(m => m.Id == pendingReq.ManagerId).SingleOrDefaultAsync();
          if (manager != null) ViewBag.PendingManagerEmail = manager.Email;
        }
      }
      return View(user);
    }

    public async Task<IActionResult> EditProfile()
    {
      var user = await GetCurrentUser();
      if (user != null)
      {
        ViewBag.DisplayName = user.DisplayName;
        //ViewBag.Id = user.Id;
        if (user.Image != null)
        {
          var b64 = Convert.ToBase64String(user.Image);
          ViewBag.Image = string.Format("data:image/png;base64," + b64);
        }
        ViewBag.ManagerEmail = await _DbContext.Users.Where(m => m.Id == user.ManagerId).SingleOrDefaultAsync();
      }
      return View();
    }

    public async Task<IActionResult> UpdateProfile(ProfileEditModel pem)
    {
      var user = await GetCurrentUser();
      if (user != null)
      {
        if (pem.Image != null)
        {
          using (var memoryStream = new MemoryStream())
          {
            await pem.Image.CopyToAsync(memoryStream);

            // Upload the file if less than 2 MB
            if (memoryStream.Length < 2097152 && memoryStream.Length > 0 && (pem.Image.ContentType == "image/png" || pem.Image.ContentType == "image/jpeg"))
            {
              user.Image = memoryStream.ToArray();
            }
          }
        }
        user.DisplayName = pem.DisplayName;
        if (pem.ManagerEmail != null)
        {
          var manager = await _DbContext.Users.Where(m => m.NormalizedEmail == pem.ManagerEmail.Trim().ToUpper()).SingleOrDefaultAsync();
          if (manager != null && user.ManagerId != manager.Id)
          {
            await _DbContext.ManagerRequests.AddAsync(new ManagerRequests
            {
              UserId = user.Id,
              ManagerId = manager.Id,
              IsAccepted = false,
              IsPending = true,
            });
          }
        }
        //_DbContext.Users.Update(user);
        await _DbContext.SaveChangesAsync();
      }
      return RedirectToAction("Profile");
    }

    [Route("/Home/ChartData")]
    public async Task<JsonResult> ChartDate()
    {
      var labels = new List<string>();
      var data = new List<int>();
      labels.Add("You");
      var gm = await _DbContext.GroupMemberships.Where(gm => IsCurrentUser(gm.UserId)).ToListAsync();
      var groups = new List<Group>();
      foreach (var g in gm)
      {
        var grp = await _DbContext.Group.FindAsync(g.GroupId);
        if (grp != null) groups.Add(grp);
      }
      foreach (var g in groups)
      {
        if (g.Name != null) labels.Add(g.Name);
      }
      data.Add(await _DbContext.Task.Where(t => IsCurrentUser(t.UserId)).CountAsync());
      foreach (var g in groups)
      {
        data.Add(await _DbContext.Task.Where(t => t.GroupId == g.Id).CountAsync());
      }
      return new JsonResult(new { labels, data });
    }
  }
}

