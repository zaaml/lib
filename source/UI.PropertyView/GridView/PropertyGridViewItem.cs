// <copyright file="PropertyGridViewItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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
	public class PropertyGridViewItem : PropertyTreeViewItem
	{
		private static readonly DependencyPropertyKey DescriptionPropertyKey = DPM.RegisterReadOnly<string, PropertyGridViewItem>
			("Description");

		private static readonly DependencyPropertyKey ItemPropertyKey = DPM.RegisterReadOnly<PropertyItem, PropertyGridViewItem>
			("PropertyItem", d => d.OnItemPropertyChangedPrivate);

		public static readonly DependencyProperty PropertyItemProperty = ItemPropertyKey.DependencyProperty;

		public static readonly DependencyProperty DescriptionProperty = DescriptionPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey EditorPropertyKey = DPM.RegisterReadOnly<PropertyEditor, PropertyGridViewItem>
			("Editor", default, p => p.OnEditorPropertyChangedPrivate);

		public static readonly DependencyProperty EditorProperty = EditorPropertyKey.DependencyProperty;
		private bool _hasValidationError;

		static PropertyGridViewItem()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<PropertyGridViewItem>();
		}

		public PropertyGridViewItem(PropertyViewControl propertyViewControl) : base(propertyViewControl)
		{
			this.OverrideStyleKey<PropertyGridViewItem>();
		}

		public string Description
		{
			get => (string)GetValue(DescriptionProperty);
			private set => this.SetReadOnlyValue(DescriptionPropertyKey, value);
		}

		public PropertyEditor Editor
		{
			get => (PropertyEditor)GetValue(EditorProperty);
			private set => this.SetReadOnlyValue(EditorPropertyKey, value);
		}

		private bool HasValidationError
		{
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

		private protected override bool IsReadOnlyState => PropertyItem?.IsReadOnly == true;

		public PropertyItem PropertyItem
		{
			get => (PropertyItem)GetValue(PropertyItemProperty);
			internal set => this.SetReadOnlyValue(ItemPropertyKey, value);
		}

		private PropertyViewItemTemplateContract TemplateContract => (PropertyViewItemTemplateContract)TemplateContractInternal;

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

		private void OnEditorIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
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
				oldEditor.IsKeyboardFocusWithinChanged -= OnEditorIsKeyboardFocusWithinChanged;
				oldEditor.PropertyViewItemBase = null;
			}

			if (newEditor != null)
			{
				newEditor.IsEditingChanged += OnIsEditingChanged;
				newEditor.GotFocus += OnEditorGotFocus;
				newEditor.LostFocus += OnEditorLostFocus;
				newEditor.IsKeyboardFocusWithinChanged += OnEditorIsKeyboardFocusWithinChanged;
				newEditor.PropertyViewItemBase = this;
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
				Editor = PropertyViewControl.ControllerInternal.RentEditorInternal(newValue);
			}
			else
			{
				Description = null;
				Content = null;

				var editor = Editor;

				if (editor == null)
					return;

				Editor = null;
				PropertyViewControl.ControllerInternal.ReturnEditorInternal(editor);
			}
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

		private void OnPropertyItemValueChanged(object sender, EventArgs e)
		{
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

		private protected override void UpdateZIndex()
		{
			Panel.SetZIndex(this, IsSelected ? 40000 : IsValid ? 30000 : IsMouseOver ? 20000 : 10000 - ActualLevel);
		}

		public int ZIndex => Panel.GetZIndex(this);

		private protected override int CalculateActualLevel()
		{
			return base.CalculateActualLevel() - 1;
		}
	}
}