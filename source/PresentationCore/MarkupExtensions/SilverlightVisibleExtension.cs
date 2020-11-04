// <copyright file="SilverlightVisibleExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;

namespace Zaaml.PresentationCore.MarkupExtensions
{
  public class SilverlightVisibleExtension : MarkupExtensionBase
  {
    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
#if SILVERLIGHT
      return Visibility.Visible;
#else
      return Visibility.Collapsed;
#endif
    }

    #endregion
  }
}