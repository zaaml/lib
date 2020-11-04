// <copyright file="MessageWindowButton.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Windows
{
  public sealed class MessageWindowButton : WindowButton
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty ResultProperty = DPM.Register<MessageWindowResultKind, MessageWindowButton>
      ("Result", MessageWindowResultKind.None, b => b.OnDialogResultChanged);

    #endregion

    #region Ctors

    static MessageWindowButton()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<MessageWindowButton>();
    }

    public MessageWindowButton()
    {
      this.OverrideStyleKey<MessageWindowButton>();
      Command = WindowBase.CloseCommand;
      CommandParameter = Result;
    }

    #endregion

    #region Properties

    public MessageWindowResultKind Result
    {
      get => (MessageWindowResultKind) GetValue(ResultProperty);
      set => SetValue(ResultProperty, value);
    }

    #endregion

    #region  Methods

    private void OnDialogResultChanged()
    {
      CommandParameter = Result;

      switch (Result)
      {
        case MessageWindowResultKind.None:
          break;
        case MessageWindowResultKind.OK:
          Content = Localization.DialogButton_OK;
          break;
        case MessageWindowResultKind.Cancel:
          Content = Localization.DialogButton_Cancel;
          break;
        case MessageWindowResultKind.Yes:
          Content = Localization.DialogButton_Yes;
          break;
        case MessageWindowResultKind.No:
          Content = Localization.DialogButton_No;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    #endregion
  }
}