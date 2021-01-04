// <copyright file="DropDownEditableSelectorBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.UI.Controls.DropDown
{
	[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
	public abstract partial class DropDownEditableSelectorBase<TItemsControl, TItem> : DropDownSelectorBase<TItemsControl, TItem>
		where TItemsControl : Control
		where TItem : Control
	{
		public static readonly DependencyProperty IsTextEditableProperty = DPM.Register<bool, DropDownEditableSelectorBase<TItemsControl, TItem>>
			("IsTextEditable", false, d => d.OnIsTextEditablePropertyChangedPrivate);

		private static readonly DependencyPropertyKey IsEditingPropertyKey = DPM.RegisterReadOnly<bool, DropDownEditableSelectorBase<TItemsControl, TItem>>
			("IsEditing", false, d => d.OnIsEditingPropertyChangedPrivate, d => d.CoerceIsEditing);

		public static readonly DependencyProperty IsEditingProperty = IsEditingPropertyKey.DependencyProperty;

		public static readonly DependencyProperty EditorTextProperty = DPM.Register<string, DropDownEditableSelectorBase<TItemsControl, TItem>>
			("EditorText", string.Empty, d => d.OnEditorTextPropertyChangedPrivate, d => CoerceEditorText);

		public static readonly DependencyProperty PostEditorTextDelayProperty = DPM.Register<TimeSpan, DropDownSelectorBase<TItemsControl, TItem>>
			("PostEditorTextDelay", TimeSpan.Zero);

		public static readonly DependencyProperty OpenDropDownOnEditingProperty = DPM.Register<bool, DropDownEditableSelectorBase<TItemsControl, TItem>>
			("OpenDropDownOnEditing", true);

		public static readonly DependencyProperty PreserveEditorTextProperty = DPM.Register<PreserveEditorTextMode, DropDownEditableSelectorBase<TItemsControl, TItem>>
			("PreserveEditorText", PreserveEditorTextMode.Auto);

		public static readonly DependencyProperty DisplayModeProperty = DPM.Register<DropDownEditableSelectorDisplayMode, DropDownEditableSelectorBase<TItemsControl, TItem>>
			("DisplayMode", default, d => d.OnDisplayModePropertyChangedPrivate);

		private static readonly DependencyPropertyKey ActualDisplayModePropertyKey = DPM.RegisterReadOnly<DropDownEditableSelectorDisplayMode, DropDownEditableSelectorBase<TItemsControl, TItem>>
			("ActualDisplayMode", DropDownEditableSelectorDisplayMode.DropDownButton);

		public static readonly DependencyProperty ActualDisplayModeProperty = ActualDisplayModePropertyKey.DependencyProperty;

		private readonly DelayAction _delayOpenDropDown;
		private byte _packedValue;

		protected DropDownEditableSelectorBase()
		{
			_delayOpenDropDown = new DelayAction(DelayOpenDropDown);
		}

		public DropDownEditableSelectorDisplayMode ActualDisplayMode
		{
			get => (DropDownEditableSelectorDisplayMode) GetValue(ActualDisplayModeProperty);
			private set => this.SetReadOnlyValue(ActualDisplayModePropertyKey, value);
		}

		public DropDownEditableSelectorDisplayMode DisplayMode
		{
			get => (DropDownEditableSelectorDisplayMode) GetValue(DisplayModeProperty);
			set => SetValue(DisplayModeProperty, value);
		}

		public string EditorText
		{
			get => (string) GetValue(EditorTextProperty);
			set => SetValue(EditorTextProperty, value);
		}

		public bool IsEditing
		{
			get => (bool) GetValue(IsEditingProperty);
			private set => this.SetReadOnlyValue(IsEditingPropertyKey, value);
		}

		public bool IsTextEditable
		{
			get => (bool) GetValue(IsTextEditableProperty);
			set => SetValue(IsTextEditableProperty, value);
		}

		public bool OpenDropDownOnEditing
		{
			get => (bool) GetValue(OpenDropDownOnEditingProperty);
			set => SetValue(OpenDropDownOnEditingProperty, value);
		}

		public TimeSpan PostEditorTextDelay
		{
			get => (TimeSpan) GetValue(PostEditorTextDelayProperty);
			set => SetValue(PostEditorTextDelayProperty, value);
		}

		public PreserveEditorTextMode PreserveEditorText
		{
			get => (PreserveEditorTextMode) GetValue(PreserveEditorTextProperty);
			set => SetValue(PreserveEditorTextProperty, value);
		}

		private void DelayOpenDropDown()
		{
			if (IsEditing == false)
				return;

			if (IsDropDownOpen == false)
			{
				ForceFilterUpdate();
				OpenDropDown();
			}
		}

		private bool IsDescendant(object value)
		{
			if (value == null)
				return false;

			if (ReferenceEquals(value, this))
				return true;

			return value is DependencyObject dependencyObject && dependencyObject.IsDescendantOf(this, MixedTreeEnumerationStrategy.VisualThenLogicalInstance);
		}

		private void OnDisplayModePropertyChangedPrivate(DropDownEditableSelectorDisplayMode oldValue, DropDownEditableSelectorDisplayMode newValue)
		{
			UpdateActualDisplayMode();
		}

		private void OnIsTextEditablePropertyChangedPrivate(bool oldValue, bool newValue)
		{
			UpdateActualDisplayMode();

			this.SetCurrentValueInternal(IsTabStopProperty, newValue ? KnownBoxes.BoolTrue : KnownBoxes.BoolFalse);
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			Popup.Closing += PopupCloseControllerOnClosing;

			if (IsEditing)
				ShowEditor();
			else
				HideEditor();
		}

		protected override void OnTemplateContractDetaching()
		{
			Popup.Closing -= PopupCloseControllerOnClosing;

			base.OnTemplateContractDetaching();
		}

		private void UpdateActualDisplayMode()
		{
			if (DisplayMode == DropDownEditableSelectorDisplayMode.Auto)
				ActualDisplayMode = IsTextEditable ? DropDownEditableSelectorDisplayMode.TextEditor : DropDownEditableSelectorDisplayMode.DropDownButton;
			else
				ActualDisplayMode = DisplayMode;
		}

		private static class PackedDefinition
		{
			public static readonly PackedUIntItemDefinition SuspendDropDownHandlerCount;
			public static readonly PackedUIntItemDefinition SuspendFocusHandlerCount;
			public static readonly PackedBoolItemDefinition SuspendOpenDropDown;

			static PackedDefinition()
			{
				var allocator = new PackedValueAllocator();

				SuspendDropDownHandlerCount = allocator.AllocateUIntItem(4);
				SuspendFocusHandlerCount = allocator.AllocateUIntItem(4);
				SuspendOpenDropDown = allocator.AllocateBoolItem();
			}
		}
	}
}