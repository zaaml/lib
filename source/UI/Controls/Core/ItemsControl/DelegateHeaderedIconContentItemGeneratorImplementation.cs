// <copyright file="DelegateHeaderedContentItemGeneratorImpl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.Core
{
	internal class DelegateHeaderedIconContentItemGeneratorImplementation<TItem, TGenerator> : DefaultHeaderedIconContentItemGeneratorImplementation<TItem, TGenerator>
		where TItem : FrameworkElement, IHeaderedIconContentControl, new()
		where TGenerator : ItemGenerator<TItem>, IDelegatedGenerator<TItem>, new()
	{
		public DelegateHeaderedIconContentItemGeneratorImplementation(IHeaderedIconContentItemsControl headeredContentControl)
			: base(headeredContentControl.ItemHeaderMember, headeredContentControl.ItemHeaderTemplate, headeredContentControl.ItemHeaderTemplateSelector, headeredContentControl.ItemHeaderStringFormat, headeredContentControl.ItemIconMember,
				headeredContentControl.ItemContentMember, headeredContentControl.ItemContentTemplate, headeredContentControl.ItemContentTemplateSelector, headeredContentControl.ItemContentStringFormat)
		{
			ItemsControlCore = headeredContentControl;
		}

		private protected IHeaderedIconContentItemsControl ItemsControlCore { get; }

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

		public void OnItemHeaderMemberChanged()
		{
			ItemHeaderMember = ItemsControlCore.ItemHeaderMember;
		}

		public void OnItemHeaderStringFormatChanged()
		{
			ItemHeaderStringFormat = ItemsControlCore.ItemHeaderStringFormat;
		}

		public void OnItemHeaderTemplateChanged()
		{
			ItemHeaderTemplate = ItemsControlCore.ItemHeaderTemplate;
		}

		public void OnItemHeaderTemplateSelectorChanged()
		{
			ItemHeaderTemplateSelector = ItemsControlCore.ItemHeaderTemplateSelector;
		}

		public void OnItemIconMemberChanged()
		{
			ItemIconMember = ItemsControlCore.ItemIconMember;
		}
	}
}