// <copyright file="DockItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Linq;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.CommandCore;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Interfaces;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;

namespace Zaaml.UI.Controls.Docking
{
  [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
  [DebuggerTypeProxy(typeof(DockItemDebugView))]
  public class DockItem : TemplateContractContentControl, INotifyPropertyChanging, INotifyPropertyChanged, ISelectionStateControl, IActiveStateControl
  {
    #region Static Fields and Constants

    private const DockItemState AllStates = DockItemState.AutoHide | DockItemState.Hidden |
                                            DockItemState.Float | DockItemState.Dock |
                                            DockItemState.Document;

    public static readonly DependencyProperty AllowedDockStatesProperty = DPM.Register<DockItemState, DockItem>
      ("AllowedDockStates", AllStates);

    private static readonly DockItemSelectionScope DummySelectionScope = new DockItemSelectionScope();
    private static readonly DockItemSelectionScope DummyPreviewSelectionScope = new DockItemSelectionScope();

    private static readonly DependencyProperty NameInternalProperty = DPM.Register<string, DockItem>
      ("NameInternal", d => d.OnNameChanged);

    public static readonly DependencyProperty TitleProperty = DPM.Register<string, DockItem>
      ("Title", string.Empty, d => OnTitleChanged, d => OnCoerceTitle);

    public static readonly DependencyProperty IconProperty = DPM.Register<IconBase, DockItem>
      ("Icon", i => i.LogicalChildMentor.OnLogicalChildPropertyChanged);

    public static readonly DependencyProperty IsSelectedProperty = DPM.Register<bool, DockItem>
      ("IsSelected", d => d.OnIsSelectedChangedPrivate);

    public static readonly DependencyProperty ShowTitleProperty = DPM.Register<bool, DockItem>
      ("ShowTitle");

    #endregion

    #region Fields

    private BaseLayout _actualLayout;
    private AutoHideTabViewItem _autoHideTabViewItem;
    private DockControllerBase _controller;
    private DockItemState _dockState;
    private FloatingDockWindow _floatingWindow;
    private bool _isArranged;
    private bool _isItemLayoutValid;
    private bool _isMeasured;
    private FloatingDockWindow _previewFloatingWindow;
    private DockItem _previewItem;
    private int _suspendIsSelectedChangedHandler;
    private DockTabViewItem _tabViewItem;
    protected internal event EventHandler ActualItemChanged;
    public event EventHandler<DockItemStateChangedEventArgs> DockStateChanged;
    public event EventHandler<DockItemStateChangingEventArgs> DockStateChanging;
    internal event EventHandler LayoutInvalidated;
    public event EventHandler IsSelectedChanged;
    internal event EventHandler<PropertyValueChangedEventArgs> LayoutPropertyChanged;
    internal event EventHandler RequestUpdateVisualState;
    internal event EventHandler IsLockedChanged;

    #endregion

    #region Ctors

    static DockItem()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<DockItem>();
    }

    public DockItem() : this(DockItemState.Float)
    {
    }

    protected DockItem(DockItemState dockState)
    {
      this.OverrideStyleKey<DockItem>();

      _dockState = dockState;

      foreach (var layoutProperty in FullLayout.LayoutProperties)
        this.AddValueChanged(layoutProperty, OnLayoutPropertyChanged);

      DragOutBehavior = new DragOutBehavior
      {
        DragOutCommand = new RelayCommand(OnDragOutCommandExecuted)
      };

      this.BindProperties(NameInternalProperty, this, NameProperty, BindingMode.TwoWay);

      LayoutUpdated += OnLayoutUpdated;
    }

    #endregion

    #region Properties

    protected internal virtual DockItem ActualItem => this;

    public BaseLayout ActualLayout
    {
      get => _actualLayout;
      internal set
      {
        if (ReferenceEquals(_actualLayout, value))
          return;

        if (_actualLayout != null)
          DetachLayoutPrivate(_actualLayout);

        _actualLayout = value;

        if (_actualLayout != null)
          AttachLayoutPrivate(_actualLayout);

        OnPropertyChanged("ActualLayout");
        OnPropertyChanged("ActualLayoutKind");
      }
    }

    public LayoutKind ActualLayoutKind => ActualLayout?.LayoutKind ?? LayoutKind.Hidden;

    internal DockItemSelectionScope ActualSelectionScope
    {
      get
      {
        if (Controller != null)
          return Controller.SelectionScope;

        return IsPreview ? DummyPreviewSelectionScope : DummySelectionScope;
      }
    }

    public DockItemState AllowedDockStates
    {
      get => (DockItemState) GetValue(AllowedDockStatesProperty);
      set => SetValue(AllowedDockStatesProperty, value);
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

        InvalidateItemArrange();
      }
    }

