using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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

    public ICommand AddTaskCommand { get; }
    public ICommand RemoveTaskCommand { get; }
    public ICommand ShowAddTaskCommand { get; }
    public ICommand CancelAddTaskCommand { get; }

    private bool _isAddingTask;
    public bool IsAddingTask
    {
        get => _isAddingTask;
        set
        {
            if (_isAddingTask == value) return;
            _isAddingTask = value;
            OnPropertyChanged();
        }
    }

    public KanbanColumnViewModel(MainWindowViewModel? owner, string title)
    {
        _owner = owner;
        Title = title;
        AddTaskCommand = new RelayCommand(AddTask);
        RemoveTaskCommand = new RelayCommand<TaskItem>(RemoveTask);
        ShowAddTaskCommand = new RelayCommand(ShowAdd);
        CancelAddTaskCommand = new RelayCommand(CancelAdd);
    }

    private void ShowAdd()
    {
        if (_owner != null)
            _owner.BeginAdding(this);
        else
            IsAddingTask = true;
    }


    private void AddTask()
    {
        // If there was nothing to add, just hide the input box
        if (string.IsNullOrWhiteSpace(NewTaskText))
        {
            IsAddingTask = false;
            return;
        }

        // Create new task
        Tasks.Add(new TaskItem(NewTaskText));

        NewTaskText = string.Empty;
        IsAddingTask = false;
    }

    private void CancelAdd()
    {
        NewTaskText = string.Empty;
        IsAddingTask = false;
    }

    // public helper used by drag & drop logic in the view
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
