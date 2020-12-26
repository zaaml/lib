// <copyright file="DelegateIconContentItemGeneratorImpl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.Core
{
	internal class DelegateIconContentItemGeneratorImplementation<TItem, TGenerator> : DefaultIconContentItemGeneratorImplementation<TItem, TGenerator>
		where TItem : FrameworkElement, IIconContentControl, new() where TGenerator : ItemGenerator<TItem>, IDelegatedGenerator<TItem>, new()
	{
		public DelegateIconContentItemGeneratorImplementation(IIconContentItemsControl itemsControl)
			: base(itemsControl.ItemIconMember, itemsControl.ItemContentMember, itemsControl.ItemContentTemplate, itemsControl.ItemContentTemplateSelector, itemsControl.ItemContentStringFormat)
		{
			ItemsControlCore = itemsControl;
		}

		private protected IIconContentItemsControl ItemsControlCore { get; }

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

		public void OnItemIconMemberChanged()
		{
			ItemIconMember = ItemsControlCore.ItemIconMember;
		}
	}
}