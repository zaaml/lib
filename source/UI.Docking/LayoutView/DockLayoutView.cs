// <copyright file="DockLayoutView.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Extensions;

namespace Zaaml.UI.Controls.Docking
{
	public class DockLayoutView : BaseLayoutView<DockLayout>
	{
		private readonly Dictionary<UIElement, UIChildDefinition> _childrenDefinitions = new();
		private readonly Dictionary<DockItem, Dock?> _itemSideDictionary = new();
		private readonly Dictionary<DockItem, DockGridSplitter> _itemSplitterDictionary = new();
		private readonly ConstraintGridSplitterDelta _splitterConstraintHelper;
		private UIChildDefinition _contentDefinition = new(null, null);
		private bool _isSizeChangeSuspended;
		private Size _splitterSize;

		static DockLayoutView()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<DockLayoutView>();
		}

		public DockLayoutView()
		{
			this.OverrideStyleKey<DockLayoutView>();

			_splitterConstraintHelper = new ConstraintGridSplitterDelta(new Size(40, 40));

			SizeChanged += OnSizeChanged;
		}

		private ContentPresenter ContentPresenter => TemplateContract.ContentPresenter;

		private Grid ItemsHost => TemplateContract.ItemsHost;

		private DockLayoutViewTemplateContract TemplateContract => (DockLayoutViewTemplateContract)TemplateContractInternal;

		private void AddGridDefinitions(DockItem item)
		{
			if (TemplateContract.IsAttached == false)
				return;

			var side = DockLayout.GetDock(item);

			_itemSideDictionary[item] = side;

			switch (side)
			{
				case Dock.Left:
				case Dock.Right:

					ItemsHost.ColumnDefinitions.Add(new ColumnDefinition());
					ItemsHost.ColumnDefinitions.Add(new ColumnDefinition());

					break;

				case Dock.Top:
				case Dock.Bottom:

					ItemsHost.RowDefinitions.Add(new RowDefinition());
					ItemsHost.RowDefinitions.Add(new RowDefinition());

					break;
			}
		}

		private void ArrangeContentPresenter(FrameworkElement element, ColumnDefinition contentColumn, RowDefinition contentRow)
		{
			element.ApplyGridPosition();

			Grid.SetColumn(element, ItemsHost.ColumnDefinitions.IndexOf(contentColumn));
			Grid.SetRow(element, ItemsHost.RowDefinitions.IndexOf(contentRow));

			if (ReferenceEquals(element, ContentPresenter))
				_childrenDefinitions[element] = new UIChildDefinition(contentColumn, contentRow);
		}

		protected internal override void ArrangeItems()
		{
			if (TemplateContract.IsAttached == false)
				return;

			_childrenDefinitions.Clear();
			ItemsHost.Children.Clear();
			ItemsHost.Children.Add(ContentPresenter);

			var leftCol = 0;
			var rightCol = ItemsHost.ColumnDefinitions.Count - 1;
			var topRow = 0;
			var bottomRow = ItemsHost.RowDefinitions.Count - 1;

			foreach (var item in ReverseOrderedItems)
			{
				var splitter = _itemSplitterDictionary[item];

				ItemsHost.Children.Add(item);
				ItemsHost.Children.Add(splitter);

				var colIndex = leftCol;
				var rowIndex = topRow;
				var splitColIndex = leftCol;
				var splitRowIndex = topRow;
				var colSpan = 1;
				var rowSpan = 1;

				switch (DockLayout.GetDock(item))
				{
					case Dock.Left:

						colIndex = leftCol;
						splitColIndex = leftCol + 1;
						leftCol += 2;

						break;

					case Dock.Top:

						rowIndex = topRow;
						splitRowIndex = topRow + 1;
						topRow += 2;

						break;

					case Dock.Right:

						colIndex = rightCol;
						splitColIndex = rightCol - 1;
						rightCol -= 2;

						break;

					case Dock.Bottom:

						rowIndex = bottomRow;
						splitRowIndex = bottomRow - 1;
						bottomRow -= 2;

						break;
				}

				switch (GetOrientation(item))
				{
					case Orientation.Horizontal:

						rowSpan = bottomRow - topRow + 1;
						SetResizeDirection(splitter, GridResizeDirection.Columns);
						ItemsHost.ColumnDefinitions[splitColIndex].Width = GridLength.Auto;

						_childrenDefinitions[splitter] = new UIChildDefinition(ItemsHost.ColumnDefinitions[splitColIndex], null);
						_childrenDefinitions[item] = new UIChildDefinition(ItemsHost.ColumnDefinitions[colIndex], null);

						break;

					case Orientation.Vertical:

						colSpan = rightCol - leftCol + 1;
						SetResizeDirection(splitter, GridResizeDirection.Rows);
						ItemsHost.RowDefinitions[splitRowIndex].Height = GridLength.Auto;

						_childrenDefinitions[splitter] = new UIChildDefinition(null, ItemsHost.RowDefinitions[splitRowIndex]);
						_childrenDefinitions[item] = new UIChildDefinition(null, ItemsHost.RowDefinitions[rowIndex]);

						break;
				}

				SetDefinitionSize(item);

				Grid.SetColumn(item, colIndex);
				Grid.SetColumnSpan(item, colSpan);
				Grid.SetRow(item, rowIndex);
				Grid.SetRowSpan(item, rowSpan);

				Grid.SetColumn(splitter, splitColIndex);
				Grid.SetColumnSpan(splitter, colSpan);
				Grid.SetRow(splitter, splitRowIndex);
				Grid.SetRowSpan(splitter, rowSpan);
			}

			var contentColumn = ItemsHost.ColumnDefinitions[leftCol];
			var contentRow = ItemsHost.RowDefinitions[topRow];

			_contentDefinition = new UIChildDefinition(contentColumn, contentRow);

			ArrangeContentPresenter(ContentPresenter, contentColumn, contentRow);

			UpdateContentDefinitionsSize();
		}

