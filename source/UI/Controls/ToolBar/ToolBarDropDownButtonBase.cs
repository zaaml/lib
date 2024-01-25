// <copyright file="ToolBarDropDownButtonBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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
using Zaaml.UI.Controls.DropDown;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;

namespace Zaaml.UI.Controls.ToolBar
{
  [TemplateContractType(typeof(ToolBarDropDownButtonBaseTemplateContract))]
	public abstract class ToolBarDropDownButtonBase : ToolBarButtonBase, IDropDownControlHost
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty PopupControlProperty = DPM.Register<PopupControlBase, ToolBarDropDownButtonBase>
			("PopupControl", d => d.OnDropDownControlChanged);

		public static readonly DependencyProperty DropDownGlyphProperty = DPM.Register<IconBase, ToolBarDropDownButtonBase>
			("DropDownGlyph");

		public static readonly DependencyProperty IsDropDownOpenProperty = DPM.Register<bool, ToolBarDropDownButtonBase>
			("IsDropDownOpen", b => b.OnIsDropDownOpenPropertyChangedPrivate);

		public static readonly DependencyProperty PlacementProperty = DependencyPropertyManager.Register<Dock, ToolBarDropDownButtonBase>
			("Placement", Dock.Bottom);

		#endregion

		#region Fields

		[UsedImplicitly] private readonly DropDownPopupWrapper _dropDownPopupWrapper;
		private bool _isDropDownOpenInt;
		private object _logicalChild;

		public event EventHandler DropDownClosed;
		public event EventHandler DropDownOpened;

		#endregion

		#region Ctors

		protected ToolBarDropDownButtonBase()
		{
			_dropDownPopupWrapper = new DropDownPopupWrapper(this, IsDropDownOpenProperty, PlacementProperty);
			LayoutUpdated += OnLayoutUpdated;
		}

		#endregion

		#region Properties

		internal virtual bool CanFocusOnClose => true;

		public IconBase DropDownGlyph
		{
			get => (IconBase) GetValue(DropDownGlyphProperty);
			set => SetValue(DropDownGlyphProperty, value);
		}

		private PopupControlHost PopupHost => TemplateContract.PopupHost;

		public bool IsDropDownOpen
		{
			get => (bool) GetValue(IsDropDownOpenProperty);
			set => SetValue(IsDropDownOpenProperty, value);
		}

		private bool IsDropDownOpenInt
		{
			get => _isDropDownOpenInt;
			set
			{
				if (_isDropDownOpenInt == value)
					return;

				_isDropDownOpenInt = value;
				UpdateVisualState(true);
			}
		}

		protected override IEnumerator LogicalChildren => _logicalChild != null ? EnumeratorUtils.Concat(_logicalChild, base.LogicalChildren) : base.LogicalChildren;

		public Dock Placement
		{
			get => (Dock) GetValue(PlacementProperty);
			set => SetValue(PlacementProperty, value);
		}

		protected FrameworkElement PlacementTarget
		{
			get => _dropDownPopupWrapper.PlacementTarget;
			set => _dropDownPopupWrapper.PlacementTarget = value;
		}

		private ToolBarDropDownButtonBaseTemplateContract TemplateContract => (ToolBarDropDownButtonBaseTemplateContract) TemplateContractCore;

		#endregion

		#region  Methods

		protected virtual void OnClosed()
		{
			DropDownClosed?.Invoke(this, EventArgs.Empty);
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
		
		private void OnIsDropDownOpenPropertyChangedPrivate()
		{
			if (IsDropDownOpen)
				OnOpenedCore();
			else
				OnClosedCore();

			OnIsDropDownOpenChangedInternal();

			UpdateVisualState(true);
		}

		private protected virtual void OnIsDropDownOpenChangedInternal()
		{
		}

		private void OnLayoutUpdated(object sender, EventArgs eventArgs)
		{
			IsDropDownOpenInt = IsDropDownOpen;
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

			GotoVisualState(IsDropDownOpen || IsDropDownOpenInt ? CommonVisualStates.PopupOpened : CommonVisualStates.PopupClosed, useTransitions);
		}

		#endregion

		#region Interface Implementations

		#region IDropDownControlHost

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

		public void OpenDropDown()
		{
			if (IsDropDownOpen)
				return;

			if (PopupControl == null)
				return;

			this.SetCurrentValueInternal(IsDropDownOpenProperty, true);

			CoerceIsDropDownOpen();
		}

		public PopupControlBase PopupControl
		{
			get => (PopupControlBase) GetValue(PopupControlProperty);
			set => SetValue(PopupControlProperty, value);
		}

		PopupControlHost IDropDownControlHost.PopupHost => PopupHost;

		PopupControlBase IDropDownControlHost.PopupControl => PopupControl;

	  #endregion

	  #endregion
	}

	public class ToolBarDropDownButtonBaseTemplateContract : ToolBarButtonBaseTemplateContract
	{
		#region Properties

		[TemplateContractPart]
		public PopupControlHost PopupHost { get; [UsedImplicitly] private set; }

		#endregion
	}
}