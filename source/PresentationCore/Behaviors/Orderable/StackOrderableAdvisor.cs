// <copyright file="StackOrderableAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Behaviors.Orderable
{
  internal abstract class StackOrderableAdvisor : IOrderableAdvisor
  {
    #region Fields

    private List<ElementSite> _elements;
    private ElementSite _mouseLockSite;
    private ElementSite _returnSite;
    private Tuple<ElementSite, ElementSite> _returnSiteData;

    #endregion

    #region  Methods

    private void ArrangeElements(ElementSite except)
    {
      var offset = 0.0;

      foreach (var element in _elements)
      {
        if (element != except)
          element.Offset = offset;

        offset += element.Size;
      }
    }

    public double CalcElementOffset(List<ElementSite> elementSites, ElementSite element)
    {
      return elementSites.TakeWhile(c => c != element).Sum(c => c.Size);
    }

    protected abstract List<ElementSite> CaptureOrderedSequence(FrameworkElement source);

    private void FinalizeOrdering(FrameworkElement element)
    {
      _returnSite = null;
      _mouseLockSite = null;

      ReleaseOrderedSequence(element, _elements);

      _elements = null;
    }
    
    protected abstract ElementSite GetActualSite(DependencyObject dependencyObject);

    private ElementSite GetArrangedElement(ElementSite element)
    {
      return new ElementSite(CalcElementOffset(_elements, element), element.Size, element.Orientation);
    }

    protected abstract Point GetMousePosition(FrameworkElement element);

    protected abstract void ReleaseOrderedSequence(FrameworkElement source, List<ElementSite> elements);

    #endregion

    #region Interface Implementations

    #region IOrderableAdvisor

    public void OnOrderEnd(FrameworkElement element)
    {
      FinalizeOrdering(element);
    }

    public void OnOrderMove(FrameworkElement element, Vector delta)
    {
      var elementSite = GetActualSite(element);
      var mousePosition = GetMousePosition(element);

      elementSite.Position = elementSite.OriginalPosition.WithOffset((Point)delta);

      if (_returnSite != null && _returnSite.Contains(mousePosition))
      {
        _elements.Swap(_returnSiteData.Item1, _returnSiteData.Item2);

        ArrangeElements(elementSite);

        _mouseLockSite = null;
        _returnSite = null;

        return;
      }

      if (_mouseLockSite != null && _mouseLockSite.Contains(mousePosition))
        return;

      _mouseLockSite = null;

      var mouseSite = _elements.SingleOrDefault(e => e != elementSite && e.Contains(mousePosition));

      if (mouseSite == null)
        return;

      _returnSite = GetArrangedElement(elementSite);
      _returnSiteData = new Tuple<ElementSite, ElementSite>(mouseSite, elementSite);
      _elements.Swap(elementSite, mouseSite);

      ArrangeElements(elementSite);

      _mouseLockSite = GetArrangedElement(mouseSite);
    }

    public void OnOrderStart(FrameworkElement element)
    {
      _elements = CaptureOrderedSequence(element);
    }

    #endregion

    #endregion
  }
}
