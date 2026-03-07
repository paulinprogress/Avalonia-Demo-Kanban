using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Avalonia_Demo_Kanban.Models;

namespace Avalonia_Demo_Kanban.ViewModels;

/// <summary>
/// Represents a column in the Kanban board, managing tasks and task creation state.
/// </summary>
public class KanbanColumnViewModel : INotifyPropertyChanged
{
    private readonly MainWindowViewModel? _owner;

    public string Title { get; }

    public ObservableCollection<TaskItem> Tasks { get; } = new();

    private bool _hasAddTaskButton;
    public bool HasAddTaskButton => _hasAddTaskButton;

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

    private string _newTaskText = "";
    public string NewTaskText
    {
        get => _newTaskText;
        set
        {
            if (_newTaskText == value) return;
            _newTaskText = value;
            OnPropertyChanged();
        }
    }

    public ICommand CreateTaskCommand { get; }
    public ICommand RemoveTaskCommand { get; }
    public ICommand ShowTaskInputCommand { get; }
    public ICommand CancelTaskInputCommand { get; }
    

    public KanbanColumnViewModel(MainWindowViewModel? owner, string title, bool hasAddTaskButton)
    {
        _owner = owner;
        Title = title;
        _hasAddTaskButton = hasAddTaskButton;

        CreateTaskCommand = new RelayCommand(CreateTask);
        RemoveTaskCommand = new RelayCommand<TaskItem>(RemoveTask);
        ShowTaskInputCommand = new RelayCommand(ShowInput);
        CancelTaskInputCommand = new RelayCommand(CancelInput);
    }
    
    private void ShowInput()
    {
        if (_owner != null)
        {
            // Turn IsCreatingTask to false for all columns
            foreach (var c in _owner.Columns)
            {
                c.IsCreatingTask = false;
            }
        }
        // Turn it true for this column
        IsCreatingTask = true;

    }

    private void CreateTask()
    {
        if (string.IsNullOrWhiteSpace(NewTaskText))
        {
            HideInput();
            return;
        }

        AddTask(new TaskItem(NewTaskText));
        HideInput();
    }

    private void CancelInput()
    {
        HideInput();
    }

    private void HideInput()
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
