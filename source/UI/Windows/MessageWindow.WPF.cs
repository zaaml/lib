// <copyright file="MessageWindow.WPF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using NativeWindow = System.Windows.Window;

namespace Zaaml.UI.Windows
{
  public sealed partial class MessageWindow
  {
    #region  Methods

    partial void PlatformCtor()
    {
      ResizeMode = ResizeMode.NoResize;
    }

    public static void Show(NativeWindow owner, string message, Action<MessageWindowResult> onMessageResult)
    {
      ShowImpl(owner, new MessageWindowOptions(message, string.Empty, MessageWindowButtons.OK, MessageBoxImage.None, MessageWindowResultKind.None), onMessageResult);
    }

    public static void Show(NativeWindow owner, string message, string caption, Action<MessageWindowResult> onMessageResult)
    {
      ShowImpl(owner, new MessageWindowOptions(message, caption, MessageWindowButtons.OK, MessageBoxImage.None, MessageWindowResultKind.None), onMessageResult);
    }

    public static void Show(NativeWindow owner, string message, string caption, MessageWindowButtons buttons, Action<MessageWindowResult> onMessageResult)
    {
      ShowImpl(owner, new MessageWindowOptions(message, caption, buttons, MessageBoxImage.None, MessageWindowResultKind.None), onMessageResult);
    }

    public static void Show(NativeWindow owner, string message, string caption, MessageWindowButtons buttons, MessageBoxImage image, Action<MessageWindowResult> onMessageResult)
    {
      ShowImpl(owner, new MessageWindowOptions(message, caption, buttons, image, MessageWindowResultKind.None), onMessageResult);
    }

    public static void Show(NativeWindow owner, string message, string caption, MessageWindowButtons buttons, MessageBoxImage image, MessageWindowResultKind defaultResult,
      Action<MessageWindowResult> onMessageResult)
    {
      ShowImpl(owner, new MessageWindowOptions(message, caption, buttons, image, defaultResult), onMessageResult);
    }

    public static void Show(NativeWindow owner, MessageWindowOptions options, Action<MessageWindowResult> onMessageResult)
    {
      ShowImpl(owner, options, onMessageResult);
    }

    private static void ShowImpl(NativeWindow owner, MessageWindowOptions windowOptions, Action<MessageWindowResult> onMessageResult)
    {
      var messageWindow = new MessageWindow(windowOptions, onMessageResult)
      {
        WindowStartupLocation = WindowStartupLocation.CenterScreen,
        SizeToContent = SizeToContent.Manual,
        HorizontalAlignment = HorizontalAlignment.Center,
      };

      if (owner != null)
        messageWindow.Owner = owner;

      messageWindow.ShowDialog();
    }

    #endregion
  }
}