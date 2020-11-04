// <copyright file="IHeaderedContentControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;

namespace Zaaml.UI.Controls.Interfaces
{
  internal interface IHeaderedContentControl : IContentControl
  {
    #region Properties

    object Header { get; set; }

    DependencyProperty HeaderProperty { get; }

    string HeaderStringFormat { get; set; }

    DependencyProperty HeaderStringFormatProperty { get; }

    DataTemplate HeaderTemplate { get; set; }

    DependencyProperty HeaderTemplateProperty { get; }

    DataTemplateSelector HeaderTemplateSelector { get; set; }

    DependencyProperty HeaderTemplateSelectorProperty { get; }

    #endregion
  }

  internal interface IContentItemsControl
  {
    DataTemplate ItemContentTemplate { get; }

    DataTemplateSelector ItemContentTemplateSelector { get; }

    string ItemContentStringFormat { get; }
  }

  internal interface IHeaderedContentItemsControl : IContentItemsControl
  {
    DataTemplate ItemHeaderTemplate { get; }

    DataTemplateSelector ItemHeaderTemplateSelector { get; }

    string ItemHeaderStringFormat { get; }
  }
}