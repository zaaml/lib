// <copyright file="SelectionItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Interfaces;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;

namespace Zaaml.UI.Controls.Core
{
	public abstract class SelectionItem : IconContentPresenter, IContentControl
	{
		private readonly TextBlock _separatorTextBlock = new TextBlock {Text = ";"};

		protected SelectionItem()
		{
			Margin = new Thickness(0, 0, 2, 0);
		}

		internal ItemsControlBase ItemsControl { get; set; }

		private protected override Dock? GetDockCore(UIElement element)
		{
			return ReferenceEquals(element, _separatorTextBlock) ? Dock.Right : base.GetDockCore(element);
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			Children.Remove(_separatorTextBlock);
			Children.Add(_separatorTextBlock);

			_separatorTextBlock.Visibility = IsLast ? Visibility.Collapsed : Visibility.Visible;

			return base.MeasureOverrideCore(availableSize);
		}

		DependencyProperty IContentControl.ContentProperty => ContentProperty;

		DependencyProperty IContentControl.ContentStringFormatProperty => ContentStringFormatProperty;

		DependencyProperty IContentControl.ContentTemplateProperty => ContentTemplateProperty;

		DependencyProperty IContentControl.ContentTemplateSelectorProperty => ContentTemplateSelectorProperty;

		private protected abstract bool IsLast { get; }
	}

	[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
	public class SelectionItem<TItem> : SelectionItem
		where TItem : FrameworkElement
	{
		private static readonly DependencyPropertyKey SelectionPropertyKey = DPM.RegisterReadOnly<Selection<TItem>, SelectionItem<TItem>>
			("Selection", Selection<TItem>.Empty, s => s.OnSelectionChanged);

		public static readonly DependencyProperty SelectionProperty = SelectionPropertyKey.DependencyProperty;

		public Selection<TItem> Selection
		{
			get => (Selection<TItem>) GetValue(SelectionProperty);
			internal set => this.SetReadOnlyValue(SelectionPropertyKey, value);
		}

		protected virtual void OnSelectionChanged(Selection<TItem> oldSelection, Selection<TItem> newSelection)
		{
		}

		private protected override bool IsLast => false;
	}
}