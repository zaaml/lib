// <copyright file="WeakContentControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Core
{
  [ContentProperty("Content")]
  public sealed class WeakContentControl : FixedTemplateControl
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty ContentProperty = DPM.Register<object, WeakContentControl>
      ("Content", w => w.OnContentChanged);

    #endregion

    #region Ctors

    public WeakContentControl()
      : base(TemplateKind.ContentPresenter)
    {
    }

    #endregion

    #region Properties

    public object Content
    {
      get => GetValue(ContentProperty);
      set => SetValue(ContentProperty, value);
    }

    #endregion

    #region  Methods

    private void OnContentChanged()
    {
      if (IsLoaded && ContentPresenter != null)
        ContentPresenter.Content = Content;
    }

    protected override void OnLoaded()
    {
      base.OnLoaded();

      if (ContentPresenter != null)
        ContentPresenter.Content = Content;
    }

    protected override void OnUnloaded()
    {
      base.OnUnloaded();

      if (ContentPresenter != null)
        ContentPresenter.Content = null;
    }

    #endregion
  }
}