// <copyright file="SplitLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Docking
{
  public sealed class SplitLayout : BaseLayout
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty OrientationProperty = DPM.Register<Orientation, SplitLayout>
      ("Orientation", Orientation.Horizontal, l => l.OnOrientationChanged);

    public static readonly DependencyProperty SplitWidthProperty = DPM.RegisterAttached<double, SplitLayout>
      ("SplitWidth", 200, OnSplitWidthChanged);

    public static readonly DependencyProperty SplitHeightProperty = DPM.RegisterAttached<double, SplitLayout>
      ("SplitHeight", 200, OnSplitHeightChanged);

    private static readonly List<DependencyProperty> SplitLayoutProperties = new List<DependencyProperty>
    {
      SplitWidthProperty,
      SplitHeightProperty
    };

    #endregion

    #region Ctors

    static SplitLayout()
    {
      RegisterLayoutProperties<SplitLayout>(SplitLayoutProperties);
    }

    #endregion

    #region Properties

    public override LayoutKind LayoutKind => LayoutKind.Split;

    public Orientation Orientation
    {
      get => (Orientation) GetValue(OrientationProperty);
      set => SetValue(OrientationProperty, value);
    }

    #endregion

    #region  Methods

    public static double GetSplitHeight(DependencyObject depObj)
    {
      return (double) depObj.GetValue(SplitHeightProperty);
    }

    public static double GetSplitWidth(DependencyObject depObj)
    {
      return (double) depObj.GetValue(SplitWidthProperty);
    }

    private void OnOrientationChanged(Orientation oldOrientation, Orientation newOrientation)
    {
    }

    private static void OnSplitHeightChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
			OnLayoutPropertyChanged(dependencyObject, e);
    }

    private static void OnSplitWidthChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
	    OnLayoutPropertyChanged(dependencyObject, e);
		}

    public static void SetSplitHeight(DependencyObject depObj, double value)
    {
      depObj.SetValue(SplitHeightProperty, value);
    }

    public static void SetSplitWidth(DependencyObject depObj, double value)
    {
      depObj.SetValue(SplitWidthProperty, value);
    }

    #endregion
  }
}