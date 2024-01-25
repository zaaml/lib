// <copyright file="PreviewPresenter.WPF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Docking
{
	internal partial class PreviewPresenter : Window
	{
		private RenderDelayAction _hideDelayAction;
		private RenderDelayAction _showDelayAction;

		partial void HideImpl()
		{
			_showDelayAction.Revoke();

			if (IsVisible)
				_hideDelayAction.Invoke();
		}

		partial void PlatformCtor()
		{
			ShowActivated = false;
			AllowsTransparency = true;
			Topmost = true;
			WindowStyle = WindowStyle.None;

			Background = Brushes.Transparent;

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

			_hideDelayAction = new RenderDelayAction(Hide, 3);
			_showDelayAction = new RenderDelayAction(Show, 1);
		}

		partial void ShowImpl()
		{
			_hideDelayAction.Revoke();

			if (IsVisible == false)
				_showDelayAction.Invoke();
		}
	}
}