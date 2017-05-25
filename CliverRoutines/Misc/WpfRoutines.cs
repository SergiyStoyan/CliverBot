using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cliver
{
    static public class WpfRoutines
    {
        public static void TrimWindowSize(this System.Windows.Window window, double screen_factor = 0.8)
        {
            System.Drawing.Size s = SystemInfo.GetPrimaryScreenSize();
            int v = (int)((float)s.Width * screen_factor);
            if (window.Width > v)
                window.Width = v;
            v = (int)((float)s.Height * screen_factor);
            if (window.Height > v)
                window.Height = v;
        }

        public static bool IsValid(this DependencyObject parent)
        {
            if (Validation.GetHasError(parent))
                return false;

            // Validate all the bindings on the children
            for (int i = 0; i != VisualTreeHelper.GetChildrenCount(parent); ++i)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (!IsValid(child)) { return false; }
            }

            return true;
        }

        public static bool IsValid2(this DependencyObject parent)
        {
            // Validate all the bindings on the parent
            bool valid = true;
            LocalValueEnumerator localValues = parent.GetLocalValueEnumerator();
            while (localValues.MoveNext())
            {
                LocalValueEntry entry = localValues.Current;
                if (BindingOperations.IsDataBound(parent, entry.Property))
                {
                    Binding binding = BindingOperations.GetBinding(parent, entry.Property);
                    foreach (ValidationRule rule in binding.ValidationRules)
                    {
                        ValidationResult result = rule.Validate(parent.GetValue(entry.Property), null);
                        if (!result.IsValid)
                        {
                            BindingExpression expression = BindingOperations.GetBindingExpression(parent, entry.Property);
                            System.Windows.Controls.Validation.MarkInvalid(expression, new ValidationError(rule, expression, result.ErrorContent, null));
                            valid = false;
                        }
                    }
                }
            }

            // Validate all the bindings on the children
            for (int i = 0; i != VisualTreeHelper.GetChildrenCount(parent); ++i)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (!IsValid2(child)) { valid = false; }
            }

            return valid;
        }

        public static T FindParentOfType<T>(this DependencyObject ob)
            where T : DependencyObject
        {
            for (DependencyObject po = ob; ; po = VisualTreeHelper.GetParent(po))
                if (po is T)
                    return (T)po;
        }

        public static IEnumerable<T> FindChildrenOfType<T>(this DependencyObject ob)
            where T : DependencyObject
        {
            foreach (var child in ob.GetChildren())
            {
                T castedChild = child as T;
                if (castedChild != null)
                {
                    yield return castedChild;
                }
                else
                {
                    foreach (var internalChild in FindChildrenOfType<T>(child))
                    {
                        yield return internalChild;
                    }
                }
            }
        }

        public static IEnumerable<DependencyObject> GetChildren(this DependencyObject ob)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(ob);

            for (int i = 0; i < childCount; i++)
            {
                yield return VisualTreeHelper.GetChild(ob, i);
            }
        }

        public static T GetVisualChild<T>(this Visual parent) where T : Visual
        {
            T child = default(T);

            for (int index = 0; index < VisualTreeHelper.GetChildrenCount(parent); index++)
            {
                Visual visualChild = (Visual)VisualTreeHelper.GetChild(parent, index);
                child = visualChild as T;

                if (child == null)
                    child = GetVisualChild<T>(visualChild);//Find Recursively
                else
                    return child;
            }
            return child;
        }
    }
}
