// <copyright file="GlobalPopup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	internal class GlobalPopup
	{
		public static readonly GlobalPopup Instance = new();

		private readonly Canvas _host = new() { Opacity = 0, IsHitTestVisible = false, Width = 0, Height = 0 };
		private readonly System.Windows.Controls.Primitives.Popup _nativePopup = new() { IsOpen = true };

		private GlobalPopup()
		{
			_nativePopup.Child = _host;
			_host.Resources = ThemeManager.CreateThemeResourceDictionary();
		}

		public void Attach(FrameworkElement frameworkElement)
		{
			_host.Children.Add(frameworkElement);
			UpdatePopup();
		}

		public void Detach(FrameworkElement frameworkElement)
		{
			_host.Children.Remove(frameworkElement);
			UpdatePopup();
		}

		public static bool IsAncestorOf(FrameworkElement element)
		{
			return ReferenceEquals(Instance._nativePopup, element.GetRoot(MixedTreeEnumerationStrategy.DisconnectedThenVisualThenLogicalInstance));
		}

		private void UpdatePopup()
		{
			//_nativePopup.IsOpen = _host.Children.Cast<UIElement>().Any();
		}
	}
}