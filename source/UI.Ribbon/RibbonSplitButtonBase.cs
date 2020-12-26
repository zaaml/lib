// <copyright file="RibbonSplitButtonBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

#if SILVERLIGHT
using RoutedEventArgsSL = System.Windows.RoutedEventArgsSL;
#else
using RoutedEventArgsSL = System.Windows.RoutedEventArgs;
#endif
using System.Windows;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.DropDown;
using Zaaml.UI.Controls.Primitives;

namespace Zaaml.UI.Controls.Ribbon
{
	[TemplateContractType(typeof(RibbonSplitButtonBaseTemplateContract))]
	public class RibbonSplitButtonBase : RibbonDropDownButtonBase
	{
		#region Fields

		private byte _packedValue;

		#endregion

		#region Nested Types

		private static class PackedDefinition
		{
			#region Static Fields and Constants

			public static readonly PackedBoolItemDefinition IsDropDownButtonMouseOver;

			#endregion

			#region Ctors

			static PackedDefinition()
			{
				var allocator = new PackedValueAllocator();

				IsDropDownButtonMouseOver = allocator.AllocateBoolItem();
			}

			#endregion
		}

		#endregion

		#region Static Fields and Constants

		public static readonly DependencyProperty DropDownPlacementProperty =
			DPM.Register<SplitButtonPopupPlacementTarget, RibbonSplitButtonBase>
				("DropDownPlacement", SplitButtonPopupPlacementTarget.SplitButton, b => b.UpdateDropDownPlacement);

		public static readonly DependencyProperty ShowDropDownButtonProperty = DPM.Register<bool, RibbonSplitButtonBase>
			("ShowDropDownButton", true);

		#endregion

		#region Properties

		internal override bool CanFocusOnClose => DropDownButton?.IsPressed == false;

		private ToggleButton DropDownButton => TemplateContract.DropDownButton;

		public SplitButtonPopupPlacementTarget DropDownPlacement
		{
			get => this.GetValue<SplitButtonPopupPlacementTarget>(DropDownPlacementProperty);
			set => this.SetValue<SplitButtonPopupPlacementTarget>(DropDownPlacementProperty, value);
		}

		private bool IsDropDownButtonMouseOver
		{
			get => PackedDefinition.IsDropDownButtonMouseOver.GetValue(_packedValue);
			set => PackedDefinition.IsDropDownButtonMouseOver.SetValue(ref _packedValue, value);
		}

		public bool ShowDropDownButton
		{
			get => this.GetValue<bool>(ShowDropDownButtonProperty);
			set => this.SetValue<bool>(ShowDropDownButtonProperty, value);
		}

		private RibbonSplitButtonBaseTemplateContract TemplateContract =>
			(RibbonSplitButtonBaseTemplateContract) TemplateContractInternal;

		#endregion

		#region Methods

		private void DropDownButtonOnMouseEnter(object sender, MouseEventArgs e)
		{
			IsDropDownButtonMouseOver = true;
			UpdateVisualState(true);
		}

		private void DropDownButtonOnClick(object sender, RoutedEventArgsSL e)
		{
			if (IsDropDownOpen == false)
				OpenDropDown();
			else
				CloseDropDown();

			UpdateDropDownButton();

			e.Handled = true;
		}

		private void DropDownButtonOnMouseLeave(object sender, MouseEventArgs e)
		{
			IsDropDownButtonMouseOver = false;
			UpdateVisualState(true);
		}

		private void UpdateDropDownButton()
		{
			DropDownButton.IsChecked = IsDropDownOpen;
		}

		private protected override void OnIsDropDownOpenChangedInternal()
		{
			base.OnIsDropDownOpenChangedInternal();

			UpdateDropDownButton();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			UpdateDropDownPlacement();

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

			UpdateDropDownPlacement();
		}

		private void UpdateDropDownPlacement()
		{
			PlacementTarget = DropDownPlacement == SplitButtonPopupPlacementTarget.SplitButton
				? this
				: (FrameworkElement) DropDownButton ?? this;
		}

		protected override void OnClick()
		{
			CloseDropDown();

			base.OnClick();
		}

		protected override void UpdateVisualState(bool useTransitions)
		{
			base.UpdateVisualState(useTransitions);

			GotoVisualState(IsDropDownButtonMouseOver ? "DropDownButtonMouseOver" : "DropDownButtonNormal", useTransitions);
		}

		#endregion
	}

	public class RibbonSplitButtonBaseTemplateContract : RibbonDropDownButtonBaseTemplateContract
	{
		#region Properties

		[TemplateContractPart]
		public ToggleButton DropDownButton { get; [UsedImplicitly] private set; }

		#endregion
	}
}