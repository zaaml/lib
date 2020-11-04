// <copyright file="ItemsControlExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;

namespace Zaaml.UI.Controls.Core
{
	internal static class ItemsControlExtensions
	{
		#region  Methods

		public static void ApplyItemStyle<TItem>(this TItem frameworkElement, DependencyProperty styleProperty, Style style) where TItem : FrameworkElement
		{
			if (style != null)
				frameworkElement.SetValue(styleProperty, style);
		}

		public static void ChangeItemStyle<TItem>(this TItem item, DependencyProperty styleProperty, Style oldStyle, Style newStyle) where TItem : FrameworkElement
		{
			var elementStyle = item.GetValue(styleProperty);

			if (oldStyle != null && ReferenceEquals(elementStyle, oldStyle) == false)
				return;

			if (newStyle != null)
				item.SetValue(styleProperty, newStyle);
			else
				item.ClearValue(styleProperty);
		}

		public static void ChangeStyle<TItem>(this IItemsControl<TItem> itemsControl, DependencyProperty styleProperty, Style oldStyle, Style newStyle) where TItem : FrameworkElement
		{
			if (newStyle != null)
				VerifyStyleType(newStyle, typeof(TItem));

			if (oldStyle != null)
			{
				foreach (var frameworkElement in itemsControl.ActualItems)
					frameworkElement.ChangeItemStyle(styleProperty, oldStyle, newStyle);
			}
			else if (newStyle != null)
			{
				foreach (var frameworkElement in itemsControl.ActualItems)
					frameworkElement.ApplyItemStyle(styleProperty, newStyle);
			}
		}

		private static bool CheckStyleTargetType(Style style, Type expected)
		{
			return !(style?.TargetType != null && expected.IsAssignableFrom(style.TargetType) == false);
		}

		public static void UndoItemStyle<TItem>(this TItem item, DependencyProperty styleProperty, Style style) where TItem : FrameworkElement
		{
			var elementStyle = item.GetValue(styleProperty);

			if (ReferenceEquals(elementStyle, style) == false)
				return;

			item.ClearValue(styleProperty);
		}

		private static void VerifyStyleType(Style style, Type itemType)
		{
			if (CheckStyleTargetType(style, itemType) == false)
				throw new InvalidOperationException($"Style target type should be of type {itemType.Name} or derived from it");
		}

		#endregion
	}
}