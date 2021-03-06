// <copyright file="TextEditorBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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
using Zaaml.UI.Controls.Editors.Core;
using Zaaml.UI.Controls.Primitives.TextPrimitives;

namespace Zaaml.UI.Controls.Editors.Text
{
	[TemplateContractType(typeof(TextEditorBaseTemplateContract))]
	public abstract class TextEditorBase : EditorBase
	{
		public static readonly DependencyProperty TextProperty = DPM.Register<string, TextEditorBase>
			("Text", string.Empty, d => d.OnTextPropertyChangedPrivate, d => d.CoerceTextPropertyPrivate);

		private static readonly DependencyPropertyKey OriginalTextPropertyKey = DPM.RegisterReadOnly<string, TextEditorBase>
			("OriginalText", string.Empty);

		public static readonly DependencyProperty UpdateModeProperty = DPM.Register<TextEditorUpdateMode, TextEditorBase>
			("UpdateMode", TextEditorUpdateMode.Explicit, d => d.OnUpdateModePropertyChangedPrivate);

		public static readonly DependencyProperty OriginalTextProperty = OriginalTextPropertyKey.DependencyProperty;

		private bool _suspendKeyboardFocusWithinChanged;
		private bool _suspendSyncText;

		static TextEditorBase()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<TextEditorBase>();
		}

		protected TextEditorBase()
		{
			this.OverrideStyleKey<TextEditorBase>();
		}

		private AutoCompleteTextBox AutoCompleteTextBox => TemplateContract.AutoCompleteTextBox;

		private TextBlock DisplayTextBlock => TemplateContract.DisplayTextBlock;

		private bool InSyncText { get; set; }

		private bool IsTextBoxVisible => AutoCompleteTextBox != null && AutoCompleteTextBox.Visibility == Visibility.Visible;

		public string OriginalText
		{
			get => (string) GetValue(OriginalTextProperty);
			private set => this.SetReadOnlyValue(OriginalTextPropertyKey, value);
		}

		protected virtual bool StaysEditingOnCommit => false;

		private TextEditorBaseTemplateContract TemplateContract => (TextEditorBaseTemplateContract) TemplateContractInternal;

		public string Text
		{
			get => (string) GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		public TextEditorUpdateMode UpdateMode
		{
			get => (TextEditorUpdateMode) GetValue(UpdateModeProperty);
			set => SetValue(UpdateModeProperty, value);
		}

		private void AutoCompleteTextBoxOnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (_suspendKeyboardFocusWithinChanged)
				return;

			if (IsEditing == false)
				BeginEdit();
		}

		private void AutoCompleteTextBoxOnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (_suspendKeyboardFocusWithinChanged)
				return;

