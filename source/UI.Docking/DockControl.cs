// <copyright file="DockControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Xml.Linq;
using Zaaml.Core.Extensions;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Windows;

namespace Zaaml.UI.Controls.Docking
{
	[ContentProperty(nameof(Items))]
	[TemplateContractType(typeof(DockControlTemplateContract))]
	public class DockControl : TemplateContractControl
	{
		public static readonly DependencyProperty LayoutProperty = DPM.Register<DockControlLayout, DockControl>
			(nameof(Layout), d => d.OnLayoutPropertyChangedPrivate);

		public static readonly DependencyProperty SelectedItemProperty = DPM.Register<DockItem, DockControl>
			(nameof(SelectedItem), d => d.OnSelectedItemChangedPrivate);

		public static readonly DependencyProperty IsPreviewEnabledProperty = DPM.Register<bool, DockControl>
			(nameof(IsPreviewEnabled), true, d => d.OnIsPreviewEnabledChanged);

		private static readonly DependencyPropertyKey ItemsPropertyKey = DPM.RegisterReadOnly<DockItemCollection, DockControl>
			("ItemsPrivate");

		public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;

		private DockController _controller;
		private DropGuide _currentDropGuide;
		private PreviewDockController _previewController;

		public event EventHandler<DockItemStateChangedEventArgs> ItemDockStateChanged;

		static DockControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<DockControl>();
		}

		public DockControl()
		{
			this.OverrideStyleKey<DockControl>();
		}

		internal bool AllowSplitDocumentsInAllDirections => true;

		internal DockController Controller
		{
			get => _controller;
			set
			{
				if (ReferenceEquals(_controller, value))
					return;

				if (_controller != null)
				{
					_controller.DockControl = null;
					_controller.Items.Clear();
				}

				_controller = value;

				if (_controller != null)
				{
					_controller.DockControl = this;
					_controller.Items.AddRange(Items);
				}

				SelectedItem = _controller?.SelectionScope.SelectedItem;
			}
		}

		private DockControlView ControlView => TemplateContract.ControlView;

		internal DropGuide CurrentDropGuide
		{
			get => _currentDropGuide;
			private set
			{
				if (ReferenceEquals(_currentDropGuide, value))
					return;

				if (_currentDropGuide != null)
					_currentDropGuide.IsActive = false;

				_currentDropGuide = value;

				if (_currentDropGuide != null)
					_currentDropGuide.IsActive = true;

				if (IsPreviewEnabled && PreviewControlView != null)
					PreviewControlView.CurrentDropGuide = value;
			}
		}

		internal DockItem DragMoveItem { get; private set; }

		private GlobalDropCompass GlobalDropCompass => TemplateContract.GlobalCompass;

		public bool IsPreviewEnabled
		{
			get => (bool)GetValue(IsPreviewEnabledProperty);
			set => SetValue(IsPreviewEnabledProperty, value.Box());
		}

		public DockItemCollection Items => this.GetValueOrCreate(ItemsPropertyKey, CreateDockItemCollection);

		public DockControlLayout Layout
		{
			get => (DockControlLayout)GetValue(LayoutProperty);
			set => SetValue(LayoutProperty, value);
		}

		private bool LayoutDirty { get; set; }

		private LocalDropCompass LocalDropCompass => TemplateContract.LocalCompass;

		internal PreviewDockController PreviewController
		{
			get => _previewController;
			set
			{
				if (ReferenceEquals(_previewController, value))
					return;

				if (_previewController != null)
				{
					_previewController.DockControl = null;
					_previewController.Items.Clear();
				}

				_previewController = value;

				if (_previewController == null)
					return;

				_previewController.DockControl = this;
				_previewController.IsEnabled = IsPreviewEnabled;
				_previewController.Items.AddRange(Items.Select(w => w.PreviewItem));
			}
		}

		private PreviewDockControlView PreviewControlView => TemplateContract.PreviewControlView;

		public DockItem SelectedItem
		{
			get => (DockItem)GetValue(SelectedItemProperty);
			set => SetValue(SelectedItemProperty, value);
		}

		private DockControlTemplateContract TemplateContract => (DockControlTemplateContract)TemplateContractCore;

		internal DockControlTemplateContract TemplateContractInternal => TemplateContract;

		private DockItemCollection CreateDockItemCollection()
		{
			return new DockItemCollection(OnItemAdded, OnItemRemoved);
		}

