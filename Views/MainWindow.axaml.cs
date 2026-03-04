using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia_Demo_Kanban.Models;
using Avalonia_Demo_Kanban.ViewModels;

namespace Avalonia_Demo_Kanban.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        AddHandler(DragDrop.DragOverEvent, Column_DragOver);
        AddHandler(DragDrop.DropEvent, Column_Drop);

        // ensure ghost overlay starts hidden
        GhostItem.IsVisible = false;
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (DataContext is MainWindowViewModel vm)
        {
            var source = e.Source as Control;
            if (source != null)
            {
                // Check if currently open add task command (exit if not)
                var addingColumn = vm.Columns.FirstOrDefault(c => c.IsAddingTask);
                if (addingColumn is null) return;

                // Cancel if PointerPressed source was another add task button (other column) or was outside of current task input (textbox or confirm button)
                if (IsAddTaskButton(source) || !IsAddTaskInputOrChild(source))
                {
                    addingColumn.CancelAddTaskCommand.Execute(null);
                }
            }
        }
    }

    private void TextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        // Submit task on Enter (without modifiers), allow Shift+Enter for new line
        if (e.Key == Key.Return && e.KeyModifiers == KeyModifiers.None)
        {
            e.Handled = true;
            if (sender is TextBox textBox && textBox.DataContext is KanbanColumnViewModel vm)
            {
                vm.AddTaskCommand.Execute(null);
            }
        }
    }

    private bool IsAddTaskInputOrChild(Control control)
    {
        // Check if the control is a TextBox or a Button with "✓" content, or their children
        if (control is TextBox)
            return true;

        if (control is Button button && button.Content?.ToString() == "✓")
            return true;

        // Check if this control is a child of a TextBox or submit button
        var parent = control.Parent;
        while (parent != null)
        {
            if (parent is TextBox)
                return true;
            if (parent is Button btn && btn.Content?.ToString() == "✓")
                return true;
            parent = parent.Parent;
        }

        return false;
    }

    private bool IsAddTaskButton(Control control)
    {
        // Check if this is a "+" add button or a child of one
        if (control is Button button && button.Content?.ToString() == "+")
            return true;

        var parent = control.Parent;
        while (parent != null)
        {
            if (parent is Button btn && btn.Content?.ToString() == "+")
                return true;
            parent = parent.Parent;
        }

        return false;
    }

    // --- drag & drop logic ------------------------------------------------

    private async void Task_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Border border) return;
        if (border.DataContext is not TaskItem task) return;
        if (DataContext is not MainWindowViewModel vm) return;
        
        var sourceColumn = vm.Columns.FirstOrDefault(c => c.Tasks.Contains(task));
        if (sourceColumn is null) return;

        vm.DraggingTaskItem = task;

        var mousePos = e.GetPosition(this);
        var borderPos = border.Bounds.TopLeft;
        GhostItem.RenderTransform = new TranslateTransform(borderPos.X, borderPos.Y);
        GhostItem.IsVisible = true;

        // Prepare the DataTransfer
        var dragData = new DataTransfer();
        var dragDataItem0 = DataTransferItem.Create(DataFormat.Text, task.Id);
        var dragDataItem1 = DataTransferItem.Create(DataFormat.Text, border.Bounds.Height.ToString());
        var dragDataItem2 = DataTransferItem.Create(DataFormat.Text, border.Bounds.Width.ToString());
        var dragDataItem3 = DataTransferItem.Create(DataFormat.Text, mousePos.X.ToString());
        var dragDataItem4 = DataTransferItem.Create(DataFormat.Text, mousePos.Y.ToString());
        var dragDataItem5 = DataTransferItem.Create(DataFormat.Text, borderPos.X.ToString());
        var dragDataItem6 = DataTransferItem.Create(DataFormat.Text, borderPos.Y.ToString());
        dragData.Add(dragDataItem0);
        dragData.Add(dragDataItem1);
        dragData.Add(dragDataItem2);
        dragData.Add(dragDataItem3);
        dragData.Add(dragDataItem4);
        dragData.Add(dragDataItem5);
        dragData.Add(dragDataItem6);

        // Start DragDrop operation
        await DragDrop.DoDragDropAsync(e, dragData, DragDropEffects.Move);
    }

    // While task is being dragged
    private void Column_DragOver(object? sender, DragEventArgs e)
    {
        if (e.DataTransfer is null) return;
        if (DataContext is not MainWindowViewModel vm) return;

        // Get task item
        var taskId = e.DataTransfer.Items[0].TryGetText();
        var task = vm.GetTaskFromId(taskId);

        // Apply effect
        if (task is not null)
        {
            e.DragEffects = DragDropEffects.Move;
        }
        else
        {
            e.DragEffects = DragDropEffects.None;
        }

        // Update ghost item
        var taskHeight = Convert.ToDouble(e.DataTransfer.Items[1].TryGetText());
        var taskWidth = Convert.ToDouble(e.DataTransfer.Items[2].TryGetText());
        GhostItem.Height = taskHeight;
        GhostItem.Width = taskWidth;
        
        var mousePos = e.GetPosition(this);
        var originalMousePosX = Convert.ToDouble(e.DataTransfer.Items[3].TryGetText());
        var originalMousePosY = Convert.ToDouble(e.DataTransfer.Items[4].TryGetText());
        var originalBorderPosX = Convert.ToDouble(e.DataTransfer.Items[5].TryGetText());
        var originalBorderPosY = Convert.ToDouble(e.DataTransfer.Items[6].TryGetText());
        var targetPosX = mousePos.X - originalMousePosX;
        var targetPosY = mousePos.Y - originalMousePosY;
        GhostItem.RenderTransform = new TranslateTransform(targetPosX, targetPosY);
    }

    // When task is dropped
    private void Column_Drop(object? sender, DragEventArgs e)
    {
        if (e.DataTransfer is null) return;
        if (DataContext is not MainWindowViewModel vm) return;

        var taskId = e.DataTransfer.Items.First().TryGetText();
        var task = vm.GetTaskFromId(taskId);

        var source = vm.Columns.FirstOrDefault(c => c.Tasks.Contains(task));
        var target = (sender as Control)?.DataContext as KanbanColumnViewModel;

        if (task == null || source == null || target == null || source == target)
        {
            // hide ghost even if drop is invalid
            GhostItem.IsVisible = false;
            return;
        }

        source.RemoveTask(task);
        target.Tasks.Add(task);

        // hide ghost after successful drop
        GhostItem.IsVisible = false;
    }

}