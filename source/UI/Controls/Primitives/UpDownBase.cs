// <copyright file="UpDownBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Control = Zaaml.UI.Controls.Core.Control;

namespace Zaaml.UI.Controls.Primitives
{
  public abstract class UpDownBase : Control
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty IsEditableProperty = DPM.Register<bool, UpDownBase>
      ("IsEditable", true);

    #endregion

    #region Fields

    private readonly UpDownBaseTemplateContract _templateContract;
    private string _text;

    #endregion

    #region Ctors

    protected UpDownBase()
    {
      _templateContract = new UpDownBaseTemplateContract(GetTemplateChild, OnTemplateContractAttach, OnTemplateContractDetach);
    }

    #endregion

    #region Properties

    public bool IsEditable
    {
	    get => (bool)GetValue(IsEditableProperty);
	    set => SetValue(IsEditableProperty, value.Box());
    }

    protected TextBox TextBox => _templateContract.TextBox;

    protected Spinner Spinner => _templateContract.Spinner;

    #endregion

    #region  Methods

    private void OnTemplateContractDetach()
    {
      TextBox.TextInput -= TextBoxOnTextInput;
      TextBox.GotFocus -= TextBoxOnGotFocus;
      TextBox.LostFocus -= TextBoxOnLostFocus;
      Spinner.Spin -= SpinnerOnSpin;
    }

    private void OnTemplateContractAttach()
    {
      TextBox.TextInput += TextBoxOnTextInput;
      TextBox.GotFocus += TextBoxOnGotFocus;
      TextBox.LostFocus += TextBoxOnLostFocus;
      Spinner.Spin += SpinnerOnSpin;
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      _templateContract.Attach();
    }

    private void SpinnerOnSpin(object sender, SpinEventArgs spinEventArgs)
    {
      ProcessSpin(spinEventArgs.Direction);
    }

    private void ProcessSpin(SpinDirection direction)
    {
      ProcessInput();
      switch (direction)
      {
        case SpinDirection.Increase:
          OnIncrement();
          break;
        case SpinDirection.Decrease:
          OnDecrement();
          break;
      }
    }

    private void TextBoxOnTextInput(object sender, TextCompositionEventArgs textCompositionEventArgs)
    {
    }

    private void TextBoxOnLostFocus(object sender, RoutedEventArgs routedEventArgs)
    {
      ProcessInput();
    }

    private void TextBoxOnGotFocus(object sender, RoutedEventArgs routedEventArgs)
    {
      SelectAllText();
    }

    protected virtual void SelectAllText()
    {
      if (TextBox?.SelectionLength == 0 && TextBox.Text != null)
        TextBox.Select(0, TextBox.Text.Length);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      base.OnKeyDown(e);

      if (e.Handled)
        return;

      switch (e.Key)
      {
        case Key.Up:
          ProcessSpin(SpinDirection.Increase);
          e.Handled = true;
          break;

        case Key.Down:
          ProcessSpin(SpinDirection.Decrease);
          e.Handled = true;
          break;

        case Key.Enter:
          ProcessInput();
          e.Handled = true;
          break;
      }
    }

#if SILVERLIGHT
		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			ProcessMouseWheel(e);
		}
#else
    protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
    {
      ProcessMouseWheel(e);
    }
