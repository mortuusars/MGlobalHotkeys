using System.Text;
using System.Windows.Input;

namespace MGlobalHotkeys.WPF;

/// <summary>
/// Represents a key with modifiers.
/// </summary>
public record Hotkey
{
    /// <summary>
    /// Main Key of the hotkey.
    /// </summary>
    public Key Key { get; init; }
    /// <summary>
    /// Modifiers of the hotkey.
    /// </summary>
    public ModifierKeys Modifiers { get; init; }

    /// <summary>
    /// Hotkey with Key.None and ModifierKeys.None.
    /// </summary>
    public static Hotkey Empty { get; } = new Hotkey(Key.None, ModifierKeys.None);

    /// <summary>
    /// Creates an instance of Hotkey class, representing a key with modifiers.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="modifiers"></param>
    public Hotkey(Key key, ModifierKeys modifiers)
    {
        Key = key;
        Modifiers = modifiers;
    }

    /// <summary>
    /// Parse Hotkey from string representation of keys separated by '+'.<br/>
    /// Input Example: 'Ctrl + Alt + B'
    /// </summary>
    /// <param name="hotkey">String representation of a hotkey with or without modifiers. Ignores case.</param>
    /// <returns>Hotkey that matches input string. </returns>
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

    /// <summary>
    /// Outputs a string representation of a Hotkey, e.g 'Ctrl + Shift + A'.
    /// </summary>
    /// <returns>Hotkey as a string.</returns>
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