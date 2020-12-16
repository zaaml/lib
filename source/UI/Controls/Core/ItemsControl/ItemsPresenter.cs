// <copyright file="ItemsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Panels.Core;
using NativeControl = System.Windows.Controls.Control;

namespace Zaaml.UI.Controls.Core
{
	[TemplateContractType(typeof(ItemsPresenterTemplateContract))]
	public sealed class ItemsPresenter : ItemsPresenterBase<ItemsControl, NativeControl, ItemCollection, ItemsPanel<NativeControl>>
	{
		#region Ctors

		static ItemsPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ItemsPresenter>();
		}

		public ItemsPresenter()
		{
			this.OverrideStyleKey<ItemsPresenter>();
		}

		#endregion
	
	}

	public sealed class ItemsPresenterTemplateContract : ItemsPresenterBaseTemplateContract<ItemsPanel<NativeControl>, NativeControl>
	{
	}
}