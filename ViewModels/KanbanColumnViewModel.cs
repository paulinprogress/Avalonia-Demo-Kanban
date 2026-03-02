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
        if (string.IsNullOrWhiteSpace(NewTaskText))
        {
            // if there was nothing to add, just hide the input box
            IsAddingTask = false;
            return;
        }

        Tasks.Add(new TaskItem(NewTaskText));
        NewTaskText = string.Empty;
        IsAddingTask = false;
    }

    private void CancelAdd()
    {
        NewTaskText = string.Empty;
        IsAddingTask = false;
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