		protected override TemplateContract CreateTemplateContract()
		{
			return new DockLayoutViewTemplateContract();
		}

		private void FinalizeSplitting()
		{
			_isSizeChangeSuspended = true;

			foreach (var item in Items)
			{
				var definition = _childrenDefinitions[item];

				switch (GetOrientation(item))
				{
					case Orientation.Horizontal:

						DockLayout.SetWidth(item, definition.Column.Width.Value);

						break;

					case Orientation.Vertical:

						DockLayout.SetHeight(item, definition.Row.Height.Value);

						break;
				}
			}

			_isSizeChangeSuspended = false;
		}

		private static Orientation GetOrientation(DockItem item)
		{
			return Util.GetOrientation(DockLayout.GetDock(item));
		}

		internal static GridResizeDirection GetResizeDirection(GridSplitter gridSplitter)
		{
			return gridSplitter.ResizeDirection;
		}

		protected override void OnItemAdded(DockItem item)
		{
			var splitter = new DockGridSplitter();

			splitter.SetZIndex(short.MaxValue);
			_itemSplitterDictionary[item] = splitter;
			splitter.LostMouseCapture += OnSplitterLostMouseCapture;
			_splitterConstraintHelper.AddSplitter(splitter);

			AddGridDefinitions(item);

			InvalidateItemsArrange();
		}

		internal void OnItemDockChanged(DockItem item)
		{
			RemoveGridDefinitions(item);
			AddGridDefinitions(item);

			InvalidateItemsArrange();
		}

		internal void OnItemSizeChanged(DockItem item)
		{
			if (_isSizeChangeSuspended)
				return;

			foreach (var w in Items)
				SetDefinitionSize(w);

			UpdateContentDefinitionsSize();
		}

		protected override void OnItemRemoved(DockItem item)
		{
			var splitter = _itemSplitterDictionary[item];

			_itemSplitterDictionary.Remove(item);
			splitter.LostMouseCapture -= OnSplitterLostMouseCapture;

			_splitterConstraintHelper.RemoveSplitter(splitter);

			ItemsHost?.Children.Remove(item);

			RemoveGridDefinitions(item);

			InvalidateItemsArrange();
		}

		private void OnMeasureSplitterLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			var splitter = (GridSplitter)sender;

			splitter.Loaded -= OnMeasureSplitterLoaded;
			ItemsHost.Children.Remove(splitter);

			if (GetResizeDirection(splitter) == GridResizeDirection.Columns)
				_splitterSize.Width = splitter.ActualWidth;

