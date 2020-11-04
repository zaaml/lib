// <copyright file="PresentationWindowService.WPF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Zaaml.Core;
using Zaaml.PresentationCore.Behaviors.Draggable;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Windows
{
	internal class PresentationWindowService : DependencyObject, IDisposable
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty IsDraggableProperty = DPM.Register<bool, PresentationWindowService>
			("IsDraggable", true, p => p.OnIsDraggableChanged);

		public static readonly DependencyProperty IsResizableProperty = DPM.Register<bool, PresentationWindowService>
			("IsResizable", true, p => p.OnIsResizableChanged);

		#endregion

		#region Fields

		private WindowPresenter _windowPresenterControl;

		#endregion

		#region Ctors

		public PresentationWindowService(WindowBase window)
		{
			Window = window;
			Window.WindowStyle = WindowStyle.None;

			try
			{
				Window.AllowsTransparency = false;
			}
			catch
			{
				// ignored
			}

			Window.BorderThickness = new Thickness(0);

			WindowChromeBehavior = new WindowChromeBehavior
			{
				CornerRadius = new CornerRadius(0)
			};

			Window.AddBehavior(WindowChromeBehavior);

			this.BindProperties(IsDraggableProperty, Window, WindowBase.IsDraggableProperty);
			this.BindProperties(IsResizableProperty, Window, WindowBase.IsResizableProperty);

			WindowChromeBehavior.IsDraggable = IsDraggable;
			WindowChromeBehavior.IsResizable = IsResizable;
		}

		#endregion

		#region Properties

		public bool IsDraggable
		{
			get => (bool) GetValue(IsDraggableProperty);
			set => SetValue(IsDraggableProperty, value);
		}

		public bool IsResizable
		{
			get => (bool) GetValue(IsResizableProperty);
			set => SetValue(IsResizableProperty, value);
		}

		[UsedImplicitly]
		private WindowBase Window { get; }

		public WindowChromeBehavior WindowChromeBehavior { get; }

		public WindowPresenter WindowPresenterControl
		{
			get => _windowPresenterControl;
			set
			{
				if (ReferenceEquals(_windowPresenterControl, value))
					return;

				if (_windowPresenterControl != null)
					_windowPresenterControl.MouseLeftButtonDown -= WindowPresenterControlOnMouseLeftButtonDown;

				_windowPresenterControl = value;

				if (_windowPresenterControl != null)
					_windowPresenterControl.MouseLeftButtonDown += WindowPresenterControlOnMouseLeftButtonDown;

				WindowChromeBehavior.WindowPresenterControl = value;
			}
		}

		#endregion

		#region  Methods

		internal void BeginDragMove(bool async)
		{
			if (Window.IsDraggable == false)
				return;

			WindowChromeBehavior.BeginDragMove(async);
		}

		public void OnClosed()
		{
			Dispose();
		}

		internal void OnClosing()
		{
			WindowChromeBehavior.GlassFrameThickness = new Thickness(0, 0, 0, 0);

			if (_windowPresenterControl == null)
				return;

			var actualWidth = (int) _windowPresenterControl.ActualWidth;
			var actualHeight = (int) _windowPresenterControl.ActualHeight;

			if (actualHeight <= 0 || actualWidth <= 0)
				return;

			var bmp = new RenderTargetBitmap(actualWidth, actualHeight, 96, 96, PixelFormats.Pbgra32);

			bmp.Render(_windowPresenterControl);

			WindowChromeBehavior.RenderBitmap = bmp;
		}

		private void OnIsDraggableChanged()
		{
			WindowChromeBehavior.IsDraggable = IsDraggable;
		}

		private void OnIsResizableChanged()
		{
			WindowChromeBehavior.IsResizable = IsResizable;
		}

		private void WindowPresenterControlOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.Handled)
				return;

			var header = _windowPresenterControl.HeaderPresenter;

			if (header != null && header.IsVisualAncestorOf((DependencyObject) e.OriginalSource) && DraggableBehavior.CanStartDragging(Window, e))
				BeginDragMove(false);
		}

		#endregion

		#region Interface Implementations

		#region IDisposable

		public void Dispose()
		{
			Window.RemoveBehavior(WindowChromeBehavior);
		}

		#endregion

		#endregion
	}
}