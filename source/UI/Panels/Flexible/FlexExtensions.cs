// <copyright file="FlexExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore.Extensions;
using Panel = Zaaml.UI.Panels.Core.Panel;

namespace Zaaml.UI.Panels.Flexible
{
	public static class FlexExtensions
	{
		public static FlexElement GetFlexElement(this UIElement child, Panel panel, Orientation orientation, FlexDefinition panelChildDefinition = null)
		{
			// ReSharper disable once SuspiciousTypeConversion.Global

			if (child is IFlexElementProvider flexElementProvider)
				return flexElementProvider.GetFlexElement();

			return new FlexElementComposition(child, orientation, null, panel, panelChildDefinition).FlexElement;
		}

		private struct FlexElementComposition
		{
			private static readonly FlexDefinition DefaultDefinition = new();

			private readonly UIElement _child;
			private readonly Orientation _orientation;
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

			public FlexElementComposition(UIElement child, Orientation orientation, FlexDefinition childExplicitDefinition, Panel panel, FlexDefinition panelExplicitDefinition) : this()
			{
				_child = child;
				_orientation = orientation;
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

				TValue actualValue = default;

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

			private void CoerceMinMaxLength(ref double minLength, ref double maxLength, ref FlexLength length)
			{
				if (_child is not FrameworkElement fre)
					return;

				var minSize = fre.GetMinSize().AsOriented(_orientation).Direct;
				var maxSize = fre.GetMaxSize().AsOriented(_orientation).Direct;

				if (minLength < minSize)
					minLength = minSize;

				if (maxLength > maxSize)
					maxLength = maxSize;

				if (maxLength < minLength)
					maxLength = minLength;

				if (length.IsAbsolute)
					length = new FlexLength(length.Value.Clamp(minLength, maxLength), FlexLengthUnitType.Pixel);
			}

			public FlexElement FlexElement
			{
				get
				{
					var minLength = MinLength;
					var maxLength = MaxLength;
					var length = Length;

					CoerceMinMaxLength(ref minLength, ref maxLength, ref length);

					return new FlexElement(minLength, maxLength, true)
					{
						Length = length,
						OverflowBehavior = OverflowBehavior,
						StretchDirection = StretchDirection,
						ExpandPriority = ExpandPriority,
						ShrinkPriority = ShrinkPriority
					};
				}
			}
		}
	}
}