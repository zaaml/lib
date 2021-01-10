// <copyright file="NavigationViewCommandItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.NavigationView
{
	[TemplateContractType(typeof(NavigationViewCommandItemTemplateContract))]
	public partial class NavigationViewCommandItem : NavigationViewHeaderedIconItem
	{
		public static readonly DependencyProperty CommandProperty = DPM.Register<ICommand, NavigationViewCommandItem>
			("Command", g => g.OnCommandChanged);

		public static readonly DependencyProperty CommandParameterProperty = DPM.Register<object, NavigationViewCommandItem>
			("CommandParameter", g => g.OnCommandParameterChanged);

		public static readonly DependencyProperty CommandTargetProperty = DPM.Register<DependencyObject, NavigationViewCommandItem>
			("CommandTarget", g => g.OnCommandTargetChanged);

		private static readonly DependencyPropertyKey IsPressedPropertyKey = DPM.RegisterReadOnly<bool, NavigationViewCommandItem>
			("IsPressed");

		public static readonly DependencyProperty IsPressedProperty = IsPressedPropertyKey.DependencyProperty;

		public event EventHandler CommandChanged;
		public event EventHandler CommandParameterChanged;
		public event EventHandler CommandTargetChanged;

		static NavigationViewCommandItem()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<NavigationViewCommandItem>();
		}

		public NavigationViewCommandItem()
		{
			this.OverrideStyleKey<NavigationViewCommandItem>();
		}

		public ICommand Command
		{
			get => (ICommand) GetValue(CommandProperty);
			set => SetValue(CommandProperty, value);
		}

		private protected override ClickMode ClickModeCore => ClickMode.Release;

		private protected override ICommand CommandCore => Command;

		public object CommandParameter
		{
			get => GetValue(CommandParameterProperty);
			set => SetValue(CommandParameterProperty, value);
		}

		private protected override object CommandParameterCore => CommandParameter;

		public DependencyObject CommandTarget
		{
			get => (DependencyObject) GetValue(CommandTargetProperty);
			set => SetValue(CommandTargetProperty, value);
		}

		private protected override DependencyObject CommandTargetCore => CommandTarget;

		public bool IsPressed
		{
			get => (bool) GetValue(IsPressedProperty);
			private set => this.SetReadOnlyValue(IsPressedPropertyKey, value);
		}

		private protected override bool IsPressedCore
		{
			get => IsPressed;
			set => IsPressed = value;
		}

		private protected override bool IsPressedVisualState => IsPressed;

		private protected override void OnClickCore()
		{
			base.OnClickCore();

			RaiseClickEvent();
		}

		private void OnCommandChanged()
		{
			OnCommandChangedCore();

			CommandChanged?.Invoke(this, EventArgs.Empty);
		}

		private void OnCommandParameterChanged()
		{
			OnCommandParameterChangedCore();

			CommandParameterChanged?.Invoke(this, EventArgs.Empty);
		}

		private void OnCommandTargetChanged()
		{
			OnCommandTargetChangedCore();

			CommandTargetChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}