		internal void EnsureFloatPositionInternal(DockItem item)
		{
			var defaultLeft = item.GetValueSource(FloatLayout.LeftProperty) == PropertyValueSource.Default;
			var defaultTop = item.GetValueSource(FloatLayout.TopProperty) == PropertyValueSource.Default;

			if (!defaultLeft || !defaultTop)
				return;

			var hostWindow = Window.GetWindow(this);

			if (hostWindow is not { IsVisible: true })
				return;

			var dockItemSize = FloatLayout.GetSize(item);
			var windowRect = new Rect(new Point(hostWindow.Left, hostWindow.Top), new Size(hostWindow.ActualWidth, hostWindow.ActualHeight));

			if (hostWindow.WindowState == WindowState.Maximized)
				windowRect.Location = Screen.FromElement(hostWindow).WorkingArea.Location;

			var alignBox = RectUtils.CalcAlignBox(windowRect, new Rect(dockItemSize), HorizontalAlignment.Center, VerticalAlignment.Center);

			FloatLayout.SetPosition(item, alignBox.Location);
		}

		private DockItem FindLocalCompassItem(DockItem dragItem)
		{
			var elementsUnderCursor = MouseInternal.HitTest().ToList();

			if (LocalDropCompass?.PlacementTarget != null && elementsUnderCursor.Any(e => e.IsVisualDescendantOf(LocalDropCompass)))
				return Controller?.GetItemIfDocumentLayout(LocalDropCompass.PlacementTarget);

			return Controller?.GetItemIfDocumentLayout(elementsUnderCursor.FirstOrDefault(uie => HitTestFilter(uie, dragItem)));
		}

		internal DockItem GetPreviewItem(DockItem dockItem)
		{
			if (ReferenceEquals(dockItem, Controller.DocumentGroup))
				return PreviewController.DocumentGroup;

			return PreviewController.GetDockItem(dockItem.Name);
		}

		private bool HitTestFilter(UIElement uie, DockItem dragItem)
		{
			if (ReferenceEquals(uie, Controller.DocumentLayout.View))
				return true;

			var hitDockItem = uie as DockItem;

			if (hitDockItem?.IsPreview == true)
				return false;

			if (hitDockItem == null)
			{
				var floatingDockWindow = uie as FloatingDockWindow;

				hitDockItem = floatingDockWindow?.DockItem;

				if (hitDockItem == null)
					return false;
			}

			if (ReferenceEquals(hitDockItem, dragItem) || hitDockItem.IsHidden() || hitDockItem.IsHitTestVisible == false || DragMoveItem.IsVisualAncestorOf(hitDockItem) ||
			    hitDockItem.IsContainer())
				return false;

			if (DragMoveItem is DocumentDockItem)
			{
				if (hitDockItem.IsDocument() == false)
					return false;
			}
			else if (hitDockItem is DocumentDockItem && hitDockItem.IsDocument() == false)
				return false;

			return true;
		}

		private void InvalidateLayoutState()
		{
			LayoutDirty = true;

			InvalidateMeasure();
		}

		public void LoadLayout(TextReader textReader)
		{
			Layout = DockControlLayout.FromXElement(XElement.Parse(textReader.ReadToEnd()));
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			UpdateDockLayout();

			var result = base.MeasureOverride(availableSize);

			return result;
		}

		internal void OnBeginDragMove(DockItem item)
		{
			DragMoveItem = item;

			if (IsPreviewEnabled)
				PreviewControlView?.EnterPreviewState(Controller.GetActualLayout());
		}

		internal void OnEndDragMove(DockItem item)
		{
			if (CurrentDropGuide != null)
			{
				Controller.DropItem(DragMoveItem, Controller.GetItemIfDocumentLayout(LocalDropCompass.PlacementTarget), CurrentDropGuide.Action);

				WindowBase.GetWindowInternal(this)?.Activate();
			}

			DragMoveItem = null;
			LocalDropCompass.PlacementTarget = null;
			GlobalDropCompass.PlacementTarget = null;
			CurrentDropGuide = null;

			if (IsPreviewEnabled)
				PreviewControlView?.ExitPreviewState();
		}

		private void OnIsPreviewEnabledChanged()
		{
			if (PreviewController != null)
				PreviewController.IsEnabled = IsPreviewEnabled;
		}

		private void OnItemAdded(DockItem item)
		{
			item.AttachDockControlInternal(this);

			Controller?.Items.Add(item);
			PreviewController?.Items.Add(item.PreviewItem);
		}

		protected virtual void OnItemDockStateChanged(DockItem item, DockItemState oldState, DockItemState newState)
		{
			ItemDockStateChanged?.Invoke(this, new DockItemStateChangedEventArgs(item, oldState, newState));
		}

		internal void OnItemDockStateChangedInternal(DockItem dockItem, DockItemState oldState, DockItemState newState)
		{
			OnItemDockStateChanged(dockItem, oldState, newState);
		}

		internal void OnItemDragMove(DockItem item)
		{
			if (DragMoveItem.IsFloat() == false)
				return;

			ProcessDropCompassDragMove();
			ProcessDropGuideDragMove();
		}

		private void OnItemRemoved(DockItem item)
		{
			Controller?.Items.Remove(item);
			PreviewController?.Items.Remove(item.PreviewItem);

			item.DetachDockControlInternal(this);
		}

		private void OnLayoutChanged(object sender, EventArgs e)
		{
			LayoutDirty = true;

			InvalidateLayoutState();
		}

