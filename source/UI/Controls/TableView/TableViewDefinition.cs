// <copyright file="TableViewDefinition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Panels.Flexible;

namespace Zaaml.UI.Controls.TableView
{
	public class TableViewDefinition : InheritanceContextObject
	{
		public static readonly DependencyProperty LengthProperty = DPM.Register<FlexLength, TableViewDefinition>
			("Length", FlexLength.Star, d => d.OnLengthPropertyChangedPrivate);

		internal double DesiredSize { get; set; }

		internal bool IsImplicit { get; set; }

		[TypeConverter(typeof(FlexLengthTypeConverter))]
		public FlexLength Length
		{
			get => (FlexLength) GetValue(LengthProperty);
			set => SetValue(LengthProperty, value);
		}

		internal TableViewControl TableViewControl { get; set; }

		private void OnLengthPropertyChangedPrivate(FlexLength oldValue, FlexLength newValue)
		{
			TableViewControl?.OnDefinitionsChanged();
		}
	}
}