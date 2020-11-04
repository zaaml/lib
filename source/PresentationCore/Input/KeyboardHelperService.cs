// <copyright file="KeyboardHelperService.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Input;
using Zaaml.PresentationCore.Services;

namespace Zaaml.PresentationCore.Input
{
  internal class KeyboardHelperService : ServiceBase<FrameworkElement>
  {
    #region Fields

    internal event EventHandler<KeyboardActionEventArgs> Action;

    #endregion

    #region  Methods

    protected virtual void OnAction(KeyboardActionEventArgs e)
    {
      Action?.Invoke(this, e);
    }


    protected override void OnAttach()
    {
      base.OnAttach();
      Target.KeyDown += OnTargetKeyDown;
    }

    protected override void OnDetach()
    {
      Target.KeyDown -= OnTargetKeyDown;
      base.OnDetach();
    }

    private void OnTargetKeyDown(object sender, KeyEventArgs keyEventArgs)
    {
      if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
      {
        switch (keyEventArgs.Key)
        {
          case Key.Z:
            OnAction(new KeyboardActionEventArgs(KeyboardAction.Undo));
            return;
          case Key.Y:
            OnAction(new KeyboardActionEventArgs(KeyboardAction.Redo));
            return;
          case Key.X:
            OnAction(new KeyboardActionEventArgs(KeyboardAction.Cut));
            return;
          case Key.C:
            OnAction(new KeyboardActionEventArgs(KeyboardAction.Copy));
            return;
          case Key.V:
            OnAction(new KeyboardActionEventArgs(KeyboardAction.Paste));
            return;
        }
      }

      if (keyEventArgs.Key == Key.Escape)
        OnAction(new KeyboardActionEventArgs(KeyboardAction.Esc));
      else if (keyEventArgs.Key == Key.Enter)
        OnAction(new KeyboardActionEventArgs(KeyboardAction.Enter));
    }

    #endregion
  }
}