// <copyright file="DropDownControlBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Windows;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;

namespace Zaaml.UI.Controls.DropDown
{
	public class DropDownControlBase : TemplateContractControl
	{
		public static readonly DependencyProperty PopupMinHeightProperty = DPM.Register<PopupLength, DropDownControlBase>
			("PopupMinHeight", new PopupLength(0.0));

		public static readonly DependencyProperty PopupMinWidthProperty = DPM.Register<PopupLength, DropDownControlBase>
			("PopupMinWidth", new PopupLength(0.0));

		public static readonly DependencyProperty PopupMaxHeightProperty = DPM.Register<PopupLength, DropDownControlBase>
			("PopupMaxHeight", new PopupLength(double.PositiveInfinity));

		public static readonly DependencyProperty PopupMaxWidthProperty = DPM.Register<PopupLength, DropDownControlBase>
			("PopupMaxWidth", new PopupLength(double.PositiveInfinity));

		public static readonly DependencyProperty PopupWidthProperty = DPM.Register<PopupLength, DropDownControlBase>
			("PopupWidth", PopupLength.Auto);

		public static readonly DependencyProperty PopupHeightProperty = DPM.Register<PopupLength, DropDownControlBase>
			("PopupHeight", PopupLength.Auto);

		[TypeConverter(typeof(PopupLengthTypeConverter))]
		public PopupLength PopupHeight
		{
			get => (PopupLength) GetValue(PopupHeightProperty);
			set => SetValue(PopupHeightProperty, value);
		}

		[TypeConverter(typeof(PopupLengthTypeConverter))]
		public PopupLength PopupMaxHeight
		{
			get => (PopupLength) GetValue(PopupMaxHeightProperty);
			set => SetValue(PopupMaxHeightProperty, value);
		}

		[TypeConverter(typeof(PopupLengthTypeConverter))]
		public PopupLength PopupMaxWidth
		{
			get => (PopupLength) GetValue(PopupMaxWidthProperty);
			set => SetValue(PopupMaxWidthProperty, value);
		}

		[TypeConverter(typeof(PopupLengthTypeConverter))]
		public PopupLength PopupMinHeight
		{
			get => (PopupLength) GetValue(PopupMinHeightProperty);
			set => SetValue(PopupMinHeightProperty, value);
		}

		[TypeConverter(typeof(PopupLengthTypeConverter))]
		public PopupLength PopupMinWidth
		{
			get => (PopupLength) GetValue(PopupMinWidthProperty);
			set => SetValue(PopupMinWidthProperty, value);
		}

		[TypeConverter(typeof(PopupLengthTypeConverter))]
		public PopupLength PopupWidth
		{
			get => (PopupLength) GetValue(PopupWidthProperty);
			set => SetValue(PopupWidthProperty, value);
		}
	}
}