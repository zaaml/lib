// <copyright file="RelayCommand.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Input;

namespace Zaaml.PresentationCore.CommandCore
{
	public class RelayCommand : ICommand
	{
		private readonly Func<object, bool> _canExecute;
		private readonly Action<object> _execute;

		public RelayCommand(Action execute, Func<bool> canExecute)
		{
			_execute = p => execute();
			_canExecute = p => canExecute();
		}

		public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
		{
			_execute = execute;
			_canExecute = canExecute;
		}

		public RelayCommand(Action execute) : this(execute, () => true)
		{
		}

		public RelayCommand(Action<object> execute) : this(execute, p => true)
		{
		}

		public virtual void OnCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute(parameter);
		}

		public event EventHandler CanExecuteChanged;

		public void Execute(object parameter)
		{
			_execute(parameter);
		}
	}

	public class RelayCommand<T> : ICommand
	{
		private readonly Func<T, bool> _canExecute;
		private readonly Action<T> _execute;

		public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
		{
			_execute = execute;
			_canExecute = canExecute;
		}

		public RelayCommand(Action<T> execute)
			: this(execute, p => true)
		{
		}

		public bool CanExecute(T parameter)
		{
			return _canExecute(parameter);
		}

		public void Execute(T parameter)
		{
			_execute(parameter);
		}

		public virtual void OnCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}

		public event EventHandler CanExecuteChanged;

		bool ICommand.CanExecute(object parameter)
		{
			return CanExecute((T)parameter);
		}

		void ICommand.Execute(object parameter)
		{
			Execute((T)parameter);
		}
	}
}