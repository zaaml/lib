// <copyright file="ValidationSummaryControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Core;
using ContentControl = System.Windows.Controls.ContentControl;
using Control = Zaaml.UI.Controls.Core.Control;
using Style = System.Windows.Style;
using ZaamlLocalization = Zaaml.UI.Localization;

#if NETCOREAPP
#else
using Zaaml.Core.Extensions;
#endif

namespace Zaaml.UI.Controls.ValidationSummary
{
	public class ValidationSummaryControl : Control
	{
		public static readonly DependencyProperty ShowErrorsInSummaryProperty = DPM.RegisterAttached<bool, ValidationSummaryControl>
			("ShowErrorsInSummary", true, OnShowErrorsInSummaryPropertyChanged);

		public static readonly DependencyProperty ErrorItemTemplateProperty = DPM.Register<DataTemplate, ValidationSummaryControl>
			("ErrorItemTemplate");

		private static readonly DependencyPropertyKey ErrorsPropertyKey = DPM.RegisterReadOnly<ValidationSummaryItemCollection, ValidationSummaryControl>
			("Errors");

		public static readonly DependencyProperty ErrorsProperty = ErrorsPropertyKey.DependencyProperty;

		public static readonly DependencyProperty ErrorStyleProperty = DPM.Register<Style, ValidationSummaryControl>
			("ErrorStyle");

		public static readonly DependencyProperty FilterProperty = DPM.Register<ValidationSummaryFilter, ValidationSummaryControl>
			("Filter", null, v => v.OnFilterPropertyChanged);

		public static readonly DependencyProperty FocusControlsOnClickProperty = DPM.Register<bool, ValidationSummaryControl>
			("FocusControlsOnClick", true);

		private static readonly DependencyPropertyKey HasDisplayedErrorsPropertyKey = DPM.RegisterReadOnly<bool, ValidationSummaryControl>
			("HasDisplayedErrors", false);

		public static readonly DependencyProperty HasDisplayedErrorsProperty = HasDisplayedErrorsPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey HasErrorsPropertyKey = DPM.RegisterReadOnly<bool, ValidationSummaryControl>
			("HasErrors", false);

		public static readonly DependencyProperty HasErrorsProperty = HasErrorsPropertyKey.DependencyProperty;

		public static readonly DependencyProperty HeaderProperty = DPM.Register<object, ValidationSummaryControl>
			("Header", v => v.OnHasHeaderPropertyChanged);

		public static readonly DependencyProperty HeaderTemplateProperty = DPM.Register<DataTemplate, ValidationSummaryControl>
			("HeaderTemplate");

		public static readonly DependencyProperty SummaryListBoxStyleProperty = DPM.Register<Style, ValidationSummaryControl>
			("SummaryListBoxStyle");

		public static readonly DependencyProperty TargetProperty = DPM.Register<UIElement, ValidationSummaryControl>
			("Target", v => v.OnTargetPropertyChanged);

		private readonly ValidationSummaryItemCollection _displayedErrorsInternal;
		private readonly Dictionary<ValidationItemKey, ValidationSummaryItem> _validationSummaryItemDictionary;
		private ValidationSummarySource _currentValidationSummarySource;
		private FrameworkElement _registeredParent;
		public event EventHandler<FocusingInvalidControlEventArgs> FocusingInvalidControl;
		public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

		static ValidationSummaryControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ValidationSummaryControl>();
		}

		public ValidationSummaryControl()
		{
			this.OverrideStyleKey<ValidationSummaryControl>();

			_validationSummaryItemDictionary = new Dictionary<ValidationItemKey, ValidationSummaryItem>();
			_displayedErrorsInternal = new ValidationSummaryItemCollection();
			DisplayedErrors = new ValidationSummaryItemDisplayCollection(_displayedErrorsInternal);

			((INotifyCollectionChanged) Errors).CollectionChanged += Errors_CollectionChanged;

			Loaded += ValidationSummary_Loaded;
			Unloaded += ValidationSummary_Unloaded;
			IsEnabledChanged += ValidationSummary_IsEnabledChanged;
		}

