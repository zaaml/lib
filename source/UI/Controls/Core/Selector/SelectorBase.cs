// <copyright file="SelectorBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Panels.Core;
using NativeControl = System.Windows.Controls.Control;

namespace Zaaml.UI.Controls.Core
{
	public abstract class SelectorBase<TControl, TItem, TCollection, TPresenter, TPanel> : ScrollableItemsControlBase<TControl, TItem, TCollection, TPresenter, TPanel>, ISelector<TItem>
		where TItem : NativeControl, ISelectable
		where TCollection : ItemCollectionBase<TControl, TItem>
		where TPresenter : ScrollableItemsPresenterBase<TControl, TItem, TCollection, TPanel>
		where TPanel : ItemsPanel<TItem>
		where TControl : SelectorBase<TControl, TItem, TCollection, TPresenter, TPanel>
	{
		public static readonly DependencyProperty SelectedItemProperty = DPM.Register<TItem, SelectorBase<TControl, TItem, TCollection, TPresenter, TPanel>>
			("SelectedItem", s => s.SelectorController.OnSelectedItemPropertyChanged, s => s.SelectorController.CoerceSelectedItem);

		public static readonly DependencyProperty SelectedItemSourceProperty = DPM.Register<object, SelectorBase<TControl, TItem, TCollection, TPresenter, TPanel>>
			("SelectedItemSource", s => s.SelectorController.OnSelectedItemSourcePropertyChanged, s => s.SelectorController.CoerceSelectedItemSource);

		public static readonly DependencyProperty SelectedValueProperty = DPM.Register<object, SelectorBase<TControl, TItem, TCollection, TPresenter, TPanel>>
			("SelectedValue", s => s.SelectorController.OnSelectedValuePropertyChanged, s => s.SelectorController.CoerceSelectedValue);

		public static readonly DependencyProperty SelectedValueMemberPathProperty = DPM.Register<string, SelectorBase<TControl, TItem, TCollection, TPresenter, TPanel>>
			("SelectedValueMemberPath", s => s.OnSelectedValuePathChanged);

		public static readonly DependencyProperty SelectedValueSourceProperty = DPM.Register<SelectedValueSource, SelectorBase<TControl, TItem, TCollection, TPresenter, TPanel>>
			("SelectedValueSource", SelectedValueSource.Auto, s => s.OnSelectedValueSourceChanged);

		private byte _packedValue;

		private MemberValueEvaluator _selectedValueEvaluator;
		private SelectorController<TControl, TItem> _selectorController;
		public event EventHandler<SelectionChangedEventArgs<TItem>> SelectionChanged;

		static SelectorBase()
		{
		}

		internal bool AllowNullSelection
		{
			get => SelectorController.AllowNullSelection;
			set => SelectorController.AllowNullSelection = value;
		}

		protected NavigateMode CurrentNavigateMode => IsItemsHostVisible ? NavigateMode.Focus : NavigateMode.Select;

		private protected virtual bool DefaultAllowNullSelection => true;

		private protected virtual bool DefaultPreferSelection => false;

		internal bool IsInitializing
		{
			get => PackedDefinition.IsInitializing.GetValue(_packedValue);
			private set => PackedDefinition.IsInitializing.SetValue(ref _packedValue, value);
		}

		internal bool PreferSelection
		{
			get => SelectorController.PreferSelection;
			set => SelectorController.PreferSelection = value;
		}

		internal virtual int SelectedIndexInternal => -1;

		public TItem SelectedItem
		{
			get => (TItem) GetValue(SelectedItemProperty);
			set => SetValue(SelectedItemProperty, value);
		}

		public object SelectedItemSource
		{
			get => GetValue(SelectedItemSourceProperty);
			set => SetValue(SelectedItemSourceProperty, value);
		}

		public object SelectedValue
		{
			get => GetValue(SelectedValueProperty);
			set => SetValue(SelectedValueProperty, value);
		}

		public string SelectedValueMemberPath
		{
			get => (string) GetValue(SelectedValueMemberPathProperty);
			set => SetValue(SelectedValueMemberPathProperty, value);
		}

		public SelectedValueSource SelectedValueSource
		{
			get => (SelectedValueSource) GetValue(SelectedValueSourceProperty);
			set => SetValue(SelectedValueSourceProperty, value);
		}

		public Selection<TItem> Selection => new Selection<TItem>(SelectedIndexInternal, SelectedItem, SelectedItemSource, SelectedValue);

		internal bool SelectItemOnFocus { get; set; } = true;

		internal SelectorController<TControl, TItem> SelectorController => _selectorController ??= CreateSelectorControllerPrivate();

		public override void BeginInit()
		{
			base.BeginInit();

			IsInitializing = true;
			SelectorController.SuspendSelectionChange();

			SelectorController.AllowNullSelection = DefaultAllowNullSelection;
			SelectorController.PreferSelection = DefaultPreferSelection;
		}

		internal virtual SelectorController<TControl, TItem> CreateSelectorController()
		{
			return new SelectorController<TControl, TItem>((TControl) this, new ItemCollectionSelectorAdvisor<TControl, TItem>(this, Items));
		}

		private SelectorController<TControl, TItem> CreateSelectorControllerPrivate()
		{
			var selectorController = CreateSelectorController();

			if (IsInitializing == false)
			{
				selectorController.AllowNullSelection = DefaultAllowNullSelection;
				selectorController.PreferSelection = DefaultPreferSelection;
			}

			return selectorController;
		}

		public override void EndInit()
		{
			SelectorController.ResumeSelectionChange();
			IsInitializing = false;

			base.EndInit();
		}

		private object GetItemValue(object item)
		{
			try
			{
				return _selectedValueEvaluator.GetValue(item);
			}
			catch (Exception e)
			{
				LogService.LogError(e);
			}

			return null;
		}

		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			if (ReferenceEquals(this, e.NewFocus))
			{
				if (SelectedItem != null)
					e.Handled = SelectedItem.Focus();

				if (e.Handled == false)
				{
					if (e.OldFocus is UIElement previousFocus && !previousFocus.IsDescendantOf(this))
					{
						if (ReferenceEquals(this, previousFocus.PredictFocus(FocusNavigationDirection.Up)))
							e.Handled = MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
						else if (ReferenceEquals(this, previousFocus.PredictFocus(FocusNavigationDirection.Down)))
							e.Handled = MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
						else if (ReferenceEquals(this, previousFocus.PredictFocus(FocusNavigationDirection.Left)))
							e.Handled = MoveFocus(new TraversalRequest(FocusNavigationDirection.Left));
						else if (ReferenceEquals(this, previousFocus.PredictFocus(FocusNavigationDirection.Right))) 
							e.Handled = MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
					}
				}

				if (e.Handled)
					return;
			}

			base.OnGotKeyboardFocus(e);
		}

