// <copyright file="MenuStripPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Windows;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Menu
{
	public sealed class MenuStripPresenter : FixedTemplateControl<MenuItemsPresenterHost>
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty MenuStripProperty = DPM.Register<MenuStrip, MenuStripPresenter>
			("MenuStrip", s => s.OnMenuStripChanged);

		#endregion

		#region Fields

		private MenuItemBase _owner;

		#endregion

		#region Properties

		private bool IsAttached { get; set; }

#if !SILVERLIGHT
		protected override IEnumerator LogicalChildren => MenuStrip != null ? EnumeratorUtils.Concat(MenuStrip, base.LogicalChildren) : base.LogicalChildren;
#endif

		public MenuStrip MenuStrip
		{
			get => (MenuStrip) GetValue(MenuStripProperty);
			set => SetValue(MenuStripProperty, value);
		}

		internal MenuItemBase Owner
		{
			get => _owner;
			set
			{
				if (ReferenceEquals(_owner, value))
					return;

				_owner = value;

				if (MenuStrip != null)
					MenuStrip.Owner = _owner;
			}
		}

		#endregion

		#region  Methods

		protected override Size ArrangeOverride(Size finalSize)
		{
			MountMenuStrip();

			return base.ArrangeOverride(finalSize);
		}

		private void InvalidateInt()
		{
			InvalidateMeasure();
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			MountMenuStrip();

			return base.MeasureOverride(availableSize);
		}

		private void MountMenuStrip()
		{
			if (MenuStrip == null) return;

			MenuStrip.Presenter = this;
			MenuStrip.Owner = Owner;
		}

		internal void OnMenuStripAttached(MenuStrip menuStrip)
		{
			IsAttached = true;

			AddLogicalChild(menuStrip);

			menuStrip.ApplyTemplate();

			TemplateRoot.ItemsPresenter = menuStrip.ItemsPresenterInt;

			InvalidateMeasure();
		}

		private void OnMenuStripChanged(MenuStrip oldMenuStrip, MenuStrip newMenuStrip)
		{
			if (IsAttached)
				MenuStrip.Presenter = null;

			InvalidateInt();
		}

		internal void OnMenuStripDetached(MenuStrip menuStrip)
		{
			IsAttached = false;

			RemoveLogicalChild(menuStrip);

			TemplateRoot.ItemsPresenter = null;

			InvalidateMeasure();
		}

		#endregion
	}
}