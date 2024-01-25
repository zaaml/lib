// <copyright file="FloatingDockWindow.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Windows;
using DispatcherPriority = System.Windows.Threading.DispatcherPriority;
using Panel = Zaaml.UI.Panels.Core.Panel;

namespace Zaaml.UI.Controls.Docking
{
	public class FloatingDockWindow : WindowBase
	{
		private static readonly DependencyPropertyKey DockItemPropertyKey = DPM.RegisterReadOnly<DockItem, FloatingDockWindow>
			("DockItem", w => w.OnDockItemChanged);

		public static readonly DependencyProperty DockItemProperty = DockItemPropertyKey.DependencyProperty;

		private readonly ContentPresenter _presenter;
		private readonly ContentPresenter _previewPresenter;
		private DockItemHeaderPresenter _dockItemHeaderPresenter;
		private DockItem _previewDockItem;
		private bool _suspendLocationSizeHandler;
		private readonly RenderDelayAction _delayShow;
		private readonly RenderDelayAction _delayHide;
		private readonly RenderDelayAction _delayDrag;

		internal event EventHandler DockItemChanged;

		static FloatingDockWindow()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<FloatingDockWindow>();

			WidthProperty.OverrideMetadata(typeof(FloatingDockWindow), new FrameworkPropertyMetadata(FloatLayout.DefaultWidth));
			HeightProperty.OverrideMetadata(typeof(FloatingDockWindow), new FrameworkPropertyMetadata(FloatLayout.DefaultHeight));
		}

		internal FloatingDockWindow(FloatingDockWindowController controller)
		{
			this.OverrideStyleKey<FloatingDockWindow>();

			Controller = controller;

			ShowInTaskbar = false;
			ShowActivated = false;
			AllowsTransparency = true;
			Opacity = 0.0;

			_presenter = new ContentPresenter();
			_previewPresenter = new ContentPresenter();

			var host = new Panel
			{
				Children =
				{
					_previewPresenter,
					_presenter
				}
			};

			Content = host;

			SizeChanged += OnSizeChanged;

			_delayDrag = new RenderDelayAction(HandleBeginDrag, 3);
			_delayShow = new RenderDelayAction(() =>
			{
				if (DockItem.IsDragMove)
				{
					Activate();
					Keyboard.Focus(this);

					_delayDrag.Invoke();
				}
				
				Opacity = 1.0;
			}, 1);

			_delayHide = new RenderDelayAction(() =>
			{
				Opacity = 0.0;

				_delayDrag.Revoke();
			}, 0);
		}

		internal FloatingDockWindowController Controller { get; }

		public DockItem DockItem
		{
			get => (DockItem) GetValue(DockItemProperty);
			private set => this.SetReadOnlyValue(DockItemPropertyKey, value);
		}

		public DockItemHeaderPresenter DockItemHeaderPresenter
		{
			get => _dockItemHeaderPresenter;
			set
			{
				if (ReferenceEquals(_dockItemHeaderPresenter, value))
					return;

				if (_dockItemHeaderPresenter != null)
					OnDockItemHeaderPresenterDetaching(_dockItemHeaderPresenter);

				_dockItemHeaderPresenter = value;

				if (_dockItemHeaderPresenter != null)
					OnDockItemHeaderPresenterAttached(_dockItemHeaderPresenter);
			}
		}

		internal DockItem PreviewDockItem
		{
			get => _previewDockItem;
			private set
			{
				if (ReferenceEquals(_previewDockItem, value))
					return;

				_previewDockItem = value;
				_previewPresenter.Content = value;
			}
		}

		internal void AttachContent()
		{
			_presenter.Content = DockItem;
			_previewPresenter.Content = PreviewDockItem;
		}

		internal void AttachItem(DockItem dockItem)
		{
			if (dockItem.IsPreview)
			{
				if (dockItem.PreviewFloatingWindow != null || PreviewDockItem != null)
					throw new InvalidOperationException();

				PreviewDockItem = dockItem;

				dockItem.PreviewFloatingWindow = this;
			}
			else
			{
				if (dockItem.FloatingWindow != null || DockItem != null)
					throw new InvalidOperationException();

				DockItem = dockItem;

				dockItem.FloatingWindow = this;

				UpdateLocationAndSize(dockItem);

				_presenter.Content = DockItem;
			}

			_delayHide.Revoke();
			_delayShow.Invoke();
		}

		private void HandleBeginDrag()
		{
			var dockItem = DockItem;

			if (dockItem == null)
				return;

			if (dockItem.IsDragMove == false) 
				return;

			dockItem.IsDragMove = false;

			if (IsContentRendered)
				Dispatcher.BeginInvoke(DispatcherPriority.Render, BeginDragMove);
		}

		internal void BeginDragMove()
		{
			if (DockItem?.EnqueueSyncDragPosition != true)
				return;

			Activate();
			SyncPosition();
			BeginDragMove(true);
		}

		internal void DetachContent()
		{
			_presenter.Content = null;
			_previewPresenter.Content = null;
		}

