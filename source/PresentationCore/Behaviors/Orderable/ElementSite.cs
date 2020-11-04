// <copyright file="ElementSite.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Behaviors.Orderable
{
  internal class ElementSite : INotifyPropertyChanged
  {
    #region Fields

    private double _offset;

    #endregion

    #region Ctors

    public ElementSite(double offset, double size, Orientation orientation)
    {
      _offset = offset;
      OriginalOffset = _offset;
      Orientation = orientation;
      Size = size;
    }

    #endregion

    #region Properties

    public double OriginalOffset { get; }

    public double Offset
    {
      get => _offset;
      set
      {
        if (_offset.IsCloseTo(value))
          return;

        _offset = value;

        OnOffsetChanged();
      }
    }

    public Orientation Orientation { get; }

    public Point Position
    {
      get => Orientation == Orientation.Horizontal ? new Point(_offset, 0) : new Point(0, _offset);
      set => Offset = Orientation == Orientation.Horizontal ? value.X : value.Y;
    }

    public Point OriginalPosition => Orientation == Orientation.Horizontal ? new Point(OriginalOffset, 0) : new Point(0, OriginalOffset);

    public double Size { get; }

    #endregion

    #region  Methods

    public bool Contains(double coor)
    {
      return coor >= _offset && coor < _offset + Size;
    }

    public bool Contains(Point point)
    {
      return Contains(Orientation == Orientation.Horizontal ? point.X : point.Y);
    }

    protected virtual void OnOffsetChanged()
    {
      OnPropertyChanged("Offset");
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

    #region Interface Implementations

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #endregion
  }
}