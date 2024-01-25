// <copyright file="DockItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Linq;
using Zaaml.Core;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore.CommandCore;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Interfaces;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;
#if !NETCOREAPP
using Zaaml.Core.Extensions;
#endif

namespace Zaaml.UI.Controls.Docking
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	[DebuggerTypeProxy(typeof(DockItemDebugView))]
	public class DockItem : TemplateContractContentControl, INotifyPropertyChanging, INotifyPropertyChanged, ISelectionStateControl, IActiveStateControl, ILayoutPropertyChangeListener
	{
		private static readonly DockItemSelectionScope DummySelectionScope = new();
		private static readonly DockItemSelectionScope DummyPreviewSelectionScope = new();

		private static readonly DependencyProperty NameInternalProperty = DPM.Register<string, DockItem>
			("NameInternal", d => d.OnNameChanged);

		public static readonly DependencyProperty TitleProperty = DPM.Register<string, DockItem>
			("Title", string.Empty, _ => OnTitleChanged, _ => OnCoerceTitle);

		public static readonly DependencyProperty IconProperty = DPM.Register<IconBase, DockItem>
			("Icon", i => i.LogicalChildMentor.OnLogicalChildPropertyChanged);

		public static readonly DependencyProperty IsSelectedProperty = DPM.Register<bool, DockItem>
			("IsSelected", d => d.OnIsSelectedChangedPrivate);

		public static readonly DependencyProperty ShowTitleProperty = DPM.Register<bool, DockItem>
			("ShowTitle");

		public static readonly DependencyProperty DockStateProperty = DPM.Register<DockItemState, DockItem>
			("DockState", DockItemState.Hidden, d => d.OnDockStatePropertyChanged, d => d.CoerceDockStateProperty);

		private static readonly DependencyPropertyKey ActualLayoutPropertyKey = DPM.RegisterReadOnly<BaseLayout, DockItem>
			("ActualLayout", d => d.OnActualLayoutPropertyChanged);

		private static readonly DependencyPropertyKey DockControlPropertyKey = DPM.RegisterReadOnly<DockControl, DockItem>
			("DockControl", d => d.OnDockControlPropertyChangedPrivate);

		public static readonly DependencyProperty DockControlProperty = DockControlPropertyKey.DependencyProperty;

		public DockControl DockControl
		{
			get => (DockControl)GetValue(DockControlProperty);
			private set => this.SetReadOnlyValue(DockControlPropertyKey, value);
		}

		private void OnDockControlPropertyChangedPrivate(DockControl oldValue, DockControl newValue)
		{
		}

		internal void AttachDockControlInternal(DockControl dockControl)
		{
			if (ReferenceEquals(DockControl, null) == false)
				throw new InvalidOperationException("DockControl is already attached.");

			AttachDockControlCore(dockControl);
		}

		protected virtual void AttachDockControlCore(DockControl dockControl)
		{
			DockControl = dockControl;
		}

		protected virtual void DetachDockControlCore(DockControl dockControl)
		{
			DockControl = null;
		}

		internal void DetachDockControlInternal(DockControl dockControl)
		{
			if (ReferenceEquals(DockControl, dockControl) == false)
				throw new InvalidOperationException("DockControl is not attached.");

			DetachDockControlCore(dockControl);
		}

		public static readonly DependencyProperty ActualLayoutProperty = ActualLayoutPropertyKey.DependencyProperty;
		private bool _attachToView = true;

		private AutoHideTabViewItem _autoHideTabViewItem;
		private DockControllerBase _controller;
		private FloatingDockWindow _floatingWindow;

		private ConditionalWeakTable<BaseLayout, LayoutOrderIndex> _layoutIndexDictionary = new();
		private FloatingDockWindow _previewFloatingWindow;
		private DockItem _previewItem;
		private int _suspendIsSelectedChangedHandler;

		private bool _syncSideProperty;
		private DockTabViewItem _tabViewItem;
		public event EventHandler<DockItemStateChangedEventArgs> DockStateChanged;
		public event EventHandler IsSelectedChanged;
		internal event EventHandler<PropertyValueChangedEventArgs> LayoutPropertyChanged;
		internal event EventHandler RequestUpdateVisualState;
		internal event EventHandler IsLockedChanged;

		static DockItem()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<DockItem>();
			FocusVisualStyleProperty.OverrideMetadata(typeof(DockItem), new FrameworkPropertyMetadata(null));
		}

		public DockItem()
		{
			this.OverrideStyleKey<DockItem>();

			foreach (var layoutProperty in FullLayout.LayoutProperties)
				this.AddValueChanged(layoutProperty, OnLayoutPropertyChanged);

			DragOutBehavior = new DragOutBehavior
			{
				DragOutCommand = new RelayCommand(OnDragOutCommandExecuted)
			};

			this.BindProperties(NameInternalProperty, this, NameProperty, BindingMode.TwoWay);

			LayoutUpdated += OnLayoutUpdated;
		}

		public BaseLayout ActualLayout
		{
			get => (BaseLayout) GetValue(ActualLayoutProperty);
			private set => this.SetReadOnlyValue(ActualLayoutPropertyKey, value);
		}

		internal LayoutKind ActualLayoutKind => ActualLayout?.LayoutKind ?? LayoutKind.Hidden;

		internal DockItemSelectionScope ActualSelectionScope
		{
			get
			{
				if (Controller != null)
					return Controller.SelectionScope;

				return IsPreview ? DummyPreviewSelectionScope : DummySelectionScope;
			}
		}

		internal bool AttachToView
		{
			get => _attachToView;
			set
			{
				if (_attachToView == value)
					return;

				_attachToView = value;

				OnAttachToViewChanged();
			}
		}

		internal AutoHideTabViewItem AutoHideTabViewItem => _autoHideTabViewItem ??= CreateAutoHideTabViewItem();

		internal DockItemContentPresenter ContentPresenter => TemplateContract.ContentPresenter;

		internal DockControllerBase Controller
		{
			get => _controller;
			set
			{
				if (ReferenceEquals(_controller, value))
					return;

				if (_controller != null)
					DetachController(_controller);

				_controller = value;

				if (_controller != null)
					AttachController(_controller);

				UpdateActualLayout();
			}
		}

		internal virtual string DebuggerDisplay
		{
			get
			{
				var dockItemLayout = CreateItemLayout();

				LayoutSettings.CopySettings(this, dockItemLayout, FullLayout.LayoutProperties);

				return dockItemLayout.Xml.ToString();
			}
		}

		public DockItemState DockState
		{
			get => (DockItemState) GetValue(DockStateProperty);
			set => SetValue(DockStateProperty, value);
		}

		private DragOutBehavior DragOutBehavior { get; }

		internal bool EnqueueSyncDragPosition { get; set; }

		internal FloatingDockWindow FloatingWindow
		{
			get => _floatingWindow;
			set
			{
				if (ReferenceEquals(_floatingWindow, value))
					return;

				_floatingWindow = value;

				if (_floatingWindow == null)
					Unlock();
				else
					Lock();
			}
		}

		internal Point? HeaderMousePosition { get; private set; }

		internal DockItemHeaderPresenter HeaderPresenter => TemplateContract.HeaderPresenter;

		public IconBase Icon
		{
			get => (IconBase) GetValue(IconProperty);
			set => SetValue(IconProperty, value);
		}

		internal bool IsActuallyVisible => ActualLayout?.IsVisible(this) == true;

		internal virtual bool IsActualSelected => IsSelected;

		internal bool IsLocked => LockCount > 0;

		public bool IsPreview { get; internal set; }

		internal bool IsRoot => ReferenceEquals(Root, this);

		internal bool IsRootItem => GetParentGroup(DockState) == null;

		public bool IsSelected
		{
			get => (bool) GetValue(IsSelectedProperty);
			set => SetValue(IsSelectedProperty, value.Box());
		}

		internal HashSet<DockItemCollection> ItemCollections { get; } = new();

		internal Dictionary<DockItemState, DockItemGroup> ItemGroups { get; } = new();

		internal XElement ItemLayoutXml
		{
			get
			{
				var itemLayout = CreateItemLayout();

				itemLayout.GetLayout(this);

				return itemLayout.Xml;
			}
		}

		public virtual DockItemKind Kind => DockItemKind.DockItem;

		private int LockCount { get; set; }

		internal DockItemGroup ParentDockGroup => GetParentGroup(DockState);

		internal FloatingDockWindow PreviewFloatingWindow
		{
			get => _previewFloatingWindow;
			set
			{
				if (ReferenceEquals(_previewFloatingWindow, value))
					return;

				_previewFloatingWindow = value;

				if (_previewFloatingWindow == null)
					Unlock();
				else
					Lock();
			}
		}

		internal DockItem PreviewItem
		{
			get
			{
				if (IsPreview)
					return null;

				return _previewItem ??= InitPreviewItem(CreatePreviewPrivate());
			}
		}

		internal DockItem Root => EnumerateAncestors().LastOrDefault() ?? this;

		public bool ShowTitle
		{
			get => (bool) GetValue(ShowTitleProperty);
			set => SetValue(ShowTitleProperty, value.Box());
		}

		internal DockTabViewItem TabViewItem => _tabViewItem ??= CreateDockTabViewItem();

		private DockItemTemplateContract TemplateContract => (DockItemTemplateContract) TemplateContractInternal;

		public string Title
		{
			get => (string) GetValue(TitleProperty);
			set => SetValue(TitleProperty, value);
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			var arrangeOverride = base.ArrangeOverride(arrangeBounds);

			Controller?.OnItemArranged(this);

			return arrangeOverride;
		}

		internal virtual void AttachController(DockControllerBase controller)
		{
		}

		internal void AttachGroup(DockItemState state, DockItemGroup dockGroup)
		{
			DetachGroup(state);

			ItemGroups[state] = dockGroup;

			UpdateActualLayout();
		}

		protected internal bool CanSetDockState(DockItemState state)
		{
			return IsDockStateAllowed(state);
		}

		internal void ClearLayoutIndex(BaseLayout layout)
		{
			SetLayoutIndex(layout, null);
		}

		private DockItemState CoerceDockStateProperty(DockItemState dockItemState)
		{
			return CanSetDockState(dockItemState) ? dockItemState : DockState;
		}

		internal void CopyTargetLayoutIndex(DockItem dockItem)
		{
			var targetLayout = GetTargetLayout();

			if (targetLayout != null)
				dockItem.SetLayoutIndex(targetLayout, GetLayoutIndex(targetLayout));
		}

		internal void CopyTargetLayoutSettings(DockItem target)
		{
			GetTargetLayout()?.CopyLayoutSetting(this, target);
		}

		internal virtual AutoHideTabViewItem CreateAutoHideTabViewItem()
		{
			return new AutoHideTabViewItem(this);
		}

		internal virtual DockTabViewItem CreateDockTabViewItem()
		{
			return new DockTabViewItem(this);
		}

		protected internal virtual DockItemLayout CreateItemLayout()
		{
			return new DockItemLayout(this);
		}

		protected virtual DockItem CreatePreviewItem(DockItemState dockState)
		{
			return new DockItem {DockState = dockState};
		}

		private DockItem CreatePreviewPrivate()
		{
			var dockItem = CreatePreviewItem(DockState);

			dockItem.IsPreview = true;

			return dockItem;
		}

		protected override TemplateContract CreateTemplateContract()
		{
			return new DockItemTemplateContract();
		}

		internal virtual void DetachController(DockControllerBase controller)
		{
		}

		internal void DetachGroup(DockItemState state)
		{
			DetachGroup(GetParentGroup(state));
		}

		internal void DetachGroup(DockItemGroup dockGroup)
		{
			foreach (var kv in ItemGroups.Where(kv => ReferenceEquals(kv.Value, dockGroup)))
			{
				ItemGroups.Remove(kv.Key);

				break;
			}

			UpdateActualLayout();
		}

		internal void DragOutInternal(Point dragOrigin)
		{
			HeaderMousePosition = dragOrigin;
			Controller?.OnDragOutItemInternal(this);
		}

		internal bool IsDragMove { get; set; }

		internal void EnqueueDragMove()
		{
			IsDragMove = true;
		}

		internal IEnumerable<DockItemGroup> EnumerateAncestors()
		{
			var parentContainer = ParentDockGroup;

			while (parentContainer != null)
			{
				yield return parentContainer;

				parentContainer = parentContainer.ParentDockGroup;
			}
		}

		internal IEnumerable<DockItem> EnumerateDescendants(bool self = false)
		{
			if (self)
				yield return this;

			if (this is not DockItemGroup groupItem)
				yield break;

			foreach (var child in groupItem.Items)
			{
				if (child is DockItemGroup childGroup)
					foreach (var childOfChild in childGroup.EnumerateDescendants())
						yield return childOfChild;

				yield return child;
			}
		}

		internal static IEnumerable<DockItemState> EnumerateDockStates()
		{
			yield return DockItemState.Float;
			yield return DockItemState.Dock;
			yield return DockItemState.Document;
			yield return DockItemState.AutoHide;
			yield return DockItemState.Hidden;
		}

		internal IEnumerable<DockItem> EnumerateItems()
		{
			return EnumerateDescendants(true);
		}

		internal IEnumerable<DockItem> EnumerateVisibleDescendants()
		{
			return EnumerateDescendants().Where(w => w.IsActuallyVisible);
		}

		internal int? GetLayoutIndex(BaseLayout layout)
		{
			return _layoutIndexDictionary.TryGetValue(layout, out var index) ? index?.Value : null;
		}

		internal DockItemGroup GetParentGroup(DockItemState dockState)
		{
			ItemGroups.TryGetValue(dockState, out var dockGroup);

			return dockGroup;
		}

		internal BaseLayout GetTargetLayout()
		{
			return ItemGroups.GetValueOrDefault(DockState)?.Layout ?? Controller?.GetTargetLayout(this);
		}

		protected virtual DockItem InitPreviewItem(DockItem previewItem)
		{
			previewItem.Name = Name;

			previewItem.SetBinding(IconProperty, new Binding {Source = this, Path = new PropertyPath(IconProperty)});
			previewItem.SetBinding(TitleProperty, new Binding {Source = this, Path = new PropertyPath(TitleProperty)});

			LayoutSettings.CopySettings(this, previewItem, FullLayout.LayoutProperties);

			return previewItem;
		}

		protected virtual bool IsDockStateAllowed(DockItemState state)
		{
			return true;
		}

		internal void Lock()
		{
			LockCount++;

			if (LockCount == 1)
				IsLockedChanged?.Invoke(this, EventArgs.Empty);
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			var measureOverride = base.MeasureOverride(availableSize);

			Controller?.OnItemMeasured(this);

			return measureOverride;
		}

		protected virtual void OnActualLayoutChanged(BaseLayout oldLayout, BaseLayout newLayout)
		{
		}

		private void OnActualLayoutPropertyChanged(BaseLayout oldLayout, BaseLayout newLayout)
		{
			oldLayout?.Items.Remove(this);
			newLayout?.Items.Add(this);

			OnActualLayoutChanged(oldLayout, newLayout);
		}

		private protected virtual void OnAttachToViewChanged()
		{
			ActualLayout?.OnItemAttachToViewChangedInternal(this);
		}

		internal void OnBeginDragMoveInternal()
		{
			Controller?.OnBeginDragMoveInternal(this);
		}

		private static object OnCoerceTitle(object title)
		{
			return title as string ?? string.Empty;
		}

		protected virtual void OnDockStateChanged(DockItemState oldState, DockItemState newState)
		{
			DockStateChanged?.Invoke(this, new DockItemStateChangedEventArgs(this, oldState, newState));
		}

		internal virtual void OnDockStateChangedInternal(DockItemState oldState, DockItemState newState)
		{
			Controller?.OnItemDockStateChangedInternal(this, oldState, newState);

			OnDockStateChanged(oldState, newState);
		}

		private void OnDockStatePropertyChanged(DockItemState oldState, DockItemState newState)
		{
			OnDockStateChangedInternal(oldState, newState);
			UpdateActualLayout();
		}

		internal void OnDragMoveInternal()
		{
			Controller?.OnDragMoveInternal(this);
		}

		private void OnDragOutCommandExecuted()
		{
			DragOutInternal(DragOutBehavior.OriginMousePosition);
		}

		internal void OnEndDragMoveInternal()
		{
			HeaderMousePosition = null;

			Controller?.OnEndDragMoveInternal(this);
		}

		internal void OnGotKeyboardFocusInternal()
		{
			OnIsKeyboardFocusWithinChangedPrivate();
		}

		protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnIsKeyboardFocusWithinChanged(e);

			OnIsKeyboardFocusWithinChangedPrivate();
		}

		private void OnIsKeyboardFocusWithinChangedPrivate()
		{
			if (DockItemFocusHelper.IsKeyboardFocusWithin(this))
				Select();

			UpdateVisualState(true);
		}

		protected virtual void OnIsSelectedChanged()
		{
			IsSelectedChanged?.Invoke(this, EventArgs.Empty);
		}

		private void OnIsSelectedChangedPrivate()
		{
			if (_suspendIsSelectedChangedHandler > 0)
				return;

			var isSelected = IsSelected;

			if (isSelected)
			{
				TabViewItem.IsSelected = true;
				ActualSelectionScope.SelectedItem = this;
			}

			if (isSelected != IsSelected)
				return;

			OnIsSelectedChanged();

			UpdateVisualState(true);
		}

		private void OnLayoutPropertyChanged(object sender, PropertyValueChangedEventArgs e)
		{
			LayoutPropertyChanged?.Invoke(this, e);
		}

		private protected virtual void OnLayoutPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			SyncSideProperty(e);
		}

		private void OnLayoutUpdated(object sender, EventArgs e)
		{
			ProcessFocusQueue();
		}

		internal void OnLostKeyboardFocusInternal()
		{
			OnIsKeyboardFocusWithinChangedPrivate();
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);

			if (e.Handled)
				return;

			Select();
		}

		private void OnNameChanged(string prevName)
		{
			foreach (var itemCollection in ItemCollections)
				itemCollection.OnItemNameChanged(this, prevName);

			if (_previewItem != null)
				_previewItem.Name = Name;
		}

		protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			Keyboard.Focus(this);
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected virtual void OnPropertyChanging(string propertyName)
		{
			PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			DragOutBehavior.Target = HeaderPresenter;

			if (HeaderPresenter != null)
				HeaderPresenter.OwnItem = this;
		}

		protected override void OnTemplateContractDetaching()
		{
			if (HeaderPresenter != null)
				HeaderPresenter.OwnItem = null;

			DragOutBehavior.Target = null;

			base.OnTemplateContractDetaching();
		}

		private static void OnTitleChanged()
		{
		}

		private void ProcessFocusQueue()
		{
			if (Controller == null || ReferenceEquals(this, Controller.EnqueueFocusItem) == false)
				return;

			if (DockState == DockItemState.Float)
				FloatingWindow?.Activate();
			else if (DockState == DockItemState.AutoHide)
			{
				AutoHideTabViewItem.IsOpen = true;
			}
			else if (ActualLayout is TabLayoutBase)
				TabViewItem.Focus();

			if (Focus())
				Controller.EnqueueFocusItem = null;
		}

		internal void ResetLayout()
		{
			LayoutSettings.ClearSettings(this, FullLayout.LayoutProperties);

			_layoutIndexDictionary = new();
		}

		internal void Select()
		{
			SelectImpl(false);
		}

		internal void SelectAndFocus()
		{
			SelectImpl(true);
		}

		private void SelectImpl(bool focus)
		{
			if (this is DockItemGroup)
				return;

			ActualSelectionScope.SelectedItem = this;

			if (focus)
			{
				if (Controller != null && Focus() == false)
					Controller.EnqueueFocusItem = this;
			}
		}

		internal void SetIsSelected(bool isSelected, bool suspend)
		{
			if (suspend)
				_suspendIsSelectedChangedHandler++;

			IsSelected = isSelected;

			if (suspend)
				_suspendIsSelectedChangedHandler--;
		}

		internal void SetLayoutIndex(BaseLayout layout, int? index)
		{
			if (index.HasValue)
			{
				if (_layoutIndexDictionary.TryGetValue(layout, out var layoutIndex))
				{
					if (layoutIndex != null)
						layoutIndex.Value = index.Value;
					else
						_layoutIndexDictionary.Add(layout, new LayoutOrderIndex(index.Value));
				}
				else
				{
					_layoutIndexDictionary.Add(layout, new LayoutOrderIndex(index.Value));
				}
			}
			else
				_layoutIndexDictionary.Remove(layout);
		}

		internal void SetNewLayoutIndex(BaseLayout layout)
		{
			SetLayoutIndex(layout, layout.GetNewLayoutIndex());
		}

		private void SyncSideProperty(DependencyPropertyChangedEventArgs e)
		{
			if (_syncSideProperty)
				return;

			if (e.Property != AutoHideLayout.DockProperty && e.Property != DockLayout.DockProperty)
				return;

			_syncSideProperty = true;

			var property = e.Property == AutoHideLayout.DockProperty ? DockLayout.DockProperty : AutoHideLayout.DockProperty;

			SetCurrentValue(property, e.NewValue);

			_syncSideProperty = false;
		}

		public override string ToString()
		{
			return $"{base.ToString()}: {Title}";
		}

		internal void Unlock()
		{
			LockCount--;

			if (LockCount == 0)
				IsLockedChanged?.Invoke(this, EventArgs.Empty);
		}

		private void UpdateActualLayout()
		{
			ActualLayout = GetTargetLayout();
		}

		protected override void UpdateVisualState(bool useTransitions)
		{
			base.UpdateVisualState(useTransitions);

			RequestUpdateVisualState?.Invoke(this, EventArgs.Empty);
		}

		bool IActiveStateControl.IsActive => DockItemFocusHelper.IsKeyboardFocusWithin(this);

		void ILayoutPropertyChangeListener.OnLayoutPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			OnLayoutPropertyChanged(e);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public event PropertyChangingEventHandler PropertyChanging;

		bool ISelectionStateControl.IsSelected => IsActualSelected;

		internal sealed class DockItemDebugView
		{
			public DockItemDebugView(DockItem dockItem)
			{
				var dockItemLayout = dockItem.CreateItemLayout();

				LayoutSettings.CopySettings(dockItem, dockItemLayout, FullLayout.LayoutProperties);

				Xml = dockItemLayout.Xml;
			}

			public XElement Xml { get; }
		}

		private sealed class LayoutOrderIndex
		{
			public LayoutOrderIndex(int value)
			{
				Value = value;
			}

			public int Value { get; set; }
		}
	}
}