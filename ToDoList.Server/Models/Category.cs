using System;
using System.Collections.Generic;

namespace ToDoList.Server.Models;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
