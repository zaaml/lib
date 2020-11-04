// <copyright file="PropertyEditor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Input;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Editors.Core;

namespace Zaaml.UI.Controls.PropertyView.Editors
{
	[TemplateContractType(typeof(PropertyEditorTemplateContract))]
	public abstract class PropertyEditor : TemplateContractControl
	{
		private static readonly DependencyPropertyKey PropertyItemPropertyKey = DPM.RegisterReadOnly<PropertyItem, PropertyEditor>
			("PropertyItem", default, d => d.OnPropertyItemPropertyChangedPrivate);

		private static readonly DependencyPropertyKey PropertyViewItemPropertyKey = DPM.RegisterReadOnly<PropertyViewItem, PropertyEditor>
			("PropertyViewItem", default, d => d.OnPropertyViewItemPropertyChangedPrivate);

		public static readonly DependencyProperty PropertyViewItemProperty = PropertyViewItemPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey IsEditingPropertyKey = DPM.RegisterReadOnly<bool, PropertyEditor>
			("IsEditing", default, d => d.OnIsEditingPropertyChangedPrivate);

		public static readonly DependencyProperty IsEditingProperty = IsEditingPropertyKey.DependencyProperty;

		public static readonly DependencyProperty PropertyItemProperty = PropertyItemPropertyKey.DependencyProperty;

		public event EventHandler IsEditingChanged;

		internal PropertyViewController Controller { get; private set; }

		protected abstract EditorBase EditorCore { get; }

		public bool IsEditing
		{
			get => (bool) GetValue(IsEditingProperty);
			private set => this.SetReadOnlyValue(IsEditingPropertyKey, value);
		}

		public PropertyItem PropertyItem
		{
			get => (PropertyItem) GetValue(PropertyItemProperty);
			private set => this.SetReadOnlyValue(PropertyItemPropertyKey, value);
		}

		public PropertyViewItem PropertyViewItem
		{
			get => (PropertyViewItem) GetValue(PropertyViewItemProperty);
			internal set => this.SetReadOnlyValue(PropertyViewItemPropertyKey, value);
		}

		public void BeginEdit()
		{
			if (EditorCore == null)
				ApplyTemplate();

			EditorCore?.BeginEdit();
		}

		private void EditorOnEditingEnded(object sender, EditingEndedEventArgs e)
		{
			if (e.Result == EditingResult.Commit)
				OnCommitEdit();
			else
				OnCancelEdit();
		}

		private void EditorOnEditingStarted(object sender, EventArgs e)
		{
			OnBeginEdit();
		}

		protected virtual void HandlePreviewKeyDown(KeyEventArgs keyEventArgs)
		{
		}

		internal void HandlePreviewKeyDownInternal(KeyEventArgs keyEventArgs)
		{
			HandlePreviewKeyDown(keyEventArgs);
		}

		internal void Mount(PropertyItem propertyItem, PropertyViewController controller)
		{
			Controller = controller;
			PropertyItem = propertyItem;
		}

		protected virtual void OnBeginEdit()
		{
			IsEditing = true;
		}

		protected virtual void OnCancelEdit()
		{
			IsEditing = false;

			OnEndEdit();
		}

		protected virtual void OnCommitEdit()
		{
			IsEditing = false;

			OnEndEdit();
		}

		protected virtual void OnEndEdit()
		{
		}

		protected virtual void OnIsEditingChanged()
		{
			IsEditingChanged?.Invoke(this, EventArgs.Empty);
		}

		private void OnIsEditingPropertyChangedPrivate(bool oldValue, bool newValue)
		{
			OnIsEditingChanged();
		}

		protected virtual void OnPropertyItemChanged(PropertyItem oldValue, PropertyItem newValue)
		{
		}

		private void OnPropertyItemPropertyChangedPrivate(PropertyItem oldValue, PropertyItem newValue)
		{
			if (oldValue != null)
			{
				oldValue.ValueChangedInternal -= OnPropertyItemValueChanged;
				oldValue.ValueUpdatedInternal -= OnPropertyItemValueUpdated;
			}

			if (newValue != null)
			{
				newValue.ValueChangedInternal += OnPropertyItemValueChanged;
				newValue.ValueUpdatedInternal += OnPropertyItemValueUpdated;
			}

			OnPropertyItemChanged(oldValue, newValue);
		}

		private void OnPropertyItemValueChanged(object sender, EventArgs e)
		{
			OnPropertyItemValueChanged();
		}

		protected virtual void OnPropertyItemValueChanged()
		{
		}

		private void OnPropertyItemValueUpdated(object sender, EventArgs e)
		{
			OnPropertyItemValueUpdated();
		}

		protected virtual void OnPropertyItemValueUpdated()
		{
		}

		private void OnPropertyViewItemPropertyChangedPrivate(PropertyViewItem oldValue, PropertyViewItem newValue)
		{
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			var editor = EditorCore;

			if (editor != null)
			{
				editor.EditingStarted += EditorOnEditingStarted;
				editor.EditingEnded += EditorOnEditingEnded;
			}
		}

		protected override void OnTemplateContractDetaching()
		{
			var editor = EditorCore;

			if (editor != null)
			{
				editor.EditingStarted -= EditorOnEditingStarted;
				editor.EditingEnded -= EditorOnEditingEnded;
			}

			base.OnTemplateContractDetaching();
		}

		internal void Release()
		{
			PropertyItem = null;
			Controller = null;
		}

		protected void SetValidationError(string validationError)
		{
			PropertyViewItem?.SetValidationErrorInternal(validationError);
		}
	}

	public class PropertyEditorTemplateContract : TemplateContract
	{
	}
}