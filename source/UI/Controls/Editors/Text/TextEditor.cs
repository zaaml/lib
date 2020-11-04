// <copyright file="TextEditor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Input;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Primitives.TextPrimitives;

namespace Zaaml.UI.Controls.Editors.Text
{
	[TemplateContractType(typeof(TextEditorTemplateContract))]
	public class TextEditor : TextEditorBase
	{
		public static readonly DependencyProperty TextProperty = DPM.Register<string, TextEditor>
			("Text", string.Empty, d => d.OnTextPropertyChangedPrivate);

		private static readonly DependencyPropertyKey OriginalTextPropertyKey = DPM.RegisterReadOnly<string, TextEditor>
			("OriginalText", string.Empty);

		public static readonly DependencyProperty UpdateModeProperty = DPM.Register<TextEditorUpdateMode, TextEditor>
			("UpdateMode", TextEditorUpdateMode.Explicit, d => d.OnUpdateModePropertyChangedPrivate);

		public static readonly DependencyProperty OriginalTextProperty = OriginalTextPropertyKey.DependencyProperty;

		private bool _suspendKeyboardFocusWithinChanged;

		private bool _suspendSyncText;

		static TextEditor()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<TextEditor>();
		}

		public TextEditor()
		{
			this.OverrideStyleKey<TextEditor>();
		}

		private AutoCompleteTextBox AutoCompleteTextBox => TemplateContract.AutoCompleteTextBox;

		public string OriginalText
		{
			get => (string) GetValue(OriginalTextProperty);
			private set => this.SetReadOnlyValue(OriginalTextPropertyKey, value);
		}

		private TextEditorTemplateContract TemplateContract => (TextEditorTemplateContract) TemplateContractInternal;

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
				CommitEdit();
		}

		private protected override void EnterEditState()
		{
			AutoCompleteTextBox?.Focus();
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
		}

		protected override void OnBeginEdit()
		{
			SyncOriginalText();

			base.OnBeginEdit();
		}

		protected override bool OnCancelEdit()
		{
			SyncAutocompleteText(OriginalText);
			SyncText();

			return true;
		}

		protected override bool OnCommitEdit()
		{
			SyncText();

			return true;
		}

		private void OnInputTextChanged(object sender, EventArgs e)
		{
			if (_suspendSyncText)
				return;

			if (UpdateMode == TextEditorUpdateMode.PropertyChanged)
				SyncText();
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

		private void OnTextPropertyChangedPrivate(string oldValue, string newValue)
		{
			if (IsEditing == false)
				SyncOriginalText();

			SyncAutocompleteText();
		}

		private void OnUpdateModePropertyChangedPrivate(TextEditorUpdateMode oldValue, TextEditorUpdateMode newValue)
		{
			SyncText();
		}

		private void SyncAutocompleteText(string text = null)
		{
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

		private void SyncOriginalText()
		{
			OriginalText = Text;
		}

		private void SyncText()
		{
			if (AutoCompleteTextBox == null)
				return;

			Text = AutoCompleteTextBox.TypedText;
		}
	}

	public class TextEditorTemplateContract : TextEditorBaseTemplateContract
	{
		[TemplateContractPart(Required = true)]
		public AutoCompleteTextBox AutoCompleteTextBox { get; private set; }
	}
}