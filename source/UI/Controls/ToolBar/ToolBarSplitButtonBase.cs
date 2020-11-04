// <copyright file="ToolBarSplitButtonBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.DropDown;
using Zaaml.UI.Controls.Primitives;
#if SILVERLIGHT
using RoutedEventArgsSL = System.Windows.RoutedEventArgsSL;
#else
using RoutedEventArgsSL = System.Windows.RoutedEventArgs;

#endif

namespace Zaaml.UI.Controls.ToolBar
{
  [TemplateContractType(typeof(ToolBarSplitButtonBaseTemplateContract))]
	public class ToolBarSplitButtonBase : ToolBarDropDownButtonBase
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty DropDownButtonDockProperty = DPM.Register<Dock, ToolBarSplitButtonBase>
			("DropDownButtonDock", Dock.Right);

		public static readonly DependencyProperty DropDownPlacementProperty = DPM.Register<SplitButtonPopupPlacementTarget, ToolBarSplitButtonBase>
			("DropDownPlacement", SplitButtonPopupPlacementTarget.SplitButton, b => b.UpdateDropDownPlacement);

		public static readonly DependencyProperty ShowDropDownButtonProperty = DPM.Register<bool, ToolBarSplitButtonBase>
			("ShowDropDownButton", true);

		public static readonly DependencyProperty ShowSeparatorProperty = DPM.Register<bool, ToolBarSplitButtonBase>
			("ShowSeparator", true);

		#endregion

		#region Fields

		private byte _packedValue;

		#endregion

		#region Properties

		internal override bool CanFocusOnClose => DropDownButton?.IsPressed == false;

		private ToggleButton DropDownButton => TemplateContract.DropDownButton;

		public Dock DropDownButtonDock
		{
			get => (Dock) GetValue(DropDownButtonDockProperty);
			set => SetValue(DropDownButtonDockProperty, value);
		}

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


		public bool ShowSeparator
		{
			get => (bool) GetValue(ShowSeparatorProperty);
			set => SetValue(ShowSeparatorProperty, value);
		}

		private ToolBarSplitButtonBaseTemplateContract TemplateContract => (ToolBarSplitButtonBaseTemplateContract) TemplateContractInternal;

    #endregion

    #region  Methods

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
			PlacementTarget = DropDownPlacement == SplitButtonPopupPlacementTarget.SplitButton ? this : (FrameworkElement) DropDownButton ?? this;
		}

		protected override void UpdateVisualState(bool useTransitions)
		{
			base.UpdateVisualState(useTransitions);

			GotoVisualState(IsDropDownButtonMouseOver ? "DropDownButtonMouseOver" : "DropDownButtonNormal", useTransitions);
		}

		#endregion

		#region  Nested Types

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
	}

	public class ToolBarSplitButtonBaseTemplateContract : ToolBarDropDownButtonBaseTemplateContract
	{
		#region Properties

		[TemplateContractPart]
		public ToggleButton DropDownButton { get; [UsedImplicitly] private set; }

		#endregion
	}
}