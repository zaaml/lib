// <copyright file="VirtualUnitStackPanelLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.ScrollView;
using Zaaml.UI.Panels.Core;
using Zaaml.UI.Panels.Interfaces;

namespace Zaaml.UI.Panels.VirtualStackPanelLayout
{
	internal partial class VirtualUnitStackPanelLayout : VirtualPanelLayoutBase<IVirtualStackPanel>
	{
		private BringIntoViewRequest _enqueueBringIntoViewRequest;
		private Size _pixelViewport;
		private double _preCacheDelta;

		public VirtualUnitStackPanelLayout(IVirtualStackPanel panel) : base(panel)
		{
		}

		private UIElementCollection Children => Panel.Children;

		private Orientation Orientation => Panel.Orientation;

		internal bool PreserveScrollInfo { get; set; }

		private VirtualUIElementCollectionInserter UIElementInserter { get; } = new VirtualUIElementCollectionInserter();

		protected override Size ArrangeCore(Size finalSize)
		{
			var orientation = Orientation;
			var offset = new OrientedPoint(orientation);
			var finalOriented = finalSize.AsOriented(orientation);

			offset.Direct -= _preCacheDelta;

			foreach (UIElement child in Children)
			{
				var size = child.DesiredSize.AsOriented(orientation);

				size.Indirect = finalOriented.Indirect;

				var rect = new Rect(offset.Point, size.Size);

				ArrangeChild(child, rect);

				offset.Direct += size.Direct;
			}

			return finalSize;
		}

		public override void BringIntoView(BringIntoViewRequest request)
		{
			var index = request.Index;

			if (request.ElementInternal != null)
				index = Source.GetIndexFromItem(request.ElementInternal);

			if (index == -1)
				index = request.FallbackIndex;

			BringIntoView(index, request.Mode);
		}

		private void BringIntoView(int index, BringIntoViewMode mode)
		{
			if (index < 0 || index >= ItemsCount)
				return;

			Offset = CalcBringIntoViewOffset(index, mode, Viewport, Extent, Offset);

			_enqueueBringIntoViewRequest = null;
		}

		private Vector CalcBringIntoViewOffset(int index, BringIntoViewMode mode, Size viewport, Size extent, Vector offset)
		{
			if (index < 0 || index >= ItemsCount)
				return offset;

			var orientation = Orientation;
			var orientedViewPort = viewport.AsOriented(orientation);
			var orientedExtent = extent.AsOriented(orientation);
			var orientedOffset = offset.AsOriented(orientation);
			var orientedViewer = new OrientedScrollView(orientation, orientedViewPort.Direct, orientedExtent.Direct,
				orientedOffset.Direct);

			orientedViewer.Offset = orientedViewer.Offset - index > 0.0 || mode == BringIntoViewMode.Top
				? index
				: index - orientedViewer.Viewport + 1.0;
			orientedOffset.Direct = orientedViewer.Offset;

			return orientedOffset.Vector;
		}

		private int CalcFirstVisibleIndex(Vector offset)
		{
			return (int) (Orientation == Orientation.Vertical ? offset.Y : offset.X).RoundToZero();
		}

		public void EnqueueBringIntoView(BringIntoViewRequest request)
		{
			_enqueueBringIntoViewRequest = request;
		}

		protected override void ExecuteScrollCommand(ScrollCommandKind command)
		{
			var commandOrientation = ScrollViewUtils.GetCommandOrientation(command);
			var orientation = Orientation;

			var orientedViewport = Viewport.AsOriented(orientation);
			var orientedExtent = Extent.AsOriented(orientation);
			var orientedOffset = Offset.AsOriented(orientation);
			var directScrollView = new OrientedScrollView(orientation, orientedViewport.Direct, orientedExtent.Direct,
				orientedOffset.Direct);
			var indirectScrollView = new OrientedScrollView(orientation.Rotate(), orientedViewport.Indirect,
				orientedExtent.Indirect, orientedOffset.Indirect);

			directScrollView.ExecuteScrollCommand(command,
				commandOrientation == orientation
					? ScrollViewUtils.DefaultUnitSmallChange
					: ScrollViewUtils.DefaultPixelSmallChange,
				commandOrientation == orientation
					? ScrollViewUtils.DefaultUnitWheelChange
					: ScrollViewUtils.DefaultPixelWheelChange);
			indirectScrollView.ExecuteScrollCommand(command,
				commandOrientation != orientation
					? ScrollViewUtils.DefaultUnitSmallChange
					: ScrollViewUtils.DefaultPixelSmallChange,
				commandOrientation != orientation
					? ScrollViewUtils.DefaultUnitWheelChange
					: ScrollViewUtils.DefaultPixelWheelChange);

			orientedOffset.Direct = directScrollView.Offset;
			orientedOffset.Indirect = indirectScrollView.Offset;

			Offset = orientedOffset.Vector;
		}