		private void OnLayoutPropertyChangedPrivate(DockControlLayout oldValue, DockControlLayout newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			if (oldValue != null)
			{
				if (ReferenceEquals(oldValue.DockControl, this) == false)
					throw new InvalidOperationException();

				oldValue.DockControl = null;

				oldValue.LayoutChanged -= OnLayoutChanged;
			}

			if (newValue != null)
			{
				if (ReferenceEquals(newValue.DockControl, null) == false)
					throw new InvalidOperationException("DockControlLayout is already attached to another DockControl.");

				newValue.DockControl = this;

				newValue.LayoutChanged += OnLayoutChanged;
			}

			InvalidateLayoutState();
		}

		protected virtual void OnSelectedItemChanged(DockItem oldItem, DockItem newItem)
		{
		}

		internal virtual void OnSelectedItemChangedInternal(DockItem oldItem, DockItem newItem)
		{
			OnSelectedItemChanged(oldItem, newItem);
		}

		private void OnSelectedItemChangedPrivate(DockItem oldItem, DockItem newItem)
		{
			OnSelectedItemChangedInternal(oldItem, newItem);
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			Controller = ControlView.Controller;
			PreviewController = PreviewControlView.Controller;
			PreviewControlView.IsHitTestVisible = false;

			UpdateDockLayout();
		}

		protected override void OnTemplateContractDetaching()
		{
			Controller = null;
			PreviewController = null;

			base.OnTemplateContractDetaching();
		}

		private void GetCompassTargets(
			out FrameworkElement localCompassTarget, 
			out DropGuideAction localCompassAction, 
			out FrameworkElement globalCompassTarget,
			out DropGuideAction globalCompassAction)
		{
			var compassTarget = FindLocalCompassItem(DragMoveItem);
			var withinDockControl = MouseInternal.HitTest().Contains(this);

			globalCompassTarget = withinDockControl ? this : null;
			globalCompassAction = DropGuideAction.DockAll | DropGuideAction.AutoHideAll;

			localCompassTarget = compassTarget;
			localCompassAction = DropGuideAction.None;

			if (compassTarget == null)
				return;

			if (compassTarget.DockState == DockItemState.Float) 
				globalCompassTarget = null;

			if (ReferenceEquals(compassTarget, Controller.DocumentGroup))
			{
				localCompassTarget = Controller.DocumentLayout.View;
				localCompassAction = DropGuideAction.TabAll;
				globalCompassTarget = DragMoveItem is DocumentDockItem ? null : this;
				globalCompassAction = DropGuideAction.DockAll | DropGuideAction.AutoHideAll;

				return;
			}

			if (DragMoveItem is DocumentDockItem)
			{
				localCompassAction = DropGuideAction.TabAll;
				globalCompassTarget = null;
			}
			else
			{
				localCompassAction = DropGuideAction.SplitAll | DropGuideAction.TabAll;
				localCompassTarget = compassTarget;
			}

			if (compassTarget.IsDocument())
			{
				var documentContainer = compassTarget.ParentDockGroup as DocumentDockItemGroup;

				if (AllowSplitDocumentsInAllDirections == false && documentContainer?.ParentDockGroup is SplitDockItemGroup splitDocumentContainer && splitDocumentContainer.Layout.Items.Count > 1)
					localCompassAction |= splitDocumentContainer.Orientation.IsHorizontal()
						? DropGuideAction.SplitDocumentHorizontal
						: DropGuideAction.SplitDocumentVertical;
				else
					localCompassAction |= DropGuideAction.SplitDocumentAll;
			}
		}

		private void ProcessDropCompassDragMove()
		{
			GetCompassTargets(out var localCompassTarget, out var localCompassAction, out var globalCompassTarget, out var globalCompassAction);

			LocalDropCompass.AllowedActions = localCompassAction;
			LocalDropCompass.PlacementTarget = localCompassTarget;

			GlobalDropCompass.AllowedActions = globalCompassAction;
			GlobalDropCompass.PlacementTarget = globalCompassTarget;
		}

		private void ProcessDropGuideDragMove()
		{
			CurrentDropGuide = MouseInternal.HitTest().OfType<DropGuide>().FirstOrDefault(g => g.IsAllowed);
		}

		public void SaveLayout(TextWriter textWriter)
		{
			Controller.GetActualLayout().Save(textWriter);
		}

		internal void SyncPreviewLayout()
		{
			PreviewController.ApplyLayout(Controller.GetActualLayout());
		}

		private void UpdateDockLayout()
		{
			if (IsTemplateAttached == false || LayoutDirty == false)
				return;

			try
			{
				var layout = Layout;

				if (layout == null)
					return;

				Controller.ApplyLayout(layout);

				if (PreviewController.IsEnabled)
					PreviewController.ApplyLayout(layout);
			}
			finally
			{
				LayoutDirty = false;
			}
		}
	}
}