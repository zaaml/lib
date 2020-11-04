// <copyright file="SplitButtonBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

#if SILVERLIGHT
using RoutedEventArgsSL = System.Windows.RoutedEventArgsSL;
#else
using RoutedEventArgsSL = System.Windows.RoutedEventArgs;
#endif
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Primitives;

namespace Zaaml.UI.Controls.DropDown
{
	public abstract class SplitButtonBase : DropDownButtonBase
	{
		public static readonly DependencyProperty DropDownButtonDockProperty = DPM.Register<Dock, SplitButtonBase>
			("DropDownButtonDock", Dock.Right);

		public static readonly DependencyProperty PopupPlacementTargetProperty = DPM.Register<SplitButtonPopupPlacementTarget, SplitButtonBase>
			("PopupPlacementTarget", SplitButtonPopupPlacementTarget.SplitButton, b => b.UpdatePopupPlacement);

		public static readonly DependencyProperty ShowDropDownButtonProperty = DPM.Register<bool, SplitButtonBase>
			("ShowDropDownButton", true);

		public static readonly DependencyProperty ShowSeparatorProperty = DPM.Register<bool, SplitButtonBase>
			("ShowSeparator", true);

		private byte _packedValue;

		internal override bool CanFocusOnClose => DropDownButton?.IsPressed == false;

		private ToggleButton DropDownButton => TemplateContract.DropDownButton;

		public Dock DropDownButtonDock
		{
			get => (Dock) GetValue(DropDownButtonDockProperty);
			set => SetValue(DropDownButtonDockProperty, value);
		}

		private bool IsDropDownButtonMouseOver
		{
			get => PackedDefinition.IsDropDownButtonMouseOver.GetValue(_packedValue);
			set => PackedDefinition.IsDropDownButtonMouseOver.SetValue(ref _packedValue, value);
		}

		public SplitButtonPopupPlacementTarget PopupPlacementTarget
		{
			get => this.GetValue<SplitButtonPopupPlacementTarget>(PopupPlacementTargetProperty);
			set => this.SetValue<SplitButtonPopupPlacementTarget>(PopupPlacementTargetProperty, value);
		}

		public bool ShowDropDownButton
		{
			get => this.GetValue<bool>(ShowDropDownButtonProperty);
			set => this.SetValue<bool>(ShowDropDownButtonProperty, value);
		}

		public bool ShowSeparator
		{
			get => (bool) GetValue(ShowSeparatorProperty);
			set => SetValue(ShowSeparatorProperty, value);
		}

		private SplitButtonBaseTemplateContract TemplateContract => (SplitButtonBaseTemplateContract) TemplateContractInternal;

		protected override TemplateContract CreateTemplateContract()
		{
			return new SplitButtonBaseTemplateContract();
		}

		private static void DropDownButtonOnClick(object sender, RoutedEventArgsSL e)
		{
			e.Handled = true;
		}

		private void DropDownButtonOnMouseEnter(object sender, MouseEventArgs e)
		{
			IsDropDownButtonMouseOver = true;

			UpdateVisualState(true);
		}

		private void DropDownButtonOnMouseLeave(object sender, MouseEventArgs e)
		{
			IsDropDownButtonMouseOver = false;

			UpdateVisualState(true);
		}

		protected override void OnClick()
		{
			IsDropDownOpen = false;

			base.OnClick();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			UpdatePopupPlacement();

			DropDownButton.Click += DropDownButtonOnClick;
			DropDownButton.MouseEnter += DropDownButtonOnMouseEnter;
			DropDownButton.MouseLeave += DropDownButtonOnMouseLeave;
		}

		protected override void OnTemplateContractDetaching()
		{
			DropDownButton.Click -= DropDownButtonOnClick;
			DropDownButton.MouseEnter -= DropDownButtonOnMouseEnter;
			DropDownButton.MouseLeave -= DropDownButtonOnMouseLeave;

			base.OnTemplateContractDetaching();

			UpdatePopupPlacement();
		}

		private void UpdatePopupPlacement()
		{
			PlacementTargetCore = PopupPlacementTarget == SplitButtonPopupPlacementTarget.SplitButton ? this : (FrameworkElement) DropDownButton ?? this;
		}

		protected override void UpdateVisualState(bool useTransitions)
		{
			base.UpdateVisualState(useTransitions);

			GotoVisualState(IsDropDownButtonMouseOver ? "DropDownButtonMouseOver" : "DropDownButtonNormal", useTransitions);
		}

		private static class PackedDefinition
		{
			public static readonly PackedBoolItemDefinition IsDropDownButtonMouseOver;

			static PackedDefinition()
			{
				var allocator = new PackedValueAllocator();

				IsDropDownButtonMouseOver = allocator.AllocateBoolItem();
			}
		}
	}

	public class SplitButtonBaseTemplateContract : DropDownButtonBaseTemplateContract
	{
		[TemplateContractPart]
		public ToggleButton DropDownButton { get; [UsedImplicitly] private set; }
	}
}