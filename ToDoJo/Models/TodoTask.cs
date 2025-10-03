using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoJo.Models
{
    [Index(nameof(Id), IsUnique = true)]
    public class TodoTask
    {
        public int Id { get; set; }

        [Required]
        [StringLength(256)]
        public string DescTask { get; set; } = string.Empty;

        public DateTime? Deadline { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsCompleted { get; set; } = false;

        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null;

        public TodoTask GetTodoTask()
        {
            return new TodoTask()
            {
                Id = this.Id,
                DescTask = this.DescTask,
                Deadline = this.Deadline,
                CreatedAt = this.CreatedAt,
                IsCompleted = this.IsCompleted,
                UserId = this.UserId
            };
        }
    }
}
