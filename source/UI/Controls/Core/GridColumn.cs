// <copyright file="GridColumn.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Panels.Flexible;

namespace Zaaml.UI.Controls.Core
{
	public class GridColumn : InheritanceContextObject
	{
		private static readonly DependencyPropertyKey ActualWidthPropertyKey = DPM.RegisterReadOnly<double, GridColumn>
			("ActualWidth", 0.0);

		public static readonly DependencyProperty WidthProperty = DPM.Register<FlexLength, GridColumn>
			("Width", FlexLength.Star, d => d.OnWidthPropertyChangedPrivate);
		
		public static readonly DependencyProperty ActualWidthProperty = ActualWidthPropertyKey.DependencyProperty;

		public GridColumn()
		{
		}

		internal GridColumn(GridController controller)
		{
			Controller = controller;
		}

		public double ActualWidth
		{
			get => (double) GetValue(ActualWidthProperty);
			internal set => SetValue(ActualWidthPropertyKey, value);
		}

		internal double AutoDesiredWidth { get; set; }

		public virtual GridController Controller { get; }

		internal double FinalWidth { get; set; }

		[TypeConverter(typeof(FlexLengthTypeConverter))]
		public FlexLength Width
		{
			get => (FlexLength) GetValue(WidthProperty);
			set => SetValue(WidthProperty, value);
		}

		private void OnWidthPropertyChangedPrivate(FlexLength oldValue, FlexLength newValue)
		{
			Controller?.OnColumnWidthChanged(this);
		}
	}
}