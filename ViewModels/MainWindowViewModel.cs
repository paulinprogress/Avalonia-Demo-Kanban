using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Controls;

using Avalonia_Demo_Kanban.Models;

namespace Avalonia_Demo_Kanban.ViewModels;

/// <summary>
/// Main ViewModel for the whole Kanban board, managing columns and inter-column task operations.
/// </summary>
public partial class MainWindowViewModel : ObservableObject
{
    public ObservableCollection<KanbanColumnViewModel> Columns { get; } = new();

    [ObservableProperty] private TaskItem _draggingTaskItem = new("");
    public Border DraggingTaskBorder { get; set; } = new();

    public MainWindowViewModel()
    {
        Columns.Add(new KanbanColumnViewModel(this, "TODO", true));
        Columns.Add(new KanbanColumnViewModel(this, "DOING", false));
        Columns.Add(new KanbanColumnViewModel(this, "DONE", false));
    }

    // Retrieves a task by its unique identifier across all columns.
    public TaskItem GetTaskFromId(string id)
    {
        var task = Columns
            .SelectMany(column => column.Tasks)
            .FirstOrDefault(task => task.Id == id);

        if (task == null)
            throw new ArgumentException($"Task with ID '{id}' not found.", nameof(id));

        return task;
    }

}
