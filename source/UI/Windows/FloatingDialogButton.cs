// <copyright file="FloatingDialogButton.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Windows
{
  public sealed class FloatingDialogButton : WindowButton
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty DialogResultProperty = DPM.Register<FloatingDialogResult, FloatingDialogButton>
      ("DialogResult", FloatingDialogResult.None, b => b.OnDialogResultChanged);

    #endregion

    #region Ctors

    static FloatingDialogButton()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<FloatingDialogButton>();
    }

    public FloatingDialogButton()
    {
      this.OverrideStyleKey<FloatingDialogButton>();
      Command = FloatingDialog.CloseDialogCommand;
      CommandParameter = DialogResult;
    }

    #endregion

    #region Properties

    public FloatingDialogResult DialogResult
    {
      get => (FloatingDialogResult) GetValue(DialogResultProperty);
      set => SetValue(DialogResultProperty, value);
    }

    #endregion

    #region  Methods

    private void OnDialogResultChanged()
    {
      CommandParameter = DialogResult;

      switch (DialogResult)
      {
        case FloatingDialogResult.None:
          break;
        case FloatingDialogResult.OK:
          Content = Localization.DialogButton_OK;
          break;
        case FloatingDialogResult.Cancel:
          Content = Localization.DialogButton_Cancel;
          break;
        case FloatingDialogResult.Abort:
          break;
        case FloatingDialogResult.Retry:
          break;
        case FloatingDialogResult.Ignore:
          break;
        case FloatingDialogResult.Yes:
          Content = Localization.DialogButton_Yes;
          break;
        case FloatingDialogResult.No:
          Content = Localization.DialogButton_No;
          break;
        case FloatingDialogResult.Apply:
          Content = Localization.DialogButton_Apply;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    #endregion
  }
}