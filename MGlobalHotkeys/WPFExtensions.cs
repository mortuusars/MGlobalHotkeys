using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MGlobalHotkeys.WPF;

internal static class WPFExtensions
{
    public static T? GetDescendantByType<T>(this Visual element) where T : class
    {
        if (element == null)
            return default;

        if (element.GetType() == typeof(T))
            return element as T;

        T? foundElement = null;

        if (element is FrameworkElement)
            ((FrameworkElement)element).ApplyTemplate();

        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
        {
            Visual? visual = VisualTreeHelper.GetChild(element, i) as Visual;
            foundElement = visual?.GetDescendantByType<T>();
            if (foundElement != null)
                break;
        }

        return foundElement;
    }

    public static T? GetParentOfType<T>(this DependencyObject obj) where T : class
    {
        DependencyObject parent = VisualTreeHelper.GetParent(obj);

        while (parent != null && !parent.GetType().Equals(typeof(T)))
        {
            parent = VisualTreeHelper.GetParent(parent);
        }

        return parent as T;
    }

    public static void MoveFocusToParent(this FrameworkElement element)
    {
        // Move to a parent that can take focus
        FrameworkElement parent = (FrameworkElement)element.Parent;
        while (parent != null && parent is IInputElement && !((IInputElement)parent).Focusable)
        {
            parent = (FrameworkElement)parent.Parent;
        }

        DependencyObject scope = FocusManager.GetFocusScope(element);
        FocusManager.SetFocusedElement(scope, parent as IInputElement);
    }
}