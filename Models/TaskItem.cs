namespace Avalonia_Demo_Kanban.Models;

public class TaskItem
{
    public string Title { get; set; }

    public TaskItem(string title)
    {
        Title = title;
    }
}
