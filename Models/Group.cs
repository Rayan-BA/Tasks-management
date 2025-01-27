using System.ComponentModel.DataAnnotations;

namespace Final_Project.Models
{
  public class Group
  {
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    public int ManagerId { get; set; }
  }
}
