// <copyright file="OrientedTransform.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Controls;
using System.Windows.Media;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore
{
  internal class OrientedTransform
  {
    #region Fields

    private readonly TranslateTransform _transform;
    private Orientation _orientation;

    #endregion

    #region Ctors

    public OrientedTransform(Orientation orientation, TranslateTransform transform)
    {
      _transform = transform;
      _orientation = orientation;
    }

    public OrientedTransform(Orientation orientation)
    {
      _transform = new TranslateTransform();
      _orientation = orientation;
    }

    #endregion

    #region Properties

    public double Direct
    {
      get => _orientation.IsHorizontal() ? _transform.X : _transform.Y;
      set
      {
        if (_orientation.IsHorizontal())
          _transform.X = value;
        else
          _transform.Y = value;
      }
    }

    public double Indirect
    {
      get => _orientation.IsVertical() ? _transform.X : _transform.Y;
      set
      {
        if (_orientation.IsVertical())
          _transform.X = value;
        else
          _transform.Y = value;
      }
    }

    public Orientation Orientation
    {
      get => _orientation;
      set
      {
        if (_orientation == value)
          return;

        Rotate();
      }
    }

    public TranslateTransform Transform => _transform;

    #endregion

    #region  Methods

    public void Rotate()
    {
      var x = _transform.X;
      _transform.X = _transform.Y;
      _transform.Y = x;

      _orientation = _orientation.Rotate();
    }

    #endregion
  }
}