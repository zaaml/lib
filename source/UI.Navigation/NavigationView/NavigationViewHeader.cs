// <copyright file="NavigationViewHeader.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.NavigationView
{
	public class NavigationViewHeader : Control
	{
		public static readonly DependencyProperty ContentProperty = DPM.Register<object, NavigationViewHeader>
			("Content");

		public object Content
		{
			get => GetValue(ContentProperty);
			set => SetValue(ContentProperty, value);
		}
	}
}