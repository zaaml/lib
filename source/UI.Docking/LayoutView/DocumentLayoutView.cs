// <copyright file="DocumentLayoutView.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Docking
{
  public sealed class DocumentLayoutView : TabLayoutViewBase<DocumentLayout>
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty RawContentProperty = DPM.Register<bool, DocumentLayoutView>
      (nameof(RawContent));

    #endregion

    #region Ctors

    static DocumentLayoutView()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<DocumentLayoutView>();
    }

    public DocumentLayoutView()
    {
      this.OverrideStyleKey<DocumentLayoutView>();
    }

    #endregion

    #region Properties

    public bool RawContent
    {
      get => (bool) GetValue(RawContentProperty);
      internal set => SetValue(RawContentProperty, value);
    }

    #endregion
  }
}