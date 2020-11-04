// <copyright file="FocusHelper.WPF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Input
{
  public static partial class FocusHelper
  {
    #region  Methods

    private static Window GetCurrentWindow()
    {
      var application = Application.Current;

      return application != null ? application.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive) ?? application.MainWindow : null;
    }

    public static object GetFocusedElement()
    {
      var scope = GetCurrentWindow();

      return scope != null ? FocusManager.GetFocusedElement(scope) : null;
    }

    public static object GetFocusedElement(DependencyObject referenceObject)
    {
      var scope = FocusManager.GetFocusScope(referenceObject) ?? GetCurrentWindow();

      return scope != null ? FocusManager.GetFocusedElement(scope) : null;
    }

    public static object GetKeyboardFocusedElement()
    {
      return Keyboard.FocusedElement;
    }

    public static bool HasFocus(Control control)
    {
      var focusedElement = GetFocusedElement(control) as DependencyObject;

      return focusedElement?.GetVisualAncestorsAndSelf().Any(r => ReferenceEquals(r, control)) == true;
    }

    public static bool HasKeyboardFocus(Control control)
    {
      return control.IsKeyboardFocusWithin;
    }

    public static object SetKeyboardFocusedElement(object element)
    {
	    return element is IInputElement inputElement ? Keyboard.Focus(inputElement) : null;
    }

    public static bool Focus(object element)
    {
	    return element is IInputElement inputElement && inputElement.Focus();
    }

    #endregion
  }
}