// <copyright file="DropDownButtonBase.Popup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Windows;
using Zaaml.Core;
using Zaaml.Core.Utils;
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

		private readonly DropDownPopupWrapper _dropDownPopupWrapper;
		private bool _isPopupOpenPrivate;
		private object _logicalChild;

		internal virtual bool CanFocusOnClose => IsPressed == false;

		public IconBase DropDownGlyph
		{
			get => (IconBase) GetValue(DropDownGlyphProperty);
			set => SetValue(DropDownGlyphProperty, value);
		}

		private bool IsPopupOpenPrivate
		{
			get => _isPopupOpenPrivate;
			set
			{
				if (_isPopupOpenPrivate == value)
					return;

				_isPopupOpenPrivate = value;

				UpdateVisualState(true);
			}
		}

		protected override IEnumerator LogicalChildren => _logicalChild != null ? EnumeratorUtils.Concat(_logicalChild, base.LogicalChildren) : base.LogicalChildren;

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

		protected override PopupControlBase PopupControlCore => PopupControl;

		private PopupControlHost PopupHost => TemplateContract.PopupHost;

		private DropDownButtonBaseTemplateContract TemplateContract => (DropDownButtonBaseTemplateContract) TemplateContractCore;

		protected override TemplateContract CreateTemplateContract()
		{
			return new DropDownButtonBaseTemplateContract();
		}

		private protected override void OnClosedCore()
		{
			if (CanFocusOnClose && FocusHelper.IsKeyboardFocusWithin(this))
				FocusHelper.SetKeyboardFocusedElement(this);

			base.OnClosedCore();
		}

		private void OnDropDownControlChanged(PopupControlBase oldControl, PopupControlBase newControl)
		{
			_dropDownPopupWrapper.Popup = newControl;

			DropDownControlHostHelper.OnDropDownControlChanged(this, oldControl, newControl);
		}

		private void OnLayoutUpdated(object sender, EventArgs eventArgs)
		{
			IsPopupOpenPrivate = IsDropDownOpen;
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

			GotoVisualState(IsDropDownOpen || IsPopupOpenPrivate ? CommonVisualStates.PopupOpened : CommonVisualStates.PopupClosed, useTransitions);
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
		[TemplateContractPart] public PopupControlHost PopupHost { get; [UsedImplicitly] private set; }
	}
}