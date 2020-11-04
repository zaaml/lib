// <copyright file="FlexExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.PropertyCore.Extensions;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Panels.Flexible
{
  public static class FlexExtensions
  {
    #region  Methods

    public static FlexElement GetFlexElement(this UIElement child, Panel panel, FlexDefinition panelChildDefinition = null)
    {
      // ReSharper disable once SuspiciousTypeConversion.Global
      var flexChild = child as IFlexElement;

      if (flexChild != null)
      {
        return new FlexElement(flexChild.MinLength, flexChild.MaxLength, true)
        {
          OverflowBehavior = flexChild.OverflowBehavior,
          StretchDirection = flexChild.StretchDirection,
          ExpandPriority = flexChild.ExpandPriority,
          ShrinkPriority = flexChild.ShrinkPriority
        };
      }

      return new FlexElementComposition(child, null, panel, panelChildDefinition).FlexElement;
    }

    #endregion

    #region  Nested Types

    private struct FlexElementComposition
    {
      private static readonly FlexDefinition DefaultDefinition = new FlexDefinition();

      private readonly UIElement _child;
      private readonly FlexDefinition _childExplicitDefinition;
      private FlexDefinition _childAttachedDefinition;
      private readonly Panel _panel;
      private readonly FlexDefinition _panelExplicitDefinition;
      private FlexDefinition _panelAttachedDefinition;
      private FlexOverflowBehavior? _overflowBehavior;
      private FlexStretchDirection? _stretchDirection;
      private FlexLength? _length;
      private short? _expandPriority;
      private short? _shrinkPriority;
      private double? _maxLength;
      private double? _minLength;

      public FlexElementComposition(UIElement child, FlexDefinition childExplicitDefinition, Panel panel, FlexDefinition panelExplicitDefinition) : this()
      {
        _child = child;
        _childExplicitDefinition = childExplicitDefinition;
        _panel = panel;
        _panelExplicitDefinition = panelExplicitDefinition;
        _childAttachedDefinition = DefaultDefinition;
        _panelAttachedDefinition = DefaultDefinition;
      }

      private FlexDefinition ActualChildAttachedDefinition
      {
        get
        {
          if (ReferenceEquals(_childAttachedDefinition, DefaultDefinition))
            _childAttachedDefinition = (FlexDefinition)_child.GetValue(FlexDefinition.DefinitionProperty);

          return _childAttachedDefinition;
        }
      }

      private FlexDefinition ActualPanelAttachedDefinition
      {
        get
        {
          if (ReferenceEquals(_panelAttachedDefinition, DefaultDefinition))
            _panelAttachedDefinition = FlexChildDefinition.GetDefinition(_panel);

          return _panelAttachedDefinition;
        }
      }

      private TValue GetActualValue<TValue>(ref TValue? store, DependencyProperty definitionProperty, DependencyProperty childDefinitionProperty, TValue defaultValue) where TValue : struct
      {
        if (store.HasValue)
          return store.Value;

        var actualValue = default(TValue);

        while (true)
        {
          // Child Explicit Definition
          if (_childExplicitDefinition?.TryGetNonDefaultValue(definitionProperty, out actualValue) == true)
            break;

          // Child Attached Definition
          if (ActualChildAttachedDefinition?.TryGetNonDefaultValue(definitionProperty, out actualValue) == true)
            break;

          // Child Attached Definition Property
          if (_child.TryGetNonDefaultValue(definitionProperty, out actualValue))
            break;

          // Panel Explicit Definition
          if (_panelExplicitDefinition?.TryGetNonDefaultValue(definitionProperty, out actualValue) == true)
            break;

          // Panel Attached Definition
          if (ActualPanelAttachedDefinition?.TryGetNonDefaultValue(definitionProperty, out actualValue) == true)
            break;

          // Panel Attached Definition Property
          if (_panel.TryGetNonDefaultValue(childDefinitionProperty, out actualValue))
            break;

          actualValue = defaultValue;
          break;
        }

        store = actualValue;

        return actualValue;
      }

      private FlexOverflowBehavior OverflowBehavior => GetActualValue(ref _overflowBehavior, FlexDefinition.OverflowBehaviorProperty, FlexChildDefinition.OverflowBehaviorProperty, FlexElement.Default.OverflowBehavior);

      private FlexStretchDirection StretchDirection => GetActualValue(ref _stretchDirection, FlexDefinition.StretchDirectionProperty, FlexChildDefinition.StretchDirectionProperty, FlexElement.Default.StretchDirection);

      private short ExpandPriority => GetActualValue(ref _expandPriority, FlexDefinition.ExpandPriorityProperty, FlexChildDefinition.ExpandPriorityProperty, FlexElement.Default.ExpandPriority);

      private short ShrinkPriority => GetActualValue(ref _shrinkPriority, FlexDefinition.ShrinkPriorityProperty, FlexChildDefinition.ShrinkPriorityProperty, FlexElement.Default.ShrinkPriority);

      private double MaxLength => GetActualValue(ref _maxLength, FlexDefinition.MaxLengthProperty, FlexChildDefinition.MaxLengthProperty, FlexElement.Default.MaxLength);

      private double MinLength => GetActualValue(ref _minLength, FlexDefinition.MinLengthProperty, FlexChildDefinition.MinLengthProperty, FlexElement.Default.MinLength);

      private FlexLength Length => GetActualValue(ref _length, FlexDefinition.LengthProperty, FlexChildDefinition.LengthProperty, FlexElement.Default.Length);

      public FlexElement FlexElement => new FlexElement(MinLength, MaxLength, true)
      {
        Length = Length,
        OverflowBehavior = OverflowBehavior,
        StretchDirection = StretchDirection,
        ExpandPriority = ExpandPriority,
        ShrinkPriority = ShrinkPriority
      };
    }

    #endregion
  }
}