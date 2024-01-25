// <copyright file="ColorPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Media;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ColorEditor
{
	public class ColorPresenter : Control
	{
		public static readonly DependencyProperty ShowTransparentPatternProperty = DPM.Register<bool, ColorPresenter>
			("ShowTransparentPattern", true);

		public static readonly DependencyProperty ColorProperty = DPM.Register<Color, ColorPresenter>
			("Color");

		static ColorPresenter()
		{
			ControlUtils.OverrideIsTabStop<ColorPresenter>(false);

			DefaultStyleKeyHelper.OverrideStyleKey<ColorPresenter>();
		}

		public ColorPresenter()
		{
			this.OverrideStyleKey<ColorPresenter>();
		}

		public Color Color
		{
			get => (Color) GetValue(ColorProperty);
			set => SetValue(ColorProperty, value);
		}

		public bool ShowTransparentPattern
		{
			get => (bool) GetValue(ShowTransparentPatternProperty);
			set => SetValue(ShowTransparentPatternProperty, value.Box());
		}
	}
}