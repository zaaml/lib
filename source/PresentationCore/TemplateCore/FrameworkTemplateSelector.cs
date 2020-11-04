// <copyright file="FrameworkTemplateSelector.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Controls;

namespace Zaaml.PresentationCore.TemplateCore
{
  public class FrameworkTemplateSelector
  {
    #region Properties

    public ControlTemplate ActualTemplate
    {
      get
      {
        switch (XamlConstants.Framework)
        {
          case FrameworkType.WPF:
            return WPFTemplate;
          case FrameworkType.Silverlight:
            return SilverlightTemplate;
          default:
            return null;
        }
      }
    }

    public ControlTemplate SilverlightTemplate { get; set; }
    public ControlTemplate WPFTemplate { get; set; }

    #endregion
  }
}