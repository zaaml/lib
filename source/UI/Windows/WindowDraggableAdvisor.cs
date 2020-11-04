// <copyright file="WindowDraggableAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Behaviors.Draggable;

namespace Zaaml.UI.Windows
{
  internal class WindowDraggableAdvisor : DraggableAdvisorBase
  {
    #region Static Fields and Constants

    public static readonly IDraggableAdvisor Instance = new WindowDraggableAdvisor();

    #endregion

    #region Ctors

    private WindowDraggableAdvisor()
    {
    }

    #endregion

    #region  Methods

    public override Point GetPosition(UIElement element)
    {
      return ((WindowBase) element).GetLocation();
    }

    public override void SetPosition(UIElement element, Point value)
    {
      var window = (WindowBase) element;
      window.SetLocation(value);
      window.IsManualLocation = true;
    }

    #endregion
  }
}