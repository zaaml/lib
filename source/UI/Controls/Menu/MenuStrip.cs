// <copyright file="MenuStrip.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Menu
{
	[TemplateContractType(typeof(MenuStripTemplateContract))]
	public class MenuStrip : MenuBase
	{
		#region Fields

		private FrameworkElement _controlRoot;
		private MenuStripPresenter _presenter;

		#endregion

		#region Ctors

		static MenuStrip()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<MenuStrip>();
		}

		public MenuStrip()
		{
			this.OverrideStyleKey<MenuStrip>();
		}

		#endregion

		#region Properties

		private FrameworkElement ControlRoot
		{
			set
			{
				if (ReferenceEquals(_controlRoot, value))
					return;

				_controlRoot = value;

				var host = Presenter;

				Presenter = null;

				Presenter = host;
			}
		}

		internal override bool IsOpenCore { get; set; }

		internal MenuItemsPresenter ItemsPresenterInt => ItemsPresenter;

		protected override Orientation Orientation => Orientation.Vertical;

		internal IMenuItemOwner Owner { get; set; }

		internal MenuStripPresenter Presenter
		{
			get => _presenter;
			set
			{
				if (ReferenceEquals(_presenter, value))
					return;

				_presenter?.OnMenuStripDetached(this);

				_presenter = value;

				_presenter?.OnMenuStripAttached(this);
			}
		}

		#endregion

		#region  Methods

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			ControlRoot = this.GetTemplateRoot();
		}

		protected override void OnTemplateContractDetaching()
		{
			ControlRoot = null;

			base.OnTemplateContractDetaching();
		}

		#endregion
	}

	public class MenuStripTemplateContract : MenuBaseTemplateContract
	{
	}
}