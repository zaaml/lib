// <copyright file="SearchTextBox.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Zaaml.Core;
using Zaaml.Core.Collections;
using Zaaml.Core.Extensions;
using Zaaml.Core.Reflection;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.DropDown;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;
using Zaaml.UI.Controls.Primitives.TextPrimitives;
using Control = System.Windows.Controls.Control;

namespace Zaaml.UI.Controls.Editors.Text
{
	[TemplateContractType(typeof(SearchTextBoxTemplateContract))]
	public class SearchTextBox : DropDownControlBase, INotifyPropertyChanged
	{
		#region Static Fields and Constants

		private static readonly DependencyPropertyKey ActualSelectedItemTextPropertyKey = DPM.RegisterReadOnly<string, SearchTextBox>
			("ActualSelectedItemText");

		public static readonly DependencyProperty WatermarkTextProperty = DPM.Register<string, SearchTextBox>
			("WatermarkText");

		public static readonly DependencyProperty WatermarkIconProperty = DPM.Register<ImageSource, SearchTextBox>
			("WatermarkIcon");

		public static readonly DependencyProperty SearchTextProperty = DPM.Register<string, SearchTextBox>
			("SearchText", string.Empty, s => s.OnSearchTextChanged, true);

		public static readonly DependencyProperty ItemTemplateProperty = DPM.Register<DataTemplate, SearchTextBox>
			("ItemTemplate");

		public static readonly DependencyProperty MaxFilteredCountProperty = DPM.Register<int, SearchTextBox>
			("MaxFilteredCount", 6);

		public static readonly DependencyProperty SourceCollectionProperty = DPM.Register<IEnumerable, SearchTextBox>
			("SourceCollection", s => s.OnSourceCollectionChanged);

		public static readonly DependencyProperty DisplayMemberProperty = DPM.Register<string, SearchTextBox>
			("DisplayMember", s => s.OnDisplayMemberChanged);

		public static readonly DependencyProperty SelectedItemProperty = DPM.Register<object, SearchTextBox>
			("SelectedItem", s => s.OnSelectedItemChanged, true);

		public static readonly DependencyProperty SelectedIndexProperty = DPM.Register<int, SearchTextBox>
			("SelectedIndex", -1, s => s.OnSelectedIndexChanged, true);

		public static readonly DependencyProperty SelectedItemTemplateProperty = DPM.Register<DataTemplate, SearchTextBox>
			("SelectedItemTemplate");

		public static readonly DependencyProperty DelayProperty = DPM.Register<TimeSpan, SearchTextBox>
			("Delay", TimeSpan.Zero);

		public static readonly DependencyProperty AutoCompleteProperty = DPM.Register<bool, SearchTextBox>
			("AutoComplete", false);

		private static readonly DependencyPropertyKey AutoCompleteTextPropertyKey = DPM.RegisterReadOnly<string, SearchTextBox>
			("AutoCompleteText", s => s.OnAutoCompleteTextChanged);

		public static readonly DependencyProperty IsCaseSensitiveProperty = DPM.Register<bool, SearchTextBox>
			("IsCaseSensitive", s => s.OnIsCaseSensitiveChanged);

		private static readonly DependencyPropertyKey IsInEditStatePropertyKey = DPM.RegisterReadOnly<bool, SearchTextBox>
			("IsInEditState", s => s.OnIsInEditStateChanged);

		private static readonly DependencyPropertyKey PreviewSelectedItemPropertyKey = DPM.RegisterReadOnly<object, SearchTextBox>
			("PreviewSelectedItem");

		private static readonly DependencyPropertyKey PreviewSelectedIndexPropertyKey = DPM.RegisterReadOnly<int, SearchTextBox>
			("PreviewSelectedIndex", -1);

		public static readonly DependencyProperty SelectedValueProperty = DPM.Register<object, SearchTextBox>
			("SelectedValue", s => s.OnSelectedValueChanged, true);

		private static readonly DependencyPropertyKey PreviewSelectedValuePropertyKey = DPM.RegisterReadOnly<object, SearchTextBox>
			("PreviewSelectedValue");

