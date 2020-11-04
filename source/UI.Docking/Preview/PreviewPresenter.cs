// <copyright file="PreviewPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Docking
{
  internal partial class PreviewPresenter
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty PreviewProperty = DPM.Register<Preview, PreviewPresenter>
      ("Preview");

		private static readonly Lazy<PreviewPresenter> LazyInstance = new Lazy<PreviewPresenter>(() => new PreviewPresenter());

		#endregion

		#region Ctors

		public PreviewPresenter()
    {
      PlatformCtor();

      PreviewElement.SetBinding(DockItemPreviewElement.GeometryProperty, new Binding("Preview.Geometry") {Source = this});
    }

    #endregion

    #region Properties

    public Preview Preview
    {
      set => SetValue(PreviewProperty, value);
      get => (Preview) GetValue(PreviewProperty);
    }

    private DockItemPreviewElement PreviewElement { get; } = new DockItemPreviewElement();

    #endregion

    #region  Methods

    partial void HideImpl();

    public void HidePresenter()
    {
      HideImpl();
    }

    partial void PlatformCtor();

    partial void ShowImpl();

    public void ShowPresenter()
    {
      ShowImpl();
    }

    #endregion
  }
}