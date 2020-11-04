// <copyright file="FrameworkElementSite.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;

namespace Zaaml.PresentationCore.Behaviors.Orderable
{
  internal class FrameworkElementSite : ElementSite
  {
    #region Fields

    private double _definitionOffset;

    #endregion

    #region Ctors

    public FrameworkElementSite(FrameworkElement element, double offset, double size, Orientation orientation)
      : base(offset, size, orientation)
    {
      Element = element;
    }

    #endregion

    #region Properties

    public FrameworkElement Element { get; }

    #endregion

    #region  Methods

    public void SetDefinitionOffset(double value)
    {
      _definitionOffset = value;
    }

    #endregion
  }
}