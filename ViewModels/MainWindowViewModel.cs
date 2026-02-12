using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using Avalonia_Demo_Kanban.Models;

namespace Avalonia_Demo_Kanban.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
{
    public ObservableCollection<TaskItem> TodoTasks { get; } = new();
    public ObservableCollection<TaskItem> DoingTasks { get; } = new();
    public ObservableCollection<TaskItem> DoneTasks { get; } = new();

    private string _newTodoText = "";
    public string NewTodoText
    {
        get => _newTodoText;
        set
        {
            _newTodoText = value;
            OnPropertyChanged();
        }
    }

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
        if (string.IsNullOrWhiteSpace(NewTodoText)) return;
        
        TodoTasks.Add(new TaskItem(NewTodoText));
        NewTodoText = "";
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    protected new void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
