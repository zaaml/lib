// <copyright file="FloatingDialog.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Windows
{
	// ReSharper disable once PartialTypeWithSinglePart
	public partial class FloatingDialog : DialogWindowBase
	{
		internal static readonly RoutedCommand CloseDialogCommand = new RoutedCommand();

		public static readonly DependencyProperty DefaultFloatingDialogResultProperty = DPM.Register<FloatingDialogResult, FloatingDialog>
			("DefaultFloatingDialogResult", FloatingDialogResult.None);

		public static readonly DependencyProperty ButtonsProperty = DPM.Register<FloatingDialogButtons, FloatingDialog>
			("Buttons", FloatingDialogButtons.Ok, d => d.OnButtonsChanged);

		private List<FloatingDialogButton> _actualButtons;
		private bool _isClosed;

		static FloatingDialog()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<FloatingDialog>();
		}

		public FloatingDialog()
		{
			this.OverrideStyleKey<FloatingDialog>();

			CommandBindings.Add(new CommandBinding(CloseDialogCommand, ExecutedCloseDialogCommand, CanExecuteCloseDialogCommand));

			UpdateButtons();
		}

		protected override IEnumerable<WindowButton> ActualButtons => _actualButtons ?? Enumerable.Empty<WindowButton>();

		public FloatingDialogButton ApplyButton { get; } = new FloatingDialogButton {DialogResult = FloatingDialogResult.Apply};

		public FloatingDialogButtons Buttons
		{
			get => (FloatingDialogButtons) GetValue(ButtonsProperty);
			set => SetValue(ButtonsProperty, value);
		}

		public FloatingDialogButton CancelButton { get; } = new FloatingDialogButton {DialogResult = FloatingDialogResult.Cancel};

		public FloatingDialogResult DefaultFloatingDialogResult
		{
			get => (FloatingDialogResult) GetValue(DefaultFloatingDialogResultProperty);
			set => SetValue(DefaultFloatingDialogResultProperty, value);
		}

		public FloatingDialogResult FloatingDialogResult { get; set; }

		public FloatingDialogButton NoButton { get; } = new FloatingDialogButton {DialogResult = FloatingDialogResult.No};

		public FloatingDialogButton OkButton { get; } = new FloatingDialogButton {DialogResult = FloatingDialogResult.OK};

		public FloatingDialogButton YesButton { get; } = new FloatingDialogButton {DialogResult = FloatingDialogResult.Yes};

		protected virtual bool CanExecuteCloseDialogCommand(FloatingDialogResult dialogResult)
		{
			return true;
		}

		private void CanExecuteCloseDialogCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = e.Parameter is FloatingDialogResult floatingDialogResult && CanExecuteCloseDialogCommand(floatingDialogResult);
		}

		protected override void CloseByEnter()
		{
			CloseWithResult(true);
		}

		protected override void CloseByEscape()
		{
			CloseWithResult(false);
		}

		public void CloseWithResult(FloatingDialogResult result)
		{
			var currentResult = FloatingDialogResult;

			FloatingDialogResult = result;

			_isClosed = false;
			DialogResult = GetDialogResult(result);

			if (_isClosed == false)
				FloatingDialogResult = currentResult;
		}

		public void CloseWithResult(bool? result)
		{
			if (DefaultFloatingDialogResult == FloatingDialogResult.None)
			{
				switch (result)
				{
					case null:
						CloseWithResult(DefaultFloatingDialogResult);
						break;
					case true:
						CloseWithResult(GetDefaultOKResult(Buttons));
						break;
					default:
						CloseWithResult(GetDefaultCancelResult(Buttons));
						break;
				}
			}
			else
				CloseWithResult(DefaultFloatingDialogResult);
		}

		private void ExecutedCloseDialogCommand(object sender, ExecutedRoutedEventArgs e)
		{
			CloseWithResult((FloatingDialogResult) e.Parameter);
		}

		private static FloatingDialogResult GetDefaultCancelResult(FloatingDialogButtons buttons)
		{
			switch (buttons)
			{
				case FloatingDialogButtons.Ok:
					return FloatingDialogResult.OK;
				case FloatingDialogButtons.OkCancel:
				case FloatingDialogButtons.YesNoCancel:
				case FloatingDialogButtons.OkCancelApply:
					return FloatingDialogResult.Cancel;
				case FloatingDialogButtons.YesNo:
					return FloatingDialogResult.No;
				default:
					throw new ArgumentOutOfRangeException(nameof(buttons));
			}
		}

		private static FloatingDialogResult GetDefaultOKResult(FloatingDialogButtons buttons)
		{
			switch (buttons)
			{
				case FloatingDialogButtons.Ok:
					return FloatingDialogResult.OK;
				case FloatingDialogButtons.OkCancel:
				case FloatingDialogButtons.OkCancelApply:
					return FloatingDialogResult.OK;
				case FloatingDialogButtons.YesNo:
				case FloatingDialogButtons.YesNoCancel:
					return FloatingDialogResult.Yes;
				default:
					throw new ArgumentOutOfRangeException(nameof(buttons));
			}
		}

		private static bool? GetDialogResult(FloatingDialogResult result)
		{
			switch (result)
			{
				case FloatingDialogResult.None:
					return null;
				case FloatingDialogResult.OK:
					return true;
				case FloatingDialogResult.Cancel:
					return false;
				case FloatingDialogResult.Abort:
					return false;
				case FloatingDialogResult.Retry:
					return true;
				case FloatingDialogResult.Ignore:
					return false;
				case FloatingDialogResult.Yes:
					return true;
				case FloatingDialogResult.No:
					return false;
				case FloatingDialogResult.Apply:
					return true;
				default:
					throw new ArgumentOutOfRangeException(nameof(result));
			}
		}

		private void OnButtonsChanged()
		{
			UpdateButtons();
		}

		protected override void OnExecutedCloseCommand(object commandParameter)
		{
			if (commandParameter is FloatingDialogResult)
				FloatingDialogResult = (FloatingDialogResult) commandParameter;
			else
				FloatingDialogResult = FloatingDialogResult.None;

			Close();
		}

		private void UpdateButtons()
		{
			DetachButtons();

			switch (Buttons)
			{
				case FloatingDialogButtons.Ok:
					_actualButtons = new List<FloatingDialogButton> {OkButton};
					break;
				case FloatingDialogButtons.OkCancel:
					_actualButtons = new List<FloatingDialogButton> {OkButton, CancelButton};
					break;
				case FloatingDialogButtons.YesNoCancel:
					_actualButtons = new List<FloatingDialogButton> {YesButton, NoButton, CancelButton};
					break;
				case FloatingDialogButtons.YesNo:
					_actualButtons = new List<FloatingDialogButton> {YesButton, NoButton};
					break;
				case FloatingDialogButtons.OkCancelApply:
					_actualButtons = new List<FloatingDialogButton> {OkButton, CancelButton, ApplyButton};
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			AttachButtons();
		}
	}
}