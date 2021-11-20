using System.Windows.Input;

namespace MGlobalHotkeys;

/// <summary>
/// Provides funcionality to register and remove global hotkeys.
/// </summary>
public class GlobalHotkeys
{
    /// <summary>
    /// Dictionary of currently registered hotkeys.
    /// </summary>
    public Dictionary<Hotkey, SharpHotkeys.WPF.Hotkey> Hotkeys { get; }

    public GlobalHotkeys()
    {
        Hotkeys = new Dictionary<Hotkey, SharpHotkeys.WPF.Hotkey>();
    }

    /// <summary>
    /// Registers global hotkey. If hotkey is already registered - it will be replaced with new hotkey.
    /// </summary>
    /// <param name="hotkey">Hotkey to register.</param>
    /// <param name="windowHandle">Handle of the window to which hotkey would be registered to. If <see cref="IntPtr.Zero"/> is provided - hotkey will be registered to current thread.</param>
    /// <param name="onHotkeyPressed">Action that will be executed on hotkey press.</param>
    /// <param name="errorMessage">Will contain a message if registering fails.</param>
    /// <returns><see langword="true"/> if registered successfully. Otherwise <see langword="false"/>.</returns>
    public bool TryRegister(Hotkey hotkey, IntPtr windowHandle, Action onHotkeyPressed, out string errorMessage)
    {
        if (hotkey.Key == Key.None)
        {
            errorMessage = $"<{hotkey}> is not a valid hotkey.";
            return false;
        }

        if (Hotkeys.ContainsKey(hotkey))
        {
            if (!TryUnregister(hotkey, out string removeErrorMessage))
            {
                errorMessage = removeErrorMessage;
                return false;
            }
        }

        var newHotkey = new SharpHotkeys.WPF.Hotkey(hotkey.Key, hotkey.Modifiers, windowHandle);

        if (!newHotkey.TryRegisterHotkey(out uint errCode))
        {
            errorMessage = $"Failed to register hotkey <{hotkey}>.\n{MessageFromCode(errCode, hotkey)}";
            return false;
        }

        newHotkey.HotkeyClicked += onHotkeyPressed;
        Hotkeys.Add(hotkey, newHotkey);
        errorMessage = string.Empty;
        return true;
    }

    /// <summary>
    /// Unregisters hotkey.
    /// </summary>
    /// <param name="hotkey">Hotkey to unregister.</param>
    /// <param name="errorMessage">Will contain a message if unregistering fails.</param>
    /// <returns><see langword="true"/> if registered successfully. Otherwise <see langword="false"/>.</returns>
    public bool TryUnregister(Hotkey hotkey, out string errorMessage)
    {
        try
        {
            var existing = Hotkeys[hotkey];

            if (!existing.TryUnregisterHotkey(out uint errCode))
            {
                errorMessage = $"Failed to unregister <{hotkey}>: {MessageFromCode(errCode, hotkey)}";
                return false;
            }

            existing.Dispose();
            Hotkeys.Remove(hotkey);
            errorMessage = string.Empty;
            return true;
        }
        catch (Exception ex)
        {
            errorMessage = $"Failed to remove or unregister global hotkey:\n{ex}";
            return false;
        }
    }

    /// <summary>
    /// Is specified hotkey already registered.
    /// </summary>
    public bool IsRegistered(Hotkey hotkey)
    {
        return Hotkeys.ContainsKey(hotkey);
    }

    private string MessageFromCode(uint errCode, Hotkey hotkey)
    {
        switch (errCode)
        {
            case 1409:
                return $"<{hotkey}> is already registered.";
            default:
                return $"Error code: {errCode}.";
        }
    }
}