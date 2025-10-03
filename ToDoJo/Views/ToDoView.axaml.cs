using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Diagnostics;
using ToDoJo.ViewModels;

namespace ToDoJo.Views;

public partial class ToDoView : UserControl
{
    public ToDoView()
    {
        InitializeComponent();
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}