    internal virtual string DebuggerDisplay
    {
      get
      {
        var dockItemLayout = CreateItemLayout();

        LayoutSettings.CopySettings(ActualItem ?? this, dockItemLayout, FullLayout.LayoutProperties);

        return dockItemLayout.Xml.ToString();
      }
    }

    internal virtual DockControl DockControl { get; set; }

    public DockItemState DockState
    {
      get => _dockState;
      set
      {
        if (_dockState == value)
          return;

        OnDockStateChangingInternal(value);

        var oldState = _dockState;
        _dockState = value;

        ScreenBox = this.GetScreenBox();

        OnDockStateChangedInternal(oldState);
      }
    }

    private DragOutBehavior DragOutBehavior { get; }

    internal bool EnqueueSyncDragPosition { get; set; }

		internal Rect ScreenBox { get; private set; }

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

    internal bool IsArranged
    {
      get => _isArranged;
      private set
      {
        if (_isArranged == value)
          return;

        _isArranged = value;

        if (_isArranged)
          Controller?.OnItemArranged(this);
      }
    }

    internal bool IsItemLayoutValid
    {
      get => _isItemLayoutValid;
      private set
      {
        if (_isItemLayoutValid == value)
          return;

        _isItemLayoutValid = value;

        if (_isItemLayoutValid)
          return;

        IsMeasured = false;
        IsArranged = false;

        OnItemArrangeInvalidated();
      }
    }

    internal bool IsLayoutComplete => IsMeasured && IsArranged;

    internal bool IsLocked => LockCount > 0;

    internal bool IsMeasured
    {
      get => _isMeasured;
      private set
      {
        if (_isMeasured == value)
          return;

        _isMeasured = value;

        if (_isMeasured)
          Controller?.OnItemMeasured(this);
      }
    }

    public bool IsPreview { get; internal set; }

    internal bool IsRoot => ReferenceEquals(Root, this);

    public bool IsSelected
    {
      get => (bool) GetValue(IsSelectedProperty);
      set => SetValue(IsSelectedProperty, value);
    }

    internal HashSet<DockItemCollection> ItemCollections { get; } = new HashSet<DockItemCollection>();

    internal Dictionary<DockItemState, DockItemGroup> ItemGroups { get; } = new Dictionary<DockItemState, DockItemGroup>();

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

