using System.ComponentModel.DataAnnotations;

namespace Final_Project.Models
{
  public class GroupMemberships
  {
    [Key]
    public int Id { get; set; }
    public int GroupId { get; set; }
    public int UserId { get; set; }
  }
}