		public ValidationSummaryItemDisplayCollection DisplayedErrors { get; }

		public DataTemplate ErrorItemTemplate
		{
			get => (DataTemplate) GetValue(ErrorItemTemplateProperty);
			set => SetValue(ErrorItemTemplateProperty, value);
		}

		public ValidationSummaryItemCollection Errors
		{
			get { return this.GetValueOrCreate(ErrorsPropertyKey, () => new ValidationSummaryItemCollection()); }
		}

		private ListBox ErrorsListBox { get; set; }

		public Style ErrorStyle
		{
			get => GetValue(ErrorStyleProperty) as Style;
			set => SetValue(ErrorStyleProperty, value);
		}

		public ValidationSummaryFilter Filter
		{
			get => (ValidationSummaryFilter) GetValue(FilterProperty);
			set => SetValue(FilterProperty, value);
		}

		public bool FocusControlsOnClick
		{
			get => (bool) GetValue(FocusControlsOnClickProperty);
			set => SetValue(FocusControlsOnClickProperty, value);
		}

		public bool HasDisplayedErrors
		{
			get => (bool) GetValue(HasDisplayedErrorsProperty);
			private set => this.SetReadOnlyValue(HasDisplayedErrorsPropertyKey, value);
		}

		public bool HasErrors
		{
			get => (bool) GetValue(HasErrorsProperty);
			private set => this.SetReadOnlyValue(HasErrorsPropertyKey, value);
		}

		public object Header
		{
			get => GetValue(HeaderProperty);
			set => SetValue(HeaderProperty, value);
		}

		private ContentControl HeaderContentControl { get; set; }

		public DataTemplate HeaderTemplate
		{
			get => GetValue(HeaderTemplateProperty) as DataTemplate;
			set => SetValue(HeaderTemplateProperty, value);
		}

		private bool IsControlLoaded { get; set; }

		public Style SummaryListBoxStyle
		{
			get => GetValue(SummaryListBoxStyleProperty) as Style;
			set => SetValue(SummaryListBoxStyleProperty, value);
		}

		public UIElement Target
		{
			get => GetValue(TargetProperty) as UIElement;
			set => SetValue(TargetProperty, value);
		}

		private static void AttachValidationEvent(FrameworkElement fre, EventHandler<ValidationErrorEventArgs> eventHandler)
		{
#if SILVERLIGHT
      fre.BindingValidationError += eventHandler;
#else
			Validation.AddErrorHandler(fre, eventHandler);
#endif
		}

		private static int CompareValidationSummaryItems(ValidationSummaryItem x, ValidationSummaryItem y)
		{
			int num;
			if (!ReferencesAreValid(x, y, out num) || TryCompareReferences(x.ItemType, y.ItemType, out num))
				return num;

			var control1 = x.Source?.Control;
			var control2 = y.Source?.Control;

			if (!ReferenceEquals(control1, control2))
			{
				if (!ReferencesAreValid(control1, control2, out num))
					return num;

				if (control1.TabIndex != control2.TabIndex)
					return control1.TabIndex.CompareTo(control2.TabIndex);

				num = SortByVisualTreeOrdering(control1, control2);
				if (num != 0 || TryCompareReferences(control1.Name, control2.Name, out num))
					return num;
			}

			if (TryCompareReferences(x.Header, y.Header, out num))
				return num;

			TryCompareReferences(x.Message, y.Message, out num);

			return num;
		}

		private static void DetachValidationEvent(FrameworkElement fre, EventHandler<ValidationErrorEventArgs> eventHandler)
		{
#if SILVERLIGHT
      fre.BindingValidationError -= eventHandler;
#else
			Validation.RemoveErrorHandler(fre, eventHandler);
#endif
		}

