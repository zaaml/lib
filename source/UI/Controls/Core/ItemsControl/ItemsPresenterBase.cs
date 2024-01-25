// <copyright file="ItemsPresenterBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Core
{
	[TemplateContractType(typeof(ItemsPresenterBaseTemplateContract))]
	public abstract class ItemsPresenterBase : TemplateContractControl
	{
		private Panel _itemsHost;

		internal ItemsPresenterBase()
		{
			IsTabStop = false;

#if !SILVERLIGHT
			Focusable = false;
#endif
		}

		internal bool IsVirtualizing => (ItemsHostPrivate as IVirtualItemsHost)?.IsVirtualizing ?? false;

		internal abstract IEnumerable ItemsCore { get; set; }

		internal Panel ItemsHostBaseInternal => ItemsHostPrivate;

		private Panel ItemsHostPrivate
		{
			get => _itemsHost;
			set
			{
				if (ReferenceEquals(_itemsHost, value))
					return;

				if (_itemsHost != null)
					OnItemsHostDetaching();

				_itemsHost = value;

				if (_itemsHost != null)
					OnItemsHostAttached();
			}
		}

		private ItemsPresenterBaseTemplateContract TemplateContract => (ItemsPresenterBaseTemplateContract) TemplateContractCore;

		protected virtual void OnItemsHostAttached()
		{
		}

		protected virtual void OnItemsHostDetaching()
		{
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			ItemsHostPrivate = TemplateContract.ItemsHostInternal;
		}

		protected override void OnTemplateContractDetaching()
		{
			ItemsHostPrivate = null;

			base.OnTemplateContractDetaching();
		}
	}
}