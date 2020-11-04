// <copyright file="FlexChildDefinition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Windows;
using Zaaml.PresentationCore.Converters;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Panels.Interfaces;

namespace Zaaml.UI.Panels.Flexible
{
  public static class FlexChildDefinition
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty MinLengthProperty = DPM.RegisterAttached
      ("MinLength", typeof(FlexChildDefinition), FlexElement.Default.MinLength, OnLayoutPropertyChanged);

    public static readonly DependencyProperty MaxLengthProperty = DPM.RegisterAttached
      ("MaxLength", typeof(FlexChildDefinition), FlexElement.Default.MaxLength, OnLayoutPropertyChanged);

    public static readonly DependencyProperty StretchDirectionProperty = DPM.RegisterAttached
      ("StretchDirection", typeof(FlexChildDefinition), FlexElement.Default.StretchDirection, OnLayoutPropertyChanged);

    public static readonly DependencyProperty OverflowBehaviorProperty = DPM.RegisterAttached
      ("OverflowBehavior", typeof(FlexChildDefinition), FlexElement.Default.OverflowBehavior, OnLayoutPropertyChanged);

    [TypeConverter(typeof(GenericTypeConverter<short>))]
    public static readonly DependencyProperty ExpandPriorityProperty = DPM.RegisterAttached
      ("ExpandPriority", typeof(FlexChildDefinition), FlexElement.Default.ExpandPriority, OnLayoutPropertyChanged);

    [TypeConverter(typeof(GenericTypeConverter<short>))]
    public static readonly DependencyProperty ShrinkPriorityProperty = DPM.RegisterAttached
      ("ShrinkPriority", typeof(FlexChildDefinition), FlexElement.Default.ShrinkPriority, OnLayoutPropertyChanged);

    public static readonly DependencyProperty DefinitionProperty = DPM.RegisterAttached<FlexDefinition>
      ("Definition", typeof(FlexChildDefinition), OnLayoutPropertyChanged);

    [TypeConverter(typeof(FlexLengthConverter))]
    public static readonly DependencyProperty LengthProperty = DPM.RegisterAttached
      ("Length", typeof(FlexChildDefinition), FlexElement.Default.Length, OnLayoutPropertyChanged);

    #endregion

    #region  Methods

    public static FlexDefinition GetDefinition(DependencyObject element)
    {
      return (FlexDefinition) element.GetValue(DefinitionProperty);
    }

    [TypeConverter(typeof(GenericTypeConverter<short>))]
    public static short GetExpandPriority(DependencyObject element)
    {
      return (short) element.GetValue(ExpandPriorityProperty);
    }

    [TypeConverter(typeof(FlexLengthConverter))]
    public static FlexLength GetLength(DependencyObject element)
    {
      return (FlexLength) element.GetValue(LengthProperty);
    }

    public static double GetMaxLength(DependencyObject element)
    {
      return (double) element.GetValue(MaxLengthProperty);
    }

    public static double GetMinLength(DependencyObject element)
    {
      return (double) element.GetValue(MinLengthProperty);
    }

    public static FlexOverflowBehavior GetOverflowBehavior(DependencyObject element)
    {
      return (FlexOverflowBehavior) element.GetValue(OverflowBehaviorProperty);
    }

    [TypeConverter(typeof(GenericTypeConverter<short>))]
    public static short GetShrinkPriority(DependencyObject element)
    {
      return (short) element.GetValue(ShrinkPriorityProperty);
    }

    public static FlexStretchDirection GetStretchDirection(DependencyObject element)
    {
      return (FlexStretchDirection) element.GetValue(StretchDirectionProperty);
    }

    private static void OnLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      (d as IFlexPanel)?.InvalidateMeasure();
    }

    public static void SetDefinition(DependencyObject element, FlexDefinition value)
    {
      element.SetValue(DefinitionProperty, value);
    }

    [TypeConverter(typeof(GenericTypeConverter<short>))]
    public static void SetExpandPriority(DependencyObject element, short value)
    {
      element.SetValue(ExpandPriorityProperty, value);
    }

    [TypeConverter(typeof(FlexLengthConverter))]
    public static void SetLength(DependencyObject element, FlexLength value)
    {
      element.SetValue(LengthProperty, value);
    }

    public static void SetMaxLength(DependencyObject element, double value)
    {
      element.SetValue(MaxLengthProperty, value);
    }

    public static void SetMinLength(DependencyObject element, double value)
    {
      element.SetValue(MinLengthProperty, value);
    }

    public static void SetOverflowBehavior(DependencyObject element, FlexOverflowBehavior value)
    {
      element.SetValue(OverflowBehaviorProperty, value);
    }

    [TypeConverter(typeof(GenericTypeConverter<short>))]
    public static void SetShrinkPriority(DependencyObject element, short value)
    {
      element.SetValue(ShrinkPriorityProperty, value);
    }

    public static void SetStretchDirection(DependencyObject element, FlexStretchDirection value)
    {
      element.SetValue(StretchDirectionProperty, value);
    }

    #endregion
  }
}