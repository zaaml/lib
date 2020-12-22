// <copyright file="SelectionItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Interfaces;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;

namespace Zaaml.UI.Controls.Core
{
	public class SelectionItem : IconContentPresenter, IContentControl
	{
		DependencyProperty IContentControl.ContentProperty => ContentProperty;

		DependencyProperty IContentControl.ContentStringFormatProperty => ContentStringFormatProperty;

		DependencyProperty IContentControl.ContentTemplateProperty => ContentTemplateProperty;

		DependencyProperty IContentControl.ContentTemplateSelectorProperty => ContentTemplateSelectorProperty;
	}

	[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
	public class SelectionItem<TItem> : SelectionItem
		where TItem : FrameworkElement
	{
		private static readonly DependencyPropertyKey SelectionPropertyKey = DPM.RegisterReadOnly<Selection<TItem>, SelectionItem<TItem>>
			("Selection", Selection<TItem>.Empty);

		public static readonly DependencyProperty SelectionProperty = SelectionPropertyKey.DependencyProperty;

		public Selection<TItem> Selection
		{
			get => (Selection<TItem>) GetValue(SelectionProperty);
			internal set => this.SetReadOnlyValue(SelectionPropertyKey, value);
		}
	}
}