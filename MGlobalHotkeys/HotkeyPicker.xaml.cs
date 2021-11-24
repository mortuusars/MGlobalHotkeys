using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MGlobalHotkeys.WPF;

public partial class HotkeyPicker : UserControl
{
    /// <summary>
    /// Allows binding. Two way by default.
    /// </summary>
    public static readonly DependencyProperty HotkeyProperty =
        DependencyProperty.Register(nameof(Hotkey), typeof(Hotkey), typeof(HotkeyPicker),
            new FrameworkPropertyMetadata(default(Hotkey), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>
    /// Controls if focus should be removed from HotkeyPicker control on cancel (Pressing ESC when picking a hotkey).
    /// </summary>
    public static readonly DependencyProperty ClearFocusOnCancelProperty =
        DependencyProperty.Register("ClearFocusOnCancel", typeof(bool), typeof(HotkeyPicker), new PropertyMetadata(true));

    /// <summary>
    /// Controls if focus should be removed from HotkeyPicker control after clearing set hotkey (Pressing Del or Backspace when picking a hotkey).
    /// </summary>
    public static readonly DependencyProperty ClearFocusOnClearProperty =
        DependencyProperty.Register("ClearFocusOnClear", typeof(bool), typeof(HotkeyPicker), new PropertyMetadata(true));

    /// <summary>
    /// Allows binding. Two way by default.
    /// </summary>
    public Hotkey Hotkey
    {
        get => (Hotkey)GetValue(HotkeyProperty);
        set => SetValue(HotkeyProperty, value);
    }

    /// <summary>
    /// Controls if focus should be removed from HotkeyPicker control after cancelling (Pressing ESC when picking a hotkey).
    /// </summary>
    public bool ClearFocusOnCancel
    {
        get { return (bool)GetValue(ClearFocusOnCancelProperty); }
        set { SetValue(ClearFocusOnCancelProperty, value); }
    }

    /// <summary>
    /// Controls if focus should be removed from HotkeyPicker control after clearing set hotkey (Pressing Del or Backspace when picking a hotkey).
    /// </summary>
    public bool ClearFocusOnClear
    {
        get { return (bool)GetValue(ClearFocusOnClearProperty); }
        set { SetValue(ClearFocusOnClearProperty, value); }
    }

    private Hotkey? _previousHotkey = null;

    /// <summary>
    /// Allows to pick hotkeys by pressing them.
    /// </summary>
    public HotkeyPicker()
    {
        InitializeComponent();
    }

    private void HotkeyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        e.Handled = true;

        if (_previousHotkey is null)
            _previousHotkey = Hotkey;

        var modifiers = Keyboard.Modifiers;
        var key = e.Key;

        if (key == Key.System)
            key = e.SystemKey;

        if (modifiers == ModifierKeys.None && key == Key.Escape)
        {
            Hotkey = _previousHotkey;
            if (ClearFocusOnCancel)
                ClearFocus();
            return;
        }

        if (modifiers == ModifierKeys.None && (key == Key.Delete || key == Key.Back))
        {
            Hotkey = Hotkey.Empty;
            if (ClearFocusOnClear)
                ClearFocus();
            return;
        }

        if (IsModifier(key))
            return;

        Hotkey = new Hotkey(key, modifiers);
    }

    private void ClearFocus()
    {
        FocusManager.SetFocusedElement(FocusManager.GetFocusScope(this), null);
        Keyboard.ClearFocus();
    }

    private bool IsModifier(Key key)
    {
        return key == Key.LeftCtrl ||
            key == Key.RightCtrl ||
            key == Key.LeftAlt ||
            key == Key.RightAlt ||
            key == Key.LeftShift ||
            key == Key.RightShift ||
            key == Key.LWin ||
            key == Key.RWin ||
            key == Key.Clear ||
            key == Key.OemClear ||
            key == Key.Apps;
    }

    private void HotkeyTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        _previousHotkey = Hotkey;
    }
}
