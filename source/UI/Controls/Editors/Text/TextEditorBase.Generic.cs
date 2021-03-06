// <copyright file="TextEditorBase.Generic.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Editors.Text
{
	public abstract class TextEditorBase<TValue> : TextEditorBase
	{
		public static readonly DependencyProperty ValueProperty = DPM.Register<TValue, TextEditorBase<TValue>>
			("Value", d => d.OnValuePropertyChangedPrivate, d => d.CoerceValueProperty);

		private int _suspendHandlerCount;

		private protected bool IsHandlerSuspended => _suspendHandlerCount > 0;

		private bool SyncingText { get; set; }

		public TValue Value
		{
			get => (TValue) GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		protected override string CoerceText(string text)
		{
			return TryParse(text, out _) ? text : Text;
		}

		private protected virtual TValue CoerceValueCore(TValue value)
		{
			return value;
		}

		private TValue CoerceValueProperty(TValue value)
		{
			return SyncingText ? value : CoerceValueCore(value);
		}

		protected override bool CommitText(string originalText, ref string currentText)
		{
			return SyncValue(currentText);
		}

		public override void EndInit()
		{
			base.EndInit();

			SyncText();
		}

		protected abstract string FormatValue(TValue value);

		protected override void OnTextChanged(string oldValue, string newValue)
		{
			if (IsHandlerSuspended)
				return;

			if (SyncValue(newValue) == false)
			{
			}
		}

		private protected virtual void OnValueChanged(TValue oldValue, TValue newValue)
		{
		}

		private void OnValuePropertyChangedPrivate(TValue oldValue, TValue newValue)
		{
			SyncText();

			OnValueChanged(oldValue, newValue);
		}

		private protected void ResumeHandler()
		{
			if (_suspendHandlerCount == 0)
				throw new InvalidOperationException();

			_suspendHandlerCount--;
		}

		private protected void SuspendHandler()
		{
			_suspendHandlerCount++;
		}

		protected void SyncText()
		{
			if (IsHandlerSuspended)
				return;

			try
			{
				SuspendHandler();

				SyncingText = true;

				SyncTextCore();
			}
			finally
			{
				SyncingText = false;

				ResumeHandler();
			}
		}

		private protected virtual void SyncTextCore()
		{
			Text = FormatValue(Value);
		}

		protected bool SyncValue(string text)
		{
			if (IsHandlerSuspended)
				return false;

			try
			{
				SuspendHandler();

				return SyncValueCore(text);
			}
			finally
			{
				ResumeHandler();
			}
		}

		protected virtual bool SyncValueCore(string text)
		{
			if (TryParse(text, out var value))
			{
				Value = value;

				return true;
			}

			return false;
		}

		protected abstract bool TryParse(string text, out TValue value);
	}
}