        return _previewItem ?? (_previewItem = InitPreviewItem(CreatePreviewPrivate()));
      }
    }

    internal DockItem Root => EnumerateAncestors().LastOrDefault() ?? this;

    public bool ShowTitle
    {
      get => (bool) GetValue(ShowTitleProperty);
      set => SetValue(ShowTitleProperty, value);
    }

    internal DockTabViewItem TabViewItem => _tabViewItem ?? (_tabViewItem = CreateDockTabViewItem());

    private DockItemTemplateContract TemplateContract => (DockItemTemplateContract) TemplateContractInternal;

    public string Title
    {
      get => (string) GetValue(TitleProperty);
      set => SetValue(TitleProperty, value);
    }

    #endregion

    #region  Methods

    internal void Lock()
    {
      LockCount++;

      if (LockCount == 1)
        IsLockedChanged?.Invoke(this, EventArgs.Empty);
    }

    internal void ApplyLayout()
    {
      if (IsItemLayoutValid)
        return;

      ApplyLayoutOverride();
    }

    protected virtual void ApplyLayoutOverride()
    {
      ActualLayout = GetTargetLayout(true);
    }

    protected override Size ArrangeOverride(Size arrangeBounds)
    {
      var arrangeOverride = base.ArrangeOverride(arrangeBounds);

      if (IsItemLayoutValid)
        IsArranged = true;

      return arrangeOverride;
    }

    internal virtual void AttachController(DockControllerBase controller)
    {
    }

    internal void AttachGroup(DockItemState state, DockItemGroup dockGroup)
    {
      DetachGroup(GetParentGroup(state));

      ItemGroups[state] = dockGroup;
      InvalidateItemArrange();
    }

    private void AttachLayoutPrivate(BaseLayout layout)
    {
      layout.Items.Add(this);
    }

    protected internal bool CanSetDockState(DockItemState state)
    {
      return IsDockStateAllowed(state) && state != DockState;
    }

    internal void CopyTargetLayoutSettings(DockItem target)
    {
      GetTargetLayout(false)?.CopyLayoutSetting(this, target);
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
      return new DockItem(dockState);
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

    internal void DetachGroup(DockItemGroup dockGroup)
    {
      foreach (var kv in ItemGroups.Where(kv => ReferenceEquals(kv.Value, dockGroup)))
      {
        ItemGroups.Remove(kv.Key);
        InvalidateItemArrange();

        break;
      }
    }

    private void DetachLayoutPrivate(BaseLayout layout)
    {
      layout.Items.Remove(this);
    }

    internal void DragOutInternal(Point dragOrigin)
    {
      HeaderMousePosition = dragOrigin;
      Controller?.OnDragOutItemInternal(this);
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

      var groupItem = this as DockItemGroup;

      if (groupItem == null)
        yield break;

      foreach (var child in groupItem.Items)
      {
        var childGroup = child as DockItemGroup;

        if (childGroup != null)
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

    internal DockItemGroup GetParentGroup(DockItemState winDockState)
    {
      DockItemGroup dockGroup;

      ItemGroups.TryGetValue(winDockState, out dockGroup);

      return dockGroup;
    }

    internal BaseLayout GetTargetLayout(bool arrange)
    {
      return GetTargetLayout(this, arrange);
    }

    internal virtual BaseLayout GetTargetLayout(DockItem item, bool arrange)
    {
      return ItemGroups.GetValueOrDefault(DockState)?.GetItemTargetLayout(item, arrange) ?? Controller?.GetTargetLayout(this, arrange);
    }

    protected virtual DockItem InitPreviewItem(DockItem previewItem)
    {
      previewItem.Name = Name;

      previewItem.SetBinding(IconProperty, new Binding {Source = this, Path = new PropertyPath(IconProperty)});
      previewItem.SetBinding(TitleProperty, new Binding {Source = this, Path = new PropertyPath(TitleProperty)});

      LayoutSettings.CopySettings(this, previewItem, FullLayout.LayoutProperties);

      return previewItem;
    }

    internal void InvalidateItemArrange()
    {
      IsItemLayoutValid = false;
    }

    protected virtual bool IsDockStateAllowed(DockItemState state)
    {
      return true;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      var measureOverride = base.MeasureOverride(availableSize);

      if (IsItemLayoutValid)
        IsMeasured = true;

      return measureOverride;
    }

    protected virtual void OnActualItemChanged()
    {
      ActualItemChanged?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnActualLayoutChanged(BaseLayout oldLayout)
    {
    }

    internal void OnBeginDragMoveInternal()
    {
      Controller?.OnBeginDragMoveInternal(this);
    }

    private static object OnCoerceTitle(object title)
    {
      return title as string ?? string.Empty;
    }

    protected virtual void OnDockStateChanged(DockItemState oldState)
    {
      DockStateChanged?.Invoke(this, new DockItemStateChangedEventArgs(oldState));

      OnPropertyChanged(nameof(DockState));

      InvalidateItemArrange();
    }

    internal virtual void OnDockStateChangedInternal(DockItemState oldState)
    {
      OnDockStateChanged(oldState);

      Controller?.OnDockStateChanged(this, oldState);
    }

    protected virtual void OnDockStateChanging(DockItemState newState)
    {
      if (IsDockStateAllowed(newState) == false)
        throw new Exception($"{nameof(DockState)} '{newState}' is not allowed for DockItem type '{GetType().Name}'");

      DockStateChanging?.Invoke(this, new DockItemStateChangingEventArgs(newState));

      OnPropertyChanging(nameof(DockState));
    }

    internal virtual void OnDockStateChangingInternal(DockItemState newState)
    {
      Controller?.OnDockStateChanging(this, newState);

      OnDockStateChanging(newState);
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

    protected virtual void OnItemArrangeInvalidated()
    {
      Controller?.InvalidateItemArrange();
    }

    protected virtual void OnLayoutInvalidated()
    {
      LayoutInvalidated?.Invoke(this, EventArgs.Empty);
    }

    private void OnLayoutPropertyChanged(object sender, PropertyValueChangedEventArgs e)
    {
      LayoutPropertyChanged?.Invoke(this, e);
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

      //e.Handled = true;
    }

    private void OnNameChanged(string prevName)
    {
      foreach (var itemCollection in ItemCollections)
        itemCollection.OnItemNameChanged(this, prevName);

      if (_previewItem != null)
        _previewItem.Name = Name;
    }

#if !SILVERLIGHT
    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      Keyboard.Focus(this);
    }
#endif

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

    internal void PostApplyLayout()
    {
      if (IsItemLayoutValid)
        return;

      PostApplyLayoutOverride();

      IsItemLayoutValid = true;
    }

    protected virtual void PostApplyLayoutOverride()
    {
    }

    internal virtual void PreApplyLayout()
    {
      if (IsItemLayoutValid)
        return;

      PreApplyLayoutOverride();
    }

    protected virtual void PreApplyLayoutOverride()
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

    internal void PushCurrentLayout(DockItem targetItem)
    {
      var actualSource = ActualItem ?? this;
      var actualTarget = targetItem.ActualItem ?? targetItem;
      var targetLayout = actualSource.GetTargetLayout(false);

      if (targetLayout == null)
        return;

      targetLayout.CopyLayoutSetting(actualSource, actualTarget);
      targetLayout.CopyLayoutSetting(actualSource, DockItemLayoutMentorService.FromItem(actualTarget).Layout);
    }

    internal void Unlock()
    {
      LockCount--;

      if (LockCount == 0)
        IsLockedChanged?.Invoke(this, EventArgs.Empty);
    }

    internal virtual void RemoveFromLayout()
    {
      ActualLayout = null;
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
        if (Controller != null && (IsItemLayoutValid == false || Focus() == false))
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

    public override string ToString()
    {
      return $"{base.ToString()}: {Title}";
    }

    protected override void UpdateVisualState(bool useTransitions)
    {
      base.UpdateVisualState(useTransitions);

      RequestUpdateVisualState?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region Interface Implementations

    #region IActiveStateControl

    bool IActiveStateControl.IsActive => DockItemFocusHelper.IsKeyboardFocusWithin(this);

    #endregion

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region INotifyPropertyChanging

    public event PropertyChangingEventHandler PropertyChanging;

    #endregion

    #region ISelectionStateControl

    bool ISelectionStateControl.IsSelected => IsActualSelected;

    #endregion

    #endregion

    #region  Nested Types

    internal sealed class DockItemDebugView
    {
      #region Ctors

      public DockItemDebugView(DockItem dockItem)
      {
        var dockItemLayout = dockItem.CreateItemLayout();

        LayoutSettings.CopySettings(dockItem.ActualItem ?? dockItem, dockItemLayout, FullLayout.LayoutProperties);

        Xml = dockItemLayout.Xml;
      }

      #endregion

      #region Properties

      public XElement Xml { get; }

      #endregion
    }

    #endregion
  }
}