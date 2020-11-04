// <copyright file="DockLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Docking
{
  public sealed class DockLayout : BaseLayout
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty DockSideProperty = DPM.RegisterAttached<Dock, DockLayout>
      ("DockSide", Dock.Left, OnDockSideChanged);

    public static readonly DependencyProperty DockWidthProperty = DPM.RegisterAttached<double, DockLayout>
      ("DockWidth", 200.0, OnDockWidthPropertyChanged);

    public static readonly DependencyProperty DockHeightProperty = DPM.RegisterAttached<double, DockLayout>
      ("DockHeight", 200.0, OnDockHeightPropertyChanged);

    private static readonly List<DependencyProperty> DockLayoutProperties = new List<DependencyProperty>
    {
      DockSideProperty,
      DockWidthProperty,
      DockHeightProperty
    };

    #endregion

    #region Ctors

    static DockLayout()
    {
      RegisterLayoutProperties<DockLayout>(DockLayoutProperties);
      RegisterLayoutSerializer<DockLayout>(new DockLayoutSerializer());
    }

    #endregion

    #region Properties

    internal DockLayoutView DockLayoutView => (DockLayoutView) View;

    public override LayoutKind LayoutKind => LayoutKind.Dock;

    #endregion

    #region  Methods

    public static double GetDockHeight(DependencyObject depObj)
    {
      return (double) depObj.GetValue(DockHeightProperty);
    }

    public static Dock GetDockSide(DependencyObject depObj)
    {
      return (Dock) depObj.GetValue(DockSideProperty);
    }

    public static Size GetDockSize(DependencyObject depObj)
    {
      return new Size(GetDockWidth(depObj), GetDockHeight(depObj));
    }

    public static double GetDockWidth(DependencyObject depObj)
    {
      return (double) depObj.GetValue(DockWidthProperty);
    }

    private static void OnDockHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      OnDockSizePropertyChanged(d, e);
			OnLayoutPropertyChanged(d, e);
    }

    private static void OnDockSideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
	    if (d is DockItem dockItem)
      {
	      var dockLayout = dockItem.ActualLayout as DockLayout;

	      dockLayout?.OnItemDockSideChanged(dockItem);
      }

			OnLayoutPropertyChanged(d, e);
    }

    private static void OnDockSizePropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
    {
      var dockItem = depObj as DockItem;

      if (dockItem == null)
        return;

      var dockLayout = dockItem.ActualLayout as DockLayout;

      dockLayout?.OnItemDockSizeChanged(dockItem);
    }

    private static void OnDockWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      OnDockSizePropertyChanged(d, e);
			OnLayoutPropertyChanged(d, e);
    }

    private void OnItemDockSideChanged(DockItem item)
    {
      DockLayoutView?.OnItemDockSideChanged(item);
    }

    private void OnItemDockSizeChanged(DockItem item)
    {
      DockLayoutView?.OnItemDockSizeChanged(item);
    }

    public static void SetDockHeight(DependencyObject depObj, double value)
    {
      depObj.SetValue(DockHeightProperty, value);
    }

    public static void SetDockSide(DependencyObject depObj, Dock value)
    {
      depObj.SetValue(DockSideProperty, value);
    }

    public static void SetDockSize(DependencyObject depObj, Size size)
    {
      SetDockWidth(depObj, size.Width);
      SetDockHeight(depObj, size.Height);
    }

    public static void SetDockWidth(DependencyObject depObj, double value)
    {
      depObj.SetValue(DockWidthProperty, value);
    }

    #endregion

    #region  Nested Types

    private sealed class DockLayoutSerializer : LayoutSerializer
    {
      #region  Methods

      public override void WriteProperties(DependencyObject dependencyObject, XElement element)
      {
        if (DockLayoutProperties.Any(l => ShouldSerializeProperty(typeof(DockLayout), dependencyObject, l)))
        {
          var propertyName = FormatProperty(typeof(DockLayout), "DockSize");

          element.Add(new XAttribute(propertyName, GetDockSize(dependencyObject).ToString(CultureInfo.InvariantCulture)));
        }
      }

      #endregion
    }

    #endregion
  }
}