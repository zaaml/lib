// <copyright file="SharedSizeContentControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Windows;
using System.Windows.Markup;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Primitives.SharedSizePrimitives
{
  [ContentProperty("Content")]
  public sealed class SharedSizeContentControl : FixedTemplateControl<SharedSizeContentPanel>
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty ContentProperty = DPM.Register<object, SharedSizeContentControl>
      ("Content", s => s.OnContentChanged);

    public static readonly DependencyProperty SharedSizeKeyProperty = DPM.Register<string, SharedSizeContentControl>
      ("SharedSizeKey", s => s.Invalidate);

    public static readonly DependencyProperty ShareWidthProperty = DPM.Register<bool, SharedSizeContentControl>
      ("ShareWidth", true, s => s.Invalidate);

    public static readonly DependencyProperty ShareHeightProperty = DPM.Register<bool, SharedSizeContentControl>
      ("ShareHeight", true, s => s.Invalidate);

    #endregion

    #region Properties

    public object Content
    {
      get => GetValue(ContentProperty);
      set => SetValue(ContentProperty, value);
    }

    protected override IEnumerator LogicalChildren => TemplateRoot == null || Content == null ? base.LogicalChildren : EnumeratorUtils.Concat(Content, base.LogicalChildren);

    public string SharedSizeKey
    {
      get => (string) GetValue(SharedSizeKeyProperty);
      set => SetValue(SharedSizeKeyProperty, value);
    }


    public bool ShareHeight
    {
      get => (bool) GetValue(ShareHeightProperty);
      set => SetValue(ShareHeightProperty, value);
    }

    public bool ShareWidth
    {
      get => (bool) GetValue(ShareWidthProperty);
      set => SetValue(ShareWidthProperty, value);
    }

    #endregion

    #region  Methods

    protected override void ApplyTemplateOverride()
    {
      base.ApplyTemplateOverride();

      var content = Content;

      if (content != null)
        RemoveLogicalChild(content);

      TemplateRoot.SharedSizeContentControl = this;
    }

    private void Invalidate()
    {
      InvalidateMeasure();
    }

    private void OnContentChanged(object oldContent, object newContent)
    {
      if (TemplateRoot != null)
        TemplateRoot.OnContentChanged();
      else
      {
        if (oldContent != null)
          RemoveLogicalChild(oldContent);

        if (newContent != null)
          AddLogicalChild(newContent);
      }
    }

    protected override void UndoTemplateOverride()
    {
      TemplateRoot.SharedSizeContentControl = null;

      var content = Content;

      if (content != null)
        AddLogicalChild(content);

      base.UndoTemplateOverride();
    }

    #endregion
  }
}