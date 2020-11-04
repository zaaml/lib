using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

using Popup = Zaaml.UI.Controls.Primitives.PopupPrimitives.Popup;

namespace Zaaml.UI.Decorators
{
  public sealed class ShadowChrome : FixedTemplateControl
  {
    #region Static Fields and Constants

    private static SolidColorBrush _backgroundBrush;
    private static LinearGradientBrush _leftBrush;
    private static LinearGradientBrush _topBrush;
    private static LinearGradientBrush _rightBrush;
    private static LinearGradientBrush _bottomBrush;
    private static RadialGradientBrush _topLeftBrush;
    private static RadialGradientBrush _topRightBrush;
    private static RadialGradientBrush _bottomRightBrush;
    private static RadialGradientBrush _bottomLeftBrush;

    public static readonly DependencyProperty BlurRadiusProperty = DPM.Register<double, ShadowChrome>
      ("BlurRadius", s => s.OnBlurRadiusChanged);

    public static readonly DependencyProperty ShadowDistanceProperty = DPM.Register<double, ShadowChrome>
      ("ShadowDistance", s => s.OnShadowDistanceChanged);

    public static readonly DependencyProperty ShadowDirectionProperty = DPM.Register<double, ShadowChrome>
      ("ShadowDirection", s => s.OnShadowDirectionChanged);

    public static readonly DependencyProperty ShadowOpacityProperty = DPM.Register<double, ShadowChrome>
      ("ShadowOpacity", 1.0, s => s.OnShadowOpacityChanged);

    #endregion

    #region Fields

    private readonly RowDefinition _bottomRow;
    private readonly ColumnDefinition _leftColumn;
    private readonly ColumnDefinition _rightColumn;
    private readonly Grid _shadowGrid;
    private readonly RowDefinition _topRow;

    #endregion

    #region Ctors

    static ShadowChrome()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<ShadowChrome>();
      CreateBrushes();
    }

    public ShadowChrome()
    {
      Popup.SetHitTestVisible(this, false);

      _leftColumn = new ColumnDefinition { Width = new GridLength(0) };
      var contentColumn = new ColumnDefinition();
      _rightColumn = new ColumnDefinition { Width = new GridLength(0) };

      _topRow = new RowDefinition { Height = new GridLength(0) };
      var contentRow = new RowDefinition();
      _bottomRow = new RowDefinition { Height = new GridLength(0) };

      _shadowGrid = new Grid
      {
        Children =
        {
          AddRect(1, 1, _backgroundBrush),
          AddRect(0, 1, _leftBrush),
          AddRect(0, 0, _topLeftBrush),
          AddRect(1, 0, _topBrush),
          AddRect(2, 0, _topRightBrush),
          AddRect(2, 1, _rightBrush),
          AddRect(2, 2, _bottomRightBrush),
          AddRect(1, 2, _bottomBrush),
          AddRect(0, 2, _bottomLeftBrush),
        },
        Margin = new Thickness(0),
        ColumnDefinitions = { _leftColumn, contentColumn, _rightColumn },
        RowDefinitions = { _topRow, contentRow, _bottomRow },
      };

      ChildInternal = _shadowGrid;

      this.OverrideStyleKey<ShadowChrome>();

      //Background = new SolidColorBrush(Colors.White);

#if !SILVERLIGHT
      Focusable = false;
#endif
      IsTabStop = false;
    }

    #endregion

    #region Properties

    private static Color ShadowColor => Color.FromArgb(255, 0, 0, 0);

    public double BlurRadius
    {
      get => (double)GetValue(BlurRadiusProperty);
      set => SetValue(BlurRadiusProperty, value);
    }

    public double ShadowDirection
    {
      get => (double)GetValue(ShadowDirectionProperty);
      set => SetValue(ShadowDirectionProperty, value);
    }

    public double ShadowDistance
    {
      get => (double)GetValue(ShadowDistanceProperty);
      set => SetValue(ShadowDistanceProperty, value);
    }


    public double ShadowOpacity
    {
      get => (double)GetValue(ShadowOpacityProperty);
      set => SetValue(ShadowOpacityProperty, value);
    }

    #endregion

    #region  Methods

    private Rectangle AddRect(int column, int row, Brush brush)
    {
      var addRect = new Rectangle { Fill = brush };
      Grid.SetColumn(addRect, column);
      Grid.SetRow(addRect, row);
      return addRect;
    }

