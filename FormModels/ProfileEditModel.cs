using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Final_Project.FormModels
{
  public class ProfileEditModel
  {
    [Required]
    public string DisplayName { get; set; }
    [Required]
    [EmailAddress]
    public string ManagerEmail { get; set; }
    public IFormFile Image { get; set; }
  }
}