		internal override void OnItemAttachedInternal(TItem item)
		{
			base.OnItemAttachedInternal(item);

			var itemSource = Items.GetItemSourceInternal(item);

			if (itemSource != null && ReferenceEquals(itemSource, SelectedItemSource))
			{
				if (ReferenceEquals(item, SelectedItem) == false)
					SelectedItem = item;
			}

			if (ReferenceEquals(item, SelectedItem))
				item.IsSelected = true;
			else if (item.IsSelected)
				item.IsSelected = false;
		}

		private protected virtual bool ActualSelectItemOnFocus => SelectItemOnFocus;
		
		private protected override void OnItemGotFocus(TItem item)
		{
			base.OnItemGotFocus(item);
			
			if (ActualSelectItemOnFocus)
				SetIsSelectedInternal(item, true);
		}

		protected override void OnLoaded()
		{
			base.OnLoaded();

			var selectedItem = SelectedItem;

			if (selectedItem != null)
				Items.BringIntoViewInternal(new BringIntoViewRequest<TItem>(selectedItem, DefaultBringIntoViewMode, 0));
		}

		protected virtual void OnSelectedIndexChanged(int oldIndex, int newIndex)
		{
		}

		protected virtual void OnSelectedItemChanged(TItem oldItem, TItem newItem)
		{
		}

		protected virtual void OnSelectedItemSourceChanged(object oldItemSource, object newItemSource)
		{
		}

		protected virtual void OnSelectedValueChanged(object oldValue, object newValue)
		{
		}

		private void OnSelectedValuePathChanged(string oldValue, string newValue)
		{
			try
			{
				_selectedValueEvaluator = new MemberValueEvaluator(newValue);
			}
			catch (Exception ex)
			{
				LogService.LogError(ex);
			}

			SelectorController.UpdateSelectedValue();
		}

		private void OnSelectedValueSourceChanged()
		{
			SelectorController.UpdateSelectedValue();
		}

		protected virtual void OnSelectionChanged(Selection<TItem> oldSelection, Selection<TItem> newSelection)
		{
		}

		private protected virtual void SetIsSelectedInternal(TItem item, bool value)
		{
			item.IsSelected = value;
		}

		DependencyProperty ISelector<TItem>.SelectedIndexProperty => null;

		DependencyProperty ISelector<TItem>.SelectedItemProperty => SelectedItemProperty;

		DependencyProperty ISelector<TItem>.SelectedItemSourceProperty => SelectedItemSourceProperty;

		DependencyProperty ISelector<TItem>.SelectedValueProperty => SelectedValueProperty;

		void ISelector<TItem>.OnSelectedIndexChanged(int oldIndex, int newIndex)
		{
			OnSelectedIndexChanged(oldIndex, newIndex);
		}

		void ISelector<TItem>.OnSelectedItemChanged(TItem oldItem, TItem newItem)
		{
			OnSelectedItemChanged(oldItem, newItem);
		}

		void ISelector<TItem>.OnSelectedItemSourceChanged(object oldItemSource, object newItemSource)
		{
			OnSelectedItemSourceChanged(oldItemSource, newItemSource);
		}

		void ISelector<TItem>.OnSelectedValueChanged(object oldValue, object newValue)
		{
			OnSelectedValueChanged(oldValue, newValue);
		}

		void ISelector<TItem>.OnSelectionChanged(Selection<TItem> oldSelection, Selection<TItem> newSelection)
		{
			OnSelectionChanged(oldSelection, newSelection);

			SelectionChanged?.Invoke(this, new SelectionChangedEventArgs<TItem>(oldSelection, newSelection));
		}

		object ISelector<TItem>.GetValue(TItem item, object itemSource)
		{
			switch (SelectedValueSource)
			{
				case SelectedValueSource.Auto:

					return GetItemValue(Items.SourceInternal == null ? item : itemSource);

				case SelectedValueSource.Item:

					return GetItemValue(item);

				case SelectedValueSource.ItemSource:

					return GetItemValue(itemSource);

				default:

					throw new ArgumentOutOfRangeException();
			}
		}

		private static class PackedDefinition
		{
			public static readonly PackedBoolItemDefinition IsInitializing;

			static PackedDefinition()
			{
				var allocator = new PackedValueAllocator();

				IsInitializing = allocator.AllocateBoolItem();
			}
		}
	}

	public abstract class SelectorBaseTemplateContract<TPresenter> : ScrollableItemsControlBaseTemplateContract<TPresenter>
		where TPresenter : ItemsPresenterBase
	{
	}
}