		internal ItemLayoutInformation GetLayoutInformation(FrameworkElement element)
		{
			return GetLayoutInformation(Source.GetIndexFromItem(element));
		}

		internal ItemLayoutInformation GetLayoutInformation(int index)
		{
			if (index < 0 || index >= ItemsCount)
				return ItemLayoutInformation.Empty;

			var item = Source.GetCurrent(index) as FrameworkElement;

			if (item == null)
				return ItemLayoutInformation.Empty;

			return new ItemLayoutInformation(item, GetLayoutSlot(item), new Rect(_pixelViewport));
		}

		private Rect GetLayoutSlot(FrameworkElement item)
		{
			if (item is ILayoutInformation layoutInformation)
				return layoutInformation.ArrangeRect;

			return LayoutInformation.GetLayoutSlot(item);
		}

		private void MeasureChild(UIElement element, Size constraint)
		{
			element.Measure(constraint);
		}

		protected override Size MeasureCore(Size availableSize)
		{
			try
			{
				Source?.EnterGeneration();

				return MeasureImpl(availableSize);
			}
			finally
			{
				Source?.LeaveGeneration();
			}
		}

		private Size MeasureImpl(Size availableSize)
		{
			HandleBringIntoView();

			var context = new VirtualMeasureContext(this, availableSize);

			try
			{
				UIElementInserter.Enter(Children);

				context.Measure(Panel.FocusedIndex);
			}
			finally
			{
				UIElementInserter.Leave();
			}

			if (PreserveScrollInfo == false)
			{
				Offset = context.CalcOffset();
				ScrollInfo = context.CalcScrollInfo();
			}

			_pixelViewport = context.CalcFinalSize();
			_preCacheDelta = context.PreCacheDelta;

			UpdateTransform();

			return _pixelViewport;
		}

		private void HandleBringIntoView()
		{
			var bringIntoViewIndex = -1;
			var bringIntoViewMode = BringIntoViewMode.Default;

			if (_enqueueBringIntoViewRequest != null)
			{
				bringIntoViewMode = _enqueueBringIntoViewRequest.Mode;

				var frameworkElement = _enqueueBringIntoViewRequest.ElementInternal;

				bringIntoViewIndex = frameworkElement != null
					? Source.GetIndexFromItem(frameworkElement)
					: _enqueueBringIntoViewRequest.Index;

				if (bringIntoViewIndex == -1)
					bringIntoViewIndex = _enqueueBringIntoViewRequest.FallbackIndex;
			}

			if (bringIntoViewIndex != -1)
			{
				var orientation = Orientation;
				var extent = Extent.AsOriented(orientation);

				extent.Direct =
					extent.Direct.Clamp(bringIntoViewIndex + Viewport.AsOriented(orientation).Direct, double.MaxValue);

				Offset = CalcBringIntoViewOffset(bringIntoViewIndex, bringIntoViewMode, Viewport, extent.Size, Offset);
			}

			_enqueueBringIntoViewRequest = null;
		}

		internal void OnItemAttachedInternal(UIElement element)
		{
		}

		internal void OnItemAttachingInternal(UIElement element)
		{
		}

		internal void OnItemDetachedInternal(UIElement element)
		{
		}

		internal void OnItemDetachingInternal(UIElement element)
		{
			if (UIElementInserter.Collection == null)
			{
				var index = Children.IndexOf(element);

				if (index != -1)
					Children.RemoveAt(index);
			}
			else
			{
				UIElementInserter.Remove(element);
			}
		}

