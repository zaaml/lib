// <copyright file="CommandController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

#if SILVERLIGHT
using BaseIsEnabledElement = System.Windows.Controls.Control;
#else
using BaseIsEnabledElement = System.Windows.UIElement;
#endif
using System;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Extensions;
using Zaaml.Core.Weak;
using Zaaml.PresentationCore.CommandCore;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.Primitives
{
	internal class CommandController : DependencyObject
	{
		public static readonly DependencyProperty IsEnabledProperty = DPM.Register<bool, CommandController>
			("IsEnabled", c => c.OnIsEnabledChanged);

		public bool IsEnabled
		{
			get => (bool) GetValue(IsEnabledProperty);
			set => SetValue(IsEnabledProperty, value);
		}

		protected virtual void OnIsEnabledChanged()
		{
		}
	}

	internal class CommandController<T> : CommandController, IDisposable where T : Control, ICommandControl
	{
		private RuntimeSetter _isEnabledSetter;
		private IDisposable _weakCanExecuteChangedListener;

		public CommandController(T control)
		{
			Control = control;
			Control.LayoutUpdated += ControlOnLayoutUpdated;
			Control.Loaded += ControlOnLoaded;

			UpdateCanExecute(true);
		}

		public T Control { get; }

		private bool CanExecute()
		{
			return CommandHelper.CanExecute(Control.Command, Control.CommandParameter, Control.CommandTarget ?? Control);
		}

		private void CanExecuteChanged()
		{
			UpdateCanExecute(true);
		}

		private void ControlOnLayoutUpdated(object sender, EventArgs eventArgs)
		{
			UpdateCanExecute(false);
		}

		private void ControlOnLoaded(object sender, RoutedEventArgs e)
		{
			UpdateCanExecute(true);
		}

		public void InvokeCommand()
		{
			TryExecute();
		}

		public void OnCommandChanged()
		{
			UpdateCommandState();
		}

		public void OnCommandParameterChanged()
		{
			UpdateCanExecute(true);
		}

		public void OnCommandTargetChanged()
		{
			UpdateCanExecute(true);
		}

		protected override void OnIsEnabledChanged()
		{
			base.OnIsEnabledChanged();

			if (_isEnabledSetter != null)
				_isEnabledSetter.Value = IsEnabled;
		}

		private void TryExecute()
		{
			if (CanExecute() == false)
				return;

			CommandHelper.Execute(Control.Command, Control.CommandParameter, Control.CommandTarget ?? Control);
		}

		private void UpdateCanExecute(bool updateIsEnabled)
		{
			if (updateIsEnabled)
				IsEnabled = CanExecute();
		}

		private protected void UpdateCanExecuteInternal()
		{
			UpdateCanExecute(true);
		}

		private void UpdateCommandState()
		{
			var newValue = Control.Command;

			_weakCanExecuteChangedListener = _weakCanExecuteChangedListener.DisposeExchange();
			_isEnabledSetter = _isEnabledSetter.DisposeExchange();

			if (newValue != null)
			{
				// ReSharper disable once AccessToStaticMemberViaDerivedType
				_isEnabledSetter = EffectiveValue.CreateAppliedSetter<ServiceRuntimeSetter>(Control, BaseIsEnabledElement.IsEnabledProperty, IsEnabled, short.MaxValue);
				_weakCanExecuteChangedListener = this.CreateWeakEventListener((t, o, e) => t.CanExecuteChanged(), a => newValue.CanExecuteChanged += a, a => newValue.CanExecuteChanged -= a);
			}

			UpdateCanExecute(true);
		}

		public void Dispose()
		{
			Control.LayoutUpdated -= ControlOnLayoutUpdated;
			Control.Loaded -= ControlOnLoaded;
			_isEnabledSetter = _isEnabledSetter.DisposeExchange();
		}
	}
}