		private static readonly DependencyPropertyKey ActualDropDownItemsSourcePropertyKey = DPM.RegisterReadOnly<IEnumerable, SearchTextBox>
			("ActualDropDownItemsSource");

		public static readonly DependencyProperty DropDownItemsModeProperty = DPM.Register<SearchTextBoxDropDownItemsMode, SearchTextBox>
			("DropDownItemsMode", SearchTextBoxDropDownItemsMode.All, s => s.OnDropDownItemsModeChanged);

		public static readonly DependencyProperty ValueMemberProperty = DPM.Register<string, SearchTextBox>
			("ValueMember", s => s.OnValueMemberChanged);

		public static readonly DependencyProperty TailContentProperty = DPM.Register<object, SearchTextBox>
			("TailContent");

		public static readonly DependencyProperty TailContentTemplateProperty = DPM.Register<DataTemplate, SearchTextBox>
			("TailContentTemplate");

		public static readonly DependencyProperty HeadContentProperty = DPM.Register<object, SearchTextBox>
			("HeadContent");

		public static readonly DependencyProperty HeadContentTemplateProperty = DPM.Register<DataTemplate, SearchTextBox>
			("HeadContentTemplate");

		public static readonly DependencyProperty ShowWatermarkProperty = DPM.Register<bool, SearchTextBox>
			("ShowWatermark", true, s => s.OnShowWatermarkChanged);

		public static readonly DependencyProperty PopupHeaderProperty = DPM.Register<object, SearchTextBox>
			("PopupHeader");

		public static readonly DependencyProperty PopupFooterProperty = DPM.Register<object, SearchTextBox>
			("PopupFooter");

		public static readonly DependencyProperty PopupHeaderTemplateProperty = DPM.Register<DataTemplate, SearchTextBox>
			("PopupHeaderTemplate");

		public static readonly DependencyProperty PopupFooterTemplateProperty = DPM.Register<DataTemplate, SearchTextBox>
			("PopupFooterTemplate");

		private static readonly DependencyPropertyKey ActualShowWatermarkPropertyKey = DPM.RegisterReadOnly<bool, SearchTextBox>
			("ActualShowWatermark", true);

		public static readonly DependencyProperty ActualShowWatermarkProperty = ActualShowWatermarkPropertyKey.DependencyProperty;
		public static readonly DependencyProperty ActualDropDownItemsSourceProperty = ActualDropDownItemsSourcePropertyKey.DependencyProperty;
		public static readonly DependencyProperty ActualSelectedItemTextProperty = ActualSelectedItemTextPropertyKey.DependencyProperty;
		public static readonly DependencyProperty AutoCompleteTextProperty = AutoCompleteTextPropertyKey.DependencyProperty;
		public static readonly DependencyProperty PreviewSelectedItemProperty = PreviewSelectedItemPropertyKey.DependencyProperty;
		public static readonly DependencyProperty PreviewSelectedIndexProperty = PreviewSelectedIndexPropertyKey.DependencyProperty;
		public static readonly DependencyProperty PreviewSelectedValueProperty = PreviewSelectedValuePropertyKey.DependencyProperty;
		public static readonly DependencyProperty IsInEditStateProperty = IsInEditStatePropertyKey.DependencyProperty;

		#endregion

		#region Fields

		private readonly DelayAction _delaySearch;
		private readonly Dictionary<Type, ValueGetter> _displayGettersCache = new Dictionary<Type, ValueGetter>();

		private readonly MultiMap<string, object> _searchDictionary = new MultiMap<string, object>();
		private readonly Dictionary<Type, ValueGetter> _valueGettersCache = new Dictionary<Type, ValueGetter>();
		private ISearchTextBoxAdvisor _advisor;
		private int _filteredSelectedIndex;

		private bool _skipAutoCompleteTextChanged;
		private bool _skipOnAutoCompleteTextBoxSearchTextChanged;
		private bool _skipSyncPreviewSelectedItem;

		private StringComparison _stringComparison;
		public event EventHandler BeginEdit;
		public event EventHandler EndEdit;

