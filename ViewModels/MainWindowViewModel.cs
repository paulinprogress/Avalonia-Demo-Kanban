using System.Collections.ObjectModel;

namespace Avalonia_Demo_Kanban.ViewModels;

public class MainWindowViewModel
{
    public ObservableCollection<KanbanColumnViewModel> Columns { get; } = new();

    public MainWindowViewModel()
    {
        Columns.Add(new KanbanColumnViewModel(this, "TODO"));
        Columns.Add(new KanbanColumnViewModel(this, "DOING"));
        Columns.Add(new KanbanColumnViewModel(this, "DONE"));
    }

    /// <summary>
    /// Sets the specified column into add-task mode, cancelling any other column that was in that state.
    /// </summary>
    public void BeginAdding(KanbanColumnViewModel column)
    {
        foreach (var c in Columns)
        {
            c.IsAddingTask = false;
        }

        column.IsAddingTask = true;
    }
}
