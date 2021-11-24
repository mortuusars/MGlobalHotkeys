using System;
using System.Windows.Input;
using Xunit;

namespace MGlobalHotkeys.Tests;
public class HotkeyTests
{
    public static readonly object[][] FromString = new object[][]
    {
        new object[] {"Ctrl + Shift + A", new Hotkey(Key.A, ModifierKeys.Control | ModifierKeys.Shift)},
        new object[] {"Ctrl + Shift + NumPad0", new Hotkey(Key.NumPad0, ModifierKeys.Control | ModifierKeys.Shift)},
        new object[] {"Ctrl + Shift + Home", new Hotkey(Key.Home, ModifierKeys.Control | ModifierKeys.Shift)},
        new object[] {"ctrl + shift + home", new Hotkey(Key.Home, ModifierKeys.Control | ModifierKeys.Shift)},
        new object[] {"Ctrl+shift+home", new Hotkey(Key.Home, ModifierKeys.Control | ModifierKeys.Shift)},
        new object[] {"Ctrl + Shift + Win + A", new Hotkey(Key.A, ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Windows) },
        new object[] {"", new Hotkey(Key.None, ModifierKeys.None) },
        new object[] {"++", new Hotkey(Key.None, ModifierKeys.None) },
        new object[] {"+a+", new Hotkey(Key.None, ModifierKeys.None) },
        new object[] {"++tab", new Hotkey(Key.Tab, ModifierKeys.None) },
        new object[] {"++ctrltab", new Hotkey(Key.None, ModifierKeys.None) },
        new object[] {"asdaw", new Hotkey(Key.None, ModifierKeys.None) },
        new object[] {"a", new Hotkey(Key.A, ModifierKeys.None) },
        new object[] {"B", new Hotkey(Key.B, ModifierKeys.None) },
        new object[] {"insert", new Hotkey(Key.Insert, ModifierKeys.None) },
        new object[] {"F1", new Hotkey(Key.F1, ModifierKeys.None) },

    };

    [Theory, MemberData(nameof(FromString))]
    public void HotkeyFromStringShouldParseProperly(string stringRepresentation, Hotkey expected)
    {
        Assert.Equal(expected, Hotkey.FromString(stringRepresentation));
    }

    [Fact]
    public void HotkeyFromStringShouldThrowArgumentNullIfStringIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => Hotkey.FromString(null!));
    }
}
