// <copyright file="DefaultHeaderedContentItemGeneratorImpl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.Core
{
	internal class DefaultHeaderedContentItemGeneratorImpl<TItem, TGenerator> : DefaultContentItemGeneratorImpl<TItem, TGenerator> where TItem : FrameworkElement, IHeaderedContentControl, new() where TGenerator : ItemGenerator<TItem>, IDelegatedGenerator<TItem>, new()
	{
		#region Fields

		private string _itemHeaderStringFormat;
		private DataTemplate _itemHeaderTemplate;
		private DataTemplateSelector _itemHeaderTemplateSelector;

		#endregion

		#region Ctors

		public DefaultHeaderedContentItemGeneratorImpl(DataTemplate itemHeaderTemplate, DataTemplateSelector itemHeaderTemplateSelector, string itemHeaderStringFormat, DataTemplate itemContentTemplate, DataTemplateSelector itemContentTemplateSelector,
		                                               string itemContentStringFormat)
			: base(itemContentTemplate, itemContentTemplateSelector, itemContentStringFormat)
		{
			_itemHeaderTemplate = itemHeaderTemplate;
			_itemHeaderTemplateSelector = itemHeaderTemplateSelector;
			_itemHeaderStringFormat = itemHeaderStringFormat;
		}

		#endregion

		#region Properties

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

		#endregion

		#region  Methods

		public override void AttachItem(TItem item, object itemSource)
		{
			base.AttachItem(item, itemSource);

			item.Header = itemSource;
			item.HeaderTemplate = ItemHeaderTemplate;
			item.HeaderTemplateSelector = ItemHeaderTemplateSelector;
			item.HeaderStringFormat = ItemHeaderStringFormat;
		}

		public override void DetachItem(TItem item, object itemSource)
		{
			if (ReferenceEquals(item.Header, itemSource))
				item.ClearValue(item.HeaderProperty);

			if (ReferenceEquals(item.HeaderTemplate, ItemHeaderTemplate))
				item.ClearValue(item.HeaderTemplateProperty);

			if (ReferenceEquals(item.HeaderTemplateSelector, ItemHeaderTemplateSelector))
				item.ClearValue(item.HeaderTemplateSelectorProperty);

			if (ReferenceEquals(item.HeaderStringFormat, ItemHeaderStringFormat))
				item.ClearValue(item.HeaderStringFormatProperty);

			base.DetachItem(item, itemSource);
		}

		#endregion
	}
}