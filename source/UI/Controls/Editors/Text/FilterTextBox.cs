// <copyright file="FilterTextBox.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Primitives.TextPrimitives;

namespace Zaaml.UI.Controls.Editors.Text
{
	[TemplateContractType(typeof(FilterTextBoxTemplateContract))]
	public class FilterTextBox : TemplateContractControl
	{
		public static readonly DependencyProperty WatermarkTextProperty = DPM.Register<string, FilterTextBox>
			("WatermarkText");

		public static readonly DependencyProperty WatermarkIconProperty = DPM.Register<ImageSource, FilterTextBox>
			("WatermarkIcon");

		public static readonly DependencyProperty FilterTextProperty = DPM.Register<string, FilterTextBox>
			("FilterText", string.Empty, s => s.OnFilterTextChanged, true);

		public static readonly DependencyProperty DelayProperty = DPM.Register<TimeSpan, FilterTextBox>
			("Delay", TimeSpan.Zero);

		public static readonly DependencyProperty AutoCompleteProperty = DPM.Register<bool, FilterTextBox>
			("AutoComplete", false);

		private static readonly DependencyPropertyKey AutoCompleteTextPropertyKey = DPM.RegisterReadOnly<string, FilterTextBox>
			("AutoCompleteText", s => s.OnAutoCompleteTextChanged);

		public static readonly DependencyProperty IsCaseSensitiveProperty = DPM.Register<bool, FilterTextBox>
			("IsCaseSensitive", s => s.OnIsCaseSensitiveChanged);

		private static readonly DependencyPropertyKey IsInEditStatePropertyKey = DPM.RegisterReadOnly<bool, FilterTextBox>
			("IsInEditState", s => s.OnIsInEditStateChanged);

		public static readonly DependencyProperty TailContentProperty = DPM.Register<object, FilterTextBox>
			("TailContent");

		public static readonly DependencyProperty TailContentTemplateProperty = DPM.Register<DataTemplate, FilterTextBox>
			("TailContentTemplate");

		public static readonly DependencyProperty HeadContentProperty = DPM.Register<object, FilterTextBox>
			("HeadContent");

		public static readonly DependencyProperty HeadContentTemplateProperty = DPM.Register<DataTemplate, FilterTextBox>
			("HeadContentTemplate");

		public static readonly DependencyProperty ShowWatermarkProperty = DPM.Register<bool, FilterTextBox>
			("ShowWatermark", true, s => s.OnShowWatermarkChanged);

		private static readonly DependencyPropertyKey ActualShowWatermarkPropertyKey = DPM.RegisterReadOnly<bool, FilterTextBox>
			("ActualShowWatermark", true);

		public static readonly DependencyProperty ActualShowWatermarkProperty = ActualShowWatermarkPropertyKey.DependencyProperty;
		public static readonly DependencyProperty AutoCompleteTextProperty = AutoCompleteTextPropertyKey.DependencyProperty;
		public static readonly DependencyProperty IsInEditStateProperty = IsInEditStatePropertyKey.DependencyProperty;

		private readonly DelayAction _delayFilter;
		private string _initialText;

		private bool _queueEvaluateAutoComplete;
		private bool _skipAutoCompleteTextChanged;
		private bool _skipOnAutoCompleteTextBoxSearchTextChanged;

		private StringComparison _stringComparison;

		private bool _suspendGotFocusHandler;
		public event EventHandler FilterTextChanged;
		public event EventHandler BeginEdit;
		public event EventHandler EndEdit;

		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler<QueryAutoCompleteTextEventArgs> QueryAutoCompleteText;

		static FilterTextBox()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<FilterTextBox>();
		}

		public FilterTextBox()
		{
			this.OverrideStyleKey<FilterTextBox>();
			UpdateStringComparison();

			_delayFilter = new DelayAction(Filter);
		}

		public bool ActualShowWatermark
		{
			get => (bool) GetValue(ActualShowWatermarkProperty);
			private set => this.SetReadOnlyValue(ActualShowWatermarkPropertyKey, value);
		}

		public bool AutoComplete
		{
			get => (bool) GetValue(AutoCompleteProperty);
			set => SetValue(AutoCompleteProperty, value);
		}

		public string AutoCompleteText
		{
			get => (string) GetValue(AutoCompleteTextProperty);
			private set => this.SetReadOnlyValue(AutoCompleteTextPropertyKey, value);
		}

		private AutoCompleteTextBox AutoCompleteTextBox => TemplateContract.AutoCompleteTextBox;

		public TimeSpan Delay
		{
			get => (TimeSpan) GetValue(DelayProperty);
			set => SetValue(DelayProperty, value);
		}

		public string FilterText
		{
			get => (string) GetValue(FilterTextProperty);
			set => SetValue(FilterTextProperty, value);
		}

		public object HeadContent
		{
			get => GetValue(HeadContentProperty);
			set => SetValue(HeadContentProperty, value);
		}

		public DataTemplate HeadContentTemplate
		{
			get => (DataTemplate) GetValue(HeadContentTemplateProperty);
			set => SetValue(HeadContentTemplateProperty, value);
		}

