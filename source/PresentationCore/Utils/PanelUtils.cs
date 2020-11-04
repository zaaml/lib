// <copyright file="PanelUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Utils
{
	public static class PanelUtils
	{
		#region  Methods

		internal static void InvalidateAncestorsArrange(UIElement element, UIElement untilAncestor)
		{
      InvalidateAncestorsArrange(element, untilAncestor, false);
		}

		internal static void InvalidateAncestorsMeasure(UIElement element, UIElement untilAncestor)
		{
      InvalidateAncestorsMeasure(element, untilAncestor, false);
		}

		internal static void InvalidateAncestorsArrange(UIElement element)
		{
      InvalidateAncestorsArrange(element, null, false);
		}

		internal static void InvalidateAncestorsMeasure(UIElement element)
		{
      InvalidateAncestorsMeasure(element, null, false);
		}

	  internal static void InvalidateAncestorsArrangeIncludeLast(UIElement element, UIElement untilAncestor)
	  {
	    InvalidateAncestorsArrange(element, untilAncestor, true);
	  }

	  internal static void InvalidateAncestorsMeasureIncludeLast(UIElement element, UIElement untilAncestor)
	  {
	    InvalidateAncestorsMeasure(element, untilAncestor, true);
	  }

    private static void InvalidateAncestorsArrange(UIElement element, UIElement untilAncestor, bool includeLast)
	  {
		  if (untilAncestor == null)
		  {
			  foreach (var ancestor in element.GetVisualAncestorsAndSelf<UIElement>())
				  ancestor.InvalidateArrange();

				return;
			}

			foreach (var ancestor in element.GetVisualAncestorsAndSelf<UIElement>().TakeWhile(a => ReferenceEquals(a, untilAncestor) == false))
	      ancestor.InvalidateArrange();

      if (includeLast)
        untilAncestor.InvalidateArrange();
	  }

	  private static void InvalidateAncestorsMeasure(UIElement element, UIElement untilAncestor, bool includeLast)
	  {
		  if (untilAncestor == null)
		  {
			  foreach (var ancestor in element.GetVisualAncestorsAndSelf<UIElement>())
				  ancestor.InvalidateMeasure();

			  return;
		  }

			foreach (var ancestor in element.GetVisualAncestorsAndSelf<UIElement>().TakeWhile(a => ReferenceEquals(a, untilAncestor) == false))
	      ancestor.InvalidateMeasure();

	    if (includeLast)
	      untilAncestor.InvalidateMeasure();
    }

    public static void SetZIndex(UIElement element, int zindex)
		{
#if SILVERLIGHT
      Canvas.SetZIndex(element, zindex);
#else
			Panel.SetZIndex(element, zindex);
#endif
		}

		#endregion
	}
}