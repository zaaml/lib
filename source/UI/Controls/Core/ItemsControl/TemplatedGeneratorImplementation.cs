// <copyright file="DefaultTemplatedGeneratorImpl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;

namespace Zaaml.UI.Controls.Core
{
	internal class TemplatedGeneratorImplementation<TItem> : IItemGenerator<TItem> where TItem : FrameworkElement, new()
	{
		#region Fields

		private readonly GeneratorDataTemplateHelper<TItem, TItem> _generatorDataTemplateHelper = new GeneratorDataTemplateHelper<TItem, TItem>();
		private DataTemplate _itemTemplate;

		#endregion

		#region Ctors

		public TemplatedGeneratorImplementation(ItemGenerator<TItem> generator)
		{
			Generator = generator;
		}

		#endregion

		#region Properties

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

		#endregion

		#region Interface Implementations

		#region IItemGenerator<TItem>

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

		#endregion

		#endregion
	}
}