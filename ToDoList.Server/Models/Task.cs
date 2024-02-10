using System;
using System.Collections.Generic;

namespace ToDoList.Server.Models;

public partial class Task
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool Completed { get; set; }

    public DateTime? Deadline { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
