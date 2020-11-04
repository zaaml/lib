// <copyright file="UIElementTransformUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Media;
using Zaaml.PresentationCore.Extensions;
#if SILVERLIGHT
using System.Windows.Controls.Primitives;
using Zaaml.Core;
using System.Collections.Generic;
using System.Linq;

#endif

namespace Zaaml.PresentationCore.Utils
{
  internal static class UIElementTransformUtils
  {
    #region  Methods

    public static GeneralTransform TransformToScreen(UIElement element)
    {
	    if (element is ILayoutInfoProvider layoutProvider)
      {
        var box = layoutProvider.HostRelativeBox;

        return new TranslateTransform { X = box.Left, Y = box.Top };
      }

#if SILVERLIGHT
      return TransformToAncestor(element);
#else
      return new ElementScreenTransform(element);
#endif
    }

    #endregion

#if SILVERLIGHT

    private static Transform GetPopupChildTransform(UIElement element)
    {
      var fre = element as FrameworkElement;
      return fre != null ? TransformUtils.CombineTransform(fre.RenderTransform, new TranslateTransform {X = fre.Margin.Left, Y = fre.Margin.Top}) : element.RenderTransform;
    }

    private static Transform TransformToPopup(UIElement element, Popup popup)
    {
      if (ReferenceEquals(element, popup.Child) || popup.Child == null)
        return GetPopupChildTransform(element);

      var transformToPopupChild = (Transform) element.TransformToVisual(popup.Child);
      var childTransform = GetPopupChildTransform(popup.Child);

      return TransformUtils.CombineTransform(transformToPopupChild, childTransform);
    }

    private static Transform GetPopupTransform(Popup popup)
    {
      return TransformUtils.CombineTransform(popup.RenderTransform, new TranslateTransform {X = popup.HorizontalOffset, Y = popup.VerticalOffset} as Transform);
    }


    private static GeneralTransform TransformToAncestor(UIElement element)
    {
      List<Transform> transformList = null;

      try
      {
        var ancestors = element.GetAncestors(MixedTreeEnumerationStrategy.VisualThenLogicalInstance);

        UIElement elementRoot = null;

        var curElement = element;
        foreach (var ancestor in ancestors.OfType<UIElement>())
        {
          elementRoot = ancestor;
          var popupAncestor = ancestor as Popup;

          if (popupAncestor == null)
            continue;

          if (transformList == null)
            transformList = new List<Transform>();

          var transformToPopup = TransformToPopup(curElement, popupAncestor);

          if (transformToPopup != null)
            transformList.Add(transformToPopup);

          var popupTransform = GetPopupTransform(popupAncestor);

          if (popupTransform != null)
            transformList.Add(popupTransform);

          curElement = popupAncestor;
        }

        if (elementRoot == null)
          return Transforms.Identity;

        var hostRoot = Application.Current.RootVisual;
        var isRootLoaded = ReferenceEquals(elementRoot, hostRoot) || ReferenceEquals(hostRoot, elementRoot.GetVisualRoot());

        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (ReferenceEquals(curElement, elementRoot) == false && (curElement is Popup == false || isRootLoaded))
        {
          var transformToVisual = isRootLoaded ? (Transform) curElement.TransformToVisual(elementRoot) : null;

          if (transformToVisual != null)
          {
            if (transformList != null)
              transformList.Add(transformToVisual);
            else
              return transformToVisual;
          }
          else if (transformList == null)
            return Transforms.Identity;
        }
      }
      catch (Exception e)
      {
        LogService.LogError(e);
      }

      if (transformList == null)
        return Transforms.Identity;

      return transformList.Count == 1 ? transformList[0] : TransformUtils.CombineTransform(transformList);
    }
#else


    internal static Rect TransformRectToClient(this FrameworkElement element, Rect screenRect)
    {
      var topLeft = element.PointFromScreen(screenRect.GetTopLeft().FromLogicalToDevice());
      var bottomRight = element.PointFromScreen(screenRect.GetBottomRight());

      return new Rect(topLeft, bottomRight).FromDeviceToLogical();
    }


    private class ElementScreenTransform : GeneralTransform
    {
      #region Fields

      private readonly UIElement _element;
      private readonly bool _inverse;

      #endregion

      #region Ctors

      public ElementScreenTransform(UIElement element)
      {
        _element = element;
      }

      private ElementScreenTransform(UIElement element, bool inverse)
      {
        _element = element;
        _inverse = inverse;
      }

      #endregion

      #region Properties

      public override GeneralTransform Inverse => new ElementScreenTransform(_element, !_inverse);

      #endregion

      #region  Methods

      protected override Freezable CreateInstanceCore()
      {
        throw new NotSupportedException();
      }

      public override Rect TransformBounds(Rect rect)
      {
        if (PresentationSource.FromVisual(_element) == null)
          return rect;

        var topLeft = TransformImpl(rect.TopLeft);
        var bottomRight = TransformImpl(rect.BottomRight);

        return new Rect(topLeft, bottomRight);
      }

      private Point TransformImpl(Point inPoint)
      {
        return _inverse ? _element.PointFromScreen(inPoint).FromLogicalToDevice() : _element.PointToScreen(inPoint).FromDeviceToLogical();
      }

      public override bool TryTransform(Point inPoint, out Point result)
      {
        result = inPoint;
        try
        {
          if (PresentationSource.FromVisual(_element) != null)
            result = TransformImpl(inPoint);

          return true;
        }
        catch (Exception)
        {
          return false;
        }
      }

      #endregion
    }
#endif
  }
}