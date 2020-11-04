// <copyright file="FrameworkValueSelector.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Data;
using System.Windows.Markup;
using Zaaml.PresentationCore.MarkupExtensions;

namespace Zaaml.PresentationCore
{
  public class FrameworkValueSelector
  {
    #region Properties

    public object ActualValue
    {
      get
      {
        switch (XamlConstants.Framework)
        {
          case FrameworkType.WPF:
            return WPFValue;
          case FrameworkType.Silverlight:
            return SilverlightValue;
          default:
            return null;
        }
      }
    }

    public object SilverlightValue { get; set; }
    public object WPFValue { get; set; }

    #endregion
  }

  public class FrameworkValueSelectorExtension : MarkupExtensionBase
  {
    #region Properties

    public object SilverlightValue { get; set; }
    public object WPFValue { get; set; }

    #endregion

    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      switch (XamlConstants.Framework)
      {
        case FrameworkType.WPF:
          return WPFValue;
        case FrameworkType.Silverlight:
          return SilverlightValue;
        default:
          return null;
      }
    }

    #endregion
  }

  public class FrameworkBindingSelectorExtension : MarkupExtensionBase
  {
    #region Properties

    public Binding SilverlightBinding { get; set; }
    public Binding WPFBinding { get; set; }

    #endregion

    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      switch (XamlConstants.Framework)
      {
        case FrameworkType.WPF:
          return WPFBinding;
        case FrameworkType.Silverlight:
          return SilverlightBinding;
        default:
          return null;
      }
    }

    #endregion
  }
}