#endif

    private void ProcessMouseWheel(MouseWheelEventArgs e)
    {
      ProcessSpin(e.Delta < 0 ? SpinDirection.Decrease : SpinDirection.Increase);
      e.Handled = true;
    }

    private void ProcessInput()
    {
      if (TextBox == null || string.Compare(_text, TextBox.Text, StringComparison.CurrentCulture) == 0) return;

      var caretPosition = TextBox.SelectionStart;

      _text = TextBox.Text;
      ApplyValue(_text);

      if (caretPosition < TextBox.Text.Length)
        TextBox.SelectionStart = caretPosition;
    }

    protected void SetTextBoxText(string formattedValue)
    {
      if (TextBox == null) return;

      _text = formattedValue ?? string.Empty;
      TextBox.Text = _text;

      // always move cursor to the right.
      TextBox.SelectionStart = _text.Length;
    }

    protected internal abstract void ApplyValue(string text);

    protected abstract void OnIncrement();
    protected abstract void OnDecrement();

    #endregion
  }

  public abstract class UpDownBase<T> : UpDownBase, ISupportInitialize
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty ValueProperty = DPM.Register<T, UpDownBase<T>>
      ("Value", default(T), u => u.OnValueChanged, u => u.CoerceValue);

    #endregion

    #region Fields

    private bool _ignoreValueChange;

    public event EventHandler<PropertyValueChangedEventArgs<T>> ValueChanged;
    public event EventHandler<PropertyChangingEventArgsXm<T>> ValueChanging;

    public event EventHandler<UpDownParsingEventArgs<T>> Parsing;
    public event EventHandler<UpDownParseErrorEventArgs> ParseError;

    #endregion

    #region Properties

    protected bool IsInitializing { get; private set; }

    public T Value
    {
      get => (T) GetValue(ValueProperty);
      set => SetValue(ValueProperty, value);
    }

    #endregion

    #region  Methods

    protected internal override void ApplyValue(string text)
    {
      var parsingArgs = new UpDownParsingEventArgs<T>(text);

      Exception parsingException = null;
      var regularParsedValue = default(T);
      try
      {
        regularParsedValue = ParseValue(text);
        parsingArgs.Value = regularParsedValue;
      }
      catch (Exception error)
      {
        parsingException = error;
      }

      try
      {
        OnParsing(parsingArgs);
      }
      catch (Exception ex)
      {
        LogService.LogError(ex);
      }

      if (parsingException == null)
      {
        var newValue = parsingArgs.Handled ? parsingArgs.Value : regularParsedValue;

        if (Value.Equals(newValue))
          SetTextBoxFormattedValue();

        Value = newValue;
      }
      else
      {
        if (parsingArgs.Handled)
        {
          if (Value.Equals(parsingArgs.Value))
            SetTextBoxFormattedValue();

          Value = parsingArgs.Value;
        }
        else
        {
          var args = new UpDownParseErrorEventArgs(text, parsingException);
          OnParseError(args);

          if (args.Handled == false)
            SetTextBoxFormattedValue();
        }
      }
    }

    protected abstract T CoerceValue(T value);
    protected internal abstract string FormatValue();

    protected virtual void OnBeginInit()
    {
    }

    protected virtual void OnEndInit()
    {
    }

    protected virtual void OnParseError(UpDownParseErrorEventArgs e)
    {
      ParseError?.Invoke(this, e);
    }

    protected virtual void OnParsing(UpDownParsingEventArgs<T> e)
    {
      Parsing?.Invoke(this, e);
    }

    private void OnValueChanged(T oldValue, T newValue)
    {
      if (_ignoreValueChange)
      {
        return;
      }

      var changingArgs = new PropertyChangingEventArgsXm<T>(this, oldValue, newValue, ValueProperty);
      OnValueChanging(changingArgs);

      if (!changingArgs.Cancel)
      {
        newValue = changingArgs.NewValue;
        var changedArgs = new PropertyValueChangedEventArgs<T>(this, oldValue, newValue, ValueProperty);
        OnValueChanged(changedArgs);
      }
      else
      {
        _ignoreValueChange = true;

        Value = oldValue;

        _ignoreValueChange = false;
      }
    }

    protected virtual void OnValueChanged(PropertyValueChangedEventArgs<T> e)
    {
      ValueChanged?.Invoke(this, e);
      SetTextBoxFormattedValue();
    }

    protected virtual void OnValueChanging(PropertyChangingEventArgsXm<T> e)
    {
      ValueChanging?.Invoke(this, e);
    }

    protected abstract T ParseValue(string text);

    protected void SetTextBoxFormattedValue()
    {
      SetTextBoxText(FormatValue());
    }

    #endregion

    #region Interface Implementations

    #region ISupportInitialize

    void ISupportInitialize.BeginInit()
    {
      IsInitializing = true;
      OnBeginInit();
    }

    void ISupportInitialize.EndInit()
    {
      OnEndInit();
      IsInitializing = false;
    }

    #endregion

    #endregion
  }

  public class UpDownBaseTemplateContract : TemplateContract
  {
    #region Ctors

    public UpDownBaseTemplateContract(GetTemplateChild templateDiscovery, Action onAttach, Action onDetach) : base(templateDiscovery, onAttach, onDetach)
    {
    }

    #endregion

    #region Properties

    [TemplateContractPart(Required = true)]
    public Spinner Spinner { get; [UsedImplicitly] private set; }

    [TemplateContractPart(Required = true)]
    public TextBox TextBox { get; [UsedImplicitly] private set; }

    #endregion
  }
}