// <copyright file="TemplatedGeneratorImplementation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;

namespace Zaaml.UI.Controls.Core
{
	internal class TemplatedGeneratorImplementation<TItem> : IItemGenerator<TItem> where TItem : FrameworkElement, new()
	{
		private readonly GeneratorDataTemplateHelper<TItem, TItem> _generatorDataTemplateHelper = new();
		private DataTemplate _itemTemplate;

		public TemplatedGeneratorImplementation(ItemGenerator<TItem> generator)
		{
			Generator = generator;
		}

		public ItemGenerator<TItem> Generator { get; }

		public DataTemplate ItemTemplate
		{
			get => _itemTemplate;
			set
			{
				if (ReferenceEquals(_itemTemplate, value))
					return;

				Generator.OnGeneratorChangingInt();

				_itemTemplate = value;
				_generatorDataTemplateHelper.DataTemplate = value;

				Generator.OnGeneratorChangedInt();
			}
		}

		public void AttachItem(TItem item, object source)
		{
			_generatorDataTemplateHelper.AttachDataContext(item, source);
		}

		public TItem CreateItem(object source)
		{
			var itemTemplate = ItemTemplate;

			if (itemTemplate == null)
				return new TItem();

			var item = _generatorDataTemplateHelper.Load(source);

			if (item == null)
				throw new InvalidOperationException();

			return item;
		}

		public void DetachItem(TItem item, object source)
		{
			if (ReferenceEquals(item.DataContext, source))
				item.ClearValue(FrameworkElement.DataContextProperty);
		}

		public void DisposeItem(TItem item, object source)
		{
		}
	}
}