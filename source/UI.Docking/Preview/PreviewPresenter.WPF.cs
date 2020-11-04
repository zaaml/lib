// <copyright file="PreviewPresenter.WPF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.Docking
{
  internal partial class PreviewPresenter : Window
  {
    #region  Methods

    partial void HideImpl()
    {
      Dispatcher.BeginInvoke(DispatcherPriority.Render, Hide);
    }

    partial void PlatformCtor()
    {
      ShowActivated = false;
      AllowsTransparency = true;
      Topmost = true;
      WindowStyle = WindowStyle.None;

      Background = new SolidColorBrush(Colors.Transparent);

      Content = PreviewElement;
      ShowInTaskbar = false;

      WindowState = WindowState.Normal;
      IsHitTestVisible = false;

      SizeToContent = SizeToContent.Manual;
      WindowStartupLocation = WindowStartupLocation.Manual;

      Left = 0;
      Top = 0;
      Width = Screen.VirtualScreenSize.Width;
      Height = Screen.VirtualScreenSize.Height;
    }

    partial void ShowImpl()
    {
      Dispatcher.BeginInvoke(DispatcherPriority.Render, Show);
    }

    #endregion
  }
}