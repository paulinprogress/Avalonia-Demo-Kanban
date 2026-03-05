using System;

namespace Avalonia_Demo_Kanban.Models;

public class TaskItem
{
    private string _id;
    private string _title;

    public string Id => _id;
    public string Title => _title;

    public TaskItem(string title)
    {
        _id = Guid.NewGuid().ToString();
        _title = title;
    }
}
