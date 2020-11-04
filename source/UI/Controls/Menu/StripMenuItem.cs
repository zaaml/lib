// <copyright file="StripMenuItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Menu
{
	public sealed class StripMenuItem : HeaderedMenuItem
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty MenuStripProperty = DPM.Register<MenuStrip, StripMenuItem>
			("MenuStrip", s => s.OnMenuStripChanged);

		#endregion

		#region Ctors

		static StripMenuItem()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<StripMenuItem>();
		}

		public StripMenuItem()
		{
			this.OverrideStyleKey<StripMenuItem>();

			MenuStripPresenter = new MenuStripPresenter { Owner = this };
		}

		#endregion

		#region Properties

		public MenuStrip MenuStrip
		{
			get => (MenuStrip) GetValue(MenuStripProperty);
			set => SetValue(MenuStripProperty, value);
		}

		private MenuStripPresenter MenuStripPresenter { get; }

		internal override FrameworkElement Submenu => SubmenuElement;

		protected override FrameworkElement SubmenuElement => MenuStripPresenter;

		#endregion

		#region  Methods

		private void OnMenuStripChanged()
		{
			var menuStrip = MenuStrip;
			MenuStripPresenter.MenuStrip = menuStrip;
			HasSubmenu = menuStrip != null;
		}

		#endregion
	}
}