// <copyright file="TemplateIcon.Attached.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Primitives.ContentPrimitives
{
  public partial class TemplateIcon
  {
    #region  Methods

    public static DataTemplate GetTemplate(DependencyObject dependencyObject)
    {
      return (DataTemplate) dependencyObject.GetValue(TemplateProperty);
    }

    public static void SetTemplate(DependencyObject dependencyObject, DataTemplate template)
    {
      dependencyObject.SetValue(TemplateProperty, template);
    }

    #endregion
  }
}