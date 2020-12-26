// <copyright file="PropertyViewItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.PropertyView.Editors;
using Zaaml.UI.Panels;
using TreeViewItem = Zaaml.UI.Controls.TreeView.TreeViewItem;

namespace Zaaml.UI.Controls.PropertyView
{
	[TemplateContractType(typeof(PropertyViewItemTemplateContract))]
	public class PropertyViewItem : PropertyTreeViewItem
	{
		private static readonly DependencyPropertyKey DescriptionPropertyKey = DPM.RegisterReadOnly<string, PropertyViewItem>
			("Description", default);

		private static readonly DependencyPropertyKey ItemPropertyKey = DPM.RegisterReadOnly<PropertyItem, PropertyViewItem>
			("PropertyItem", default, d => d.OnItemPropertyChangedPrivate);

		public static readonly DependencyProperty PropertyItemProperty = ItemPropertyKey.DependencyProperty;

		public static readonly DependencyProperty DescriptionProperty = DescriptionPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey EditorPropertyKey = DPM.RegisterReadOnly<PropertyEditor, PropertyViewItem>
			("Editor", default, p => p.OnEditorPropertyChangedPrivate);

		public static readonly DependencyProperty EditorProperty = EditorPropertyKey.DependencyProperty;
		private bool _hasValidationError;

		static PropertyViewItem()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<PropertyViewItem>();
		}

		public PropertyViewItem(PropertyViewControl propertyView) : base(propertyView)
		{
			this.OverrideStyleKey<PropertyViewItem>();
		}

		public string Description
		{
			get => (string) GetValue(DescriptionProperty);
			private set => this.SetReadOnlyValue(DescriptionPropertyKey, value);
		}

		public PropertyEditor Editor
		{
			get => (PropertyEditor) GetValue(EditorProperty);
			private set => this.SetReadOnlyValue(EditorPropertyKey, value);
		}

		private bool HasValidationError
		{
			get => _hasValidationError;
			set
			{
				if (_hasValidationError == value)
					return;

				if (_hasValidationError)
					ItemCollection.UnlockItemInternal(this);

				_hasValidationError = value;

				if (_hasValidationError)
					ItemCollection.LockItemInternal(this);
			}
		}

		protected override bool IsActuallyFocused => IsEditorFocused || IsFocused;

		private bool IsEditorFocused
		{
			get
			{
				if (Editor == null)
					return false;

				return Editor.IsFocused || Editor.IsKeyboardFocusWithin;
			}
		}

		public PropertyItem PropertyItem
		{
			get => (PropertyItem) GetValue(PropertyItemProperty);
			internal set => this.SetReadOnlyValue(ItemPropertyKey, value);
		}

		private PropertyViewItemTemplateContract TemplateContract => (PropertyViewItemTemplateContract) TemplateContractInternal;

		private ValidationErrorControl ValidationErrorControl => TemplateContract.ValidationErrorControl;

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			var arrangeOverride = base.ArrangeOverride(arrangeBounds);

			if (ValidationErrorControl == null || ValidationErrorControl.ActualShowValidationError == false)
				return arrangeOverride;

			if (TreeViewControl?.ItemsPresenterInternal?.ItemsHostBaseInternal is IItemsHost<TreeViewItem> host)
				ValidationErrorControl.ForceHideToolTip = host.GetLayoutInformation(this).Visibility != ItemLayoutInformationVisibility.Visible;
			else
				ValidationErrorControl.ForceHideToolTip = false;

			return arrangeOverride;
		}

		private void OnEditorGotFocus(object sender, RoutedEventArgs e)
		{
			IsSelected = true;

			UpdateVisualState(true);
		}

		private void OnEditorLostFocus(object sender, RoutedEventArgs e)
		{
			UpdateVisualState(true);
		}

		private void OnEditorPropertyChangedPrivate(PropertyEditor oldEditor, PropertyEditor newEditor)
		{
			if (oldEditor != null)
			{
				oldEditor.IsEditingChanged -= OnIsEditingChanged;
				oldEditor.GotFocus -= OnEditorGotFocus;
				oldEditor.LostFocus -= OnEditorLostFocus;
				oldEditor.PropertyViewItem = null;
			}

			if (newEditor != null)
			{
				newEditor.IsEditingChanged += OnIsEditingChanged;
				newEditor.GotFocus += OnEditorGotFocus;
				newEditor.LostFocus += OnEditorLostFocus;
				newEditor.PropertyViewItem = this;
			}

			UpdateVisualState(true);
		}

		private void OnIsEditingChanged(object sender, EventArgs e)
		{
			if (Editor.IsEditing)
				IsSelected = true;

			UpdateVisualState(true);
		}

		protected override void OnIsSelectedChanged()
		{
			base.OnIsSelectedChanged();

			UpdateZIndex();
		}

		private void OnItemPropertyChangedPrivate(PropertyItem oldValue, PropertyItem newValue)
		{
			if (oldValue != null)
				oldValue.ValueChangedInternal -= OnPropertyItemValueChanged;

			if (newValue != null)
			{
				newValue.ValueChangedInternal -= OnPropertyItemValueChanged;

				Description = newValue.Description;
				Content = newValue.DisplayName;
				Editor = PropertyView.ControllerInternal.RentEditorInternal(newValue);
			}
			else
			{
				Description = null;
				Content = null;

				var editor = Editor;

				if (editor == null)
					return;

				Editor = null;
				PropertyView.ControllerInternal.ReturnEditorInternal(editor);
			}
		}

		private void OnPropertyItemValueChanged(object sender, EventArgs e)
		{
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			Editor?.HandlePreviewKeyDownInternal(e);

			if (e.Handled)
				return;

			base.OnPreviewKeyDown(e);
		}

		protected override void OnPreviewTextInput(TextCompositionEventArgs e)
		{
			if (Editor == null)
			{
				base.OnPreviewTextInput(e);

				return;
			}

			if (string.Equals(e.Text, "\u001b") && Editor.IsEditing == false)
				return;

			Editor.BeginEdit();

			base.OnPreviewTextInput(e);
		}

		internal void SetValidationErrorInternal(string validationError)
		{
			if (string.IsNullOrEmpty(validationError))
				this.ClearValidationError();
			else
				this.SetValidationError(validationError);

			HasValidationError = this.HasValidationError();

			UpdateZIndex();
			UpdateVisualState(true);
		}

		private void UpdateZIndex()
		{
			var zIndex = 0;

			if (IsValid == false)
				zIndex = 1;

			if (IsSelected)
				zIndex = 2;

			Panel.SetZIndex(this, zIndex);
		}
	}

	public class PropertyViewItemTemplateContract : IconContentControlTemplateContract
	{
		[TemplateContractPart]
		public ValidationErrorControl ValidationErrorControl { get; private set; }
	}
}