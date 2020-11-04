// <copyright file="DockControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.PropertyCore;
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
			get => (bool) GetValue(IsPreviewEnabledProperty);
			set => SetValue(IsPreviewEnabledProperty, value);
		}

		public DockItemCollection Items => this.GetValueOrCreate(ItemsPropertyKey, CreateDockItemCollection);

		public DockControlLayout Layout
		{
			get => (DockControlLayout) GetValue(LayoutProperty);
			set => SetValue(LayoutProperty, value);
		}

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
			get => (DockItem) GetValue(SelectedItemProperty);
			set => SetValue(SelectedItemProperty, value);
		}

		private DockControlTemplateContract TemplateContract => (DockControlTemplateContract) TemplateContractInternal;

		private DockItemCollection CreateDockItemCollection()
		{
			return new DockItemCollection(OnItemAdded, OnItemRemoved);
		}

		private DockItem FindLocalCompassTarget(DockItem dragItem)
		{
			var screenPosition = MouseInternal.ScreenPosition;
			var elementsUnderCursor = HitTestUtils.ScreenHitTest(screenPosition).ToList();

			if (LocalDropCompass?.PlacementTarget != null && elementsUnderCursor.Any(e => e.IsVisualDescendantOf(LocalDropCompass)))
				return Controller?.GetItemIfDocumentLayout(LocalDropCompass.PlacementTarget);

			return Controller?.GetItemIfDocumentLayout(elementsUnderCursor.FirstOrDefault(uie => HitTestFilter(uie, dragItem)));
		}

		internal DockItem GetPreviewItem(DockItem dockItem)
		{
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

		protected override Size MeasureOverride(Size availableSize)
		{
			UpdateDockLayout();

			Controller?.BeforeMeasure();

			var result = base.MeasureOverride(availableSize);

			Controller?.AfterMeasure();

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
			item.DockControl = this;

			Controller?.Items.Add(item);
			PreviewController?.Items.Add(item.PreviewItem);
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

			item.DockControl = null;
		}

		private void OnLayoutPropertyChangedPrivate(DockControlLayout oldValue, DockControlLayout newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			if (oldValue != null)
				oldValue.LayoutChanged -= OnLayoutChanged;
			
			if (newValue != null)
				newValue.LayoutChanged += OnLayoutChanged;

			LayoutDirty = true;
		}

		private bool LayoutDirty { get; set; }

		private void OnLayoutChanged(object sender, EventArgs e)
		{
			LayoutDirty = true;
			
			InvalidateMeasure();
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

		private void UpdateDockLayout()
		{
			if (IsTemplateAttached == false || LayoutDirty == false)
				return;

			try
			{
				var layout = Layout;

				if (layout == null)
					return;

				Controller.ApplyLayout(layout, false);

				if (PreviewController.IsEnabled)
					PreviewController.ApplyLayout(layout, false);
			}
			finally
			{
				LayoutDirty = false;
			}
		}

		protected override void OnTemplateContractDetaching()
		{
			Controller = null;
			PreviewController = null;

			base.OnTemplateContractDetaching();
		}

		private void ProcessDropCompassDragMove()
		{
			var compassTarget = FindLocalCompassTarget(DragMoveItem);

			if (ReferenceEquals(compassTarget, Controller.GetItemIfDocumentLayout(LocalDropCompass.PlacementTarget)))
				return;

			if (compassTarget == null)
			{
				LocalDropCompass.PlacementTarget = null;
				GlobalDropCompass.PlacementTarget = HitTestUtils.ScreenHitTest(MouseInternal.ScreenPosition).Contains(this) ? this : null;

				return;
			}

			if (ReferenceEquals(compassTarget, Controller.DocumentGroup))
			{
				LocalDropCompass.PlacementTarget = Controller.DocumentLayout.View;
				LocalDropCompass.AllowedActions = DropGuideAction.TabAll;
				GlobalDropCompass.PlacementTarget = DragMoveItem is DocumentDockItem ? null : this;
				GlobalDropCompass.AllowedActions = DropGuideAction.DockAll | DropGuideAction.AutoHideAll;

				return;
			}

			if (DragMoveItem is DocumentDockItem)
			{
				LocalDropCompass.AllowedActions = DropGuideAction.TabAll;

				LocalDropCompass.PlacementTarget = compassTarget;
				GlobalDropCompass.PlacementTarget = null;
			}
			else
			{
				LocalDropCompass.AllowedActions = DropGuideAction.SplitAll | DropGuideAction.TabAll;

				LocalDropCompass.PlacementTarget = compassTarget;
				GlobalDropCompass.PlacementTarget = this;
			}

			if (compassTarget.IsDocument())
			{
				var documentContainer = compassTarget.ParentDockGroup as DocumentDockItemGroup;
				var splitDocumentContainer = documentContainer?.ParentDockGroup as SplitDockItemGroup;

				if (AllowSplitDocumentsInAllDirections == false && splitDocumentContainer != null && splitDocumentContainer.Layout.Items.Count > 1)
					LocalDropCompass.AllowedActions |= splitDocumentContainer.Orientation.IsHorizontal()
						? DropGuideAction.SplitDocumentHorizontal
						: DropGuideAction.SplitDocumentVertical;
				else
					LocalDropCompass.AllowedActions |= DropGuideAction.SplitDocumentAll;
			}
		}

		private void ProcessDropGuideDragMove()
		{
			CurrentDropGuide = HitTestUtils.ScreenHitTest(MouseInternal.ScreenPosition).OfType<DropGuide>().FirstOrDefault(g => g.IsAllowed);
		}

		internal void SyncPreviewLayout()
		{
			PreviewController.ApplyLayout(Controller.GetActualLayout(), true);
		}
	}
}