		protected override void OnOffsetChanged(OffsetChangedEventArgs e)
		{
			base.OnOffsetChanged(e);

			if (CalcFirstVisibleIndex(e.OldOffset) != CalcFirstVisibleIndex(e.NewOffset))
				Panel.InvalidateMeasure();

			UpdateTransform();
		}

		private UIElement Realize(int index)
		{
			return Source.Realize(index);
		}

		private void UpdateTransform()
		{
			var transform = Panel.Transform;
			var offset = ScrollInfo.ClampOffset(Offset);

			if (Orientation == Orientation.Vertical)
				transform.TranslateX = -offset.X;
			else
				transform.TranslateY = -offset.Y;
		}

		private sealed class VirtualUIElementCollectionInserter
		{
			private int _headIndex;
			private int _sequentialCount;
			private RobustInserterState _state;

			public UIElementCollection Collection { get; private set; }

			public int Count => _state == RobustInserterState.SequentialAdd ? _sequentialCount : Collection.Count;

			public UIElement this[int index] => Collection[_headIndex + index];

			public void Add(UIElement item)
			{
				if (_state == RobustInserterState.RandomAdd)
				{
					Collection.Add(item);

					return;
				}

				if (_state == RobustInserterState.Initialized)
				{
					_state = RobustInserterState.SequentialAdd;

					var index = Collection.IndexOf(item);

					if (index == -1)
					{
						Collection.Insert(0, item);
					}
					else
					{
						if (index > 0)
							Collection.RemoveRange(0, index);
					}

					_headIndex = 0;
					_sequentialCount = 1;
				}
				else if (_state == RobustInserterState.SequentialAdd)
				{
					if (ReferenceEquals(Collection[_headIndex + _sequentialCount], item))
					{
						_sequentialCount++;
					}
					else
					{
						var findNextIndex = Collection.IndexOf(item);

						if (findNextIndex != -1)
						{
							Collection.RemoveRange(_headIndex + _sequentialCount, findNextIndex - (_headIndex + _sequentialCount));

							_sequentialCount++;
						}
						else
						{
							Collection.Insert(_headIndex + _sequentialCount, item);

							_sequentialCount++;
						}
					}
				}

				if (_headIndex + _sequentialCount == Collection.Count)
					CommitSequentialAdd();
			}

			private void Commit()
			{
				if (_state == RobustInserterState.Initialized)
				{
					Collection.Clear();

					_state = RobustInserterState.RandomAdd;
				}
				else if (_state == RobustInserterState.SequentialAdd)
				{
					CommitSequentialAdd();
				}
			}

			private void CommitSequentialAdd()
			{
				if (_state == RobustInserterState.SequentialAdd)
				{
					if (_headIndex > 0)
					{
						Collection.RemoveRange(0, _headIndex);

						_headIndex = 0;
					}

					if (_sequentialCount < Collection.Count)
						Collection.RemoveRange(_sequentialCount, Collection.Count - _sequentialCount);

					_state = RobustInserterState.RandomAdd;
				}
			}

			public void Enter(UIElementCollection collection)
			{
				_headIndex = 0;
				_sequentialCount = 0;
				_state = RobustInserterState.Initialized;

				Collection = collection;

				if (Collection.Count == 0)
					_state = RobustInserterState.RandomAdd;
			}

			public void Insert(int index, UIElement item)
			{
				Commit();

				Collection.Insert(index, item);
			}

			public void Leave()
			{
				Commit();

				Collection = null;
			}

			public void Remove(UIElement element)
			{
				var index = Collection.IndexOf(element);

				if (index == -1)
					return;

				if (_state == RobustInserterState.SequentialAdd)
					if (index >= _headIndex && index < _headIndex + _sequentialCount)
					{
						if (index == _headIndex)
							_headIndex++;

						_sequentialCount--;
					}

				Collection.RemoveAt(index);
			}

			private enum RobustInserterState
			{
				Initialized,
				SequentialAdd,
				RandomAdd
			}
		}
	}
}