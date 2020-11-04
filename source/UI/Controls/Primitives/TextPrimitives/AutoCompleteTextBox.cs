// <copyright file="AutoCompleteTextBox.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Primitives.TextPrimitives
{
  public class AutoCompleteTextBox : WatermarkTextBox
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty AutoCompleteTextProperty = DPM.Register<string, AutoCompleteTextBox>
      ("AutoCompleteText", a => a.OnAutoCompleteTextChanged);

    public static readonly DependencyProperty TypedTextProperty = DPM.Register<string, AutoCompleteTextBox>
      ("TypedText", string.Empty, s => s.OnTypedTextChanged, true);

    public static readonly DependencyProperty DelayProperty = DPM.Register<TimeSpan, AutoCompleteTextBox>
      ("Delay", TimeSpan.Zero);

    #endregion

    #region Fields

    private bool _skipOnFilterSelectionChanged;
    private bool _skipOnFilterTextBoxTextChanged;
    private readonly DelayAction _delayEvaluateAutoComplete;

    public event EventHandler<QueryAutoCompleteTextEventArgs> QueryAutoCompleteText;
    public event EventHandler TypedTextChanged;
    public event EventHandler AutoCompleteTextChanged;

    #endregion

    #region Ctors

    public AutoCompleteTextBox()
    {
      _delayEvaluateAutoComplete = new DelayAction(EvaluateAutoComplete);

      TextChanged += OnTextChanged;
      SelectionChanged += OnSelectionChanged;
    }

    #endregion

    #region Properties

    public string AutoCompleteText
    {
      get => (string) GetValue(AutoCompleteTextProperty);
      set => SetValue(AutoCompleteTextProperty, value);
    }

    internal bool IsInAutoCompleteState
    {
      get
      {
        if (AutoCompleteText == null)
          return false;

        if (SelectionLength == 0)
          return false;

        if (Text.StartsWith(TypedText, StringComparison.OrdinalIgnoreCase) == false)
          return false;

        if (Text.EndsWith(SelectedText, StringComparison.OrdinalIgnoreCase) == false)
          return false;

        if (TypedText.Length + SelectionLength != Text.Length)
          return false;

        return true;
      }
    }

    public string TypedText
    {
      get => (string) GetValue(TypedTextProperty);
      set => SetValue(TypedTextProperty, value);
    }

    public TimeSpan Delay
    {
      get => (TimeSpan) GetValue(DelayProperty);
      set => SetValue(DelayProperty, value);
    }

    #endregion

    #region  Methods

    private void DelayEvaluateAutoComplete()
    {
      _delayEvaluateAutoComplete.Invoke(Delay);
    }

    private void EvaluateAutoComplete()
    {
      if (string.IsNullOrEmpty(TypedText))
        AutoCompleteText = null;
      else
        OnQueryAutoCompleteText();
    }

    private void OnAutoCompleteTextChanged()
    {
      UpdateAutoCompleteTextBox();
      AutoCompleteTextChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnQueryAutoCompleteText()
    {
      var handler = QueryAutoCompleteText;
      if (handler == null)
        return;

      var args = new QueryAutoCompleteTextEventArgs(TypedText);
      handler(this, args);

      AutoCompleteText = args.AutoCompleteText;
    }

    private void OnTypedTextChanged()
    {
      UpdateTextBox(TypedText);

      TypedTextChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnSelectionChanged(object sender, RoutedEventArgs routedEventArgs)
    {
      if (_skipOnFilterSelectionChanged)
        return;

      CommitAutoComplete(false, true);
    }

    internal void CommitAutoComplete(bool forceSelection, bool raiseEvent)
    {
      var typedText = TypedText;
      try
      {
        if (SelectionLength == 0 || forceSelection)
          UpdateTypedText(Text);

        if (forceSelection)
          MoveCaretToEnd(true);
      }
      finally
      {
        RaiseTypedTextChanged(typedText);
      }
    }

    private void MoveCaretToEnd(bool suspend = false)
    {
      Select(Text.Length, 0, suspend);
    }

    private void RaiseTypedTextChanged(string oldTypedText)
    {
      if (Equals(oldTypedText, TypedText) == false)
        TypedTextChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
      var oldTypedText = TypedText;

      try
      {
        if (_skipOnFilterTextBoxTextChanged)
          return;

        if (string.Equals(TypedText, Text, StringComparison.OrdinalIgnoreCase))
          return;

        if (string.Equals(Text, AutoCompleteText, StringComparison.OrdinalIgnoreCase) && string.Equals(TypedText, AutoCompleteText, StringComparison.OrdinalIgnoreCase) == false)
        {
          if (SelectionLength == 0)
            UpdateTypedText(Text);

          return;
        }

        UpdateTypedText(Text);
        DelayEvaluateAutoComplete();
        UpdateAutoCompleteTextBox();
      }
      finally
      {
        RaiseTypedTextChanged(oldTypedText);
      }
    }

    private void UpdateAutoCompleteTextBox()
    {
      if (AutoCompleteText == null || AutoCompleteText.StartsWith(TypedText, StringComparison.OrdinalIgnoreCase) == false)
        return;

      if (AutoCompleteText.Length == TypedText.Length || _suspendAutoComplete)
      {
        if (string.Equals(Text, TypedText, StringComparison.Ordinal) == false)
        {
          UpdateTextBox(TypedText, true);
          MoveCaretToEnd(_suspendAutoComplete);
        }
        return;
      }

      var selectStart = TypedText.Length;
      var selectLength = AutoCompleteText.Length - TypedText.Length;

      UpdateTextBox(TypedText + AutoCompleteText.Right(selectStart), true);
      Select(selectStart, selectLength, true);
    }

    private void Select(int start, int length, bool suspend)
    {
      var skipOnFilterSelectionChanged = _skipOnFilterSelectionChanged;
      _skipOnFilterSelectionChanged = suspend;

      Select(start, length);

      _skipOnFilterSelectionChanged = skipOnFilterSelectionChanged;
    }

    private void UpdateTypedText(string text)
    {
      this.SetValue(TypedTextProperty, text, true);
    }

    private void UpdateTextBox(string text, bool suspendHandler = false)
    {
      var skipOnFilterTextBoxTextChanged = _skipOnFilterTextBoxTextChanged;
      var skipOnFilterSelectionChanged = _skipOnFilterSelectionChanged;

      try
      {
        if (suspendHandler)
        {
          _skipOnFilterTextBoxTextChanged = true;
          _skipOnFilterSelectionChanged = true;
        }

        Text = text;
      }
      finally
      {
        _skipOnFilterTextBoxTextChanged = skipOnFilterTextBoxTextChanged;
        _skipOnFilterSelectionChanged = skipOnFilterSelectionChanged;
      }
    }

#if SILVERLIGHT
		protected override void OnKeyDown(KeyEventArgs e)
		{
			PreHandleKeyDown(e);

			if (e.Handled == false)
				base.OnKeyDown(e);

			PostHandleKeyDown(e);
		}
#else
    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
      PreHandleKeyDown(e);

      if (e.Handled == false)
        base.OnPreviewKeyDown(e);

      PostHandleKeyDown(e);
    }
#endif

    internal bool SuspendOnDeletion { get; set; } = true;

    private bool _suspendAutoComplete;

    protected virtual void PreHandleKeyDown(KeyEventArgs e)
    {
      _suspendAutoComplete = e.Key == Key.Back || e.Key == Key.Delete;

      if (SuspendOnDeletion && _suspendAutoComplete)
        return;

      if (e.Key == Key.Back)
        ExitAutoCompleteState();
      else if (e.Key == Key.Delete && IsInAutoCompleteState)
        e.Handled = true;

      _suspendAutoComplete = false;

      if (e.Key == Key.Tab)
      {
        if (IsInAutoCompleteState)
        {
          CommitAutoComplete(true, true);

          e.Handled = true;
        }
      }
    }

    protected virtual void PostHandleKeyDown(KeyEventArgs e)
    {
    }

    private void ExitAutoCompleteState()
    {
      if (IsInAutoCompleteState == false)
        return;

      var selectionStart = SelectionStart;

      _skipOnFilterSelectionChanged = true;

      UpdateTextBox(TypedText, true);
      Select(selectionStart, 0);

      _skipOnFilterSelectionChanged = false;
    }

    #endregion

#if SILVERLIGHT
		protected override void OnGotFocus(RoutedEventArgs e)
		{
			base.OnGotFocus(e);
			IsFocused = true;
		}

		public bool IsFocused { get; set; }

		protected override void OnLostFocus(RoutedEventArgs e)
		{
			base.OnLostFocus(e);
			IsFocused = false;
		}
#endif
  }
}