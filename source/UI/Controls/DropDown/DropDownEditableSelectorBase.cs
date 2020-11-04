// <copyright file="DropDownEditableSelectorBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Core;
using Control = System.Windows.Controls.Control;

namespace Zaaml.UI.Controls.DropDown
{
	public abstract partial class DropDownEditableSelectorBase<TItemsControl, TItem> : DropDownSelectorBase<TItemsControl, TItem>
		where TItemsControl : Control
		where TItem : Control, ISelectable
	{
		public static readonly DependencyProperty IsEditableProperty = DPM.Register<bool, DropDownEditableSelectorBase<TItemsControl, TItem>>
			("IsEditable", false, d => d.OnIsEditablePropertyChangedPrivate);

		private static readonly DependencyPropertyKey IsEditingPropertyKey = DPM.RegisterReadOnly<bool, DropDownEditableSelectorBase<TItemsControl, TItem>>
			("IsEditing", false, d => d.OnIsEditingPropertyChangedPrivate, d => d.CoerceIsEditing);

		public static readonly DependencyProperty IsEditingProperty = IsEditingPropertyKey.DependencyProperty;

		public static readonly DependencyProperty TextProperty = DPM.Register<string, DropDownEditableSelectorBase<TItemsControl, TItem>>
			("Text", string.Empty, d => d.OnTextPropertyChangedPrivate, d => CoerceText);

		public static readonly DependencyProperty OpenDropDownOnEditingProperty = DPM.Register<bool, DropDownEditableSelectorBase<TItemsControl, TItem>>
			("OpenDropDownOnEditing", true);

		public static readonly DependencyProperty PreserveTextProperty = DPM.Register<PreserveTextMode, DropDownEditableSelectorBase<TItemsControl, TItem>>
			("PreserveText", PreserveTextMode.Auto);

		private readonly DelayAction _delayOpenDropDown;

		private byte _packedValue;

		protected DropDownEditableSelectorBase()
		{
			_delayOpenDropDown = new DelayAction(DelayOpenDropDown);
		}

		public bool IsEditable
		{
			get => (bool) GetValue(IsEditableProperty);
			set => SetValue(IsEditableProperty, value);
		}

		public bool IsEditing
		{
			get => (bool) GetValue(IsEditingProperty);
			private set => this.SetReadOnlyValue(IsEditingPropertyKey, value);
		}

		public bool OpenDropDownOnEditing
		{
			get => (bool) GetValue(OpenDropDownOnEditingProperty);
			set => SetValue(OpenDropDownOnEditingProperty, value);
		}

		public PreserveTextMode PreserveText
		{
			get => (PreserveTextMode) GetValue(PreserveTextProperty);
			set => SetValue(PreserveTextProperty, value);
		}

		public string Text
		{
			get => (string) GetValue(TextProperty);
			set => SetValue(TextProperty, value);
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

		private void OnIsEditablePropertyChangedPrivate(bool oldValue, bool newValue)
		{
			this.SetCurrentValueInternal(IsTabStopProperty, newValue ? KnownBoxes.BoolTrue : KnownBoxes.BoolFalse);
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			Popup.Closing += PopupCloseControllerOnClosing;

			if (Editor != null)
				Editor.Visibility = Visibility.Collapsed;
		}

		protected override void OnTemplateContractDetaching()
		{
			Popup.Closing -= PopupCloseControllerOnClosing;

			base.OnTemplateContractDetaching();
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