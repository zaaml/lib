// <copyright file="ItemsPresenterBase.Generic.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Windows;
using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Core
{
	public abstract class ItemsPresenterBase<TControl, TItem, TCollection, TPanel> : ItemsPresenterBase
		where TItem : FrameworkElement
		where TCollection : ItemCollectionBase<TControl, TItem>
		where TPanel : ItemsPanel<TItem>
		where TControl : System.Windows.Controls.Control
	{
		private TCollection _items;

		internal TCollection Items
		{
			get => _items;
			set
			{
				if (ReferenceEquals(_items, value))
					return;

				if (_items != null)
					_items.ItemsHost = null;

				_items = value;

				if (_items != null)
					_items.ItemsHost = ItemsHost;

				InvalidateMeasure();
			}
		}

		internal override IEnumerable ItemsCore
		{
			get => Items;
			set
			{
				if (ReferenceEquals(Items, value))
					return;

				Items = (TCollection)value;

				InvalidateMeasure();
			}
		}

		protected TPanel ItemsHost => TemplateContract.ItemsHost;

		internal TPanel ItemsHostInternal => ItemsHost;

		private ItemsPresenterBaseTemplateContract<TPanel, TItem> TemplateContract => (ItemsPresenterBaseTemplateContract<TPanel, TItem>)TemplateContractCore;

		protected override TemplateContract CreateTemplateContract()
		{
			var templateContract = base.CreateTemplateContract();

			if (templateContract is ItemsPresenterBaseTemplateContract<TPanel, TItem> == false)
				throw new InvalidOperationException("Invalid template contract. Must be derived from ItemsPresenterBaseTemplateContract<>");

			return templateContract;
		}

		protected override void OnItemsHostAttached()
		{
			if (Items != null)
				Items.ItemsHost = ItemsHost;

			base.OnItemsHostAttached();
		}

		protected override void OnItemsHostDetaching()
		{
			if (Items != null)
				Items.ItemsHost = null;

			base.OnItemsHostDetaching();
		}
	}

	public class ItemsPresenterBaseTemplateContract<TPanel, TItem> : ItemsPresenterBaseTemplateContract where TPanel : ItemsPanel<TItem>
		where TItem : FrameworkElement
	{
		[TemplateContractPart(Required = true)]
		public TPanel ItemsHost { get; [UsedImplicitly] private set; }

		protected override Panel ItemsHostBase => ItemsHost;
	}
}