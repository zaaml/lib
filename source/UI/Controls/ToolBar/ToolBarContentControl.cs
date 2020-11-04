// <copyright file="ToolBarItemContentControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.ToolBar
{
  [ContentProperty("Content")]
  public class ToolBarContentControl : ToolBarItem
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty ContentProperty = DPM.Register<object, ToolBarContentControl>
      ("Content");

    public static readonly DependencyProperty ContentTemplateProperty = DPM.Register<DataTemplate, ToolBarContentControl>
      ("ContentTemplate");

    #endregion

    #region Ctors

    static ToolBarContentControl()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<ToolBarContentControl>();
    }

    public ToolBarContentControl()
    {
      this.OverrideStyleKey<ToolBarContentControl>();
    }

    #endregion

    #region Properties

    public object Content
    {
      get => GetValue(ContentProperty);
      set => SetValue(ContentProperty, value);
    }


    public DataTemplate ContentTemplate
    {
      get => (DataTemplate) GetValue(ContentTemplateProperty);
      set => SetValue(ContentTemplateProperty, value);
    }

    #endregion
  }
}