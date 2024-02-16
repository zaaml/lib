// <copyright file="MenuBar.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;

namespace Zaaml.UI.Controls.Menu
{
	[TemplateContractType(typeof(MenuBarTemplateContract))]
	public class MenuBar : MenuBase, IManagedPopupControl
	{
		private static readonly DependencyProperty PlacementProperty = DPM.Register<PopupPlacement, MenuBar>
			("Placement", c => c.Controller.OnPlacementChanged);

		private static readonly DependencyProperty IsOpenProperty = DPM.Register<bool, MenuBar>
			("IsOpen", c => c.Controller.OnIsOpenChanged);

		private static readonly DependencyPropertyKey OwnerPropertyKey = DPM.RegisterReadOnly<FrameworkElement, MenuBar>
			("Owner", c => c.Controller.OnOwnerChanged);

		static MenuBar()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<MenuBar>();
		}

		public MenuBar()
		{
			this.OverrideStyleKey<MenuBar>();

			Controller = new PopupControlController<MenuBar>(this)
			{
				IsModalMenu = true,
				CloseOnLostKeyboardFocus = true
			};
		}

		internal PopupControlController<MenuBar> Controller { get; }

		private bool IsOpen
		{
			get => (bool)GetValue(IsOpenProperty);
			set => SetValue(IsOpenProperty, value.Box());
		}

		internal override bool IsOpenCore
		{
			get => IsOpen;
			set => IsOpen = value;
		}

		protected override IEnumerator LogicalChildren
		{
			get { yield return Controller.Popup; }
		}

		protected override Orientation Orientation => Orientation.Horizontal;

		internal FrameworkElement Owner
		{
			get => this.GetReadOnlyValue<FrameworkElement>(OwnerPropertyKey);
			set => this.SetReadOnlyValue(OwnerPropertyKey, value);
		}

		private Popup Popup => TemplateContract.Popup;

		internal override PopupControlController PopupController => Controller;

		private MenuBarTemplateContract TemplateContract => (MenuBarTemplateContract)TemplateContractCore;

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			Controller.Popup = Popup;
		}

		protected override void OnTemplateContractDetaching()
		{
			Controller.Popup = null;

			base.OnTemplateContractDetaching();
		}

		DependencyProperty IManagedPopupControl.IsOpenProperty => IsOpenProperty;

		DependencyPropertyKey IManagedPopupControl.OwnerPropertyKey => OwnerPropertyKey;

		DependencyProperty IManagedPopupControl.PlacementProperty => PlacementProperty;

		void IManagedPopupControl.OnClosed()
		{
			MenuController.OnClosed();
		}

		void IManagedPopupControl.OnClosing(PopupCancelEventArgs cancelEventArgs)
		{
			MenuController.OnClosing();
		}

		void IManagedPopupControl.OnIsOpenChanged()
		{
		}

		void IManagedPopupControl.OnOpened()
		{
			MenuController.OnOpened();
		}

		void IManagedPopupControl.OnOpening(PopupCancelEventArgs cancelEventArgs)
		{
			MenuController.OnOpening();
		}

		void IManagedPopupControl.OnOwnerChanged(FrameworkElement oldOwner, FrameworkElement newOwner)
		{
		}

		void IManagedPopupControl.OnPlacementChanged(PopupPlacement oldPlacement, PopupPlacement newPlacement)
		{
		}
	}

	public class MenuBarTemplateContract : MenuBaseTemplateContract
	{
		[TemplateContractPart] public Popup Popup { get; [UsedImplicitly] private set; }
	}
}