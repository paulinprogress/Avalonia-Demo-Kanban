using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using Avalonia_Demo_Kanban.Models;

namespace Avalonia_Demo_Kanban.ViewModels;

public class KanbanColumnViewModel : INotifyPropertyChanged
{
    private readonly MainWindowViewModel? _owner;

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

    public ICommand CreateTaskCommand { get; }
    public ICommand RemoveTaskCommand { get; }
    public ICommand ShowTaskInputCommand { get; }
    public ICommand CancelTaskInputCommand { get; }

    private bool _isCreatingTask;
    public bool IsCreatingTask
    {
        get => _isCreatingTask;
        set
        {
            if (_isCreatingTask == value) return;
            _isCreatingTask = value;
            OnPropertyChanged();
        }
    }

    public KanbanColumnViewModel(MainWindowViewModel? owner, string title)
    {
        _owner = owner;
        Title = title;
        CreateTaskCommand = new RelayCommand(CreateTask);
        RemoveTaskCommand = new RelayCommand<TaskItem>(RemoveTask);
        ShowTaskInputCommand = new RelayCommand(ShowInput);
        CancelTaskInputCommand = new RelayCommand(CancelInput);
    }

    private void ShowInput()
    {
        if (_owner != null)
            _owner.BeginCreatingTask(this);
        else
            IsCreatingTask = true;
    }


    private void CreateTask()
    {
        // If there was nothing to add, just hide the input box
        if (string.IsNullOrWhiteSpace(NewTaskText))
        {
            IsCreatingTask = false;
            return;
        }

        // Create new task
        AddTask(new TaskItem(NewTaskText));

        NewTaskText = string.Empty;
        IsCreatingTask = false;
    }

    private void CancelInput()
    {
        NewTaskText = string.Empty;
        IsCreatingTask = false;
    }

    public void AddTask(TaskItem task)
    {
        Tasks.Add(task);
    }

    public void RemoveTask(TaskItem task)
    {
        Tasks.Remove(task);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