    private static void CreateBrushes()
    {
      _backgroundBrush = new SolidColorBrush(ShadowColor);

      _leftBrush = InitBrush(new LinearGradientBrush
      {
        GradientStops = CreateGradient(),
        StartPoint = new Point(1.0, 0.0),
        EndPoint = new Point(0.0, 0.0)
      });

      _topBrush = InitBrush(new LinearGradientBrush
      {
        GradientStops = CreateGradient(),
        StartPoint = new Point(0.0, 1.0),
        EndPoint = new Point(0.0, 0.0)
      });

      _rightBrush = InitBrush(new LinearGradientBrush
      {
        GradientStops = CreateGradient(),
        StartPoint = new Point(0.0, 0.0),
        EndPoint = new Point(1.0, 0.0)
      });

      _bottomBrush = InitBrush(new LinearGradientBrush
      {
        GradientStops = CreateGradient(),
        StartPoint = new Point(0.0, 0.0),
        EndPoint = new Point(0.0, 1.0)
      });

      var cornerRadius = 1;

      _topLeftBrush = InitBrush(new RadialGradientBrush(CreateGradient())
      {
        GradientOrigin = new Point(1.0, 1.0),
        Center = new Point(1.0, 1.0),
        RadiusX = cornerRadius,
        RadiusY = cornerRadius
      });

      _topRightBrush = InitBrush(new RadialGradientBrush(CreateGradient())
      {
        GradientOrigin = new Point(0.0, 1.0),
        Center = new Point(0.0, 1.0),
        RadiusX = cornerRadius,
        RadiusY = cornerRadius
      });

      _bottomRightBrush = InitBrush(new RadialGradientBrush(CreateGradient())
      {
        GradientOrigin = new Point(0.0, 0.0),
        Center = new Point(0.0, 0.0),
        RadiusX = cornerRadius,
        RadiusY = cornerRadius
      });

      _bottomLeftBrush = InitBrush(new RadialGradientBrush(CreateGradient())
      {
        GradientOrigin = new Point(1.0, 0.0),
        Center = new Point(1.0, 0.0),
        RadiusX = cornerRadius,
        RadiusY = cornerRadius
      });
    }

    private static GradientStopCollection CreateGradient()
    {
      var shadowValues = new byte[]
      {
        115, 113, 111, 107, 102, 97, 90, 81, 72, 63, 53, 43, 35, 26, 19, 13, 9, 5, 3, 1, 0
      };

      var gradient = new GradientStopCollection();

      for (var i = 0; i < shadowValues.Length; i++)
      {
        var opacity = 255.0 * shadowValues[i] / shadowValues[0];
        gradient.Add(new GradientStop
        {
          Color = Color.FromArgb((byte)opacity, 0, 0, 0),
          Offset = (double)i / shadowValues.Length
        });
      }

      return gradient;
    }

    private static RadialGradientBrush InitBrush(RadialGradientBrush brush)
    {
      brush.ColorInterpolationMode = ColorInterpolationMode.ScRgbLinearInterpolation;
      return brush;
    }

    private static LinearGradientBrush InitBrush(LinearGradientBrush brush)
    {
      brush.ColorInterpolationMode = ColorInterpolationMode.ScRgbLinearInterpolation;
      return brush;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      var measureOverride = base.MeasureOverride(availableSize);
      return measureOverride;
    }

    private void OnBlurRadiusChanged()
    {
      var actualDepth = BlurRadius;
      var shadowGridLength = new GridLength(actualDepth, GridUnitType.Pixel);

      _leftColumn.Width = shadowGridLength;
      _rightColumn.Width = shadowGridLength;
      _topRow.Height = shadowGridLength;
      _bottomRow.Height = shadowGridLength;
    }

    private void OnShadowDirectionChanged()
    {
      UpdateShadowPlacement();
    }

    private void OnShadowDistanceChanged()
    {
      UpdateShadowPlacement();
    }

    private void OnShadowOpacityChanged()
    {
      foreach (var shadowPart in _shadowGrid.Children.Cast<Rectangle>())
        shadowPart.Opacity = ShadowOpacity;
    }

    private void UpdateShadowPlacement()
    {
      var radAngle = ShadowDirection * Math.PI / 360.0;
      _shadowGrid.RenderTransform = new TranslateTransform
      {
        X = ShadowDistance * Math.Cos(radAngle),
        Y = ShadowDistance * Math.Sin(radAngle)
      };
    }

    #endregion
  }
}