// <copyright file="ContentPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Panels
{
  internal sealed class ContentPanel : PanelBase
  {
    #region Fields

    private object _content;

    #endregion

    #region Properties

    public object Content
    {
      get => _content;
      set
      {
        if (ReferenceEquals(_content, value))
          return;

        if (_content != null && value == null)
          Children.Clear();
        else if (_content == null && value != null)
          Children.Add(WrapContent(value));
        else
          Children[0] = WrapContent(value);

        _content = value;
      }
    }

    #endregion

    #region  Methods

    private static FrameworkElement WrapContent(object content)
    {
      return content as FrameworkElement ?? new TextBlock {Text = content.ToString()};
    }

    #endregion
  }
}