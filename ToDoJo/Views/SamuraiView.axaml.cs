using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ToDoJo.Views;

public partial class SamuraiView : UserControl
{
    public SamuraiView()
    {
        InitializeComponent();
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}