using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia_Demo_Kanban.Models;

namespace Avalonia_Demo_Kanban.ViewModels;

public class MainWindowViewModel
{
    public ObservableCollection<KanbanColumnViewModel> Columns { get; } = new();

    public TaskItem DraggingTaskItem { get; set; } = new TaskItem("");

    public MainWindowViewModel()
    {
        Columns.Add(new KanbanColumnViewModel(this, "TODO"));
        Columns.Add(new KanbanColumnViewModel(this, "DOING"));
        Columns.Add(new KanbanColumnViewModel(this, "DONE"));
    }

    // Sets the specified column into add-task mode, cancelling any other column that was in that state.
    public void BeginAdding(KanbanColumnViewModel column)
    {
        foreach (var c in Columns)
        {
            c.IsAddingTask = false;
        }

        column.IsAddingTask = true;
    }

    // Find TaskItem from unique string Id by checking all columns
    public TaskItem GetTaskFromId(string id)
    {
        foreach (KanbanColumnViewModel column in Columns)
        {
            foreach (TaskItem task in column.Tasks)
            {
                if (task.Id == id) return task;
            }
        }

        return null;
    }

}
