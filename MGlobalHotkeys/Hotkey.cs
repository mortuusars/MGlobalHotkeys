using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MGlobalHotkeys;

/// <summary>
/// Represents a key with modifiers.
/// </summary>
public record Hotkey
{
    public Key Key { get; init; }
    public ModifierKeys Modifiers { get; init; }

    /// <summary>
    /// Hotkey with Key.None and ModifierKeys.None.
    /// </summary>
    public static Hotkey Empty { get; } = new Hotkey(Key.None, ModifierKeys.None);

    public Hotkey(Key key, ModifierKeys modifiers)
    {
        Key = key;
        Modifiers = modifiers;
    }

    /// <summary>
    /// Parse Hotkey from string representation.
    /// </summary>
    /// <param name="hotkey"></param>
    /// <exception cref="ArgumentNullException">Thrown if input string is null.</exception>
    public static Hotkey FromString(string hotkey)
    {
        if (hotkey is null)
            throw new ArgumentNullException(nameof(hotkey));

        var keys = hotkey.Split('+');

        if (!Enum.TryParse<Key>(keys[^1], ignoreCase:true, out Key regularKey))
            return Hotkey.Empty;

        Key key = regularKey;
        ModifierKeys modifierKeys = ModifierKeys.None;

        for (int i = 0; i < keys.Length - 1; i++)
        {
            if (keys[i].Trim().Equals("Ctrl", StringComparison.InvariantCultureIgnoreCase))
                modifierKeys |= ModifierKeys.Control;
            else if (keys[i].Trim().Equals("Alt", StringComparison.InvariantCultureIgnoreCase))
                modifierKeys |= ModifierKeys.Alt;
            else if (keys[i].Trim().Equals("Shift", StringComparison.InvariantCultureIgnoreCase))
                modifierKeys |= ModifierKeys.Shift;
            else if (keys[i].Trim().Equals("Win", StringComparison.InvariantCultureIgnoreCase))
                modifierKeys |= ModifierKeys.Windows;
        }

        return new Hotkey(key, modifierKeys);
    }

    public override string ToString()
    {
        var str = new StringBuilder();

        if (Modifiers.HasFlag(ModifierKeys.Control))
            str.Append("Ctrl + ");
        if (Modifiers.HasFlag(ModifierKeys.Shift))
            str.Append("Shift + ");
        if (Modifiers.HasFlag(ModifierKeys.Alt))
            str.Append("Alt + ");
        if (Modifiers.HasFlag(ModifierKeys.Windows))
            str.Append("Win + ");

        str.Append(Key);

        return str.ToString();
    }
}