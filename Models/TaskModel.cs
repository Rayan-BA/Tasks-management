﻿using System.ComponentModel.DataAnnotations;

namespace Final_Project.Models
{
  public class TaskModel
  {
    [Key]
    public int Id { get; set; }
    public required int AssignerId { get; set; }
    public int? GroupId { get; set; }
    public int? UserId { get; set; }
    [Required]
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime DueAt { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsImportant { get; set; }
  }
}
