using System.Collections.ObjectModel;

namespace Avalonia_Demo_Kanban.ViewModels;

public class MainWindowViewModel
{
    public ObservableCollection<KanbanColumnViewModel> Columns { get; } = new();

    public MainWindowViewModel()
    {
        Columns.Add(new KanbanColumnViewModel("TODO"));
        Columns.Add(new KanbanColumnViewModel("DOING"));
        Columns.Add(new KanbanColumnViewModel("DONE"));
    }
}
