using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Final_Project.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Final_Project.Models;

// Add profile data for application users by adding properties to the User class
public class User : IdentityUser<int>
{
  public int? ManagerId { get; set; }
  public string? DisplayName { get; set; }
  public byte[]? Image { get; set; }
}