		private void Errors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				foreach (ValidationSummaryItem oldItem in e.OldItems)
				{
					if (oldItem != null)
						oldItem.DisplayRelatedPropertyChanged -= ValidationSummaryItem_PropertyChanged;
				}
			}

			if (e.NewItems != null)
			{
				foreach (ValidationSummaryItem newItem in e.NewItems)
				{
					if (newItem != null)
						newItem.DisplayRelatedPropertyChanged += ValidationSummaryItem_PropertyChanged;
				}
			}

			HasErrors = Errors.Count > 0;
			UpdateDisplayedErrors();
		}

		private void ErrorsListBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;
			ExecuteClick(sender);
		}

		private void ErrorsListBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			ExecuteClick(sender);
		}

		private void ErrorsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var selectionChanged = SelectionChanged;
			selectionChanged?.Invoke(this, e);
		}

		private void ExecuteClick(object sender)
		{
			var listBox = sender as ListBox;
			var selectedItem = listBox?.SelectedItem as ValidationSummaryItem;

			if (selectedItem == null || !FocusControlsOnClick)
				return;

			_currentValidationSummarySource = selectedItem.Source;

			var e = new FocusingInvalidControlEventArgs(selectedItem, _currentValidationSummarySource);
			OnFocusingInvalidControl(e);

			if (!e.Handled)
				e.Target?.Control?.Focus();
		}

		private string GetHeaderString()
		{
			return _displayedErrorsInternal.Count != 1 ? string.Format(CultureInfo.CurrentCulture, ZaamlLocalization.ValidationSummaryHeaderErrors, _displayedErrorsInternal.Count) : ZaamlLocalization.ValidationSummaryHeaderError;
		}

		public static bool GetShowErrorsInSummary(DependencyObject inputControl)
		{
			if (inputControl == null)
				throw new ArgumentNullException(nameof(inputControl));

			return (bool) inputControl.GetValue(ShowErrorsInSummaryProperty);
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			MouseButtonEventHandler buttonEventHandler = ErrorsListBox_MouseLeftButtonUp;
			KeyEventHandler keyEventHandler = ErrorsListBox_KeyDown;
			SelectionChangedEventHandler changedEventHandler = ErrorsListBox_SelectionChanged;

			if (ErrorsListBox != null)
			{
				ErrorsListBox.MouseLeftButtonUp -= buttonEventHandler;
				ErrorsListBox.KeyDown -= keyEventHandler;
				ErrorsListBox.SelectionChanged -= changedEventHandler;
			}

			ErrorsListBox = GetTemplateChild("SummaryListBox") as ListBox;

			if (ErrorsListBox != null)
			{
				ErrorsListBox.MouseLeftButtonUp += buttonEventHandler;
				ErrorsListBox.KeyDown += keyEventHandler;
				ErrorsListBox.ItemsSource = DisplayedErrors;
				ErrorsListBox.SelectionChanged += changedEventHandler;
			}

			HeaderContentControl = GetTemplateChild("HeaderContentControl") as ContentControl;

			UpdateDisplayedErrors();
			UpdateCommonState(false);
			UpdateValidationState(false);
		}

		private void OnFilterFilterChanged(object sender, EventArgs eventArgs)
		{
			UpdateDisplayedErrors();
		}

		private void OnFilterPropertyChanged(ValidationSummaryFilter oldFilter, ValidationSummaryFilter newFilter)
		{
			if (oldFilter != null)
				oldFilter.FilterChanged -= OnFilterFilterChanged;

			if (newFilter != null)
				newFilter.FilterChanged += OnFilterFilterChanged;

			UpdateDisplayedErrors();
		}

		protected virtual void OnFocusingInvalidControl(FocusingInvalidControlEventArgs e)
		{
			var focusingInvalidControl = FocusingInvalidControl;
			focusingInvalidControl?.Invoke(this, e);
		}

		private void OnHasHeaderPropertyChanged()
		{
			UpdateHeaderText();
		}

		private static void OnShowErrorsInSummaryPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
#if SILVERLIGHT
      var frameworkElement = Application.Current != null ? Application.Current.RootVisual as FrameworkElement : null;
      if (frameworkElement == null)
        return;
#else
			var frameworkElement = PresentationSource.FromDependencyObject(o)?.RootVisual as FrameworkElement;
