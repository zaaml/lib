// <copyright file="MenuItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Zaaml.Core;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Interfaces;
using Zaaml.UI.Controls.Primitives;
using Style = System.Windows.Style;

#pragma warning disable 109

namespace Zaaml.UI.Controls.Menu
{
	[ContentProperty(nameof(ItemCollection))]
	public partial class MenuItem : HeaderedMenuItem, IButton
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty CommandProperty = DPM.Register<ICommand, MenuItem>
			(nameof(Command), m => m.OnCommandChanged);

		public static readonly DependencyProperty CommandParameterProperty = DPM.Register<object, MenuItem>
			(nameof(CommandParameter), m => m.OnCommandParameterChanged);

		public static readonly DependencyProperty CommandTargetProperty = DPM.Register<DependencyObject, MenuItem>
			(nameof(CommandTarget), m => m.OnCommandTargetChanged);

		public static readonly DependencyProperty ItemContainerStyleProperty = DPM.Register<Style, MenuItem>
			(nameof(ItemContainerStyle));

		private static readonly DependencyPropertyKey ItemCollectionPropertyKey = DPM.RegisterReadOnly<MenuItemCollection, MenuItem>
			("ItemCollectionPrivate");

		public static readonly DependencyProperty ItemCollectionProperty = ItemCollectionPropertyKey.DependencyProperty;

		public static readonly DependencyProperty SourceCollectionProperty = DPM.Register<IEnumerable, MenuItem>
			(nameof(SourceCollection), m => m.OnSourceCollectionPropertyChangedPrivate);

		public static readonly DependencyProperty ItemGeneratorProperty = DPM.Register<MenuItemGeneratorBase, MenuItem>
			(nameof(ItemGenerator), m => m.OnItemGeneratorChanged);

		public static readonly DependencyProperty StaysOpenOnClickProperty = DPM.Register<bool, MenuItem>
			(nameof(StaysOpenOnClick), false);

		public static readonly DependencyProperty SubmenuHeaderProperty = DPM.Register<string, MenuItem>
			(nameof(SubmenuHeader));

		#endregion

		#region Fields

		[UsedImplicitly] private readonly CommandController<MenuItem> _commandController;

		#endregion

		#region Ctors

		static MenuItem()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<MenuItem>();
		}

		public MenuItem()
		{
			this.OverrideStyleKey<MenuItem>();

			ItemCollection = new MenuItemCollection(this);

			MenuItemsPresenter = new MenuItemsPresenter
			{
				Items = ItemCollection,
				ActualOrientation = Orientation.Vertical
			};

			_commandController = new CommandController<MenuItem>(this);
		}

		#endregion

		#region Properties

		private MenuItemGeneratorBase ActualGenerator => ItemGenerator ?? ParentGenerator;

		private bool IsItem => ItemCollection.Count == 0;

		public Style ItemContainerStyle
		{
			get => (Style) GetValue(ItemContainerStyleProperty);
			set => SetValue(ItemContainerStyleProperty, value);
		}

		public MenuItemGeneratorBase ItemGenerator
		{
			get => (MenuItemGeneratorBase) GetValue(ItemGeneratorProperty);
			set => SetValue(ItemGeneratorProperty, value);
		}

		public MenuItemCollection ItemCollection
		{
			get => (MenuItemCollection) GetValue(ItemCollectionProperty);
			private set => this.SetReadOnlyValue(ItemCollectionPropertyKey, value);
		}

		internal override IMenuItemCollection ItemsCore => ItemCollection;

		public IEnumerable SourceCollection
		{
			get => (IEnumerable) GetValue(SourceCollectionProperty);
			set => SetValue(SourceCollectionProperty, value);
		}

		private MenuItemsPresenter MenuItemsPresenter { get; }

		public bool StaysOpenOnClick
		{
			get => (bool) GetValue(StaysOpenOnClickProperty);
			set => SetValue(StaysOpenOnClickProperty, value);
		}

		internal override bool StaysOpenOnClickInternal => StaysOpenOnClick;

		internal override FrameworkElement Submenu => null;

		protected override FrameworkElement SubmenuElement => MenuItemsPresenter;

		public string SubmenuHeader
		{
			get => (string) GetValue(SubmenuHeaderProperty);
			set => SetValue(SubmenuHeaderProperty, value);
		}

		#endregion

		#region  Methods

		protected internal override bool ExecuteCommand()
		{
			base.ExecuteCommand();

			if (IsItem == false)
				return false;

      OnClickPrivate();

      return true;
		}

		protected virtual void OnClick()
		{
		}

		private void OnClickPrivate()
		{
			MenuBase?.MenuController.OnMenuItemClick(this);

			RaiseOnClick();

			_commandController.InvokeCommand();

      OnClick();
    }

		private void OnCommandChanged(ICommand oldCommand, ICommand newCommand)
		{
			_commandController.OnCommandChanged();

			CommandChanged?.Invoke(this, EventArgs.Empty);
		}

		private void OnCommandParameterChanged()
		{
			_commandController.OnCommandParameterChanged();

			CommandParameterChanged?.Invoke(this, EventArgs.Empty);
		}

		private void OnCommandTargetChanged()
		{
			_commandController.OnCommandTargetChanged();

			CommandTargetChanged?.Invoke(this, EventArgs.Empty);
		}

		internal override void OnHeaderMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnHeaderMouseLeftButtonDown(e);

			e.Handled = true;
		}

		internal override void OnHeaderMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			base.OnHeaderMouseLeftButtonUp(e);

			if (IsItem == false)
				return;

			OnClickPrivate();

			e.Handled = true;
		}

		private void OnItemGeneratorChanged(MenuItemGeneratorBase oldGenerator, MenuItemGeneratorBase newGenerator)
		{
			UpdateGenerator();
		}

		private void OnSourceCollectionPropertyChangedPrivate(IEnumerable oldSource, IEnumerable newSource)
		{
			ItemCollection.SourceCollectionInternal = newSource;

			UpdateHasSubmenu();
		}

		protected override void OnMenuItemsChanged()
		{
			base.OnMenuItemsChanged();

			UpdateHasSubmenu();
		}

		internal override void OnParentGeneratorChanged()
		{
			base.OnParentGeneratorChanged();

			UpdateGenerator();
		}

		internal override void ResetPopupContent()
		{
			base.ResetPopupContent();

			MenuItemsPresenter.ResetScrollViewer();
		}

		private void UpdateGenerator()
		{
			ItemCollection.Generator = ActualGenerator;
		}

		private void UpdateHasSubmenu()
		{
			HasSubmenu = ItemCollection.ActualCountInternal > 0;
		}

		#endregion

		#region Interface Implementations

		#region IButton

		bool IButton.IsPressed => false;

		#endregion

		#region ICommandControl

		public event EventHandler CommandChanged;
		public event EventHandler CommandParameterChanged;
		public event EventHandler CommandTargetChanged;

		public DependencyObject CommandTarget
		{
			get => (DependencyObject) GetValue(CommandTargetProperty);
			set => SetValue(CommandTargetProperty, value);
		}

		public ICommand Command
		{
			get => (ICommand) GetValue(CommandProperty);
			set => SetValue(CommandProperty, value);
		}

		public object CommandParameter
		{
			get => GetValue(CommandParameterProperty);
			set => SetValue(CommandParameterProperty, value);
		}

		#endregion

		#endregion
	}
}