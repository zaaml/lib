// <copyright file="DefaultHeaderedContentItemGeneratorImpl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Zaaml.UI.Controls.Interfaces;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;

namespace Zaaml.UI.Controls.Core
{
	internal class DefaultHeaderedIconContentItemGeneratorImplementation<TItem, TGenerator> : DefaultIconContentItemGeneratorImplementation<TItem, TGenerator>
		where TItem : FrameworkElement, IHeaderedIconContentControl, new() where TGenerator : ItemGenerator<TItem>, IDelegatedGenerator<TItem>, new()
	{
		private string _itemHeaderMember;

		private string _itemHeaderStringFormat;
		private DataTemplate _itemHeaderTemplate;
		private DataTemplateSelector _itemHeaderTemplateSelector;

		public DefaultHeaderedIconContentItemGeneratorImplementation(
			string itemHeaderMember, 
			DataTemplate itemHeaderTemplate, 
			DataTemplateSelector itemHeaderTemplateSelector,
			string itemHeaderStringFormat, 
			string itemIconMember, 
			IIconSelector itemIconSelector,
			string itemContentMember,
			DataTemplate itemContentTemplate,
			DataTemplateSelector itemContentTemplateSelector,
			string itemContentStringFormat)
			: base(
				itemIconMember,
				itemIconSelector,
				itemContentMember, 
				itemContentTemplate, 
				itemContentTemplateSelector, 
				itemContentStringFormat)
		{
			_itemHeaderMember = itemHeaderMember;
			_itemHeaderTemplate = itemHeaderTemplate;
			_itemHeaderTemplateSelector = itemHeaderTemplateSelector;
			_itemHeaderStringFormat = itemHeaderStringFormat;

			CreateItemContentMemberBinding();
		}

		public string ItemHeaderMember
		{
			get => _itemHeaderMember;
			set
			{
				if (string.Equals(_itemHeaderMember, value, StringComparison.OrdinalIgnoreCase))
					return;

				Generator.OnGeneratorChangingInt();

				_itemHeaderMember = value;

				CreateItemContentMemberBinding();

				Generator.OnGeneratorChangedInt();
			}
		}

		private Binding ItemHeaderMemberBinding { get; set; }

		public string ItemHeaderStringFormat
		{
			get => _itemHeaderStringFormat;
			set
			{
				if (ReferenceEquals(_itemHeaderStringFormat, value))
					return;

				Generator.OnGeneratorChangingInt();

				_itemHeaderStringFormat = value;

				Generator.OnGeneratorChangedInt();
			}
		}

		public DataTemplate ItemHeaderTemplate
		{
			get => _itemHeaderTemplate;
			set
			{
				if (ReferenceEquals(_itemHeaderTemplate, value))
					return;

				Generator.OnGeneratorChangingInt();

				_itemHeaderTemplate = value;

				Generator.OnGeneratorChangedInt();
			}
		}

		public DataTemplateSelector ItemHeaderTemplateSelector
		{
			get => _itemHeaderTemplateSelector;
			set
			{
				if (ReferenceEquals(_itemHeaderTemplateSelector, value))
					return;

				Generator.OnGeneratorChangingInt();

				_itemHeaderTemplateSelector = value;

				Generator.OnGeneratorChangedInt();
			}
		}

		public override void AttachItem(TItem item, object source)
		{
			base.AttachItem(item, source);

			if (ItemHeaderMember == null)
				item.Header = source;
			else
				ItemGenerator<TItem>.InstallBinding(item, item.HeaderProperty, ItemHeaderMemberBinding);

			item.HeaderTemplate = ItemHeaderTemplate;
			item.HeaderTemplateSelector = ItemHeaderTemplateSelector;
			item.HeaderStringFormat = ItemHeaderStringFormat;
		}

		private void CreateItemContentMemberBinding()
		{
			ItemHeaderMemberBinding = _itemHeaderMember != null ? new Binding(_itemHeaderMember) : null;
		}

		public override void DetachItem(TItem item, object source)
		{
			if (ReferenceEquals(item.Header, source))
				item.ClearValue(item.HeaderProperty);
			else
				ItemGenerator<TItem>.UninstallBinding(item, item.HeaderProperty, ItemHeaderMemberBinding);

			if (ReferenceEquals(item.HeaderTemplate, ItemHeaderTemplate))
				item.ClearValue(item.HeaderTemplateProperty);

			if (ReferenceEquals(item.HeaderTemplateSelector, ItemHeaderTemplateSelector))
				item.ClearValue(item.HeaderTemplateSelectorProperty);

			if (ReferenceEquals(item.HeaderStringFormat, ItemHeaderStringFormat))
				item.ClearValue(item.HeaderStringFormatProperty);

			base.DetachItem(item, source);
		}
	}
}