using MGlobalHotkeys.WPF;
using System.Windows;

namespace MGlobalHotkeys.Demo;

public partial class MainWindow : Window
{
    public Hotkey GlobalHotkey
    {
        get { return (Hotkey)GetValue(GlobalHotkeyProperty); }
        set { SetValue(GlobalHotkeyProperty, value); }
    }

    public static readonly DependencyProperty GlobalHotkeyProperty =
        DependencyProperty.Register("GlobalHotkey", typeof(Hotkey), typeof(MainWindow), new PropertyMetadata(null));

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
    }
}
