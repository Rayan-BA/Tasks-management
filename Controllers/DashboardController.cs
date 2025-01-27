using Final_Project.Data;
using Microsoft.AspNetCore.Mvc;
using Final_Project.Models;
using Microsoft.AspNetCore.Identity;
using Final_Project.FormModels;
using Microsoft.EntityFrameworkCore;

namespace Final_Project.Controllers
{
  public class DashboardController : Controller
  {

    private readonly ApplicationDbContext _DbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<User> _userManager;
    private readonly IUserStore<User> _userStore;

    public DashboardController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, IUserStore<User> userStore)
    {
      _DbContext = context;
      _httpContextAccessor = httpContextAccessor;
      _userManager = userManager;
      _userStore = userStore;
    }

    private bool IsCurrentUser(int? Id)
    {
      return GetCurrentUser().Id == Id;
    }

    private async Task<User?> GetCurrentUser()
    {
      var httpContext = _httpContextAccessor.HttpContext;
      if (httpContext == null) return null;
      return await _userManager.GetUserAsync(httpContext.User);
    }

    public async Task<IActionResult> Index()
    {
      var dt = DateTime.Now;
      var tasks = _DbContext.Task.Where(t => IsCurrentUser(t.AssignerId));
      ViewBag.PendingReq = await _DbContext.ManagerRequests.Where(r => IsCurrentUser(r.ManagerId) && r.IsPending).CountAsync();
      ViewBag.Uncompleted = await tasks.Where(t => !t.IsCompleted).CountAsync();
      var overdue = tasks.Where(t => !t.IsCompleted && dt.Date > t.DueAt);
      ViewBag.Overdue = await overdue.ToListAsync();
      ViewBag.OverdueCount = await overdue.CountAsync();
      ViewBag.CloseDeadline = await tasks.Where(t => t.DueAt.Day - dt.Date.Day <= 1 && t.DueAt.Day - dt.Date.Day > 0).ToListAsync();
      var total = await tasks.CountAsync();
      var completed = await tasks.Where(t => t.IsCompleted).CountAsync();

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

    public async Task<JsonResult> ChartData()
    {
      var users = await _DbContext.Users.Where(u => IsCurrentUser(u.ManagerId)).ToListAsync();
      IEnumerable<string?> labels = users.Select(u => u.DisplayName);
      List<int> dataTotal = [];
      List<int> dataCompleted = [];
      foreach (var user in users)
      {
        dataTotal.Add(await _DbContext.Task.Where(t => IsCurrentUser(t.UserId)).CountAsync());
        dataCompleted.Add(await _DbContext.Task.Where(t => IsCurrentUser(t.UserId) && t.IsCompleted).CountAsync());
      }
      return new JsonResult(new
      {
        total = dataTotal,
        completed = dataCompleted,
        labels = labels
      });
    }

    public async Task<IActionResult> Tasks()
    {
      //var users = _DbContext.Users.Where(u => u.ManagerId == _currentUserId);
      ViewBag.Users = await _DbContext.Users.Where(u => IsCurrentUser(u.ManagerId)).ToListAsync();
      ViewBag.Groups = await _DbContext.Group.Where(u => IsCurrentUser(u.ManagerId)).ToListAsync();
      ViewBag.Tasks = await _DbContext.Task.OrderByDescending(t => t.CreatedAt).ToListAsync();
      return View();
    }

    public async Task<IActionResult> CreateTask(TaskModel task)
    {
      if (ModelState.IsValid)
      {
        var user = await GetCurrentUser();
        if (user != null)
        {
          task.AssignerId = user.Id;
          _DbContext.Task.Add(task);
          _DbContext.SaveChanges();
        }
      }
      return RedirectToAction("Tasks");
    }

    public async Task<IActionResult> TaskDetails(int Id)
    {
      var task = await _DbContext.Task.Where(t => t.Id == Id).SingleOrDefaultAsync();
      if (task != null)
      {
        var user = await _DbContext.Users.Where(u => u.Id == task.UserId).SingleOrDefaultAsync();
        var group = await _DbContext.Group.Where(u => u.Id == task.GroupId).SingleOrDefaultAsync();
        if (user != null)
        {
          ViewBag.User = user.DisplayName;
        }
        if (group != null)
        {
          ViewBag.Group = group.Name;
        }
      }
      return View(task);
    }

    public async Task<IActionResult> EditTask(int Id)
    {
      ViewBag.Users = await _DbContext.Users.Where(u => IsCurrentUser(u.ManagerId)).ToListAsync();
      ViewBag.Groups = await _DbContext.Group.Where(u => IsCurrentUser(u.ManagerId)).ToListAsync();
      var task = await _DbContext.Task.SingleOrDefaultAsync(t => t.Id == Id);
      return View(task);
    }

    // todo: take id from url, find task, then assign fields
    public async Task<IActionResult> UpdateTask(TaskModel task)
    {
      _DbContext.Task.Update(task);
      await _DbContext.SaveChangesAsync();
      return RedirectToAction("TaskDetails", new { Id = task.Id });
    }

    public async Task<IActionResult> DeleteTask(int Id)
    {
      var task = await _DbContext.Task.SingleOrDefaultAsync(t => t.Id == Id);
      if (task != null)
      {
        _DbContext.Task.Remove(task);
        await _DbContext.SaveChangesAsync();
      }
      return RedirectToAction("Tasks");
    }

    public async Task<IActionResult> Users()
    {
      var users = await _DbContext.Users.Where(u => IsCurrentUser(u.ManagerId)).ToListAsync();
      var requests = await _DbContext.ManagerRequests.Where(r => IsCurrentUser(r.ManagerId) && r.IsPending).ToListAsync();
      var requestingUsers = new List<User>();
      requests.ForEach(async req =>
      {
        var u = await _DbContext.Users.Where(u => u.Id == req.UserId).SingleOrDefaultAsync();
        if (u != null) requestingUsers.Add(u);
      });
      ViewBag.ReqUsers = requestingUsers;
      return View(users);
    }

    public async Task<IActionResult> UserDetails(int Id)
    {
      var user = await _DbContext.Users.FindAsync(Id);
      var groupIds = await _DbContext.GroupMemberships.Where(gm => gm.UserId == Id).Select(gm => gm.GroupId).ToListAsync();
      var groups = new List<Group>();
      groupIds.ForEach(async groupId =>
          groups.Add(await _DbContext.Group.Where(g => g.Id == groupId && IsCurrentUser(g.ManagerId)).SingleAsync())
      );
      ViewBag.Groups = groups;
      ViewBag.Tasks = await _DbContext.Task.Where(t => t.UserId == Id).ToListAsync();
      return View(user);
    }

    public async Task<IActionResult> RemoveUser(int Id)
    {
      var user = await _DbContext.Users.FindAsync(Id);
      if (user != null)
      {
        user.ManagerId = null;
        //_DbContext.Users.Update(user);
        await _DbContext.SaveChangesAsync();
      }
      return RedirectToAction("Users");
    }

    public async Task<IActionResult> AcceptRequest(int Id)
    {
      var requests = await _DbContext.ManagerRequests.Where(r => r.UserId == Id).SingleAsync();
      requests.IsPending = false;
      requests.IsAccepted = true;
      //_DbContext.ManagerRequests.Update(requests);
      var user = await _DbContext.Users.FindAsync(Id);
      if (user != null)
      {
        user.ManagerId = requests.ManagerId;
        _DbContext.SaveChanges();
      }
      return RedirectToAction("Users");
    }

    public async Task<IActionResult> RejectRequest(int Id)
    {
      var requests = await _DbContext.ManagerRequests.Where(r => r.UserId == Id).SingleAsync();
      requests.IsPending = false;
      requests.IsAccepted = false;
      //_DbContext.ManagerRequests.Update(requests);
      _DbContext.SaveChanges();
      return RedirectToAction("Users");
    }

    public async Task<IActionResult> Groups()
    {
      ViewBag.Groups = await _DbContext.Group.Where(g => IsCurrentUser(g.ManagerId)).ToListAsync();
      ViewBag.Users = await _DbContext.Users.Where(u => IsCurrentUser(u.ManagerId)).ToListAsync();
      return View();
    }

    public async Task<IActionResult> CreateGroup(Group group, List<string> users)
    {
      if (ModelState.IsValid)
      {
        var currentUser = await GetCurrentUser();
        if (currentUser != null)
        {
          group.ManagerId = currentUser.Id;
          await _DbContext.Group.AddAsync(group);
          await _DbContext.SaveChangesAsync();
        }
        foreach (var userId in users)
        {
          await _DbContext.GroupMemberships.AddAsync(new GroupMemberships() { GroupId = group.Id, UserId = Convert.ToInt32(userId) });
        }
        await _DbContext.SaveChangesAsync();
      }
      return RedirectToAction("Groups");
    }

    public async Task<IActionResult> GroupDetails(int Id)
    {
      var group = await _DbContext.Group.FindAsync(Id);
      var userIds = await _DbContext.GroupMemberships.Where(gm => gm.GroupId == Id).Select(gm => gm.UserId).ToListAsync();
      var users = new List<User>();
      userIds.ForEach(async Id =>
          users.Add(await _DbContext.Users.Where(u => u.Id == Id && IsCurrentUser(u.ManagerId)).SingleAsync())
      );
      ViewBag.Users = users;
      ViewBag.Tasks = await _DbContext.Task.Where(t => t.GroupId == Id).ToListAsync();
      return View(group);
    }

    public async Task<IActionResult> Profile()
    {
      var currentUser = await GetCurrentUser();
      User? user = null;
      if (currentUser != null)
      {
        user = await _DbContext.Users.FindAsync(currentUser.Id);
      }
      if (user != null && user.Image != null)
      {
        var b64 = Convert.ToBase64String(user.Image);
        ViewBag.Image = string.Format("data:image/png;base64," + b64);
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
        _DbContext.Users.Update(user);
        _DbContext.SaveChanges();
      }
      return RedirectToAction("Profile");
    }

  }
}
