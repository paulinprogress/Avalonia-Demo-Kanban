using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia_Demo_Kanban.Models;

namespace Avalonia_Demo_Kanban.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<TaskItem> TodoTasks { get; } = new();
    public ObservableCollection<TaskItem> DoingTasks { get; } = new();
    public ObservableCollection<TaskItem> DoneTasks { get; } = new();

    public ICommand AddTodoCommand { get; }

    public MainWindowViewModel()
    {
        // Seed example data
        TodoTasks.Add(new TaskItem("Example Task 1"));
        TodoTasks.Add(new TaskItem("Example Task 2"));
        DoingTasks.Add(new TaskItem("Working on feature"));
        DoneTasks.Add(new TaskItem("Initial setup"));

        // Add command
        AddTodoCommand = new RelayCommand(AddTodo);
    }

    private void AddTodo()
    {
        TodoTasks.Add(new TaskItem("New Task"));
    }
}
