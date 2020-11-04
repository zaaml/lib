// <copyright file="Preview.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Media;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Docking
{
  internal sealed class Preview : DependencyObject
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty GeometryProperty = DPM.Register<Geometry, Preview>
      ("Geometry");

    #endregion

    #region Fields

    public readonly DropGuideAction Action;
    public readonly DockItem DropTarget;

    #endregion

    #region Ctors

    public Preview(DropGuideAction action, DockItem dropTarget)
    {
      Action = action;
      DropTarget = dropTarget;
    }

    #endregion

    #region Properties

    public bool AwaitGeometry { get; set; }

    public Geometry Geometry
    {
      set => SetValue(GeometryProperty, value);
      get => (Geometry) GetValue(GeometryProperty);
    }

    public bool IsArrangePassed { get; set; }

    public bool IsMeasurePassed { get; set; }

    #endregion
  }
}