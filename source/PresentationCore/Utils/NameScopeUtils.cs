// <copyright file="NameScopeUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.PresentationCore.Utils
{
  internal static class NameScopeUtils
  {
    #region  Methods

    public static object FindName(DependencyObject dataObject, string elementName)
    {
      var templateElement = dataObject.GetTemplateElement(elementName);
      if (templateElement != null)
        return templateElement;

      var fre = dataObject as FrameworkElement;
      if (fre == null)
        return null;

      var root = fre.GetVisualRoot() as FrameworkElement;
      var el = root?.FindName(elementName);

      var templatedParent = fre.GetTemplatedParent();
      if (templatedParent != null)
      {
      }

      return null;
    }

    #endregion
  }
}