			if (GetResizeDirection(splitter) == GridResizeDirection.Rows)
				_splitterSize.Height = splitter.ActualHeight;
		}

		private void OnSizeChanged(object sender, SizeChangedEventArgs args)
		{
			UpdateContentDefinitionsSize();
		}

		private void OnSplitterLostMouseCapture(object sender, MouseEventArgs e)
		{
			FinalizeSplitting();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			ItemsHost.ColumnDefinitions.Clear();
			ItemsHost.RowDefinitions.Clear();
			ItemsHost.ColumnDefinitions.Add(new ColumnDefinition());
			ItemsHost.RowDefinitions.Add(new RowDefinition());

			foreach (var item in Items)
				AddGridDefinitions(item);

			_splitterConstraintHelper.Grid = ItemsHost;

			// Enqueue splitter measurements
			var columnSplitter = new DockGridSplitter { Opacity = 0, IsHitTestVisible = false };
			var rowSplitter = new DockGridSplitter { Opacity = 0, IsHitTestVisible = false };

			SetResizeDirection(columnSplitter, GridResizeDirection.Columns);
			SetResizeDirection(rowSplitter, GridResizeDirection.Rows);

			ItemsHost.Children.Add(columnSplitter);
			ItemsHost.Children.Add(rowSplitter);

			columnSplitter.Loaded += OnMeasureSplitterLoaded;
			rowSplitter.Loaded += OnMeasureSplitterLoaded;

			InvalidateItemsArrange();
		}

		protected override void OnTemplateContractDetaching()
		{
			foreach (var splitter in ItemsHost.Children.OfType<DockGridSplitter>())
				splitter.Loaded -= OnMeasureSplitterLoaded;

			ItemsHost.Children.Clear();

			_splitterConstraintHelper.Grid = null;

			base.OnTemplateContractDetaching();
		}

		private void RemoveGridDefinitions(DockItem item)
		{
			if (TemplateContract.IsAttached == false)
				return;

			var side = _itemSideDictionary[item];

			_itemSideDictionary.Remove(item);

			switch (side)
			{
				case Dock.Left:
				case Dock.Right:

					ItemsHost.ColumnDefinitions.RemoveAt(0);
					ItemsHost.ColumnDefinitions.RemoveAt(0);

					break;

				case Dock.Top:
				case Dock.Bottom:

					ItemsHost.RowDefinitions.RemoveAt(0);
					ItemsHost.RowDefinitions.RemoveAt(0);

					break;
			}
		}

		private void SetDefinitionSize(DockItem item)
		{
			if (_childrenDefinitions.TryGetValue(item, out var definition) == false)
				return;

			var dockSize = new Size(DockLayout.GetWidth(item), DockLayout.GetHeight(item));

			dockSize.Width = dockSize.Width > 0 ? dockSize.Width : 1;
			dockSize.Height = dockSize.Height > 0 ? dockSize.Height : 1;

			definition.SetSize(dockSize);
		}

		internal static void SetResizeDirection(GridSplitter gridSplitter, GridResizeDirection resizeDirection)
		{
			switch (resizeDirection)
			{
				case GridResizeDirection.Columns:

					gridSplitter.VerticalAlignment = VerticalAlignment.Stretch;
					gridSplitter.HorizontalAlignment = HorizontalAlignment.Center;

					break;

				case GridResizeDirection.Rows:

					gridSplitter.VerticalAlignment = VerticalAlignment.Center;
					gridSplitter.HorizontalAlignment = HorizontalAlignment.Stretch;

					break;
			}

			gridSplitter.ResizeDirection = resizeDirection;
		}

		private void UpdateContentDefinitionsSize()
		{
			var contentWidth = ActualWidth;
			var contentHeight = ActualHeight;

			foreach (var item in Items)
			{
				var dockSize = new Size(DockLayout.GetWidth(item), DockLayout.GetHeight(item));

				dockSize.Width = dockSize.Width > 0 ? dockSize.Width : 1;
				dockSize.Height = dockSize.Height > 0 ? dockSize.Height : 1;

				switch (GetOrientation(item))
				{
					case Orientation.Horizontal:

						contentWidth -= dockSize.Width + _splitterSize.Width;

						break;
					case Orientation.Vertical:

						contentHeight -= dockSize.Height + _splitterSize.Height;

						break;
				}
			}

			contentWidth = contentWidth > 1 ? contentWidth : 1;
			contentHeight = contentHeight > 1 ? contentHeight : 1;

			_contentDefinition.SetSize(new Size(contentWidth, contentHeight));
		}

		internal readonly struct UIChildDefinition
		{
			private static readonly ColumnDefinition NullColumn = new();
			private static readonly RowDefinition NullRow = new();

			public readonly ColumnDefinition Column;
			public readonly RowDefinition Row;

			public UIChildDefinition(ColumnDefinition column, RowDefinition row)
			{
				Column = column ?? NullColumn;
				Row = row ?? NullRow;
			}

			public void SetSize(Size size)
			{
				Column.Width = new GridLength(size.Width, GridUnitType.Star);
				Row.Height = new GridLength(size.Height, GridUnitType.Star);
			}
		}
	}

	internal sealed class DockLayoutViewTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = true)]
		public ContentPresenter ContentPresenter { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public Grid ItemsHost { get; [UsedImplicitly] private set; }
	}
}