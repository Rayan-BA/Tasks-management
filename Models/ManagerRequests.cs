using System.ComponentModel.DataAnnotations;

namespace Final_Project.Models
{
  public class ManagerRequests
  {
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ManagerId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsPending { get; set; }
    public bool IsAccepted { get; set; }
  }
}
