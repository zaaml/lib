// <copyright file="DelegateContentItemGeneratorImpl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.Core
{
	internal class DelegateContentItemGeneratorImpl<TItem, TGenerator> : DefaultContentItemGeneratorImpl<TItem, TGenerator>
		where TItem : FrameworkElement, IContentControl, new() where TGenerator : ItemGenerator<TItem>, IDelegatedGenerator<TItem>, new()
	{
		public DelegateContentItemGeneratorImpl(IContentItemsControl itemsControl)
			: base(itemsControl.ItemContentMember, itemsControl.ItemContentTemplate, itemsControl.ItemContentTemplateSelector, itemsControl.ItemContentStringFormat)
		{
			ItemsControlCore = itemsControl;
		}

		private protected IContentItemsControl ItemsControlCore { get; }

		public void OnItemContentMemberChanged()
		{
			ItemContentMember = ItemsControlCore.ItemContentMember;
		}

		public void OnItemContentStringFormatChanged()
		{
			ItemContentStringFormat = ItemsControlCore.ItemContentStringFormat;
		}

		public void OnItemContentTemplateChanged()
		{
			ItemContentTemplate = ItemsControlCore.ItemContentTemplate;
		}

		public void OnItemContentTemplateSelectorChanged()
		{
			ItemContentTemplateSelector = ItemsControlCore.ItemContentTemplateSelector;
		}
	}
}