			if (IsEditing)
				CommitEditCore(true);
		}

		private void CancelTypedText()
		{
			SyncAutocompleteText(OriginalText);
			SyncText();
		}

		protected virtual string CoerceText(string text)
		{
			return text;
		}

		private string CoerceTextPropertyPrivate(string text)
		{
			return CoerceText(text);
		}

		protected virtual bool CommitText(string originalText, ref string currentText)
		{
			return true;
		}

		private bool EnsureTemplate()
		{
			if (TemplateContract.IsAttached)
				return true;

			ApplyTemplate();

			return TemplateContract.IsAttached;
		}

		private protected override void EnterEditState()
		{
			ShowTextBox();

			AutoCompleteTextBox?.Focus();
		}

		private void HideTextBox()
		{
			if (TemplateContract.IsAttached == false)
				return;

			if (IsTextBoxVisible == false)
				return;

			DisplayTextBlock.Visibility = Visibility.Visible;
			AutoCompleteTextBox.Visibility = Visibility.Collapsed;

			SyncDisplayText();
		}

		private protected override void LeaveEditState()
		{
			if (IsKeyboardFocusWithin)
			{
				_suspendKeyboardFocusWithinChanged = true;

				Keyboard.ClearFocus();
				Focus();

				_suspendKeyboardFocusWithinChanged = false;
			}

			HideTextBox();
		}

		protected override void OnBeginEdit()
		{
			SyncOriginalText();

			base.OnBeginEdit();
		}

		protected override bool OnCancelEdit()
		{
			CancelTypedText();

			return true;
		}

		protected override bool OnCommitEdit()
		{
			if (AutoCompleteTextBox == null)
				return true;

			var originalText = OriginalText;
			var typedText = AutoCompleteTextBox.TypedText;
			var currentText = typedText;

			if (string.Equals(originalText, currentText, StringComparison.OrdinalIgnoreCase))
				return StaysEditingOnCommit == false;

			if (CommitText(originalText, ref currentText))
			{
				if (string.Equals(typedText, currentText) == false)
					SyncAutocompleteText(currentText);

				SyncText();
				SyncOriginalText();
			}
			else
				CancelTypedText();

			return StaysEditingOnCommit == false;
		}

		private void OnInputTextChanged(object sender, EventArgs e)
		{
			if (_suspendSyncText)
				return;

			if (UpdateMode == TextEditorUpdateMode.PropertyChanged)
				SyncText();
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);

			if (IsEditing == false)
				ShowTextBox();
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			if (IsEditing == false)
				HideTextBox();

			base.OnMouseLeave(e);
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Enter:

					e.Handled = true;

					CommitEdit();

					break;
				case Key.Escape:

					e.Handled = true;

					CancelEdit();

					break;

				default:

					base.OnPreviewKeyDown(e);

					break;
			}
		}

		protected override void OnPreviewTextInput(TextCompositionEventArgs e)
		{
			if (IsEditing == false)
				BeginEdit();

			base.OnPreviewTextInput(e);
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			AutoCompleteTextBox.TypedTextChanged += OnInputTextChanged;
			AutoCompleteTextBox.GotKeyboardFocus += AutoCompleteTextBoxOnGotKeyboardFocus;
			AutoCompleteTextBox.LostKeyboardFocus += AutoCompleteTextBoxOnLostKeyboardFocus;

			SyncAutocompleteText();
		}

		protected override void OnTemplateContractDetaching()
		{
			AutoCompleteTextBox.LostKeyboardFocus -= AutoCompleteTextBoxOnLostKeyboardFocus;
			AutoCompleteTextBox.GotKeyboardFocus -= AutoCompleteTextBoxOnGotKeyboardFocus;
			AutoCompleteTextBox.TypedTextChanged -= OnInputTextChanged;

			base.OnTemplateContractDetaching();
		}

		protected virtual void OnTextChanged(string oldValue, string newValue)
		{
		}

		private void OnTextPropertyChangedPrivate(string oldValue, string newValue)
		{
			if (IsEditing == false || InSyncText == false)
				SyncOriginalText();

			SyncDisplayText();
			SyncAutocompleteText();

			OnTextChanged(oldValue, newValue);
		}

		private void OnUpdateModePropertyChangedPrivate(TextEditorUpdateMode oldValue, TextEditorUpdateMode newValue)
		{
			SyncText();
		}

		private void ShowTextBox()
		{
			if (EnsureTemplate() == false)
				return;

			if (IsTextBoxVisible)
				return;

			DisplayTextBlock.Visibility = Visibility.Collapsed;
			AutoCompleteTextBox.Visibility = Visibility.Visible;

			SyncAutocompleteText();
		}

		private void SyncAutocompleteText(string text = null)
		{
			if (IsTextBoxVisible == false)
			{
				SyncDisplayText();

				return;
			}

			if (AutoCompleteTextBox == null)
				return;

			try
			{
				_suspendSyncText = true;

				AutoCompleteTextBox.TypedText = text ?? Text;
			}
			finally
			{
				_suspendSyncText = false;
			}
		}

		private void SyncDisplayText(string text = null)
		{
			if (DisplayTextBlock == null)
				return;

			DisplayTextBlock.Text = text ?? Text;
		}

		private void SyncOriginalText()
		{
			OriginalText = Text;
		}

		private void SyncText()
		{
			if (InSyncText)
				return;

			try
			{
				InSyncText = true;

				if (AutoCompleteTextBox == null)
					return;

				SetCurrentValue(TextProperty, AutoCompleteTextBox.TypedText);
			}
			finally
			{
				InSyncText = false;
			}
		}
	}

	public class TextEditorBaseTemplateContract : EditorBaseTemplateContract
	{
		[TemplateContractPart(Required = true)]
		public AutoCompleteTextBox AutoCompleteTextBox { get; private set; }

		[TemplateContractPart(Required = true)]
		public TextBlock DisplayTextBlock { get; private set; }
	}
}