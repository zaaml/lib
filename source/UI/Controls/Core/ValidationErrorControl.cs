// <copyright file="ValidationErrorControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.Core.Weak;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.Interfaces;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using NativeControl = System.Windows.Controls.Control;
using Panel = Zaaml.UI.Panels.Core.Panel;

namespace Zaaml.UI.Controls.Core
{
	public enum ValidationToolTipDisplayMode
	{
		Hidden,
		ShowFocused,
		ShowAlways,
		Manual
	}

	[TemplateContractType(typeof(ValidationErrorControlTemplateContract))]
	public class ValidationErrorControl : TemplateContractControl, IPanelAdvisor
	{
		public static readonly DependencyProperty EnableValidationProperty = DPM.RegisterAttached<bool, ValidationErrorControl>
			("EnableValidation", true);

		private static readonly DependencyProperty EnableValidationIntProperty = DPM.Register<bool, ValidationErrorControl>
			("EnableValidationInt", true, v => v.OnValidationVisibilityStateDirty);

		private static readonly DependencyPropertyKey ActualShowValidationToolTipPropertyKey = DPM.RegisterReadOnly<bool, ValidationErrorControl>
			("ActualShowValidationToolTip", v => v.OnActualShowValidationToolTipChanged);

		private static readonly DependencyPropertyKey ActualShowValidationErrorPropertyKey = DPM.RegisterReadOnly<bool, ValidationErrorControl>
			("ActualShowValidationError", false, v => v.OnActualShowValidationErrorPropertyChangedPrivate);

		public static readonly DependencyProperty ActualShowValidationErrorProperty = ActualShowValidationErrorPropertyKey.DependencyProperty;

		public static readonly DependencyProperty ActualShowValidationToolTipProperty = ActualShowValidationToolTipPropertyKey.DependencyProperty;

		public static readonly DependencyProperty ValidationToolTipDisplayModeProperty = DPM.Register<ValidationToolTipDisplayMode, ValidationErrorControl>
			("ValidationToolTipDisplayMode", ValidationToolTipDisplayMode.ShowFocused, v => v.UpdateActualShowValidationToolTip);

		public static readonly DependencyProperty ShowValidationToolTipProperty = DPM.Register<bool, ValidationErrorControl>
			("ShowValidationToolTip", v => v.UpdateActualShowValidationToolTip);

		public static readonly DependencyProperty ValidatedControlProperty = DPM.Register<NativeControl, ValidationErrorControl>
			("ValidatedControl", v => v.OnValidationSourceControlChanged);

		private static readonly DependencyPropertyKey ActualValidatedControlPropertyKey = DPM.RegisterReadOnly<NativeControl, ValidationErrorControl>
			("ActualValidatedControl", default, d => d.OnActualValidatedControlPropertyChangedPrivate);

		public static readonly DependencyProperty ActualValidatedControlProperty = ActualValidatedControlPropertyKey.DependencyProperty;

		private static readonly DependencyProperty ActualValidatedControlHasErrorsProperty = DPM.Register<bool, ValidationErrorControl>
			("ActualValidatedControlHasErrors", v => v.OnActualValidatedControlHasErrorsChanged);

		private static readonly DependencyPropertyKey HasErrorPropertyKey = DPM.RegisterReadOnly<bool, ValidationErrorControl>
			("HasError", v => v.OnHasErrorChanged);

		private static readonly DependencyPropertyKey ErrorPropertyKey = DPM.RegisterReadOnly<string, ValidationErrorControl>
			("Error");

		public static readonly DependencyProperty ErrorProperty = ErrorPropertyKey.DependencyProperty;
		public static readonly DependencyProperty HasErrorProperty = HasErrorPropertyKey.DependencyProperty;

		private static readonly PropertyPath ValidationHasErrorPropertyPath = new PropertyPath(Validation.HasErrorProperty);
		private static readonly Binding EnableValidationBinding = new Binding { Path = new PropertyPath(EnableValidationProperty), RelativeSource = XamlConstants.TemplatedParent, FallbackValue = true };

		private IDisposable _focusListener;
		[UsedImplicitly] private  VisualStateListenerBase _invalidFocusedListener;

		private bool _invalidFocusedVisualState;
		private bool _isMeasured;
		private bool _treeHasFocus;
		private bool _forceHideToolTip;