#endif

			UpdateDisplayedErrorsOnAllValidationSummaries(frameworkElement);
		}

		private void OnTargetPropertyChanged(UIElement oldValue, UIElement newValue)
		{
			var oldValueFre = oldValue as FrameworkElement;

			EventHandler<ValidationErrorEventArgs> eventHandler = Target_BindingValidationError;
			if (_registeredParent != null)
			{
				DetachValidationEvent(_registeredParent, eventHandler);
				_registeredParent = null;
			}

			if (oldValueFre != null)
				DetachValidationEvent(oldValueFre, eventHandler);

			var newValueFre = newValue as FrameworkElement;
			if (newValueFre != null)
				AttachValidationEvent(newValueFre, eventHandler);

			Errors.ClearErrors(ValidationSummaryItemType.PropertyError);
			UpdateDisplayedErrors();
		}

		private static bool ReferencesAreValid(object x, object y, out int val)
		{
			if (x == null)
			{
				val = y == null ? 0 : -1;
				return false;
			}

			if (y == null)
			{
				val = 1;
				return false;
			}

			val = 0;
			return true;
		}

		public static void SetShowErrorsInSummary(DependencyObject inputControl, bool value)
		{
			if (inputControl == null)
				throw new ArgumentNullException(nameof(inputControl));

			inputControl.SetValue(ShowErrorsInSummaryProperty, value);
		}

		private static int SortByVisualTreeOrdering(DependencyObject controlX, DependencyObject controlY)
		{
			if (controlX == null || controlY == null || ReferenceEquals(controlX, controlY))
				return 0;

			var dependencyObjectList = new List<DependencyObject>();
			var reference1 = controlX;

			dependencyObjectList.Add(reference1);

			while ((reference1 = VisualTreeHelper.GetParent(reference1)) != null)
				dependencyObjectList.Add(reference1);

			var reference2 = controlY;
			var dependencyObject1 = reference2;

			while ((reference2 = VisualTreeHelper.GetParent(reference2)) != null)
			{
				var num = dependencyObjectList.IndexOf(reference2);
				if (num == 0)
					return -1;

				if (num > 0)
				{
					var dependencyObject2 = dependencyObjectList[num - 1];

					if (dependencyObject2 == null)
						return 0;

					var childrenCount = VisualTreeHelper.GetChildrenCount(reference2);

					for (var childIndex = 0; childIndex < childrenCount; ++childIndex)
					{
						var child = VisualTreeHelper.GetChild(reference2, childIndex);
						if (ReferenceEquals(child, dependencyObject1))
							return 1;

						if (ReferenceEquals(child, dependencyObject2))
							return -1;
					}
				}

				dependencyObject1 = reference2;
			}

			return 0;
		}

		private void Target_BindingValidationError(object sender, ValidationErrorEventArgs e)
		{
			var originalSource = PresentationTreeUtils.GetUIElementEventSource(e.OriginalSource) as FrameworkElement;
			if (originalSource == null || e.Error?.ErrorContent == null)
				return;

			var message = e.Error.ErrorContent.ToString();
			var key = new ValidationItemKey(originalSource, message);

			var currentItem = _validationSummaryItemDictionary.GetValueOrDefault(key);
			if (currentItem != null && e.Action == ValidationErrorEventAction.Removed)
			{
				currentItem.ReferenceCount--;

				if (currentItem.ReferenceCount <= 0)
				{
					Errors.Remove(currentItem);
					_validationSummaryItemDictionary.Remove(key);
				}
			}

			if (e.Action != ValidationErrorEventAction.Added)
				return;

			string str = null;
			object entity;

			BindingExpression bindingExpression;

			var metadata = ValidationHelper.ParseMetadata(originalSource, false, out entity, out bindingExpression);

			if (metadata != null)
				str = metadata.Caption;

			if (currentItem == null)
			{
				currentItem = new ValidationSummaryItem(message, str, ValidationSummaryItemType.PropertyError, new ValidationSummarySource(str, originalSource as System.Windows.Controls.Control));

				_validationSummaryItemDictionary[key] = currentItem;
				Errors.Add(currentItem);
			}

			currentItem.ReferenceCount++;
		}

		private static bool TryCompareReferences(object x, object y, out int returnVal)
		{
			if (x == null && y == null || x != null && x.Equals(y))
			{
				returnVal = 0;
				return false;
			}

			if (!ReferencesAreValid(x, y, out returnVal))
				return true;

			var comparable1 = x as IComparable;
			var comparable2 = y as IComparable;
			if (comparable1 != null && comparable2 != null)
			{
				returnVal = comparable1.CompareTo(comparable2);
				return true;
			}

			returnVal = 0;
			return false;
		}

		private void UpdateCommonState(bool useTransitions)
		{
			GotoVisualState(IsEnabled ? "Normal" : "Disabled", useTransitions);
		}

		private void UpdateDisplayedErrors()
		{
			var validationSummaryItemList = new List<ValidationSummaryItem>();
			var filter = Filter;
			foreach (var error in Errors)
			{
				if (error == null)
					continue;

				if (error.IsActive == false || string.IsNullOrEmpty(error.Message))
					continue;

				var control = error.Source?.Control;

				if (control != null && GetShowErrorsInSummary(control) == false)
					continue;

				if (filter == null || filter.ShowInSummaryInt(error))
					validationSummaryItemList.Add(error);
			}

			validationSummaryItemList.Sort(CompareValidationSummaryItems);
			_displayedErrorsInternal.Clear();

			foreach (var validationSummaryItem in validationSummaryItemList)
				_displayedErrorsInternal.Add(validationSummaryItem);

			UpdateValidationState(true);
			UpdateHeaderText();
		}

		private static void UpdateDisplayedErrorsOnAllValidationSummaries(DependencyObject parent)
		{
			if (parent == null)
				return;

			var validationSummary = parent as ValidationSummaryControl;

			if (validationSummary != null)
				validationSummary.UpdateDisplayedErrors();
			else
			{
				for (var childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount(parent); ++childIndex)
					UpdateDisplayedErrorsOnAllValidationSummaries(VisualTreeHelper.GetChild(parent, childIndex));
			}
		}

		private void UpdateHeaderText()
		{
			if (HeaderContentControl == null)
				return;

			HeaderContentControl.Content = Header ?? GetHeaderString();
		}

		private void UpdateValidationState(bool useTransitions)
		{
			HasDisplayedErrors = _displayedErrorsInternal.Count > 0;
			GotoVisualState(HasDisplayedErrors ? "HasErrors" : "Empty", useTransitions);
		}

		private void ValidationSummary_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			UpdateCommonState(true);
		}

		private void ValidationSummary_Loaded(object sender, RoutedEventArgs e)
		{
			if (Target == null && _registeredParent == null)
			{
				_registeredParent = VisualTreeHelper.GetParent(this) as FrameworkElement;
				if (_registeredParent != null)
					AttachValidationEvent(_registeredParent, Target_BindingValidationError);
			}

			Loaded -= ValidationSummary_Loaded;
			IsControlLoaded = true;
		}

		private void ValidationSummary_Unloaded(object sender, RoutedEventArgs e)
		{
			if (_registeredParent != null)
				DetachValidationEvent(_registeredParent, Target_BindingValidationError);

			Unloaded -= ValidationSummary_Unloaded;
			IsControlLoaded = false;
		}

		private void ValidationSummaryItem_PropertyChanged(object sender, EventArgs e)
		{
			UpdateDisplayedErrors();
		}

		private struct ValidationItemKey
		{
			public ValidationItemKey(FrameworkElement source, string message)
			{
				_source = source;
				_message = message;
			}

			private readonly FrameworkElement _source;
			private readonly string _message;

			private bool Equals(ValidationItemKey other)
			{
				return Equals(_source, other._source) && string.Equals(_message, other._message);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				return obj is ValidationItemKey && Equals((ValidationItemKey) obj);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return ((_source != null ? _source.GetHashCode() : 0) * 397) ^ (_message != null ? _message.GetHashCode() : 0);
				}
			}
		}
	}
}