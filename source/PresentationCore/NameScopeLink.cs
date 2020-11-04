// <copyright file="NameScopeLink.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore
{
  public sealed class NameScopeLink : InheritanceContextObject
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty ReferenceProperty = DPM.Register<FrameworkElement, NameScopeLink>
      ("Reference");

    #endregion

    #region Properties

    public FrameworkElement Reference
    {
      get => (FrameworkElement) GetValue(ReferenceProperty);
      set => SetValue(ReferenceProperty, value);
    }

    #endregion
  }
}