		static ValidationErrorControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ValidationErrorControl>();
		}

		public ValidationErrorControl()
		{
			this.OverrideStyleKey<ValidationErrorControl>();

#if !SILVERLIGHT
			Focusable = false;
#endif
			IsTabStop = false;
		}

		public bool ActualShowValidationError
		{
			get => (bool) GetValue(ActualShowValidationErrorProperty);
			private set => this.SetReadOnlyValue(ActualShowValidationErrorPropertyKey, value);
		}

		public bool ActualShowValidationToolTip
		{
			get => (bool) GetValue(ActualShowValidationToolTipProperty);
			private set => this.SetReadOnlyValue(ActualShowValidationToolTipPropertyKey, value);
		}

		public NativeControl ActualValidatedControl
		{
			get => (NativeControl) GetValue(ActualValidatedControlProperty);
			private set => this.SetReadOnlyValue(ActualValidatedControlPropertyKey, value);
		}

		public string Error
		{
			get => (string) GetValue(ErrorProperty);
			private set => this.SetReadOnlyValue(ErrorPropertyKey, value);
		}

		public bool HasError
		{
			get => (bool) GetValue(HasErrorProperty);
			private set => this.SetReadOnlyValue(HasErrorPropertyKey, value);
		}

		internal bool InvalidFocusedVisualState
		{
			get => _invalidFocusedVisualState;
			private set
			{
				_invalidFocusedVisualState = value;
				UpdateActualShowValidationToolTip();
			}
		}

		public bool ShowValidationToolTip
		{
			get => (bool) GetValue(ShowValidationToolTipProperty);
			set => SetValue(ShowValidationToolTipProperty, value);
		}

		private bool TreeHasFocus
		{
			get => _treeHasFocus;
			set
			{
				if (_treeHasFocus == value)
					return;

				_treeHasFocus = value;

				UpdateActualShowValidationToolTip();
			}
		}

		public NativeControl ValidatedControl
		{
			get => (NativeControl) GetValue(ValidatedControlProperty);
			set => SetValue(ValidatedControlProperty, value);
		}

		public ValidationToolTipDisplayMode ValidationToolTipDisplayMode
		{
			get => (ValidationToolTipDisplayMode) GetValue(ValidationToolTipDisplayModeProperty);
			set => SetValue(ValidationToolTipDisplayModeProperty, value);
		}

		private void AttachEvents(NativeControl control)
		{
			SetBinding(ActualValidatedControlHasErrorsProperty, new Binding { Path = ValidationHasErrorPropertyPath, Source = control });
		}

		private void DetachEvents()
		{
			ClearValue(ActualValidatedControlHasErrorsProperty);
		}

		public static bool GetEnableValidation(DependencyObject element)
		{
			return (bool) element.GetValue(EnableValidationProperty);
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			base.ArrangeOverride(arrangeBounds);

			return arrangeBounds;
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			_isMeasured = true;

			return base.MeasureOverride(availableSize);
		}

		private void UpdateListeners()
		{
			_focusListener = _focusListener.DisposeExchange();
			_invalidFocusedListener = _invalidFocusedListener.DisposeExchange();

			if (ActualShowValidationError)
			{
				_focusListener = this.CreateWeakEventListener((t, o, e) => t.OnFocusedElementChanged(), h => FocusObserver.KeyboardFocusedElementChanged += h, h => FocusObserver.KeyboardFocusedElementChanged -= h);
				_invalidFocusedListener = this.DelegateVisualStateListener("InvalidFocused", () => InvalidFocusedVisualState = true, () => InvalidFocusedVisualState = false);
			}
		}

		private void OnActualShowValidationErrorPropertyChangedPrivate()
		{
			UpdateListeners();

			UpdateActualShowValidationToolTip();

			if (_isMeasured)
				return;

			InvalidateMeasure();
			(Parent as Panel)?.InvalidateMeasure();
		}

		private void OnActualShowValidationToolTipChanged()
		{
		}

		private void OnActualValidatedControlHasErrorsChanged()
		{
			UpdateHasError();
		}

		private void OnActualValidatedControlPropertyChangedPrivate(NativeControl oldValue, NativeControl newValue)
		{
			OnActualValidationSourceControlChanged(oldValue, newValue);
		}

