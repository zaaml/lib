// <copyright file="CommandDefinition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Input;
using Zaaml.Core.Extensions;
using Zaaml.Core.Weak;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.CommandCore
{
  public class CommandDefinition : InheritanceContextObject, ICommand
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty CommandProperty = DPM.Register<ICommand, CommandDefinition>
      ("Command", c => c.OnCommandChanged);

    public static readonly DependencyProperty CommandTargetProperty = DPM.Register<DependencyObject, CommandDefinition>
      ("CommandTarget", c => c.OnCommandTargetChanged);

    public static readonly DependencyProperty CommandParameterProperty = DPM.Register<object, CommandDefinition>
      ("CommandParameter", c => c.OnCommandParameterChanged);

    #endregion

    #region Fields

    private IDisposable _weakEventListener;

    #endregion

    #region Properties

    public ICommand Command
    {
      get => (ICommand) GetValue(CommandProperty);
      set => SetValue(CommandProperty, value);
    }

    public object CommandParameter
    {
      get => GetValue(CommandParameterProperty);
      set => SetValue(CommandParameterProperty, value);
    }

    public DependencyObject CommandTarget
    {
      get => (DependencyObject) GetValue(CommandTargetProperty);
      set => SetValue(CommandTargetProperty, value);
    }

    #endregion

    #region  Methods

    protected virtual void OnCanExecuteChanged()
    {
      CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnCommandCanExecuteChanged()
    {
      OnCanExecuteChanged();
    }

    private void OnCommandChanged(ICommand oldCommand, ICommand newCommand)
    {
      _weakEventListener = _weakEventListener.DisposeExchange();
      _weakEventListener = this.CreateWeakEventListener
        ((t, o, e) => t.OnCommandCanExecuteChanged(), a => newCommand.CanExecuteChanged += a, a => newCommand.CanExecuteChanged -= a);

      OnCanExecuteChanged();
    }

    private void OnCommandParameterChanged()
    {
      OnCanExecuteChanged();
    }

    private void OnCommandTargetChanged()
    {
      //(Command as TargetCommand)?.RaiseCanExecuteChanged();
    }

    #endregion

    #region Interface Implementations

    #region ICommand

    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter)
    {
      return CommandHelper.CanExecute(Command, parameter ?? CommandParameter, CommandTarget);
    }

    public void Execute(object parameter)
    {
      CommandHelper.Execute(Command, parameter ?? CommandParameter, CommandTarget);
    }

    #endregion

    #endregion
  }
}