		public bool IsCaseSensitive
		{
			get => (bool) GetValue(IsCaseSensitiveProperty);
			set => SetValue(IsCaseSensitiveProperty, value);
		}

		public bool IsInEditState
		{
			get => (bool) GetValue(IsInEditStateProperty);
			private set => this.SetReadOnlyValue(IsInEditStatePropertyKey, value);
		}

		public bool ShowWatermark
		{
			get => (bool) GetValue(ShowWatermarkProperty);
			set => SetValue(ShowWatermarkProperty, value);
		}

		public object TailContent
		{
			get => GetValue(TailContentProperty);
			set => SetValue(TailContentProperty, value);
		}

		public DataTemplate TailContentTemplate
		{
			get => (DataTemplate) GetValue(TailContentTemplateProperty);
			set => SetValue(TailContentTemplateProperty, value);
		}

		private FilterTextBoxTemplateContract TemplateContract => (FilterTextBoxTemplateContract) TemplateContractInternal;

		private string TypedText => AutoCompleteTextBox?.TypedText;

		public ImageSource WatermarkIcon
		{
			get => (ImageSource) GetValue(WatermarkIconProperty);
			set => SetValue(WatermarkIconProperty, value);
		}

		public string WatermarkText
		{
			get => (string) GetValue(WatermarkTextProperty);
			set => SetValue(WatermarkTextProperty, value);
		}

		private void CancelEdit()
		{
			FinishEdit(_initialText);
		}

		private void CommitEdit()
		{
			AutoCompleteTextBox?.CommitAutoComplete(true, true);
			FinishEdit(TypedText ?? _initialText);
		}

		private void EvaluateAutoComplete(bool search)
		{
			if (AutoComplete == false || TypedText.IsNullOrEmpty())
			{
				AutoCompleteText = null;
				return;
			}

			if (search == false)
				return;

			AutoCompleteText = OnQueryAutoCompleteText();
		}

		private void Filter()
		{
			if (_queueEvaluateAutoComplete)
				EvaluateAutoComplete(true);

			_queueEvaluateAutoComplete = false;

			UpdateFilterText(AutoCompleteTextBox?.TypedText, true);
		}

		private void FinishEdit(string finalText)
		{
			FilterText = finalText;

			try
			{
				_suspendGotFocusHandler = true;

				if (AutoCompleteTextBox != null && AutoCompleteTextBox.IsKeyboardFocusWithin)
				{
					Keyboard.ClearFocus();
					Focus();
				}
				else
					Focus();
			}
			finally
			{
				_suspendGotFocusHandler = false;
			}
		}

		private bool HandleKey(Key key)
		{
			if (IsInEditState == false)
			{
				switch (key)
				{
					case Key.Down:
					case Key.Up:
					case Key.Left:
					case Key.Right:
					case Key.Escape:
						return true;
					default:
						return false;
				}
			}

			switch (key)
			{
				case Key.Enter:

					CommitEdit();

					return true;

				case Key.Escape:

					CancelEdit();

					return true;

				default:
					return false;
			}
		}

		private void OnAutoCompleteTextBoxAutoCompleteTextChanged(object sender, EventArgs eventArgs)
		{
			if (_skipAutoCompleteTextChanged)
				return;

			AutoCompleteText = AutoCompleteTextBox.AutoCompleteText;
		}

		private void OnAutoCompleteTextBoxGotFocus(object sender, RoutedEventArgs routedEventArgs)
		{
			OnBeginEditInt();
		}

		private void OnAutoCompleteTextBoxLostFocus(object sender, RoutedEventArgs routedEventArgs)
		{
			UpdateFilterText(TypedText ?? _initialText, true);
			OnEndEditInt();
		}

		private void OnAutoCompleteTextBoxPreviewTextInput(object sender, TextCompositionEventArgs textCompositionEventArgs)
		{
			_delayFilter.Revoke();
		}

		private void OnAutoCompleteTextBoxTypedTextChanged(object sender, EventArgs e)
		{
			if (_skipOnAutoCompleteTextBoxSearchTextChanged)
				return;

			_queueEvaluateAutoComplete = string.Equals(AutoCompleteText, AutoCompleteTextBox.TypedText, _stringComparison) == false;

			EvaluateAutoComplete(false);

			_delayFilter.Invoke(Delay);
		}

		private void OnAutoCompleteTextChanged()
		{
			_skipAutoCompleteTextChanged = true;
			AutoCompleteTextBox.AutoCompleteText = AutoCompleteText;
			_skipAutoCompleteTextChanged = false;
		}

		protected virtual void OnBeginEdit()
		{
			BeginEdit?.Invoke(this, EventArgs.Empty);
		}

		private void OnBeginEditInt()
		{
			_initialText = AutoCompleteTextBox?.Text ?? FilterText;

			IsInEditState = true;

			OnBeginEdit();

			UpdateWatermark();
		}

		protected virtual void OnEndEdit()
		{
			EndEdit?.Invoke(this, EventArgs.Empty);
		}

		private void OnEndEditInt()
		{
			_delayFilter.Revoke();

			AutoCompleteText = null;

			OnEndEdit();

			IsInEditState = false;

			UpdateWatermark();
		}

