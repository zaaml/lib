// <copyright file="DropDownItemsControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;
using Zaaml.UI.Controls.ScrollView;
using NativeControl = System.Windows.Controls.Control;

namespace Zaaml.UI.Controls.DropDown
{
	[TemplateContractType(typeof(DropDownItemsControlTemplateContract))]
	public abstract class DropDownItemsControl : DropDownControlBase
	{
		public static readonly DependencyProperty DropDownHeaderProperty = DPM.Register<object, DropDownItemsControl>
			("DropDownHeader");

		public static readonly DependencyProperty DropDownFooterProperty = DPM.Register<object, DropDownItemsControl>
			("DropDownFooter");

		public static readonly DependencyProperty DropDownHeaderTemplateProperty = DPM.Register<DataTemplate, DropDownItemsControl>
			("DropDownHeaderTemplate");

		public static readonly DependencyProperty DropDownFooterTemplateProperty = DPM.Register<DataTemplate, DropDownItemsControl>
			("DropDownFooterTemplate");

		public static readonly DependencyProperty ShowDropDownButtonProperty = DPM.Register<bool, DropDownItemsControl>
			("ShowDropDownButton", true);

		public object DropDownFooter
		{
			get => GetValue(DropDownFooterProperty);
			set => SetValue(DropDownFooterProperty, value);
		}

		public DataTemplate DropDownFooterTemplate
		{
			get => (DataTemplate) GetValue(DropDownFooterTemplateProperty);
			set => SetValue(DropDownFooterTemplateProperty, value);
		}

		public object DropDownHeader
		{
			get => GetValue(DropDownHeaderProperty);
			set => SetValue(DropDownHeaderProperty, value);
		}

		public DataTemplate DropDownHeaderTemplate
		{
			get => (DataTemplate) GetValue(DropDownHeaderTemplateProperty);
			set => SetValue(DropDownHeaderTemplateProperty, value);
		}

		private bool IsActuallyFocused => FocusHelper.IsKeyboardFocusWithin(this);

		protected PopupBar PopupBar => TemplateContract.PopupBar;

		protected override PopupControlBase PopupControlCore => PopupBar;

		protected virtual ScrollViewControl ScrollView => null;

		public bool ShowDropDownButton
		{
			get => (bool) GetValue(ShowDropDownButtonProperty);
			set => SetValue(ShowDropDownButtonProperty, value.Box());
		}

		private DropDownItemsControlTemplateContract TemplateContract => (DropDownItemsControlTemplateContract) TemplateContractCore;

		private protected override void OnIsDropDownOpenChangedInternal()
		{
			if (IsDropDownOpen == false)
				ScrollView?.ResetScrollBarVisibilityShown();

			base.OnIsDropDownOpenChangedInternal();
		}

		private protected virtual void OnQueryCloseDropDownOnClick(object sender, QueryCloseOnClickEventArgs e)
		{
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			PopupBar.PopupController.QueryCloseOnClick += OnQueryCloseDropDownOnClick;
		}

		protected override void OnTemplateContractDetaching()
		{
			PopupBar.PopupController.QueryCloseOnClick -= OnQueryCloseDropDownOnClick;

			base.OnTemplateContractDetaching();
		}

		protected override void UpdateVisualState(bool useTransitions)
		{
			var isMouseOver = IsMouseOver;

			// Common states
			if (!IsEnabled)
				GotoVisualState(CommonVisualStates.Disabled, useTransitions);
			else if (isMouseOver)
				GotoVisualState(CommonVisualStates.MouseOver, useTransitions);
			else
				GotoVisualState(CommonVisualStates.Normal, useTransitions);

			// Focus states
			if (IsActuallyFocused)
				GotoVisualState(CommonVisualStates.Focused, useTransitions);
			else
				GotoVisualState(CommonVisualStates.Unfocused, useTransitions);
		}
	}

	public abstract class DropDownItemsControl<TItemsControl, TItem> : DropDownItemsControl
		where TItemsControl : NativeControl
		where TItem : NativeControl
	{
		protected abstract ItemCollectionBase<TItemsControl, TItem> ItemCollection { get; }

		protected abstract TItemsControl ItemsControl { get; }

		private protected virtual void OnItemsControlChanged(TItemsControl oldControl, TItemsControl newControl)
		{
			if (oldControl != null)
				FocusManager.SetIsFocusScope(oldControl, false);

			if (newControl != null)
				FocusManager.SetIsFocusScope(newControl, true);

			LogicalChildMentor.OnLogicalChildPropertyChanged(oldControl, newControl);
		}
	}

	public class DropDownItemsControlTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = true)]
		public PopupBar PopupBar { get; [UsedImplicitly] private set; }
	}
}