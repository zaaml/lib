// <copyright file="AutoHideDockItemPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.PresentationCore.Converters;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core;
using ContentControl = Zaaml.UI.Controls.Core.ContentControl;

namespace Zaaml.UI.Controls.Docking
{
	[TemplateContractType(typeof(AutoHideDockItemPresenterTemplateContract))]
	public sealed class AutoHideDockItemPresenter : TemplateContractControl
	{
		private static readonly PropertyPath AutoHideWidthPropertyPath = new PropertyPath(AutoHideLayout.AutoHideWidthProperty);
		private static readonly PropertyPath AutoHideHeightPropertyPath = new PropertyPath(AutoHideLayout.AutoHideHeightProperty);

		private bool _isAttached;

		public AutoHideDockItemPresenter(AutoHideTabViewItem autoHideTabViewItem)
		{
			AutoHideTabViewItem = autoHideTabViewItem;

			ArrangeContentPresenter();
		}

		public AutoHideTabViewItem AutoHideTabViewItem { get; }

		public DockItem DockItem => AutoHideTabViewItem.DockItem;

		private Grid DockItemGrid => TemplateContract.DockItemGrid;

		private ContentControl DockItemHost => TemplateContract.DockItemHost;

		private DockGridSplitter GridSplitter => TemplateContract.GridSplitter;

		public bool IsAttached
		{
			get => _isAttached;
			set
			{
				if (_isAttached == value)
					return;

				_isAttached = value;

				UpdateDockItemHost();
			}
		}

		private AutoHideDockItemPresenterTemplateContract TemplateContract => (AutoHideDockItemPresenterTemplateContract) TemplateContractInternal;

		private void ArrangeContentPresenter()
		{
			if (DockItemGrid == null)
				return;

			DockItemGrid.ColumnDefinitions.Clear();
			DockItemGrid.RowDefinitions.Clear();

			var dockSide = AutoHideLayout.GetDockSide(DockItem);

			if (dockSide == Dock.Left || dockSide == Dock.Right)
			{
				var itemColumn = new ColumnDefinition
				{
					Width = new GridLength(AutoHideLayout.GetAutoHideWidth(DockItem))
				};

				itemColumn.SetBinding(ColumnDefinition.WidthProperty, new Binding {Path = AutoHideWidthPropertyPath, Source = DockItem, Mode = BindingMode.TwoWay, Converter = XamlConverter.Instance});

				GridSplitter.VerticalAlignment = VerticalAlignment.Stretch;

				DockItemGrid.ColumnDefinitions.Add(new ColumnDefinition());

				var index = dockSide == Dock.Left ? 0 : 1;

				DockItemGrid.ColumnDefinitions.Insert(index, itemColumn);

				Grid.SetColumn(DockItemHost, index);
				Grid.SetColumn(GridSplitter, index);

				GridSplitter.HorizontalAlignment = dockSide == Dock.Left ? HorizontalAlignment.Right : HorizontalAlignment.Left;
			}
			else
			{
				var itemRow = new RowDefinition
				{
					Height = new GridLength(AutoHideLayout.GetAutoHideHeight(DockItem))
				};

				itemRow.SetBinding(RowDefinition.HeightProperty, new Binding {Path = AutoHideHeightPropertyPath, Source = DockItem, Mode = BindingMode.TwoWay, Converter = XamlConverter.Instance});

				GridSplitter.HorizontalAlignment = HorizontalAlignment.Stretch;

				DockItemGrid.RowDefinitions.Add(new RowDefinition());

				var index = dockSide == Dock.Top ? 0 : 1;

				DockItemGrid.RowDefinitions.Insert(index, itemRow);

				Grid.SetRow(DockItemHost, index);
				Grid.SetRow(GridSplitter, index);

				GridSplitter.VerticalAlignment = dockSide == Dock.Top ? VerticalAlignment.Bottom : VerticalAlignment.Top;
			}
		}

		public void AttachItem()
		{
			IsAttached = true;
		}

		public void DetachItem()
		{
			IsAttached = false;
		}

		public void OnItemDockSideChanged(Dock oldDockSide, Dock newDockSide)
		{
			ArrangeContentPresenter();
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);

			AutoHideTabViewItem.OnPresenterMouseEnter(this);
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);

			AutoHideTabViewItem.OnPresenterMouseLeave(this);
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

#if !SILVERLIGHT
			GridSplitter.Focusable = false;
#endif

			GridSplitter.IsTabStop = false;

			Panel.SetZIndex(GridSplitter, 1);

			UpdateDockItemHost();
			ArrangeContentPresenter();
		}

		protected override void OnTemplateContractDetaching()
		{
			DockItemHost.Content = null;
			DockItemGrid.Children.Clear();

			base.OnTemplateContractDetaching();
		}

		private void UpdateDockItemHost()
		{
			if (DockItemHost == null)
				return;

			DockItemHost.Content = IsAttached ? DockItem : null;
		}
	}

	public sealed class AutoHideDockItemPresenterTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = true)]
		public Grid DockItemGrid { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public ContentControl DockItemHost { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public DockGridSplitter GridSplitter { get; [UsedImplicitly] private set; }
	}
}