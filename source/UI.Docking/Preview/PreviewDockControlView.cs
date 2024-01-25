// <copyright file="PreviewDockControlView.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.Docking
{
	public sealed class PreviewDockControlView : DockControlViewBase
	{
		private readonly Dictionary<string, Preview> _previewGeometryCache = new(StringComparer.OrdinalIgnoreCase);
		private readonly Queue<Preview> _previewQueue = new();
		private Preview _actualPreview;
		private DropGuide _currentDropGuide;
		private bool _isInPreviewState;
		private bool _isPresenterVisible;
		private DockControlLayout _layout;
		private Preview _renderPreview;

		public PreviewDockControlView()
		{
			Controller = new PreviewDockController(this);
		}

		internal Preview ActualPreview
		{
			get => _actualPreview;
			set
			{
				if (ReferenceEquals(_actualPreview, value))
					return;

				_actualPreview = value;

				if (_actualPreview != null)
					InvalidateMeasure();
			}
		}

		internal PreviewDockController Controller { get; }

		internal override DockControllerBase ControllerCore => Controller;

		internal DropGuide CurrentDropGuide
		{
			get => _currentDropGuide;
			set
			{
				if (ReferenceEquals(_currentDropGuide, value))
					return;

				_currentDropGuide = value;

				UpdatePreview();
			}
		}

		private DockItem DragMovePreviewItem => Controller.DockControl.GetPreviewItem(Controller.DockControl.DragMoveItem);

		private bool IsPresenterVisible
		{
			get => _isPresenterVisible;
			set
			{
				_isPresenterVisible = value;

				if (value)
					PreviewPresenter?.ShowPresenter();
				else
					PreviewPresenter?.HidePresenter();
			}
		}

		private IEnumerable<DockItem> PreviewItems => Controller.DockControl.DragMoveItem?.EnumerateItems().Where(w => w.IsSimple()).Select(w => w.PreviewItem);

		private PreviewPresenter PreviewPresenter { get; set; }

		private Preview RenderPreview
		{
			get => _renderPreview;
			set
			{
				_renderPreview = value;

				if (PreviewPresenter != null)
					PreviewPresenter.Preview = value;
			}
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			var arrangeOverride = base.ArrangeOverride(arrangeBounds);

			if (ActualPreview == null)
				return arrangeOverride;

			ActualPreview.IsArrangePassed = true;

			UpdateGeometry();

			return arrangeOverride;
		}

		internal void ClearPreviewCache()
		{
			ActualPreview = null;

			_previewQueue.Clear();
			_previewGeometryCache.Clear();
		}

		private Geometry CreateGeometry()
		{
			var winRects = new List<Rect>();
			var tabViewItemRectDictionary = new Dictionary<Dock, List<Rect>>
			{
				[Dock.Left] = new(),
				[Dock.Top] = new(),
				[Dock.Right] = new(),
				[Dock.Bottom] = new()
			};

			foreach (var dockItem in PreviewItems)
			{
				if (dockItem.IsActuallyVisible)
					winRects.Add(dockItem.GetScreenLogicalBox());

				var tabViewItem = dockItem.TabViewItem;

				if (tabViewItem.TabViewControl == null)
					continue;

				var tabViewItemRect = tabViewItem.GetScreenLogicalBox();

				tabViewItemRectDictionary[tabViewItem.TabViewControl.TabStripPlacement].Add(tabViewItemRect);
			}

			return CreatePreviewGeometry(winRects, tabViewItemRectDictionary);
		}

		internal void CreatePreview(DockItem target, DropGuideAction action)
		{
			var key = (target?.Name ?? string.Empty) + action;

			if (_previewGeometryCache.TryGetValue(key, out var preview))
			{
				RenderPreview = preview;

				return;
			}

			preview = new Preview(action, target);

			_previewGeometryCache[key] = preview;

			RenderPreview = preview;

			_previewQueue.Enqueue(preview);

			InvalidateMeasure();
		}

		private static Geometry CreatePreviewGeometry(IEnumerable<Rect> winRects, IDictionary<Dock, List<Rect>> tabRects)
		{
			var geometryGroup = new GeometryGroup();

			geometryGroup.Children.AddRange(tabRects.Values.SelectMany(v => v).Concat(winRects).Select(r => new RectangleGeometry(r)));

			return geometryGroup.GetOutlinedPathGeometry();
		}

		internal void EnterPreviewState(DockControlLayout layout)
		{
			_isInPreviewState = true;

			ClearPreviewCache();

			RenderPreview = null;
			IsPresenterVisible = true;

			_layout = layout;
		}

		internal void ExitPreviewState()
		{
			RenderPreview = null;
			IsPresenterVisible = false;
			
			ClearPreviewCache();

			_layout = null;
			_isInPreviewState = false;
		}

		private static bool IsItemLayoutValid(DockItem dockItem)
		{
			return dockItem.IsActuallyVisible && dockItem.IsArrangeValid && dockItem.IsMeasureValid;
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			if (_isInPreviewState == false || _previewQueue.Count == 0 || ActualPreview != null)
				return base.MeasureOverride(availableSize);

			ActualPreview = _previewQueue.Dequeue();

			Controller.ApplyLayout(_layout);

			var dropTarget = ActualPreview.DropTarget != null ? Controller.DockControl.GetPreviewItem(ActualPreview.DropTarget) : null;
			var dropSource = DragMovePreviewItem;

			Controller.DropItem(dropSource, dropTarget, ActualPreview.Action);

			ActualPreview.IsMeasurePassed = true;

			return base.MeasureOverride(availableSize);
		}

		internal void OnItemArranged(DockItem item)
		{
			if (_isInPreviewState == false)
				return;

			if (ActualPreview != null)
				UpdateGeometry();
		}

		internal void OnItemMeasured(DockItem item)
		{
		}

		private void OnLayoutUpdated(object sender, EventArgs e)
		{
			if (ActualPreview == null)
				return;

			UpdateGeometry();

			if (ActualPreview.AwaitGeometry == false)
				return;

			if (_isInPreviewState == false)
				return;

			var geometry = CreateGeometry();

			if (geometry == null)
			{
				InvalidateMeasure();

				return;
			}

			if (geometry.IsEmpty())
			{
				InvalidateMeasure();

				return;
			}

			ActualPreview.Geometry = geometry;
			ActualPreview = null;

			if (_previewQueue.Any())
				InvalidateMeasure();
		}

		protected override void OnLoaded()
		{
			base.OnLoaded();

			LayoutUpdated += OnLayoutUpdated;

			PreviewPresenter = new PreviewPresenter {Preview = RenderPreview};

			if (IsPresenterVisible)
				PreviewPresenter.ShowPresenter();
		}

		protected override void OnUnloaded()
		{
			LayoutUpdated -= OnLayoutUpdated;

			PreviewPresenter.Preview = null;
			PreviewPresenter.Close();
			PreviewPresenter = null;

			base.OnUnloaded();
		}

		private void UpdateGeometry()
		{
			if (ActualPreview == null)
				return;

			if (ActualPreview.AwaitGeometry)
				return;

			if (ActualPreview.IsMeasurePassed == false)
			{
				InvalidateMeasure();

				return;
			}

			if (ActualPreview.IsArrangePassed == false)
			{
				InvalidateArrange();

				return;
			}

			if (IsMeasureValid == false || IsArrangeValid == false)
				return;

			if (PreviewItems == null || PreviewItems.Where(w => w.IsActuallyVisible).Any(w => IsItemLayoutValid(w) == false))
				return;

			ActualPreview.AwaitGeometry = true;

			Dispatcher.BeginInvoke(InvalidateMeasure);
		}

		private void UpdatePreview()
		{
			if (_currentDropGuide == null)
			{
				RenderPreview = null;

				return;
			}

			var compass = _currentDropGuide.Compass;

			if (compass is LocalDropCompass)
			{
				var compassTarget = Controller.DockControl.Controller.GetItemIfDocumentLayout(compass.PlacementTarget);

				if (compassTarget == null)
					return;

				CreatePreview(compassTarget, _currentDropGuide.Action);
			}
			else
				CreatePreview(null, _currentDropGuide.Action);
		}
	}
}