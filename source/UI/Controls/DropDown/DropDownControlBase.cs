// <copyright file="DropDownControlBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.DropDown
{
	public class DropDownControlBase : TemplateContractControl
	{
		public static readonly DependencyProperty PopupMinHeightProperty = DPM.Register<double, DropDownControlBase>
			("PopupMinHeight", 0.0);

		public static readonly DependencyProperty PopupMinWidthProperty = DPM.Register<double, DropDownControlBase>
			("PopupMinWidth", 0.0);

		public static readonly DependencyProperty PopupMaxHeightProperty = DPM.Register<double, DropDownControlBase>
			("PopupMaxHeight", double.PositiveInfinity);

		public static readonly DependencyProperty PopupMaxWidthProperty = DPM.Register<double, DropDownControlBase>
			("PopupMaxWidth", double.PositiveInfinity);

		public static readonly DependencyProperty PopupWidthProperty = DPM.Register<double, DropDownControlBase>
			("PopupWidth", double.NaN);

		public static readonly DependencyProperty PopupHeightProperty = DPM.Register<double, DropDownControlBase>
			("PopupHeight", double.NaN);

		public double PopupHeight
		{
			get => (double) GetValue(PopupHeightProperty);
			set => SetValue(PopupHeightProperty, value);
		}

		public double PopupMaxHeight
		{
			get => (double) GetValue(PopupMaxHeightProperty);
			set => SetValue(PopupMaxHeightProperty, value);
		}

		public double PopupMaxWidth
		{
			get => (double) GetValue(PopupMaxWidthProperty);
			set => SetValue(PopupMaxWidthProperty, value);
		}

		public double PopupMinHeight
		{
			get => (double) GetValue(PopupMinHeightProperty);
			set => SetValue(PopupMinHeightProperty, value);
		}

		public double PopupMinWidth
		{
			get => (double) GetValue(PopupMinWidthProperty);
			set => SetValue(PopupMinWidthProperty, value);
		}

		public double PopupWidth
		{
			get => (double) GetValue(PopupWidthProperty);
			set => SetValue(PopupWidthProperty, value);
		}
	}
}