		private void OnFilterTextChanged()
		{
			UpdateAutoCompleteTextBox(FilterText);
		}

		protected override void OnGotFocus(RoutedEventArgs e)
		{
			base.OnGotFocus(e);

			if (_suspendGotFocusHandler)
				return;

			UpdateFocus();
		}

		private void OnIsCaseSensitiveChanged()
		{
			UpdateStringComparison();
		}

		private void OnIsInEditStateChanged()
		{
			UpdateVisualState(true);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			var focusedElement = FocusHelper.GetFocusedElement();

			if (ReferenceEquals(this, focusedElement))
			{
				AutoCompleteTextBox.Focus();

				e.Handled = true;
			}
			else
				e.Handled = HandleKey(e.Key);
		}

#if !SILVERLIGHT
		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			e.Handled = HandleKey(e.Key);

			if (e.Handled == false)
				base.OnPreviewKeyDown(e);
		}
#endif

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private string OnQueryAutoCompleteText()
		{
			var handler = QueryAutoCompleteText;

			if (handler == null)
				return null;

			var args = new QueryAutoCompleteTextEventArgs(TypedText);

			handler(this, args);

			return args.AutoCompleteText;
		}

		private void OnShowWatermarkChanged()
		{
			UpdateWatermark();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			AutoCompleteTextBox.TypedTextChanged += OnAutoCompleteTextBoxTypedTextChanged;
			AutoCompleteTextBox.AutoCompleteTextChanged += OnAutoCompleteTextBoxAutoCompleteTextChanged;
			AutoCompleteTextBox.TextInput += OnAutoCompleteTextBoxPreviewTextInput;

			AutoCompleteTextBox.SuspendOnDeletion = true;

#if SILVERLIGHT
			AutoCompleteTextBox.LostFocus += OnAutoCompleteTextBoxLostFocus;
			AutoCompleteTextBox.GotFocus += OnAutoCompleteTextBoxGotFocus;
#else
			AutoCompleteTextBox.LostKeyboardFocus += OnAutoCompleteTextBoxLostFocus;
			AutoCompleteTextBox.GotKeyboardFocus += OnAutoCompleteTextBoxGotFocus;
#endif

			UpdateAutoCompleteTextBox(FilterText);

			UpdateFocus();
		}

		protected override void OnTemplateContractDetaching()
		{
			AutoCompleteTextBox.TypedTextChanged -= OnAutoCompleteTextBoxTypedTextChanged;
			AutoCompleteTextBox.AutoCompleteTextChanged -= OnAutoCompleteTextBoxAutoCompleteTextChanged;
			AutoCompleteTextBox.TextInput -= OnAutoCompleteTextBoxPreviewTextInput;

			AutoCompleteTextBox.SuspendOnDeletion = true;

#if SILVERLIGHT
			AutoCompleteTextBox.LostFocus -= OnAutoCompleteTextBoxLostFocus;
			AutoCompleteTextBox.GotFocus -= OnAutoCompleteTextBoxGotFocus;
#else
			AutoCompleteTextBox.LostKeyboardFocus -= OnAutoCompleteTextBoxLostFocus;
			AutoCompleteTextBox.GotKeyboardFocus -= OnAutoCompleteTextBoxGotFocus;
#endif

			base.OnTemplateContractDetaching();
		}

		private void UpdateAutoCompleteTextBox(string text)
		{
			var skipOnFilterTextBoxTextChanged = _skipOnAutoCompleteTextBoxSearchTextChanged;

			try
			{
				if (_skipOnAutoCompleteTextBoxSearchTextChanged == false)
					_skipOnAutoCompleteTextBoxSearchTextChanged = true;

				if (AutoCompleteTextBox != null)
					AutoCompleteTextBox.TypedText = text;
			}
			finally
			{
				_skipOnAutoCompleteTextBoxSearchTextChanged = skipOnFilterTextBoxTextChanged;
			}
		}

		private void UpdateFilterText(string text, bool raiseEvent)
		{
			if (string.Equals(FilterText, text))
				return;

			this.SetValue(FilterTextProperty, text, true);

			if (raiseEvent)
				FilterTextChanged?.Invoke(this, EventArgs.Empty);
		}

		private void UpdateFocus()
		{
			if (AutoCompleteTextBox == null)
				return;

			if (ReferenceEquals(FocusHelper.GetKeyboardFocusedElement(), this))
				FocusHelper.SetKeyboardFocusedElement(AutoCompleteTextBox);
		}

		private void UpdateStringComparison()
		{
			_stringComparison = IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
		}

		protected override void UpdateVisualState(bool useTransitions)
		{
			base.UpdateVisualState(useTransitions);

			GotoVisualState(IsInEditState ? CommonVisualStates.Edit : CommonVisualStates.Display, useTransitions);
		}

		private void UpdateWatermark()
		{
			ActualShowWatermark = ShowWatermark && IsInEditState == false && string.IsNullOrEmpty(AutoCompleteTextBox?.Text);
		}
	}

	public class FilterTextBoxTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = true)]
		public AutoCompleteTextBox AutoCompleteTextBox { get; private set; }
	}
}