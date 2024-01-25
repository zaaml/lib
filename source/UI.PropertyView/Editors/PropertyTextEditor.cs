// <copyright file="PropertyTextEditor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Editors.Core;
using Zaaml.UI.Controls.Editors.Text;

namespace Zaaml.UI.Controls.PropertyView.Editors
{
	[TemplateContractType(typeof(PropertyTextEditorTemplateContract))]
	public class PropertyTextEditor : PropertyEditor
	{
		public static readonly DependencyProperty TextValueProperty = DPM.Register<string, PropertyTextEditor>
			("TextValue", default, d => d.OnTextValuePropertyChangedPrivate);

		public static readonly DependencyProperty UpdateModeProperty = DPM.Register<TextEditorUpdateMode, PropertyTextEditor>
			("UpdateMode", TextEditorUpdateMode.Explicit);

		private bool _suspendTextValueChangedHandler;

		static PropertyTextEditor()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<PropertyTextEditor>();
		}

		public PropertyTextEditor()
		{
			this.OverrideStyleKey<PropertyTextEditor>();
		}

		private TextEditor Editor => TemplateContract.Editor;

		protected override EditorBase EditorCore => Editor;

		protected PropertyStringConverter StringConverter { get; private set; }

		private PropertyTextEditorTemplateContract TemplateContract => (PropertyTextEditorTemplateContract)TemplateContractCore;

		public string TextValue
		{
			get => (string) GetValue(TextValueProperty);
			set => SetValue(TextValueProperty, value);
		}

		public TextEditorUpdateMode UpdateMode
		{
			get => (TextEditorUpdateMode) GetValue(UpdateModeProperty);
			set => SetValue(UpdateModeProperty, value);
		}

		private void ApplyCurrentValue()
		{
			try
			{
				if (StringConverter == null)
					return;

				var textValue = TextValue;

				if (string.IsNullOrEmpty(textValue))
					PropertyItem.ResetValue();
				else
					StringConverter.SetStringValue(PropertyItem, textValue);

				SetValidationError(null);
			}
			catch (Exception e)
			{
				SetValidationError(e.Message);
			}
		}

		private string GetTextValue()
		{
			var propertyItem = PropertyItem;

			if (propertyItem == null)
				return string.Empty;

			return StringConverter?.GetStringValue(propertyItem) ?? propertyItem.RawValueInternal?.ToString() ?? string.Empty;
		}

		protected override void HandlePreviewKeyDown(KeyEventArgs keyEventArgs)
		{
			if (PropertyItem == null || IsEditing)
				return;

			if (keyEventArgs.Key == Key.Delete)
			{
				keyEventArgs.Handled = true;

				try
				{
					PropertyItem.ResetValue();

					SetValidationError(null);
				}
				catch (Exception e)
				{
					SetValidationError(e.Message);
				}
				finally
				{
					UpdateAfterEdit();
				}
			}

			base.HandlePreviewKeyDown(keyEventArgs);
		}

		protected override void OnEndEdit()
		{
			base.OnEndEdit();

			UpdateAfterEdit();
		}

		protected override void OnPropertyItemChanged(PropertyItem oldValue, PropertyItem newValue)
		{
			base.OnPropertyItemChanged(oldValue, newValue);

			StringConverter = newValue != null ? Controller.GetStringConverterInternal(newValue) : null;

			UpdateTextValue();
			UpdateEditor();
		}

		protected override void OnPropertyItemValueChanged()
		{
			base.OnPropertyItemValueChanged();

			UpdateTextValue();
		}

		protected override void OnPropertyItemValueUpdated()
		{
			base.OnPropertyItemValueChanged();

			UpdateTextValue();
		}

		private void OnTextValuePropertyChangedPrivate(string oldValue, string newValue)
		{
			if (_suspendTextValueChangedHandler)
				return;

			ApplyCurrentValue();
		}

		private void UpdateAfterEdit()
		{
			if (PropertyViewItemBase != null && PropertyViewItemBase.HasValidationError() == false)
				UpdateTextValue();
		}

		private void UpdateEditor()
		{
			if (Editor == null)
				return;

			Editor.IsReadOnly = PropertyItem?.IsReadOnly ?? true;
		}

		private void UpdateTextValue()
		{
			try
			{
				_suspendTextValueChangedHandler = true;

				TextValue = GetTextValue();

				SetValidationError(null);
			}
			finally
			{
				_suspendTextValueChangedHandler = false;
			}
		}
	}

	public class PropertyTextEditorTemplateContract : PropertyEditorTemplateContract
	{
		[TemplateContractPart(Required = true)]
		public TextEditor Editor { get; [UsedImplicitly] private set; }
	}
}