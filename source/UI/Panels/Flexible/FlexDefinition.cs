// <copyright file="FlexDefinition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using Zaaml.Core.Weak.Collections;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Converters;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Panels.Interfaces;

namespace Zaaml.UI.Panels.Flexible
{
  public sealed class FlexDefinition : InheritanceContextObject
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty MinLengthProperty = DPM.RegisterAttached<double, FlexDefinition>
      ("MinLength", FlexElement.Default.MinLength, OnLayoutPropertyChanged);

    public static readonly DependencyProperty MaxLengthProperty = DPM.RegisterAttached<double, FlexDefinition>
      ("MaxLength", FlexElement.Default.MaxLength, OnLayoutPropertyChanged);

    public static readonly DependencyProperty StretchDirectionProperty = DPM.RegisterAttached<FlexStretchDirection, FlexDefinition>
      ("StretchDirection", FlexElement.Default.StretchDirection, OnLayoutPropertyChanged);

    [TypeConverter(typeof(GenericTypeConverter<short>))]
    public static readonly DependencyProperty ExpandPriorityProperty = DPM.RegisterAttached<short, FlexDefinition>
      ("ExpandPriority", FlexElement.Default.ExpandPriority, OnLayoutPropertyChanged);

    [TypeConverter(typeof(GenericTypeConverter<short>))]
    public static readonly DependencyProperty ShrinkPriorityProperty = DPM.RegisterAttached<short, FlexDefinition>
      ("ShrinkPriority", FlexElement.Default.ShrinkPriority, OnLayoutPropertyChanged);

    public static readonly DependencyProperty OverflowBehaviorProperty = DPM.RegisterAttached<FlexOverflowBehavior, FlexDefinition>
      ("OverflowBehavior", FlexElement.Default.OverflowBehavior, OnLayoutPropertyChanged);

    public static readonly DependencyProperty DefinitionProperty = DPM.RegisterAttached<FlexDefinition, FlexDefinition>
      ("Definition", null, OnDefinitionPropertyChanged);

    public static readonly DependencyProperty LengthProperty = DPM.RegisterAttached<FlexLength, FlexDefinition>
      ("Length", FlexElement.Default.Length, OnLayoutPropertyChanged);

    #endregion

    #region Fields

    private readonly WeakLinkedList<UIElement> _attachedElements = new WeakLinkedList<UIElement>();

    public event EventHandler DefinitionChanged;

    #endregion

    #region Properties

    [TypeConverter(typeof(GenericTypeConverter<short>))]
    public short ExpandPriority
    {
      get => (short) GetValue(ExpandPriorityProperty);
      set => SetValue(ExpandPriorityProperty, value);
    }

    [TypeConverter(typeof(FlexLengthTypeConverter))]
    public FlexLength Length
    {
      get => (FlexLength) GetValue(LengthProperty);
      set => SetValue(LengthProperty, value);
    }

    public double MaxLength
    {
      get => (double) GetValue(MaxLengthProperty);
      set => SetValue(MaxLengthProperty, value);
    }

    public double MinLength
    {
      get => (double) GetValue(MinLengthProperty);
      set => SetValue(MinLengthProperty, value);
    }

    public FlexOverflowBehavior OverflowBehavior
    {
      get => (FlexOverflowBehavior) GetValue(OverflowBehaviorProperty);
      set => SetValue(OverflowBehaviorProperty, value);
    }

    [TypeConverter(typeof(GenericTypeConverter<short>))]
    public short ShrinkPriority
    {
      get => (short) GetValue(ShrinkPriorityProperty);
      set => SetValue(ShrinkPriorityProperty, value);
    }

    public FlexStretchDirection StretchDirection
    {
      get => (FlexStretchDirection) GetValue(StretchDirectionProperty);
      set => SetValue(StretchDirectionProperty, value);
    }

    #endregion

    #region  Methods

    private void AttachDefinition(UIElement uie)
    {
      _attachedElements.Add(uie);
    }

    private void DetachDefinition(UIElement uie)
    {
      _attachedElements.Remove(uie);
    }

    public static FlexDefinition GetDefinition(DependencyObject element)
    {
      return (FlexDefinition) element.GetValue(DefinitionProperty);
    }

    [TypeConverter(typeof(GenericTypeConverter<short>))]
    public static short GetExpandPriority(DependencyObject element)
    {
      return (short) element.GetValue(ExpandPriorityProperty);
    }

    [TypeConverter(typeof(FlexLengthTypeConverter))]
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

    private void OnDefinitionChanged()
    {
      foreach (var uie in _attachedElements)
        (uie.GetVisualParent() as IFlexPanel)?.InvalidateMeasure();

      DefinitionChanged?.Invoke(this, EventArgs.Empty);
    }

    private static void OnDefinitionPropertyChanged(DependencyObject dependencyObject, FlexDefinition oldDefinition, FlexDefinition newDefinition)
    {
      (dependencyObject.GetVisualParent() as IFlexPanel)?.InvalidateMeasure();

      var uie = dependencyObject as UIElement;

      if (uie == null)
        return;

      oldDefinition?.DetachDefinition(uie);
      newDefinition?.AttachDefinition(uie);
    }

    private static void OnLayoutPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
      (dependencyObject.GetVisualParent() as IFlexPanel)?.InvalidateMeasure();
      (dependencyObject as FlexDefinition)?.OnDefinitionChanged();
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

    [TypeConverter(typeof(FlexLengthTypeConverter))]
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