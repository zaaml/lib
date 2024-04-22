﻿// <copyright file="Control.WPF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Input;

namespace Zaaml.UI.Controls.Core
{
	public partial class Control
	{
		protected override void OnGotFocus(RoutedEventArgs e)
		{
			base.OnGotFocus(e);

			UpdateVisualState(true);
		}

		protected override void OnLostFocus(RoutedEventArgs e)
		{
			base.OnLostFocus(e);

			UpdateVisualState(true);
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);

			UpdateVisualState(true);
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);

			UpdateVisualState(true);
		}

		partial void PlatformCtor()
		{
			Loaded += (_, _) => OnLoaded();
			Unloaded += (_, _) => OnUnloaded();
		}
	}
}