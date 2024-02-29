// <copyright file="PopupBarBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	public enum PopupBorderStyle
	{
		None,
		Border,
		ArrowBorder
	}

	public abstract class PopupBarBase : PopupControlBase
	{
		public static readonly DependencyProperty BorderStyleProperty = DPM.Register<PopupBorderStyle, PopupBarBase>
			("BorderStyle", PopupBorderStyle.Border);

		public PopupBorderStyle BorderStyle
		{
			get => (PopupBorderStyle)GetValue(BorderStyleProperty);
			set => SetValue(BorderStyleProperty, value);
		}
	}
}