		public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;
		public event EventHandler<SelectedIndexChangedEventArgs> SelectedIndexChanged;
		public event EventHandler<SelectedValueChangedEventArgs> SelectedValueChanged;

		#endregion

		#region Ctors

		static SearchTextBox()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<SearchTextBox>();
		}

		public SearchTextBox()
		{
			this.OverrideStyleKey<SearchTextBox>();

			FilteredItemsSource = new SearchResultCollection();

			_delaySearch = new DelayAction(Search);

			UpdateStringComparison();
			UpdateActualDropDownItemsSource();
		}

		#endregion

		#region Properties

		public IEnumerable ActualDropDownItemsSource
		{
			get => (IEnumerable)GetValue(ActualDropDownItemsSourceProperty);
			private set => this.SetReadOnlyValue(ActualDropDownItemsSourcePropertyKey, value);
		}

		public string ActualSelectedItemText
		{
			get => (string) GetValue(ActualSelectedItemTextProperty);
			private set => this.SetReadOnlyValue(ActualSelectedItemTextPropertyKey, value);
		}

		public bool ActualShowWatermark
		{
			get => (bool) GetValue(ActualShowWatermarkProperty);
			private set => this.SetReadOnlyValue(ActualShowWatermarkPropertyKey, value);
		}

		public ISearchTextBoxAdvisor Advisor
		{
			get => _advisor;
			set
			{
				if (ReferenceEquals(_advisor, value))
					return;

				_advisor = value;

				OnAdvisorChanged();
			}
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

		public string DisplayMember
		{
			get => (string) GetValue(DisplayMemberProperty);
			set => SetValue(DisplayMemberProperty, value);
		}

		public SearchTextBoxDropDownItemsMode DropDownItemsMode
		{
			get => (SearchTextBoxDropDownItemsMode) GetValue(DropDownItemsModeProperty);
			set => SetValue(DropDownItemsModeProperty, value);
		}

		private Control DummyFocus => TemplateContract.DummyFocus;

		private IEnumerable<object> EnumerableSource => (SourceCollection as IEnumerable)?.Cast<object>() ?? Enumerable.Empty<object>();

		public SearchResultCollection FilteredItemsSource { get; }

		private int FilteredSelectedIndex
		{
			get => _filteredSelectedIndex;
			set
			{
				_filteredSelectedIndex = value;

				UpdatePreviewSelectedItem(FilteredSelectedItem);

				SyncListBoxSelectedItem();
			}
		}

		private object FilteredSelectedItem => FilteredItemsSource.Cast<object>().ElementAtOrDefault(_filteredSelectedIndex);

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

		private bool IsPopupOpen => PopupBar?.IsOpen == true;

		public IEnumerable SourceCollection
		{
			get => (IEnumerable)GetValue(SourceCollectionProperty);
			set => SetValue(SourceCollectionProperty, value);
		}

		public DataTemplate ItemTemplate
		{
			get => (DataTemplate) GetValue(ItemTemplateProperty);
			set => SetValue(ItemTemplateProperty, value);
		}

		private ListBox ListBox => TemplateContract.DropDownListBox;

		public int MaxFilteredCount
		{
			get => (int) GetValue(MaxFilteredCountProperty);
			set => SetValue(MaxFilteredCountProperty, value);
		}

		private PopupBar PopupBar => TemplateContract.PopupBar;

		public object PopupFooter
		{
			get => GetValue(PopupFooterProperty);
			set => SetValue(PopupFooterProperty, value);
		}

		public DataTemplate PopupFooterTemplate
		{
			get => (DataTemplate) GetValue(PopupFooterTemplateProperty);
			set => SetValue(PopupFooterTemplateProperty, value);
		}

		public object PopupHeader
		{
			get => GetValue(PopupHeaderProperty);
			set => SetValue(PopupHeaderProperty, value);
		}

		public DataTemplate PopupHeaderTemplate
		{
			get => (DataTemplate) GetValue(PopupHeaderTemplateProperty);
			set => SetValue(PopupHeaderTemplateProperty, value);
		}

		public int PreviewSelectedIndex
		{
			get => (int) GetValue(PreviewSelectedIndexProperty);
			private set => this.SetReadOnlyValue(PreviewSelectedIndexPropertyKey, value);
		}

		public object PreviewSelectedItem
		{
			get => GetValue(PreviewSelectedItemProperty);
			private set => this.SetReadOnlyValue(PreviewSelectedItemPropertyKey, value);
		}

		public object PreviewSelectedValue
		{
			get => GetValue(PreviewSelectedValueProperty);
			private set => this.SetReadOnlyValue(PreviewSelectedValuePropertyKey, value);
		}

		public string SearchText
		{
			get => (string) GetValue(SearchTextProperty);
			set => SetValue(SearchTextProperty, value);
		}

		public int SelectedIndex
		{
			get => (int) GetValue(SelectedIndexProperty);
			set => SetValue(SelectedIndexProperty, value);
		}

		public object SelectedItem
		{
			get => GetValue(SelectedItemProperty);
			set => SetValue(SelectedItemProperty, value);
		}

		public DataTemplate SelectedItemTemplate
		{
			get => (DataTemplate) GetValue(SelectedItemTemplateProperty);
			set => SetValue(SelectedItemTemplateProperty, value);
		}

		public object SelectedValue
		{
			get => GetValue(SelectedValueProperty);
			set => SetValue(SelectedValueProperty, value);
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

		private SearchTextBoxTemplateContract TemplateContract => (SearchTextBoxTemplateContract) TemplateContractInternal;

		public string ValueMember
		{
			get => (string) GetValue(ValueMemberProperty);
			set => SetValue(ValueMemberProperty, value);
		}

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

		#endregion

		#region  Methods

		private void CancelEdit()
		{
			FinishEdit();
		}

		private void CheckSelectedIndex(int index)
		{
			if ((index != -1 && index >= EnumerableSource.Count()) || index < -1)
				throw new IndexOutOfRangeException(nameof(index));
		}

		private void CheckSelectedItem(object item)
		{
			if (item != null && EnumerableSource.Contains(item) == false)
				throw new ArgumentOutOfRangeException(nameof(item));
		}

		private void ClosePopup()
		{
			PopupBar?.Close();
		}

		private void CommitEdit()
		{
			this.SetValue(SelectedItemProperty, PreviewSelectedItem, true);
			this.SetValue(SelectedIndexProperty, FindItem(SelectedItem), true);

			SelectedValue = PreviewSelectedValue;

			UpdateActualSelectedItemText();

			FinishEdit();
		}

		private static ValueGetter CreateGetter(Type type, string member)
		{
			return AccessorFactory.CreateGetter(type, member);
		}

		private void EvaluateAutoComplete(bool search)
		{
			if (AutoComplete == false || SearchText.IsNullOrEmpty() || (search && FilteredItemsSource.Count == 0))
			{
				AutoCompleteText = null;

				return;
			}

			if (search == false)
				return;

			AutoCompleteText = GetAutoCompleteText(FilteredSelectedItem);
		}

		private int FindItem(object item)
		{
			return EnumerableSource.IndexOfReference(item);
		}

		private void FinishEdit()
		{
			SyncSelectedItemSearchText(SelectedItem);
			MoveCaretToEnd();

			ClearFocus();

			ClosePopup();
		}

		private void ClearFocus()
		{
			if (DummyFocus != null)
				DummyFocus.Focus();
			else
				Focus();
		}

		private string GetAutoCompleteText(object item)
		{
			var advisor = Advisor;

			return advisor == null || advisor.SupportAutoComplete() == false ? GetDisplayValue(item) : advisor.GetAutoCompleteText(item, SearchText);
		}

		private string GetDisplayValue(object item)
		{
			if (item == null)
				return string.Empty;

			if (Advisor != null && Advisor.SupportDisplayValue())
				return Advisor.GetDisplayValue(item);

			if (string.IsNullOrEmpty(DisplayMember))
				return item.ToString();

			var type = item.GetType();
			var getter = _displayGettersCache.GetValueOrCreate(type, t => CreateGetter(t, DisplayMember));

			if (getter == null)
				return string.Empty;

			return (string) getter(item) ?? string.Empty;
		}

		private object GetItemValue(object item)
		{
			if (item == null)
				return null;

			if (Advisor != null && Advisor.SupportValue())
				return Advisor.GetValue(item);

			if (string.IsNullOrEmpty(ValueMember))
				return item;

			var type = item.GetType();
			var getter = _valueGettersCache.GetValueOrCreate(type, t => CreateGetter(t, ValueMember));

			if (getter == null)
				return item;

			return getter(item);
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
				case Key.Down:

					if (IsPopupOpen)
						PreviewSelectNext();

					return true;

				case Key.Up:

					if (IsPopupOpen)
						PreviewSelectPrev();

					return true;

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

		private void ListBoxOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			var freSource = PresentationTreeUtils.GetUIElementEventSource(e.OriginalSource) as FrameworkElement;
			var listBoxItem = freSource?.GetVisualAncestors().OfType<ListBoxItem>().FirstOrDefault();

			if (listBoxItem == null)
				return;

			listBoxItem.IsSelected = true;

			CommitEdit();
		}

		private void MoveCaretToEnd()
		{
			AutoCompleteTextBox?.Select(AutoCompleteTextBox.Text.Length, 0);
		}

		private void OnAdvisorChanged()
		{
			UpdateSearchDictionary();
			SyncSelectedItemSearchText(SelectedItem);
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
			if (AutoCompleteTextBox?.HasKeyboardFocus() != true)
				OnEndEditInt();
		}

		private void OnAutoCompleteTextBoxPreviewTextInput(object sender, TextCompositionEventArgs textCompositionEventArgs)
		{
			_delaySearch.Revoke();
		}

		private void OnAutoCompleteTextBoxTypedTextChanged(object sender, EventArgs e)
		{
			if (_skipOnAutoCompleteTextBoxSearchTextChanged)
				return;

			//var commitAutoComplete = string.Equals(AutoCompleteText, _autoCompleteTextBox.TypedText, _stringComparison);

			UpdateSearchText(AutoCompleteTextBox.TypedText);

			//if (commitAutoComplete)
			//	return;

			EvaluateAutoComplete(false);
			StartSearch();
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
			IsInEditState = true;

			SyncPreviewSelection();

			OnBeginEdit();

			UpdateWatermark();
		}

		private void OnDisplayMemberChanged()
		{
			_displayGettersCache.Clear();
			UpdateSearchDictionary();
		}

		private void OnDropDownItemsModeChanged()
		{
			UpdateActualDropDownItemsSource();
		}

		protected virtual void OnEndEdit()
		{
			EndEdit?.Invoke(this, EventArgs.Empty);
		}

		private void OnEndEditInt()
		{
			if (IsKeyboardFocusWithin == false)
				ClosePopup();

			OnEndEdit();

			SyncPreviewSelection();

			IsInEditState = false;

			UpdateWatermark();
		}

		protected override void OnGotFocus(RoutedEventArgs e)
		{
			base.OnGotFocus(e);

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

		private void OnSourceCollectionChanged()
		{
			_displayGettersCache.Clear();

			UpdateActualDropDownItemsSource();
			UpdateSearchDictionary();
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			var focusedElement = FocusHelper.GetFocusedElement();

			if (ReferenceEquals(this, focusedElement) || ReferenceEquals(DummyFocus, focusedElement))
			{
				AutoCompleteTextBox.Focus();
				e.Handled = true;
			}
			else
				e.Handled = HandleKey(e.Key);
		}

		private void OnListBoxSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
		{
			FilteredSelectedIndex = ListBox.SelectedIndex;
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

		private void OnSearchTextChanged()
		{
			UpdateAutoCompleteBoxSearchText(SearchText);
		}

		private void OnSelectedIndexChanged(int oldIndex)
		{
			CheckSelectedIndex(SelectedIndex);

			var oldItem = SelectedItem;
			var oldValue = SelectedValue;

			this.SetValue(SelectedItemProperty, EnumerableSource.ElementAtOrDefault(SelectedIndex), true);
			this.SetValue(SelectedValueProperty, GetItemValue(SelectedItem), true);

			OnSelectionChangedInt();

			RaiseSelectionChanged(oldItem, oldIndex, oldValue);
		}

		private void OnSelectedItemChanged(object oldItem)
		{
			CheckSelectedItem(SelectedItem);

			var oldIndex = SelectedIndex;
			var oldValue = SelectedValue;

			this.SetValue(SelectedIndexProperty, FindItem(SelectedItem), true);
			this.SetValue(SelectedValueProperty, GetItemValue(SelectedItem), true);

			OnSelectionChangedInt();

			RaiseSelectionChanged(oldItem, oldIndex, oldValue);
		}

		private void OnSelectedValueChanged(object oldValue)
		{
			var value = SelectedValue;
			var index = 0;

			foreach (var item in EnumerableSource)
			{
				var itemValue = GetItemValue(item);

				if (Equals(value, itemValue))
				{
					var oldItem = SelectedItem;
					var oldIndex = SelectedIndex;

					this.SetValue(SelectedItemProperty, item, true);
					this.SetValue(SelectedIndexProperty, index, true);

					OnSelectionChangedInt();

					RaiseSelectionChanged(oldItem, oldIndex, oldValue);

					return;
				}

				index++;
			}
		}

		private void OnSelectionChangedInt()
		{
			SyncPreviewSelection();
			SyncSelectedItemSearchText(SelectedItem);

			UpdateWatermark();

			if (AutoCompleteTextBox?.HasFocus() == false)
				CommitEdit();
		}

		private void OnShowWatermarkChanged()
		{
			UpdateWatermark();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			PopupBar.Closing += PopupCloseControllerOnClosing;

			AutoCompleteTextBox.TypedTextChanged += OnAutoCompleteTextBoxTypedTextChanged;
			AutoCompleteTextBox.AutoCompleteTextChanged += OnAutoCompleteTextBoxAutoCompleteTextChanged;
			AutoCompleteTextBox.TextInput += OnAutoCompleteTextBoxPreviewTextInput;

#if SILVERLIGHT
      _autoCompleteTextBox.LostFocus += OnAutoCompleteTextBoxLostFocus;
      _autoCompleteTextBox.GotFocus += OnAutoCompleteTextBoxGotFocus;
#else
			AutoCompleteTextBox.LostKeyboardFocus += OnAutoCompleteTextBoxLostFocus;
			AutoCompleteTextBox.GotKeyboardFocus += OnAutoCompleteTextBoxGotFocus;
#endif

			ListBox.AddHandler(MouseLeftButtonDownEvent, (MouseButtonEventHandler) ListBoxOnMouseLeftButtonDown, true);
			ListBox.SelectionChanged += OnListBoxSelectionChanged;

			UpdateAutoCompleteBoxSearchText(SearchText);

			UpdateFocus();
		}

		protected override void OnTemplateContractDetaching()
		{
			PopupBar.Closing -= PopupCloseControllerOnClosing;
			AutoCompleteTextBox.TypedTextChanged -= OnAutoCompleteTextBoxTypedTextChanged;
			AutoCompleteTextBox.AutoCompleteTextChanged -= OnAutoCompleteTextBoxAutoCompleteTextChanged;
			AutoCompleteTextBox.TextInput -= OnAutoCompleteTextBoxPreviewTextInput;

#if SILVERLIGHT
      _autoCompleteTextBox.LostFocus -= OnAutoCompleteTextBoxLostFocus;
      _autoCompleteTextBox.GotFocus -= OnAutoCompleteTextBoxGotFocus;
#else
			AutoCompleteTextBox.LostKeyboardFocus -= OnAutoCompleteTextBoxLostFocus;
			AutoCompleteTextBox.GotKeyboardFocus -= OnAutoCompleteTextBoxGotFocus;
#endif

			ListBox.RemoveHandler(MouseLeftButtonDownEvent, (MouseButtonEventHandler) ListBoxOnMouseLeftButtonDown);
			ListBox.SelectionChanged -= OnListBoxSelectionChanged;

			base.OnTemplateContractDetaching();
		}

		private void OnValueMemberChanged()
		{
			this.SetValue(SelectedValueProperty, GetItemValue(SelectedItem), true);

			if (IsInEditState)
				PreviewSelectedValue = GetItemValue(PreviewSelectedItem);
		}

		private void OpenPopup()
		{
			if (PopupBar != null)
				PopupBar.IsOpen = true;
		}

		private void PopupCloseControllerOnClosing(object sender, CancelEventArgs cancelEventArgs)
		{
			CancelEdit();
		}

		private void PreviewSelectNext()
		{
			FilteredSelectedIndex = FilteredSelectedIndex < FilteredItemsSource.Count - 1 ? FilteredSelectedIndex + 1 : 0;
		}

		private void PreviewSelectPrev()
		{
			FilteredSelectedIndex = FilteredSelectedIndex > 0 ? FilteredSelectedIndex - 1 : FilteredItemsSource.Count - 1;
		}

		private void RaiseSelectionChanged(object oldItem, int oldIndex, object oldValue)
		{
			SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(SelectedItem, oldItem));
			SelectedIndexChanged?.Invoke(this, new SelectedIndexChangedEventArgs(SelectedIndex, oldIndex));
			SelectedValueChanged?.Invoke(this, new SelectedValueChangedEventArgs(SelectedValue, oldValue));
		}

		private void Search()
		{
			try
			{
				_skipSyncPreviewSelectedItem = true;

				var enumerableSource = EnumerableSource;

				if (string.IsNullOrWhiteSpace(SearchText))
				{
					FilteredItemsSource.SetResult(enumerableSource.Take(MaxFilteredCount));
					FilteredSelectedIndex = -1;
					return;
				}

				var filteredSelectedItem = FilteredSelectedItem;

				FilteredItemsSource.SetResult(Advisor == null ? SearchThroughDictionary(MaxFilteredCount) : SearchThroughAdvisor(MaxFilteredCount));

				if (ReferenceEquals(filteredSelectedItem, FilteredSelectedItem) == false || filteredSelectedItem == null)
					FilteredSelectedIndex = FilteredItemsSource.Count > 0 ? 0 : -1;
				else
					SyncListBoxSelectedItem();

				EvaluateAutoComplete(true);
			}
			finally
			{
				_skipSyncPreviewSelectedItem = false;

				ShowDropDown();
			}
		}

		private IEnumerable<object> SearchThroughAdvisor(int count)
		{
			var advisor = Advisor;
			var searchText = SearchText;

			return EnumerableSource.Where(i => advisor.AcceptItem(i, searchText)).Take(count);
		}

		private IEnumerable<object> SearchThroughDictionary(int count)
		{
			return _searchDictionary.Keys.Where(ShouldTakeItem).Select(key => _searchDictionary[key]).SelectMany(l => l).Take(count);
		}

		private bool ShouldOpenResultPopup()
		{
			return SourceCollection != null;
		}

		private bool ShouldTakeItem(string item)
		{
			return item.StartsWith(SearchText, _stringComparison);
		}

		private void ShowDropDown()
		{
			if (ShouldOpenResultPopup())
				OpenPopup();
		}

		private void StartSearch()
		{
			_delaySearch.Invoke(Delay);
		}

		private void SyncListBoxSelectedItem()
		{
			if (ListBox != null)
				ListBox.SelectedIndex = _filteredSelectedIndex;
		}

		private void SyncPreviewSelectedItem()
		{
			var previewSelectedItem = PreviewSelectedItem;

			if (previewSelectedItem == null)
				return;

			UpdateSearchText(GetDisplayValue(previewSelectedItem));
			UpdateAutoCompleteBoxSearchText(SearchText);

			MoveCaretToEnd();
		}

		private void SyncPreviewSelection()
		{
			PreviewSelectedItem = SelectedItem;
			PreviewSelectedIndex = SelectedIndex;
			PreviewSelectedValue = SelectedValue;
		}

		private void SyncSelectedItemSearchText(object item)
		{
			var text = GetDisplayValue(item);

			UpdateSearchText(text);
			UpdateAutoCompleteBoxSearchText(text);
			UpdateActualSelectedItemText();
		}

		private void UpdateActualDropDownItemsSource()
		{
			ActualDropDownItemsSource = DropDownItemsMode == SearchTextBoxDropDownItemsMode.All ? SourceCollection : FilteredItemsSource;
		}

		private void UpdateActualSelectedItemText()
		{
			ActualSelectedItemText = GetDisplayValue(SelectedItem);
		}

		private void UpdateAutoCompleteBoxSearchText(string text)
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

		private void UpdateFocus()
		{
			if (AutoCompleteTextBox == null)
				return;

#if SILVERLIGHT
      if (ReferenceEquals(this, FocusHelper.GetFocusedElement()))
        _autoCompleteTextBox.Focus();
#else
			if (ReferenceEquals(this, Keyboard.FocusedElement))
				Keyboard.Focus(AutoCompleteTextBox);
#endif
		}

		private void UpdatePreviewSelectedItem(object previewSelectedItem)
		{
			if (ReferenceEquals(PreviewSelectedItem, previewSelectedItem) == false)
			{
				PreviewSelectedItem = previewSelectedItem;
				PreviewSelectedIndex = FindItem(previewSelectedItem);
				PreviewSelectedValue = GetItemValue(previewSelectedItem);
			}

			if (_skipSyncPreviewSelectedItem)
				return;

			SyncPreviewSelectedItem();
		}

		private void UpdateSearchDictionary()
		{
			_searchDictionary.Clear();

			if (Advisor != null)
				return;

			foreach (var item in EnumerableSource.SkipNull())
			{
				var searchData = GetDisplayValue(item);

				if (string.IsNullOrWhiteSpace(searchData))
					continue;

				_searchDictionary.AddValue(searchData, item);
			}
		}

		private void UpdateSearchText(string text)
		{
			this.SetValue(SearchTextProperty, text, true);
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
			ActualShowWatermark = ShowWatermark && IsInEditState == false && SelectedItem == null;
		}

		#endregion

		#region Interface Implementations

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#endregion
	}

	public interface ISearchTextBoxAdvisor
	{
		#region Properties

		SearchTextBoxAdvisorFeature Features { get; }

		#endregion

		#region  Methods

		bool AcceptItem(object item, string searchText);
		string GetAutoCompleteText(object item, string searchText);
		string GetDisplayValue(object item);
		object GetValue(object item);

		#endregion
	}

	internal static class SearchBoxAdvisorExtensions
	{
		#region  Methods

		public static bool SupportAutoComplete(this ISearchTextBoxAdvisor advisor)
		{
			return (advisor.Features & SearchTextBoxAdvisorFeature.AutoComplete) != 0;
		}

		public static bool SupportDisplayValue(this ISearchTextBoxAdvisor advisor)
		{
			return (advisor.Features & SearchTextBoxAdvisorFeature.DisplayValue) != 0;
		}

		public static bool SupportValue(this ISearchTextBoxAdvisor advisor)
		{
			return (advisor.Features & SearchTextBoxAdvisorFeature.Value) != 0;
		}

		#endregion
	}

	public class SearchTextBoxTemplateContract : TemplateContract
	{
		#region Properties

		[TemplateContractPart]
		public AutoCompleteTextBox AutoCompleteTextBox { get; [UsedImplicitly] private set; }

		[TemplateContractPart]
		public Control DummyFocus { get; [UsedImplicitly] private set; }

		[TemplateContractPart]
		public ListBox DropDownListBox { get; [UsedImplicitly] private set; }

		[TemplateContractPart]
		public PopupBar PopupBar { get; [UsedImplicitly] private set; }

		#endregion
	}

	[Flags]
	public enum SearchTextBoxAdvisorFeature
	{
		None = 0,
		AutoComplete = 1,
		DisplayValue = 2,
		Value = 4,
		All = AutoComplete | DisplayValue | Value
	}

	public enum SearchTextBoxDropDownItemsMode
	{
		All,
		Filtered
	}
}