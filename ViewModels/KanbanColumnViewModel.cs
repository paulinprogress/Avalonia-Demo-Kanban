using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using Avalonia_Demo_Kanban.Models;

namespace Avalonia_Demo_Kanban.ViewModels;

public class KanbanColumnViewModel : INotifyPropertyChanged
{
    public string Title { get; }

    public ObservableCollection<TaskItem> Tasks { get; } = new();

    private string _newTaskText = "";
    public string NewTaskText
    {
        get => _newTaskText;
        set
        {
            _newTaskText = value;
            OnPropertyChanged();
        }
    }

    public ICommand AddTaskCommand { get; }
    public ICommand RemoveTaskCommand { get; }

    public KanbanColumnViewModel(string title)
    {
        Title = title;
        AddTaskCommand = new RelayCommand(AddTask);
        RemoveTaskCommand = new RelayCommand<TaskItem>(RemoveTask);
    }

    private void AddTask()
    {
        if (string.IsNullOrWhiteSpace(NewTaskText)) return;

        Tasks.Add(new TaskItem(NewTaskText));
        NewTaskText = "";
    }

    private void RemoveTask(TaskItem task)
    {
        Tasks.Remove(task);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
