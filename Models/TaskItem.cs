using System;

namespace Avalonia_Demo_Kanban.Models;

public class TaskItem
{
    public string Id { get; set; }
    public string Title { get; set; }

    public TaskItem(string title)
    {
        Id = Guid.NewGuid().ToString();
        Title = title;
    }
}
