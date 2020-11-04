// <copyright file="DefaultContentItemGeneratorImpl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.Core
{
	internal class DefaultContentItemGeneratorImpl<TItem, TGenerator> : DefaultGeneratorImpl<TItem, TGenerator> where TItem : FrameworkElement, IContentControl, new() where TGenerator : ItemGenerator<TItem>, IDelegatedGenerator<TItem>, new()
	{
		#region Fields

		private string _itemContentStringFormat;
		private DataTemplate _itemContentTemplate;
		private DataTemplateSelector _itemContentTemplateSelector;

		#endregion

		#region Ctors

		public DefaultContentItemGeneratorImpl(DataTemplate itemContentTemplate, DataTemplateSelector itemContentTemplateSelector, string itemContentStringFormat)
		{
			_itemContentTemplate = itemContentTemplate;
			_itemContentTemplateSelector = itemContentTemplateSelector;
			_itemContentStringFormat = itemContentStringFormat;
		}

		#endregion

		#region Properties

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

		#endregion

		#region  Methods

		public override void AttachItem(TItem item, object itemSource)
		{
			item.DataContext = itemSource;
			item.Content = itemSource;
			item.ContentTemplate = ItemContentTemplate;
			item.ContentTemplateSelector = ItemContentTemplateSelector;
			item.ContentStringFormat = ItemContentStringFormat;
		}

		public override TItem CreateItem(object itemSource)
		{
			return new TItem();
		}

		public override void DetachItem(TItem item, object itemSource)
		{
			if (ReferenceEquals(item.DataContext, itemSource))
				item.ClearValue(FrameworkElement.DataContextProperty);

			if (ReferenceEquals(item.Content, itemSource))
				item.ClearValue(item.ContentProperty);

			if (ReferenceEquals(item.ContentTemplate, ItemContentTemplate))
				item.ClearValue(item.ContentTemplateProperty);

			if (ReferenceEquals(item.ContentTemplateSelector, ItemContentTemplateSelector))
				item.ClearValue(item.ContentTemplateSelectorProperty);

			if (ReferenceEquals(item.ContentStringFormat, ItemContentStringFormat))
				item.ClearValue(item.ContentStringFormatProperty);
		}

		public override void DisposeItem(TItem item, object itemSource)
		{
		}

		#endregion
	}
}