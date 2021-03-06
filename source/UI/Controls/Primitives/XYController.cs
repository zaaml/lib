// <copyright file="XYController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Primitives
{
	[TemplateContractType(typeof(XYControllerTemplateContract))]
	[ContentProperty(nameof(ItemCollection))]
	public class XYController : TemplateContractControl
	{
		public static readonly DependencyProperty MinimumXProperty = DPM.Register<double, XYController>
			("MinimumX", 0.0, d => d.OnMinimumXPropertyChangedPrivate);

		public static readonly DependencyProperty MaximumXProperty = DPM.Register<double, XYController>
			("MaximumX", 1.0, d => d.OnMaximumXPropertyChangedPrivate);

		public static readonly DependencyProperty MinimumYProperty = DPM.Register<double, XYController>
			("MinimumY", 0.0, d => d.OnMinimumYPropertyChangedPrivate);

		public static readonly DependencyProperty MaximumYProperty = DPM.Register<double, XYController>
			("MaximumY", 1.0, d => d.OnMaximumYPropertyChangedPrivate);

		private static readonly DependencyPropertyKey ItemCollectionPropertyKey = DPM.RegisterReadOnly<XYControllerItemCollection, XYController>
			("ItemCollectionPrivate");

		public static readonly DependencyProperty ItemCollectionProperty = ItemCollectionPropertyKey.DependencyProperty;

		private readonly DragController _dragController;

		static XYController()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<XYController>();
		}

		public XYController()
		{
			_dragController = new DragController(this);

			this.OverrideStyleKey<XYController>();
		}

		public XYControllerItemCollection ItemCollection => this.GetValueOrCreate(ItemCollectionPropertyKey, () => new XYControllerItemCollection(this));

		public double MaximumX
		{
			get => (double) GetValue(MaximumXProperty);
			set => SetValue(MaximumXProperty, value);
		}

		public double MaximumY
		{
			get => (double) GetValue(MaximumYProperty);
			set => SetValue(MaximumYProperty, value);
		}

		public double MinimumX
		{
			get => (double) GetValue(MinimumXProperty);
			set => SetValue(MinimumXProperty, value);
		}

		public double MinimumY
		{
			get => (double) GetValue(MinimumYProperty);
			set => SetValue(MinimumYProperty, value);
		}

		private XYControllerTemplateContract TemplateContract => (XYControllerTemplateContract) TemplateContractInternal;

		private XYControllerPanel XYControllerPanel => TemplateContract.XYControllerPanel;

		private void OnMaximumXPropertyChangedPrivate(double oldValue, double newValue)
		{
			UpdateRanges();
		}

		private void OnMaximumYPropertyChangedPrivate(double oldValue, double newValue)
		{
			UpdateRanges();
		}

		private void OnMinimumXPropertyChangedPrivate(double oldValue, double newValue)
		{
			UpdateRanges();
		}

		private void OnMinimumYPropertyChangedPrivate(double oldValue, double newValue)
		{
			UpdateRanges();
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			_dragController.OnMouseLeftButtonDown(e);
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			_dragController.OnMouseLeftButtonUp(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			_dragController.OnMouseMove(e);
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			XYControllerPanel.XYController = this;
		}

		protected override void OnTemplateContractDetaching()
		{
			XYControllerPanel.XYController = null;

			base.OnTemplateContractDetaching();
		}

		internal void OnXChangedInternal(XYControllerItem xyControllerItem, double oldValue, double newValue)
		{
			XYControllerPanel?.InvalidateArrange();
		}

		internal void OnYChangedInternal(XYControllerItem xyControllerItem, double oldValue, double newValue)
		{
			XYControllerPanel?.InvalidateArrange();
		}

		private void UpdateRanges()
		{
			foreach (var xyControllerItem in ItemCollection)
				xyControllerItem.Clamp();

			XYControllerPanel?.OnRangesChanged();
		}

		private sealed class DragController : XYRangeDragController<XYController, XYControllerItem>
		{
			public DragController(XYController control) : base(control)
			{
			}

			protected override Range<double> RangeX => new Range<double>(Control.MinimumX, Control.MaximumX);

			protected override Range<double> RangeY => new Range<double>(Control.MinimumY, Control.MaximumY);

			protected override void DragSyncValue(XYControllerItem item, double x, double y)
			{
				if (double.IsNaN(x) == false)
					item.X = x.Clamp(Control.MinimumX, Control.MaximumX);

				if (double.IsNaN(y) == false)
					item.Y = y.Clamp(Control.MinimumY, Control.MaximumY);

				Control?.XYControllerPanel?.ArrangeItem(item);
			}

			protected override XYControllerItem GetDragItem(MouseButtonEventArgs e)
			{
				var xyControllerItem = base.GetDragItem(e);

				if (xyControllerItem != null || Control.XYControllerPanel == null)
					return xyControllerItem;

				xyControllerItem = Control.ItemCollection.FirstOrDefault();

				if (xyControllerItem == null)
					return null;

				Control.XYControllerPanel.MoveItem(xyControllerItem, e.GetPosition(Control.XYControllerPanel));

				return xyControllerItem;
			}

			protected override double GetPixelRatioX(XYControllerItem item)
			{
				return Control?.XYControllerPanel.GetPixelRatioX(item) ?? 0.0;
			}

			protected override double GetPixelRatioY(XYControllerItem item)
			{
				return Control?.XYControllerPanel.GetPixelRatioY(item) ?? 0.0;
			}

			protected override double GetX(XYControllerItem item)
			{
				return item.X;
			}

			protected override double GetY(XYControllerItem item)
			{
				return item.Y;
			}
		}
	}
}