		private void OnActualValidationSourceControlChanged(NativeControl oldControl, NativeControl newControl)
		{
			if (oldControl != null && IsLoaded)
				DetachEvents();

			if (newControl != null && IsLoaded)
				AttachEvents(newControl);

			UpdateHasError();
			UpdateError();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			UpdateActualShowValidationError();
		}

		private void OnFocusedElementChanged()
		{
			UpdateTreeHasFocus();
		}

		private void UpdateTreeHasFocus()
		{
			TreeHasFocus = FocusObserver.KeyboardFocusedElement is FrameworkElement focusedElement && ActualValidatedControl != null &&
			               (ReferenceEquals(focusedElement, ActualValidatedControl) || focusedElement.IsVisualDescendantOf(ActualValidatedControl));
		}

		private void OnHasErrorChanged()
		{
			if (IsTemplateAttached == false) 
				ApplyTemplate();

			UpdateError();
			UpdateTreeHasFocus();
			UpdateActualShowValidationError();
			UpdateActualShowValidationToolTip();
		}

		protected override void OnLoaded()
		{
			base.OnLoaded();

			UpdateActualValidatedControl();

			if (ActualValidatedControl != null)
				AttachEvents(ActualValidatedControl);

			UpdateActualShowValidationToolTip();
		}

		protected override void OnUnloaded()
		{
			base.OnUnloaded();

			if (ActualValidatedControl != null)
				DetachEvents();

			UpdateActualShowValidationToolTip();
		}

		private void OnValidationSourceControlChanged(NativeControl oldControl, NativeControl newControl)
		{
			UpdateActualValidatedControl();
		}

		private void OnValidationVisibilityStateDirty()
		{
			UpdateActualShowValidationError();
		}

		public static void SetEnableValidation(DependencyObject element, bool value)
		{
			element.SetValue(EnableValidationProperty, value);
		}

		private void UpdateActualShowValidationError()
		{
			ActualShowValidationError = HasError && this.GetValue<bool>(EnableValidationIntProperty);
		}

		private void UpdateActualShowValidationToolTip()
		{
			if (ForceHideToolTip || IsLoaded == false || ActualShowValidationError == false)
			{
				ActualShowValidationToolTip = false;

				return;
			}

			switch (ValidationToolTipDisplayMode)
			{
				case ValidationToolTipDisplayMode.Hidden:
					ActualShowValidationToolTip = false;
					break;
				case ValidationToolTipDisplayMode.ShowFocused:
					ActualShowValidationToolTip = InvalidFocusedVisualState || (TreeHasFocus && HasError);
					break;
				case ValidationToolTipDisplayMode.ShowAlways:
					ActualShowValidationToolTip = true;
					break;
				case ValidationToolTipDisplayMode.Manual:
					ActualShowValidationToolTip = ShowValidationToolTip;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		internal bool ForceHideToolTip
		{
			get => _forceHideToolTip;
			set
			{
				if (_forceHideToolTip == value)
					return;

				_forceHideToolTip = value;

				UpdateActualShowValidationToolTip();
			}
		}

		private void UpdateActualValidatedControl()
		{
			ActualValidatedControl = ValidatedControl ?? (NativeControl) this.GetTemplatedParent();
		}

		private void UpdateError()
		{
			Error = HasError ? Validation.GetErrors(ActualValidatedControl).FirstOrDefault()?.ErrorContent?.ToString() : null;
		}

		private void UpdateHasError()
		{
			HasError = ActualValidatedControl != null && Validation.GetHasError(ActualValidatedControl);
		}

		bool IPanelAdvisor.ShouldMeasure
		{
			get
			{
				if (this.HasLocalValue(EnableValidationIntProperty) == false)
					SetBinding(EnableValidationIntProperty, EnableValidationBinding);

				return ActualShowValidationError;
			}
		}

		bool IPanelAdvisor.ShouldArrange => _isMeasured;

		~ValidationErrorControl()
		{
			// TODO: Possibly need to Dispose _invalidFocusedListener and	_invalidUnfocusedListener but not on this thread (do not use Dispatcher here)
			_focusListener = _focusListener.DisposeExchange();
		}
	}

	public class ValidationErrorControlTemplateContract : TemplateContract
	{
	}
}