// <copyright file="ElementBounds.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Media;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore
{
  internal struct ElementBounds
  {
    private double _scaleX;
    private double _scaleY;
    private double _centerX;
    private double _centerY;
    private double _x;
    private double _y;
    private double _width;
    private double _height;
    private double _translateX;
    private double _translateY;

    public double X
    {
      get => _x;
      set => _x = value;
    }

    public double Y
    {
      get => _y;
      set => _y = value;
    }

    public double Width
    {
      get => _width;
      set => _width = value;
    }

    public double Height
    {
      get => _height;
      set => _height = value;
    }

    public Point Translate
    {
      get => new Point(_translateX, _translateY);
      set
      {
        _translateX = value.X;
        _translateY = value.Y;
      }
    }

    public double TranslateX
    {
      get => _translateX;
      set => _translateX = value;
    }

    public double TranslateY
    {
      get => _translateY;
      set => _translateY = value;
    }

    public Rect Bounds
    {
      get => new Rect(_x, _y, _width, _height);
      set
      {
        _x = value.X;
        _y = value.Y;
        _width = value.Width;
        _height = value.Height;
      }
    }

    public Point Location
    {
      get => new Point(_x, _y);
      set
      {
        _x = value.X;
        _y = value.Y;
      }
    }

    public double ScaleX
    {
      get => _scaleX;
      set => _scaleX = value;
    }

    public double LocalCenterX
    {
      get => _centerX - _x;
      set => _centerX = value + _x;
    }

    public Point LocalCenter
    {
      get => new Point(LocalCenterX, LocalCenterY);
      set
      {
        LocalCenterX = value.X;
        LocalCenterY = value.Y;
      }
    }

    public Point TransformToLocal(Point point)
    {
      return new Point(point.X - _x, point.Y - _y);
    }

    public double LocalCenterY
    {
      get => _centerY - _y;
      set => _centerY = value + _y;
    }

    public double RelativeCenterX
    {
      get => _width.IsZero()  ? _x : (_centerX - _x) / _width;
      set => _centerX = value * _width + _x;
    }

    public Point RelativeCenter
    {
      get => new Point(RelativeCenterX, RelativeCenterY);
      set
      {
        RelativeCenterX = value.X;
        RelativeCenterY = value.Y;
      }
    }

    public double RelativeCenterY
    {
      get => _width.IsZero() ? _y :  (_centerY - _y) / _height;
      set => _centerY = value * _height + _y;
    }

    public double CenterX
    {
      get => _centerX;
      set => _centerX = value;
    }

    public double CenterY
    {
      get => _centerY;
      set => _centerY = value;
    }

    public double ScaleY
    {
      get => _scaleY;
      set => _scaleY = value;
    }

    public Point Center
    {
      get => new Point(_centerX, _centerY);
      set
      {
        _centerX = value.X;
        _centerY = value.Y;
      }
    }

    public Point TransformedCenter
    {
      get => Matrix.Transform(Center);
      set => Center = InvertedMatrix.Transform(value);
    }

    public Point TransformedLocalCenter
    {
      get
      {
        var transformedBounds = TransformedBounds;
        var transformedCenter = TransformedCenter;

        return new Point(transformedCenter.X - transformedBounds.X, transformedCenter.Y - transformedBounds.Y);
      }
      set
      {
        var transformedBounds = TransformedBounds;

        TransformedCenter = new Point(transformedBounds.X + value.X, transformedBounds.Y + value.Y);
      }
    }

    public double TransformedCenterX
    {
      get => TransformedCenter.X;
      set
      {
        var center = TransformedCenter;

        center.X = value;

        TransformedCenter = center;
      }
    }

    public double TransformedCenterY
    {
      get => TransformedCenter.Y;
      set
      {
        var center = TransformedCenter;

        center.Y = value;

        TransformedCenter = center;
      }
    }

    public Rect TransformedBounds
    {
      get => Matrix.TransformRect(Bounds);
      set => Bounds = InvertedMatrix.TransformRect(value);
    }

    public Point TransformedLocation
    {
      get => Matrix.TransformPoint(Location);
      set => Location = InvertedMatrix.TransformPoint(value);
    }

    public Matrix InvertedMatrix => Matrix.GetInvertedMatrix();

    public Matrix Matrix => new Matrix(_scaleX, 0.0, 0.0, _scaleY, _centerX - _scaleX * _centerX + _translateX, _centerY - _scaleY * _centerY + _translateY);

    public void ApplyTransform()
    {
      var bounds = TransformedBounds;

      _scaleX = 1;
      _scaleY = 1;
      _translateX = 0;
      _translateY = 0;

      Bounds = bounds;
    }

    public void ApplyTranslate()
    {
      var bounds = TransformedBounds;

      _translateX = 0;
      _translateY = 0;

      TransformedBounds = bounds;
    }

    public void ApplyScale()
    {
      var bounds = TransformedBounds;

      _scaleX = 1.0;
      _scaleY = 1.0;

      TransformedBounds = bounds;
    }

    internal static Point TransformImpl(double x, double y, double scaleX, double scaleY, double centerX, double centerY, double translateX, double translateY)
    {
      return new Point(x - (centerX - x) * (scaleX - 1) + translateX, y - (centerY - y) * (scaleY - 1) + translateY);
    }

    public void Scale(double scaleX, double scaleY)
    {
      Scale(scaleX, scaleY, CenterX, CenterY);
    }

    public void Scale(double scaleX, double scaleY, double cx, double cy)
    {
      var rcx = RelativeCenterX;
      var rcy = RelativeCenterY;

      var ctx = _x - (_centerX - _x) * (_scaleX - 1) + _translateX;
      var cty = _y - (_centerY - _y) * (_scaleY - 1) + _translateY;

      var tx = ctx - (cx - ctx) * (scaleX / _scaleX - 1);
      var ty = cty - (cy - cty) * (scaleY / _scaleY - 1);

      _scaleX = scaleX;
      _scaleY = scaleY;

      _translateX = tx - (_x - (_centerX - _x) * (_scaleX - 1));
      _translateY = ty - (_y - (_centerY - _y) * (_scaleY - 1));

      ChangeRelativeCenterPreserveTransform(rcx, rcy);
    }

    public void ChangeRelativeCenterPreserveTransform(double relativeCenterX, double relativeCenterY)
    {
      var centerX = relativeCenterX * _width + _x;
      var centerY = relativeCenterY * _height + _y;

      ChangeCenterPreserveTransform(centerX, centerY);
    }

    public void ChangeCenterPreserveTransform(double centerX, double centerY)
    {
      _translateX = _translateX - (_centerX - centerX) * (_scaleX - 1);
      _translateY = _translateY - (_centerY - centerY) * (_scaleY - 1);

      _centerX = centerX;
      _centerY = centerY;
    }

    public void ChangeTranslatePreserveTransform(double translateX, double translateY)
    {
      _centerX = ((_centerX - _x) * (_scaleX - 1) - _translateX + translateX) / (_scaleX - 1) + _x;
      _centerY = ((_centerY - _y) * (_scaleY - 1) - _translateY + translateY) / (_scaleY - 1) + _y;

      _translateX = translateX;
      _translateY = translateY;
    }

    public void RecalculateTranslate(double transformedX, double transformedY)
    {
      _translateX = transformedX - _x + (_centerX - _x) * (_scaleX - 1);
      _translateY = transformedY - _y + (_centerY - _y) * (_scaleY - 1);
    }
  }
}