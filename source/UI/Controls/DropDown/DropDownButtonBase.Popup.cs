// <copyright file="DropDownButtonBase.Popup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Primitives;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;

namespace Zaaml.UI.Controls.DropDown
{
	public abstract partial class DropDownButtonBase : DropDownControlBase, IDropDownControlHost
	{
		public static readonly DependencyProperty PopupControlProperty = DPM.Register<PopupControlBase, DropDownButtonBase>
			("PopupControl", d => d.OnDropDownControlChanged);

		public static readonly DependencyProperty DropDownGlyphProperty = DPM.Register<IconBase, DropDownButtonBase>
			("DropDownGlyph");

		public static readonly DependencyProperty IsDropDownOpenProperty = DPM.Register<bool, DropDownButtonBase>
			("IsDropDownOpen", b => b.OnIsDropDownOpenChanged);

		public static readonly DependencyProperty PlacementProperty = DPM.Register<Dock, DropDownButtonBase>
			("Placement", Dock.Bottom);

		private readonly DropDownPopupWrapper _dropDownPopupWrapper;
		private bool _isPopupOpenInt;
		private object _logicalChild;

		public event EventHandler DropDownClosed;
		public event EventHandler DropDownOpened;

		internal virtual bool CanFocusOnClose => IsPressed == false;

		public IconBase DropDownGlyph
		{
			get => (IconBase) GetValue(DropDownGlyphProperty);
			set => SetValue(DropDownGlyphProperty, value);
		}

		public bool IsDropDownOpen
		{
			get => (bool) GetValue(IsDropDownOpenProperty);
			set => SetValue(IsDropDownOpenProperty, value);
		}

		private bool IsPopupOpenInt
		{
			get => _isPopupOpenInt;
			set
			{
				if (_isPopupOpenInt == value)
					return;

				_isPopupOpenInt = value;

				UpdateVisualState(true);
			}
		}

		protected override IEnumerator LogicalChildren => _logicalChild != null ? EnumeratorUtils.Concat(_logicalChild, base.LogicalChildren) : base.LogicalChildren;

		public Dock Placement
		{
			get => (Dock) GetValue(PlacementProperty);
			set => SetValue(PlacementProperty, value);
		}

		protected FrameworkElement PlacementTargetCore
		{
			get => _dropDownPopupWrapper.PlacementTarget;
			set => _dropDownPopupWrapper.PlacementTarget = value;
		}

		public PopupControlBase PopupControl
		{
			get => (PopupControlBase) GetValue(PopupControlProperty);
			set => SetValue(PopupControlProperty, value);
		}

		private PopupControlHost PopupHost => TemplateContract.PopupHost;

		private DropDownButtonBaseTemplateContract TemplateContract => (DropDownButtonBaseTemplateContract) TemplateContractInternal;

		protected override TemplateContract CreateTemplateContract()
		{
			return new DropDownButtonBaseTemplateContract();
		}

		protected virtual void OnClosed()
		{
			DropDownClosed?.Invoke(this, EventArgs.Empty);
		}

		public void OpenDropDown()
		{
			if (IsDropDownOpen)
				return;
			
			this.SetCurrentValueInternal(IsDropDownOpenProperty, true);

			CoerceIsDropDownOpen();
		}

		private void CoerceIsDropDownOpen()
		{
			var popupIsOpen = PopupControl?.PopupController?.Popup?.IsOpen;
			
			if (popupIsOpen == false)
				this.SetCurrentValueInternal(IsDropDownOpenProperty, false);
			else if (popupIsOpen == true)
				this.SetCurrentValueInternal(IsDropDownOpenProperty, true);
		}

		public void CloseDropDown()
		{
			if (IsDropDownOpen == false)
				return;

			this.SetCurrentValueInternal(IsDropDownOpenProperty, false);

			CoerceIsDropDownOpen();
		}

		private void OnClosedCore()
		{
			if (CanFocusOnClose && FocusHelper.IsKeyboardFocusWithin(this))
				FocusHelper.SetKeyboardFocusedElement(this);

			OnClosed();
		}

		private void OnDropDownControlChanged(PopupControlBase oldControl, PopupControlBase newControl)
		{
			_dropDownPopupWrapper.Popup = newControl;

			DropDownControlHostHelper.OnDropDownControlChanged(this, oldControl, newControl);
		}

		private void OnIsDropDownOpenChanged()
		{
			if (IsDropDownOpen)
				OnOpenedCore();
			else
				OnClosedCore();

			UpdateVisualState(true);
		}

		private void OnLayoutUpdated(object sender, EventArgs eventArgs)
		{
			IsPopupOpenInt = IsDropDownOpen;
		}

		protected virtual void OnOpened()
		{
			DropDownOpened?.Invoke(this, EventArgs.Empty);
		}

		private void OnOpenedCore()
		{
			OnOpened();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			DropDownControlHostHelper.OnHostAttached(this);
		}

		protected override void OnTemplateContractDetaching()
		{
			DropDownControlHostHelper.OnHostDetaching(this);

			base.OnTemplateContractDetaching();
		}

		protected override void UpdateVisualState(bool useTransitions)
		{
			base.UpdateVisualState(useTransitions);

			GotoVisualState(IsDropDownOpen || IsPopupOpenInt ? CommonVisualStates.PopupOpened : CommonVisualStates.PopupClosed, useTransitions);
		}

		PopupControlHost IDropDownControlHost.PopupHost => PopupHost;

		object IDropDownControlHost.LogicalChild
		{
			get => _logicalChild;
			set
			{
				if (ReferenceEquals(_logicalChild, value))
					return;

				if (_logicalChild != null)
					RemoveLogicalChild(_logicalChild);

				_logicalChild = value;

				if (_logicalChild != null)
					AddLogicalChild(_logicalChild);
			}
		}

		PopupControlBase IDropDownControlHost.PopupControl => PopupControl;
	}

	public class DropDownButtonBaseTemplateContract : ButtonBaseTemplateContract
	{
		[TemplateContractPart]
		public PopupControlHost PopupHost { get; [UsedImplicitly] private set; }
	}
}