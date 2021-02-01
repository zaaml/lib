// <copyright file="ArtboardScrollViewControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.ScrollView;

namespace Zaaml.UI.Controls.Artboard
{
	[TemplateContractType(typeof(ArtboardScrollViewControlTemplateContract))]
	public sealed class ArtboardScrollViewControl : ScrollViewControlBase<ArtboardScrollViewPresenter, ArtboardScrollViewPanel>
	{
		private static readonly DependencyPropertyKey ArtboardPropertyKey = DPM.RegisterReadOnly<ArtboardControl, ArtboardScrollViewControl>
			("Artboard", default, d => d.OnArtboardPropertyChangedPrivate);

		public static readonly DependencyProperty TopContentProperty = DPM.Register<object, ArtboardScrollViewControl>
			("TopContent");

		public static readonly DependencyProperty BottomContentProperty = DPM.Register<object, ArtboardScrollViewControl>
			("BottomContent");

		public static readonly DependencyProperty ArtboardProperty = ArtboardPropertyKey.DependencyProperty;

		private static readonly List<double> ZoomTable = new List<double>
		{
			6.25,
			8.33,
			12.5,
			16.67,
			25,
			33.33,
			50,
			66.67,
			100,
			150,
			200,
			300,
			400,
			600,
			800,
			1200,
			1600,
			2400,
			3200,
			4800,
			6400
		};

		public static readonly DependencyProperty ZoomProperty = DPM.Register<double, ArtboardScrollViewControl>
			("Zoom", 1.0, z => z.OnZoomChangedPrivate);

		static ArtboardScrollViewControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ArtboardScrollViewControl>();
		}

		public ArtboardScrollViewControl()
		{
			this.OverrideStyleKey<ArtboardScrollViewControl>();
		}

		public ArtboardControl Artboard
		{
			get => (ArtboardControl) GetValue(ArtboardProperty);
			internal set => this.SetReadOnlyValue(ArtboardPropertyKey, value);
		}

		private ArtboardScrollViewPanel ArtboardScrollViewPanel => (ArtboardScrollViewPanel) ScrollViewPanelCore;

		public object BottomContent
		{
			get => GetValue(BottomContentProperty);
			set => SetValue(BottomContentProperty, value);
		}

		private Point LocalOffset { get; set; }

		protected override ScrollViewPanelBase ScrollViewPanelCore => ScrollViewPresenterInternal?.ScrollViewPanel;

		private ArtboardScrollViewPresenter ScrollViewPresenter => TemplateContract.ScrollViewPresenter;

		private ArtboardScrollViewControlTemplateContract TemplateContract => (ArtboardScrollViewControlTemplateContract) TemplateContractInternal;

		public object TopContent
		{
			get => GetValue(TopContentProperty);
			set => SetValue(TopContentProperty, value);
		}
		
		public double Zoom
		{
			get => (double) GetValue(ZoomProperty);
			set => SetValue(ZoomProperty, value);
		}

		private double CalcZoom(int cnt)
		{
			var zoom = Zoom;

			var index = ZoomTable.BinarySearch(zoom * 100);

			if (index < 0)
				index = ~index;

			if (index + cnt <= 0)
				zoom = ZoomTable.First() / 100;
			else if (index + cnt >= ZoomTable.Count)
				zoom = ZoomTable.Last() / 100;
			else
				zoom = ZoomTable[index + cnt] / 100;

			return zoom;
		}

		private void OnArtboardPropertyChangedPrivate(ArtboardControl oldValue, ArtboardControl newValue)
		{
			UpdateOffset();
		}

		internal void OnDesignSizeChangedInternal()
		{
			UpdateOffset();
		}

		private protected override void OnHorizontalOffsetChangedInternal()
		{
			base.OnHorizontalOffsetChangedInternal();

			UpdateOffset();
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Add:
					OnMouseZoom(CalcZoom(1));

					e.Handled = true;

					break;
				case Key.Subtract:
					OnMouseZoom(CalcZoom(-1));

					e.Handled = true;

					break;
				case Key.Left:
					ExecuteScrollCommand(ScrollCommandKind.LineLeft);

					e.Handled = true;

					break;
				case Key.Right:
					ExecuteScrollCommand(ScrollCommandKind.LineRight);

					e.Handled = true;

					break;
				case Key.Up:
					ExecuteScrollCommand(ScrollCommandKind.LineUp);

					e.Handled = true;

					break;
				case Key.Down:
					ExecuteScrollCommand(ScrollCommandKind.LineDown);

					e.Handled = true;

					break;
			}

			if (e.Handled == false)
				base.OnKeyDown(e);
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);

			if (e.Handled == false)
				Focus();
		}

		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			var delta = -e.Delta / 120;

			if (Keyboard.Modifiers == ModifierKeys.Shift)
			{
			}
			else if (Keyboard.Modifiers == ModifierKeys.Control)
			{
			}
			else
			{
				OnMouseZoom(e, CalcZoom(-delta));
			}

			e.Handled = true;
		}

		private void OnMouseZoom(double zoom)
		{
			try
			{
				EnterScrollCommand();

				if (ArtboardScrollViewPanel != null)
				{
					var mousePoint = MouseInternal.GetPosition(ArtboardScrollViewPanel);
					var viewPortPoint = ArtboardScrollViewPanel.GetViewportPoint(mousePoint);

					ArtboardScrollViewPanel?.OnMouseZoom(viewPortPoint, zoom);
				}

				this.SetCurrentValueInternal(ZoomProperty, zoom);
			}
			finally
			{
				LeaveScrollCommand();
			}
		}

		private void OnMouseZoom(MouseWheelEventArgs mouseEventArgs, double zoom)
		{
			try
			{
				EnterScrollCommand();

				if (ArtboardScrollViewPanel != null)
				{
					var mousePoint = mouseEventArgs.GetPosition(ArtboardScrollViewPanel);
					var viewPortPoint = ArtboardScrollViewPanel.GetViewportPoint(mousePoint);

					ArtboardScrollViewPanel?.OnMouseZoom(viewPortPoint, zoom);
				}

				this.SetCurrentValueInternal(ZoomProperty, zoom);
			}
			finally
			{
				LeaveScrollCommand();
			}
		}

		protected override void OnScrollBarDragCompleted(ScrollBar scrollBar)
		{
			base.OnScrollBarDragCompleted(scrollBar);

			ArtboardScrollViewPanel?.UpdatePaddingInternal();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			ScrollViewPresenterInternal.ScrollView = this;

			UpdateScrollViewPanelInternal();
		}

		protected override void OnTemplateContractDetaching()
		{
			ScrollViewPresenterInternal.ScrollView = null;

			UpdateScrollViewPanelInternal();

			base.OnTemplateContractDetaching();
		}

		private protected override void OnVerticalOffsetChangedInternal()
		{
			base.OnVerticalOffsetChangedInternal();

			UpdateOffset();
		}

		private void OnZoomChangedPrivate(double oldZoom, double newZoom)
		{
			InvalidateScroll();

			ArtboardScrollViewPanel?.UpdateZoom();
		}

		private void UpdateOffset()
		{
			var childBounds = GetChildBoundsInternal(true);

			if (childBounds == null)
				return;

			var transformedBounds = childBounds.Value.TransformedLocation;

			LocalOffset = new Point(-transformedBounds.X, -transformedBounds.Y);

			Artboard?.UpdateScrollPanelOffset(LocalOffset.X, LocalOffset.Y);
		}

		protected override void UpdateScroll()
		{
			base.UpdateScroll();

			UpdateOffset();
		}

	}
}