using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MGlobalHotkeys.WPF;

public partial class HotkeyPicker : UserControl
{
    /// <summary>
    /// Indicates whether HotkeyPicker is focused and accepts input.
    /// </summary>
    public new bool IsFocused
    {
        get { return (bool)GetValue(IsFocusedProperty); }
        set { SetValue(IsFocusedProperty, value); }
    }

    /// <summary>
    /// propdp of IsFocused.
    /// </summary>
    public  static new readonly DependencyProperty IsFocusedProperty =
        DependencyProperty.Register(nameof(IsFocused), typeof(bool), typeof(HotkeyPicker), new PropertyMetadata(false));

    /// <summary>
    /// Allows binding. Two way by default.
    /// </summary>
    public Hotkey Hotkey
    {
        get => (Hotkey)GetValue(HotkeyProperty);
        set => SetValue(HotkeyProperty, value);
    }

    /// <summary>
    /// Allows binding. Two way by default.
    /// </summary>
    public static readonly DependencyProperty HotkeyProperty =
        DependencyProperty.Register(nameof(Hotkey), typeof(Hotkey), typeof(HotkeyPicker),
            new FrameworkPropertyMetadata(default(Hotkey), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>
    /// Controls if focus should be removed from HotkeyPicker control after cancelling (Pressing ESC when picking a hotkey).
    /// </summary>
    public bool ClearFocusOnCancel
    {
        get { return (bool)GetValue(ClearFocusOnCancelProperty); }
        set { SetValue(ClearFocusOnCancelProperty, value); }
    }

    /// <summary>
    /// Controls if focus should be removed from HotkeyPicker control on cancel (Pressing ESC when picking a hotkey).
    /// </summary>
    public static readonly DependencyProperty ClearFocusOnCancelProperty =
        DependencyProperty.Register("ClearFocusOnCancel", typeof(bool), typeof(HotkeyPicker), new PropertyMetadata(true));

    /// <summary>
    /// Controls if focus should be removed from HotkeyPicker control after clearing set hotkey (Pressing Del or Backspace when picking a hotkey).
    /// </summary>
    public bool ClearFocusOnClear
    {
        get { return (bool)GetValue(ClearFocusOnClearProperty); }
        set { SetValue(ClearFocusOnClearProperty, value); }
    }

    /// <summary>
    /// Controls if focus should be removed from HotkeyPicker control after clearing set hotkey (Pressing Del or Backspace when picking a hotkey).
    /// </summary>
    public static readonly DependencyProperty ClearFocusOnClearProperty =
        DependencyProperty.Register("ClearFocusOnClear", typeof(bool), typeof(HotkeyPicker), new PropertyMetadata(true));

    /// <summary>
    /// Corner radius of a control's border.
    /// </summary>
    public CornerRadius CornerRadius
    {
        get { return (CornerRadius)GetValue(CornerRadiusProperty); }
        set { SetValue(CornerRadiusProperty, value); }
    }

    /// <summary>
    /// propdp of a CornerRadius.
    /// </summary>
    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(HotkeyPicker), new PropertyMetadata(new CornerRadius(0)));

    /// <summary>
    /// Brush of the selected text.
    /// </summary>
    public Brush SelectionBrush
    {
        get { return (Brush)GetValue(SelectionBrushProperty); }
        set { SetValue(SelectionBrushProperty, value); }
    }

    /// <summary>
    /// propdp of the SelectionBrush
    /// </summary>
    public static readonly DependencyProperty SelectionBrushProperty =
        DependencyProperty.Register(nameof(SelectionBrush), typeof(Brush), typeof(HotkeyPicker), new PropertyMetadata(Brushes.SkyBlue));


    private Hotkey? _previousHotkey = null;
    private bool _mouseCaptured = false;

    /// <summary>
    /// Allows to pick hotkeys by pressing them.
    /// </summary>
    public HotkeyPicker()
    {
        InitializeComponent();

        MouseLeftButtonDown += HotkeyPicker_MouseLeftButtonDown;

        HotkeyTextBox.IsKeyboardFocusedChanged += HotkeyTextBox_IsKeyboardFocusedChanged;
        HotkeyTextBox.PreviewMouseLeftButtonDown += HotkeyTextBox_PreviewMouseLeftButtonDown;
        HotkeyTextBox.LostMouseCapture += HotkeyPicker_LostMouseCapture;
    }

    private void HotkeyPicker_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!_mouseCaptured)
        {
            ActivateEditMode();
            e.Handled = true;
        }
    }

    private void ActivateEditMode()
    {
        _mouseCaptured = true;
        HotkeyTextBox.Focus();
        HotkeyTextBox.CaptureMouse();

        // Clear selection. Setting it directly does not work. This just delays it a tiny bit.
        Dispatcher.BeginInvoke(() => HotkeyTextBox.SelectionLength = 0);
    }

    private void HotkeyTextBox_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        IsFocused = (bool)e.NewValue;
    }

    private void HotkeyTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {        
        if (!_mouseCaptured)
        {
            e.Handled = true;
            ActivateEditMode();
        }
        else
        {
            var hitTestResult = VisualTreeHelper.HitTest(this, Mouse.GetPosition(this));

            if (hitTestResult is null)
            {
                e.Handled = true;
                ReleaseMouseRemoveFocus();
            }
        }
    }

    private void HotkeyPicker_LostMouseCapture(object sender, MouseEventArgs e)
    {
        if (_mouseCaptured)
            HotkeyTextBox.CaptureMouse();
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
                ReleaseMouseRemoveFocus();
            return;
        }

        if (modifiers == ModifierKeys.None && (key == Key.Delete || key == Key.Back))
        {
            Hotkey = Hotkey.Empty;
            if (ClearFocusOnClear)
                ReleaseMouseRemoveFocus();
            return;
        }

        if (IsModifier(key))
            return;

        Hotkey = new Hotkey(key, modifiers);
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

    private void ReleaseMouseRemoveFocus()
    {
        _previousHotkey = Hotkey;
        _mouseCaptured = false;
        HotkeyTextBox.ReleaseMouseCapture();
        HotkeyTextBox.MoveFocusToParent();

        //FocusManager.SetFocusedElement(FocusManager.GetFocusScope(this), null);
        //    Keyboard.ClearFocus();
    }
}
