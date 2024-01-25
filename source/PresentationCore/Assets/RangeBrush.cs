// <copyright file="RangeBrush.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using Zaaml.Core.Monads;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Assets
{
	[ContentProperty("ColorRangeCollection")]
	public class RangeBrush : RangeValueAsset
	{
		private static readonly DependencyPropertyKey ActualBrushPropertyKey = DPM.RegisterReadOnly<Brush, RangeBrush>
			("ActualBrush");

		public static readonly DependencyProperty ActualBrushProperty = ActualBrushPropertyKey.DependencyProperty;

		private readonly SolidColorBrush _brush = new SolidColorBrush();
		private readonly ColorRangeCollection _colorRangeCollection;

		public RangeBrush()
		{
			_colorRangeCollection = new ColorRangeCollection();
			_colorRangeCollection.CollectionChanged += (sender, args) => UpdateActualBrush();
			ActualBrush = _brush;
		}

		public Brush ActualBrush
		{
			get => (Brush)GetValue(ActualBrushProperty);
			private set => this.SetReadOnlyValue(ActualBrushPropertyKey, value);
		}

		public ColorRangeCollection ColorRangeCollection => _colorRangeCollection;

		protected override void OnRelativeValueChanged()
		{
			UpdateActualBrush();
		}

		private void UpdateActualBrush()
		{
			var doubleValue = CoercedPercentageValue;
			var colorRange = DoubleUtils.GreaterThanOrClose(Value, Maximum) ? ColorRangeCollection.LastOrDefault() : ColorRangeCollection.FirstOrDefault(c => c.Contains(doubleValue));

			_brush.Color = colorRange.Return(c => c.Color, Colors.Transparent);
		}
	}

	public class ColorRange
	{
		public Color Color { get; set; }

		public double From { get; set; }

		public double To { get; set; }

		public bool Contains(double doubleValue)
		{
			return doubleValue >= From && doubleValue < To;
		}
	}

	public class ColorRangeCollection : ObservableCollection<ColorRange>
	{
	}
}