// <copyright file="DockItemPreviewElement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Media;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Docking
{
  public sealed class DockItemPreviewElement : Control
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty GeometryProperty = DPM.Register<Geometry, DockItemPreviewElement>
      ("Geometry", e => e.OnGeometryChanged);

    #endregion

    #region Ctors

    static DockItemPreviewElement()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<DockItemPreviewElement>();
    }

    public DockItemPreviewElement()
    {
      this.OverrideStyleKey<DockItemPreviewElement>();
    }

    #endregion

    #region Properties

    public Geometry Geometry
    {
      get => (Geometry) GetValue(GeometryProperty);
      set => SetValue(GeometryProperty, value);
    }

    #endregion

    #region  Methods

    private void OnGeometryChanged(Geometry oldGeometry, Geometry newGeometry)
    {
    }

    #endregion
  }
}