// <copyright file="OrderAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;

namespace Zaaml.PresentationCore.Behaviors.Orderable
{
  internal abstract class OrderAdvisor
  {
    #region  Methods

    public abstract List<ElementSite> CaptureOrderedSequence(FrameworkElement source);

    public abstract UIElement GetParentPanel(FrameworkElement source);

    public abstract void ReleaseOrderedSequence(FrameworkElement source, List<ElementSite> elements);

    #endregion
  }
}