		internal void DetachItem(DockItem dockItem)
		{
			if (dockItem.IsPreview)
			{
				if (ReferenceEquals(dockItem.PreviewFloatingWindow, this) == false || ReferenceEquals(PreviewDockItem, dockItem) == false)
					throw new InvalidOperationException();

				PreviewDockItem = null;
				dockItem.PreviewFloatingWindow = null;
			}
			else
			{
				if (ReferenceEquals(dockItem.FloatingWindow, this) == false || ReferenceEquals(DockItem, dockItem) == false)
					throw new InvalidOperationException();

				DockItem = null;
				dockItem.FloatingWindow = null;

				_presenter.Content = null;
			}

			_delayShow.Revoke();
			_delayHide.Invoke();
		}

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);

			DockItem?.Select();
		}

		protected override void OnBeginDragMove()
		{
			SyncPosition();

			DockItem.OnBeginDragMoveInternal();
		}

		protected override void OnContentRendered(EventArgs e)
		{
			base.OnContentRendered(e);

			BeginDragMove();
		}

		private void OnDockItemChanged()
		{
			DockItemChanged?.Invoke(this, EventArgs.Empty);
		}

		private void OnDockItemHeaderPresenterAttached(DockItemHeaderPresenter dockItemHeaderPresenter)
		{
			dockItemHeaderPresenter.FloatingWindow = this;
		}

		private void OnDockItemHeaderPresenterDetaching(DockItemHeaderPresenter dockItemHeaderPresenter)
		{
			dockItemHeaderPresenter.FloatingWindow = null;
		}

		protected override void OnDragMove()
		{
			base.OnDragMove();

			DockItem.OnDragMoveInternal();
		}

		protected override void OnEndDragMove()
		{
			base.OnEndDragMove();

			if (DockItem.DockControl?.CurrentDropGuide == null)
				SyncItemFloatPosition();

			DockItem.OnEndDragMoveInternal();
		}

		protected override void OnEndResize()
		{
			base.OnEndResize();

			SyncItemFloatSize();
		}

		internal override void OnHeaderPresenterAttachedInternal(WindowHeaderPresenter headerPresenter)
		{
			base.OnHeaderPresenterAttachedInternal(headerPresenter);

			headerPresenter.TemplateContractAttached += OnHeaderPresenterTemplateContractAttached;
			headerPresenter.TemplateContractDetaching += OnHeaderPresenterTemplateContractDetaching;

			DockItemHeaderPresenter = (DockItemHeaderPresenter) HeaderPresenter.FindName("DockItemHeaderPresenter");
		}

		internal override void OnHeaderPresenterDetachingInternal(WindowHeaderPresenter headerPresenter)
		{
			headerPresenter.TemplateContractAttached -= OnHeaderPresenterTemplateContractAttached;
			headerPresenter.TemplateContractDetaching -= OnHeaderPresenterTemplateContractDetaching;

			DockItemHeaderPresenter = null;

			base.OnHeaderPresenterDetachingInternal(headerPresenter);
		}

		private void OnHeaderPresenterTemplateContractAttached(object sender, EventArgs e)
		{
			DockItemHeaderPresenter = (DockItemHeaderPresenter) HeaderPresenter.GetTemplateChildInternal("DockItemHeaderPresenter");
		}

		private void OnHeaderPresenterTemplateContractDetaching(object sender, EventArgs e)
		{
			DockItemHeaderPresenter = null;
		}

		protected override void OnLocationChanged(EventArgs e)
		{
			base.OnLocationChanged(e);

			SyncItemFloatPosition();
		}

		private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
		{
			SyncItemFloatSize();
		}

		private void SyncItemFloatPosition()
		{
			var dockItem = DockItem;

			if (_suspendLocationSizeHandler || dockItem == null || IsMoving || dockItem.EnqueueSyncDragPosition)
				return;

			_suspendLocationSizeHandler = true;

			FloatLayout.SetLeft(dockItem, Left);
			FloatLayout.SetTop(dockItem, Top);

			_suspendLocationSizeHandler = false;
		}

		private void SyncItemFloatSize()
		{
			var dockItem = DockItem;

			if (_suspendLocationSizeHandler || dockItem == null || IsResizing)
				return;

			_suspendLocationSizeHandler = true;

			FloatLayout.SetWidth(dockItem, Width);
			FloatLayout.SetHeight(dockItem, Height);

			_suspendLocationSizeHandler = false;
		}

		private void SyncPosition()
		{
			var mousePosition = MouseInternal.ScreenLogicalPosition;
			var rect = new Rect(new Point(Left, Top), new Size(Width, Height));

			if (DockItem != null)
			{
				if (DockItem.EnqueueSyncDragPosition)
				{
					DockItem.EnqueueSyncDragPosition = false;

					rect.Size = FloatLayout.GetSize(DockItem);
				}
				else
					rect = FloatLayout.GetRect(DockItem);

				var dragOrigin = DockItem.HeaderMousePosition;

				if (dragOrigin.HasValue)
				{
					rect.X = mousePosition.X - dragOrigin.Value.X;
					rect.Y = mousePosition.Y - dragOrigin.Value.Y;
				}
			}
			else
			{
				rect.X = mousePosition.X;
				rect.Y = mousePosition.Y;
			}

			UpdateLocationAndSize(rect);
		}

		private void UpdateLocationAndSize(Rect rect)
		{
			_suspendLocationSizeHandler = true;

			if (Left.IsCloseTo(rect.Left) == false)
				Left = rect.Left;

			if (Top.IsCloseTo(rect.Top) == false)
				Top = rect.Top;

			if (Width.IsCloseTo(rect.Width) == false)
				Width = rect.Width;

			if (Height.IsCloseTo(rect.Height) == false)
				Height = rect.Height;

			_suspendLocationSizeHandler = false;
		}

		internal void UpdateLocationAndSize(DockItem dockItem)
		{
			if (ReferenceEquals(DockItem, dockItem) == false)
				return;

			if (_suspendLocationSizeHandler)
				return;

			UpdateLocationAndSize(FloatLayout.GetRect(dockItem));
		}

		internal void ShowDockWindow()
		{
			Show();
		}

		internal void HideDockWindow()
		{
			Hide();
		}
	}
}