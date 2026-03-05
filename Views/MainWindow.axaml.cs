using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia_Demo_Kanban.Models;
using Avalonia_Demo_Kanban.ViewModels;

namespace Avalonia_Demo_Kanban.Views;

/// <summary>
/// Main window for the Kanban board application, handling task interaction and drag-drop operations.
/// </summary>
public partial class MainWindow : Window
{
    private const string AddTaskButtonContent = "+";
    private const string ConfirmTaskButtonContent = "✓";

    public MainWindow()
    {
        InitializeComponent();

        AddHandler(DragDrop.DragOverEvent, Task_DragOver);
        AddHandler(DragDrop.DropEvent, Task_Drop);

        // Ensure ghost overlay starts hidden
        GhostItem.IsVisible = false;
    }

    // Handles pointer press events to cancel task input when clicking outside the input area
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (DataContext is MainWindowViewModel vm)
        {
            var source = e.Source as Control;
            if (source != null)
            {
                // Check if a column is currently in task creation mode
                var creatingColumn = vm.Columns.FirstOrDefault(c => c.IsCreatingTask);
                if (creatingColumn is null) return;

                // Cancel task input if user clicked another add button or outside the input area
                if (IsAddTaskButton(source) || !IsAddTaskInputOrChild(source))
                {
                    creatingColumn.CancelTaskInputCommand.Execute(null);
                }
            }
        }
    }

    // Handles key down events in the task input text box.
    // Submits the task on Enter (without modifiers), allowing Shift+Enter for new lines.
    private void TextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        // Submit task on Enter (without modifiers, allow Shift+Enter for new line)
        if (e.Key == Key.Return && e.KeyModifiers == KeyModifiers.None)
        {
            e.Handled = true;
            if (sender is TextBox textBox && textBox.DataContext is KanbanColumnViewModel vm)
            {
                vm.CreateTaskCommand.Execute(null);
            }
        }
    }

    // Determines if a control is part of the task input area (text box or confirm button) or is a child of one
    private bool IsAddTaskInputOrChild(Control control)
    {
        return control is TextBox ||
               IsButtonWithContent(control, ConfirmTaskButtonContent) ||
               HasAncestorMatching(control, c => c is TextBox || IsButtonWithContent(c, ConfirmTaskButtonContent));
    }

    // Determines if a control is an "add task" button or is a child of one
    private bool IsAddTaskButton(Control control)
    {
        return IsButtonWithContent(control, AddTaskButtonContent) ||
               HasAncestorMatching(control, c => IsButtonWithContent(c, AddTaskButtonContent));
    }

    // Checks if a control is a button with the specified content
    private static bool IsButtonWithContent(Control control, string expectedContent)
    {
        return control is Button button && button.Content?.ToString() == expectedContent;
    }

    // Walks up the parent hierarchy to find an ancestor matching the given predicate
    private static bool HasAncestorMatching(Control control, Func<Control, bool> predicate)
    {
        var parent = control.Parent as Control;
        while (parent != null)
        {
            if (predicate(parent))
                return true;
            parent = parent.Parent as Control;
        }
        return false;
    }

    // --- Drag & Drop Logic ------------------------------------------------

    // Handles the start of a drag operation when a task is pressed.
    // Sets up the ghost image and initiates the drag-drop operation.
    private async void Task_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Border taskBorder) return;
        if (taskBorder.DataContext is not TaskItem task) return;
        if (DataContext is not MainWindowViewModel vm) return;
        
        var sourceColumn = vm.Columns.FirstOrDefault(c => c.Tasks.Contains(task));
        if (sourceColumn is null) return;

        vm.DraggingTaskItem = task;
        vm.DraggingTaskBorder = taskBorder;

        var mousePosition = e.GetPosition(this);
        var mouseOffset = e.GetPosition(taskBorder);

        // Set up ghost item at current mouse position
        GhostItem.RenderTransform = new TranslateTransform(mousePosition.X - mouseOffset.X, mousePosition.Y - mouseOffset.Y);
        GhostItem.IsVisible = true;
        GhostItem.Height = taskBorder.Bounds.Height;
        GhostItem.Width = taskBorder.Bounds.Width;

        // Hide original task while dragging
        taskBorder.IsVisible = false;

        // Prepare drag data: task ID and mouse offset for proper positioning during drag
        var dragData = new DataTransfer();
        dragData.Add(DataTransferItem.Create(DataFormat.Text, task.Id));
        dragData.Add(DataTransferItem.Create(DataFormat.Text, mouseOffset.X.ToString()));
        dragData.Add(DataTransferItem.Create(DataFormat.Text, mouseOffset.Y.ToString()));

        // Start the drag-drop operation
        var effects = await DragDrop.DoDragDropAsync(e, dragData, DragDropEffects.Move);

        // Restore original task visibility if drag was cancelled or dropped outside valid target
        if (effects == DragDropEffects.None)
        {
            GhostItem.IsVisible = false;
            taskBorder.IsVisible = true;
        }
    }

    // Handles the drag over event, updating the ghost image position to follow the mouse.
    private void Task_DragOver(object? sender, DragEventArgs e)
    {
        if (e.DataTransfer is null) return;

        var mousePosition = e.GetPosition(this);
        var mouseOffsetX = Convert.ToDouble(e.DataTransfer.Items[1].TryGetText());
        var mouseOffsetY = Convert.ToDouble(e.DataTransfer.Items[2].TryGetText());
        GhostItem.RenderTransform = new TranslateTransform(mousePosition.X - mouseOffsetX, mousePosition.Y - mouseOffsetY);
    }

    // Handles the drop event, moving the task to the target column if valid.
    private void Task_Drop(object? sender, DragEventArgs e)
    {
        if (e.DataTransfer is null) return;
        if (DataContext is not MainWindowViewModel vm) return;

        var taskId = e.DataTransfer.Items[0].TryGetText();
        if (taskId != null)
        {
            var task = vm.GetTaskFromId(taskId);
            var sourceColumn = vm.Columns.FirstOrDefault(c => c.Tasks.Contains(task));
            var targetColumn = (sender as Border)?.DataContext as KanbanColumnViewModel;

            // Move task between columns if source and target are different
            if (sourceColumn != null && targetColumn != null && sourceColumn != targetColumn)
            {
                sourceColumn.RemoveTask(task);
                targetColumn.AddTask(task);
            }
        }

        // Clean up drag-drop UI state
        GhostItem.IsVisible = false;
        vm.DraggingTaskBorder.IsVisible = true;
    }

}