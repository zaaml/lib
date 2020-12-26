// <copyright file="DefaultContentItemGeneratorImpl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.Core
{
	internal class DefaultContentItemGeneratorImplementation<TItem, TGenerator> : DefaultGeneratorImplementation<TItem, TGenerator>
		where TItem : FrameworkElement, IContentControl, new()
		where TGenerator : ItemGenerator<TItem>, IDelegatedGenerator<TItem>, new()
	{
		private string _itemContentMember;
		private string _itemContentStringFormat;
		private DataTemplate _itemContentTemplate;
		private DataTemplateSelector _itemContentTemplateSelector;

		public DefaultContentItemGeneratorImplementation(string itemContentMember, DataTemplate itemContentTemplate, DataTemplateSelector itemContentTemplateSelector, string itemContentStringFormat)
		{
			_itemContentMember = itemContentMember;
			_itemContentTemplate = itemContentTemplate;
			_itemContentTemplateSelector = itemContentTemplateSelector;
			_itemContentStringFormat = itemContentStringFormat;

			CreateItemContentMemberBinding();
		}

		protected virtual bool HandleContentCore => true;

		protected virtual bool HandleContentStringFormatCore => true;

		protected virtual bool HandleContentTemplateCore => true;

		protected virtual bool HandleContentTemplateSelectorCore => true;

		protected virtual bool HandleDataContextCore => true;

		public string ItemContentMember
		{
			get => _itemContentMember;
			set
			{
				if (string.Equals(_itemContentMember, value, StringComparison.OrdinalIgnoreCase))
					return;

				Generator.OnGeneratorChangingInt();

				_itemContentMember = value;
				CreateItemContentMemberBinding();

				Generator.OnGeneratorChangedInt();
			}
		}

		private protected Binding ItemContentMemberBinding { get; private set; }

		internal Binding ItemContentMemberBindingInternal => ItemContentMemberBinding;

		public string ItemContentStringFormat
		{
			get => _itemContentStringFormat;
			set
			{
				if (ReferenceEquals(_itemContentStringFormat, value))
					return;

				Generator.OnGeneratorChangingInt();

				_itemContentStringFormat = value;

				Generator.OnGeneratorChangedInt();
			}
		}

		public DataTemplate ItemContentTemplate
		{
			get => _itemContentTemplate;
			set
			{
				if (ReferenceEquals(_itemContentTemplate, value))
					return;

				Generator.OnGeneratorChangingInt();

				_itemContentTemplate = value;

				Generator.OnGeneratorChangedInt();
			}
		}

		public DataTemplateSelector ItemContentTemplateSelector
		{
			get => _itemContentTemplateSelector;
			set
			{
				if (ReferenceEquals(_itemContentTemplateSelector, value))
					return;

				Generator.OnGeneratorChangingInt();

				_itemContentTemplateSelector = value;

				Generator.OnGeneratorChangedInt();
			}
		}

		public override void AttachItem(TItem item, object source)
		{
			if (HandleDataContextCore)
				item.DataContext = source;

			if (HandleContentCore)
			{
				if (ItemContentMemberBinding == null)
					item.Content = source;
				else
					ItemGenerator<TItem>.InstallBinding(item, item.ContentProperty, ItemContentMemberBinding);
			}

			if (HandleContentTemplateCore)
				item.ContentTemplate = ItemContentTemplate;

			if (HandleContentTemplateSelectorCore)
				item.ContentTemplateSelector = ItemContentTemplateSelector;

			if (HandleContentStringFormatCore)
				item.ContentStringFormat = ItemContentStringFormat;
		}

		public override TItem CreateItem(object source)
		{
			return new TItem();
		}

		private void CreateItemContentMemberBinding()
		{
			ItemContentMemberBinding = _itemContentMember != null ? new Binding(_itemContentMember) : null;
		}

		public override void DetachItem(TItem item, object source)
		{
			if (HandleDataContextCore && ReferenceEquals(item.DataContext, source))
				item.ClearValue(FrameworkElement.DataContextProperty);

			if (HandleContentCore)
			{
				if (ReferenceEquals(item.Content, source))
					item.ClearValue(item.ContentProperty);
				else
					ItemGenerator<TItem>.UninstallBinding(item, item.ContentProperty, ItemContentMemberBinding);
			}

			if (HandleContentTemplateCore && ReferenceEquals(item.ContentTemplate, ItemContentTemplate))
				item.ClearValue(item.ContentTemplateProperty);

			if (HandleContentTemplateSelectorCore && ReferenceEquals(item.ContentTemplateSelector, ItemContentTemplateSelector))
				item.ClearValue(item.ContentTemplateSelectorProperty);

			if (HandleContentStringFormatCore && ReferenceEquals(item.ContentStringFormat, ItemContentStringFormat))
				item.ClearValue(item.ContentStringFormatProperty);
		}

		public override void DisposeItem(TItem item, object source)
		{
		}
	}
}