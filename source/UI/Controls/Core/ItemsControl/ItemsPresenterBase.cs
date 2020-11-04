// <copyright file="ItemsPresenterBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.ScrollView;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Core
{
	[TemplateContractType(typeof(ItemsPresenterBaseTemplateContract))]
	public abstract class ItemsPresenterBase : TemplateContractControl, IDelegateScrollViewPanel
	{
		#region Fields

		private Panel _itemsHost;
		private ScrollViewControl _scrollView;

		#endregion

		#region Ctors

		internal ItemsPresenterBase()
		{
			IsTabStop = false;

#if !SILVERLIGHT
			Focusable = false;
#endif
		}

		#endregion

		#region Properties

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

		internal ScrollViewControl ScrollView
		{
			get => _scrollView;
			set
			{
				if (ReferenceEquals(_scrollView, value))
					return;

				var oldScrollView = _scrollView;

				_scrollView = value;

				OnScrollViewChanged(oldScrollView, value);
			}
		}

		private protected abstract void OnScrollViewChanged(ScrollViewControl oldScrollView, ScrollViewControl newScrollView);

		private ItemsPresenterBaseTemplateContract TemplateContract => (ItemsPresenterBaseTemplateContract) TemplateContractInternal;

		#endregion

		#region  Methods

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

		#endregion

		#region Interface Implementations

		#region IDelegateScrollViewPanel

		IScrollViewPanel IDelegateScrollViewPanel.ScrollViewPanel => ItemsHostPrivate as IScrollViewPanel;

		#endregion

		#endregion
	}
}