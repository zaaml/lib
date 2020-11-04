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
    #region Static Fields and Constants

    private static readonly DependencyPropertyKey ActualBrushPropertyKey = DPM.RegisterReadOnly<Brush, RangeBrush>
      ("ActualBrush");

    public static readonly DependencyProperty ActualBrushProperty = ActualBrushPropertyKey.DependencyProperty;

    #endregion

    #region Fields

    private readonly SolidColorBrush _brush = new SolidColorBrush();
    private readonly ColorRangeCollection _colorRangeCollection;

    #endregion

    #region Ctors

    public RangeBrush()
    {
      _colorRangeCollection = new ColorRangeCollection();
      _colorRangeCollection.CollectionChanged += (sender, args) => UpdateActualBrush();
      ActualBrush = _brush;
    }

    #endregion

    #region Properties

    public Brush ActualBrush
    {
      get => (Brush) GetValue(ActualBrushProperty);
      private set => this.SetReadOnlyValue(ActualBrushPropertyKey, value);
    }

    public ColorRangeCollection ColorRangeCollection => _colorRangeCollection;

    #endregion

    #region  Methods

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

    #endregion
  }

  public class ColorRange
  {
    #region Properties

    public Color Color { get; set; }

    public double From { get; set; }

    public double To { get; set; }

    #endregion

    #region  Methods

    public bool Contains(double doubleValue)
    {
      return doubleValue >= From && doubleValue < To;
    }

    #endregion
  }

  public class ColorRangeCollection : ObservableCollection<ColorRange>
  {
  }
}