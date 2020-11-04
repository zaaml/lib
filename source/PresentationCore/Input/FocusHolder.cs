// <copyright file="FocusHolder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore.Input
{
  internal struct FocusHolder
  {
    private WeakReference _previousFocusWeak;

    private object PreviousFocus
    {
      get => _previousFocusWeak?.Target;
      set
      {
        if (ReferenceEquals(PreviousFocus, value))
          return;

        _previousFocusWeak = value != null ? new WeakReference(value) : null;
      }
    }

    public void Save()
    {
      PreviousFocus = FocusHelper.GetKeyboardFocusedElement();
    }

    public void Restore()
    {
      if (PreviousFocus != null)
        FocusHelper.SetKeyboardFocusedElement(PreviousFocus);

      PreviousFocus = null;
    }
  }
}