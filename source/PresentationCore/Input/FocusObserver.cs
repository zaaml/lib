// <copyright file="FocusObserver.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Media;
using Zaaml.Core.Extensions;
using Zaaml.Core.Weak;

namespace Zaaml.PresentationCore.Input
{
  internal static class FocusObserver
  {
    #region Static Fields and Constants

    private static WeakReference _focusedElementReference;
    private static WeakReference _keyboardFocusedElementReference;

    #endregion

    #region Ctors

    static FocusObserver()
    {
	    UpdateFocus();

			CompositionTarget.Rendering += CompositionTargetOnRendering;
    }

    #endregion

    #region Properties

    public static object FocusedElement
    {
      get => _focusedElementReference?.GetTarget<object>();
      private set
			{
        if (ReferenceEquals(FocusedElement, value))
          return;

				_focusedElementReference = value != null ? new WeakReference(value) : null;

				OnFocusedElementChanged();
      }
    }

    public static object KeyboardFocusedElement
    {
      get => _keyboardFocusedElementReference?.GetTarget<object>();
      private set
      {
        if (ReferenceEquals(KeyboardFocusedElement, value))
          return;

        _keyboardFocusedElementReference = value != null ? new WeakReference(value) : null;
        OnKeyboardFocusedElementChanged();
      }
    }

    #endregion

    #region  Methods

    private static void CompositionTargetOnRendering(object sender, EventArgs eventArgs)
    {
	    UpdateFocus();
    }

    private static void UpdateFocus()
    {
	    FocusedElement = FocusHelper.GetFocusedElement();
	    KeyboardFocusedElement = FocusHelper.GetKeyboardFocusedElement();
    }

    internal static IDisposable CreateWeakFocusChangeListener<T>(this T obj, Func<T, Action> onSizeChangedHandlerFactory)
    {
      return obj.CreateWeakEventListener((t, o, e) => onSizeChangedHandlerFactory(t)(), a => FocusedElementChanged += a, a => FocusedElementChanged -= a);
    }

    private static void OnFocusedElementChanged()
    {
      FocusedElementChanged?.Invoke(null, EventArgs.Empty);
    }

    private static void OnKeyboardFocusedElementChanged()
    {
      KeyboardFocusedElementChanged?.Invoke(null, EventArgs.Empty);
    }

    #endregion

    public static event EventHandler FocusedElementChanged;
    public static event EventHandler KeyboardFocusedElementChanged;
  }
}