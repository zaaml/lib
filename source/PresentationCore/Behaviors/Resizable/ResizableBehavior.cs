// <copyright file="ResizableBehavior.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Behaviors.Resizable
{
	internal class ResizableBehavior : BehaviorBase
	{
		public static readonly DependencyProperty AdvisorProperty = DPM.RegisterAttached<IResizableAdvisor, ResizableBehavior>
			("Advisor");

		public static readonly DependencyProperty HandleProperty = DPM.Register<IResizableHandle, ResizableBehavior>
			("Handle", h => h.OnHandlePropertyChangedPrivate);

		private readonly ResizableBorderHandle _defaultHandle = new ResizableBorderHandle {BorderThickness = new Thickness(6)};
		private IResizableHandle _actualHandle;

		private Rect _initialRect;
		private double _maxDx;
		private double _maxDy;
		private double _minDx;
		private double _minDy;

		internal event EventHandler ResizeStarted;
		internal event EventHandler ResizeEnded;
		internal event EventHandler Resize;

		public ResizableBehavior()
		{
			ActualHandle = _defaultHandle;
		}

		private protected virtual FrameworkElement ActualElement => FrameworkElement;

		protected IResizableHandle ActualHandle
		{
			get => _actualHandle;
			private set
			{
				if (ReferenceEquals(_actualHandle, value))
					return;

				if (_actualHandle != null)
					DetachHandle(_actualHandle);

				_actualHandle = value;

				if (_actualHandle != null)
					AttachHandle(_actualHandle);

				UpdateDefaultHandle(ActualElement);
			}
		}

		public IResizableAdvisor Advisor
		{
			get => (IResizableAdvisor) GetValue(AdvisorProperty);
			set => SetValue(AdvisorProperty, value);
		}

		private IResizableAdvisor CurrentAdvisor { get; set; }

		public IResizableHandle Handle
		{
			get => (IResizableHandle) GetValue(HandleProperty);
			set => SetValue(HandleProperty, value);
		}

		private static bool IsResizing { get; set; }

		public virtual ResizeInfo ResizeInfo => new ResizeInfo(ActualHandle.OriginLocation, ActualHandle.CurrentLocation, ActualHandle.HandleKind);

		private void AttachHandle(IResizableHandle handle)
		{
			if (handle.Behavior != null)
				throw new InvalidOperationException("Handle is already attached");

			handle.Behavior = this;
		}

		private void CalcConstraints(Rect initialRect, FrameworkElement actualTarget, ResizableHandleKind resizableHandleKind)
		{
			var minWidth = actualTarget.MinWidth;
			var maxWidth = actualTarget.MaxWidth;
			var minHeight = actualTarget.MinHeight;
			var maxHeight = actualTarget.MaxHeight;

			// Calculate min/max drag delta constraints
			_maxDx = (resizableHandleKind & ResizableHandleKind.Left) != 0 ? initialRect.Width - minWidth : maxWidth - initialRect.Width;
			_minDx = (resizableHandleKind & ResizableHandleKind.Left) != 0 ? initialRect.Width - maxWidth : minWidth - initialRect.Width;
			_maxDy = (resizableHandleKind & ResizableHandleKind.Top) != 0 ? initialRect.Height - minHeight : maxHeight - initialRect.Height;
			_minDy = (resizableHandleKind & ResizableHandleKind.Top) != 0 ? initialRect.Height - maxHeight : minHeight - initialRect.Height;
		}

		private void DetachHandle(IResizableHandle handle)
		{
			if (ReferenceEquals(handle.Behavior, this))
				handle.Behavior = null;
		}

		protected virtual IResizableAdvisor GetActualAdvisor()
		{
			return Advisor ?? FindAdvisorFromElement(ActualElement);
		}

		public static IResizableAdvisor GetAdvisor(DependencyObject element)
		{
			return (IResizableAdvisor)element.GetValue(AdvisorProperty);
		}

		protected IResizableAdvisor FindAdvisorFromElement(FrameworkElement element)
		{
			return element.GetAncestorsAndSelf(MixedTreeEnumerationStrategy.VisualThenLogicalInstance).Select(GetElementAdvisor).FirstOrDefault(a => a != null);
		}

		private IResizableAdvisor GetElementAdvisor(DependencyObject dependencyObject)
		{
			if (dependencyObject is IResizableAdvisorProvider advisorProvider)
				return advisorProvider.GetAdvisor(ActualElement);

			return GetAdvisor(dependencyObject);
		}

		private void GetInitialRect()
		{
			_initialRect = CurrentAdvisor.GetBoundingBox(ActualElement);

			if (_initialRect.Width.IsNaN())
				_initialRect.Width = ActualElement.ActualWidth;

			if (_initialRect.Height.IsNaN())
				_initialRect.Width = ActualElement.ActualHeight;
		}

		protected override void OnAttached()
		{
			base.OnAttached();

			UpdateDefaultHandle(ActualElement);
		}

		protected override void OnDetaching()
		{
			UpdateDefaultHandle(null);

			base.OnDetaching();
		}

		internal void OnHandleDragEnded()
		{
			if (IsResizing == false)
				return;

			OnResizeEnded();

			IsResizing = false;
			CurrentAdvisor = null;
		}

		internal void OnHandleDragMove()
		{
			if (CurrentAdvisor == null)
				return;

			UpdatePosition();
			OnResize();
		}

		internal void OnHandleDragStarted()
		{
			CurrentAdvisor = GetActualAdvisor();

			if (CurrentAdvisor == null)
				return;

			var resizableHandleKind = ActualHandle.HandleKind;

			IsResizing = true;

			GetInitialRect();

			CalcConstraints(_initialRect, ActualElement, resizableHandleKind);

			OnResizeStarted();
		}

		private void OnHandlePropertyChangedPrivate(IResizableHandle oldValue, IResizableHandle newValue)
		{
			ActualHandle = newValue ?? _defaultHandle;
		}

		protected virtual void OnResize()
		{
			Resize?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void OnResizeEnded()
		{
			ResizeEnded?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void OnResizeStarted()
		{
			ResizeStarted?.Invoke(this, EventArgs.Empty);
		}

		public static void SetAdvisor(DependencyObject element, IResizableAdvisor value)
		{
			element.SetValue(AdvisorProperty, value);
		}

		internal static void UpdateCursor(FrameworkElement element, ResizableHandleKind part)
		{
			switch (part)
			{
				case ResizableHandleKind.TopLeft:
				case ResizableHandleKind.BottomRight:
					element.Cursor = Cursors.SizeNWSE;
					break;
				case ResizableHandleKind.TopRight:
				case ResizableHandleKind.BottomLeft:
					element.Cursor = Cursors.SizeNESW;
					break;
				case ResizableHandleKind.Top:
				case ResizableHandleKind.Bottom:
					element.Cursor = Cursors.SizeNS;
					break;
				case ResizableHandleKind.Left:
				case ResizableHandleKind.Right:
					element.Cursor = Cursors.SizeWE;
					break;
				default:
					element.Cursor = Cursors.Arrow;
					break;
			}
		}

		private void UpdateDefaultHandle(FrameworkElement element)
		{
			_defaultHandle.Element = ReferenceEquals(ActualHandle, _defaultHandle) ? element : null;
		}

		private protected void UpdatePosition()
		{
			if (IsResizing == false || CurrentAdvisor == null)
				return;

			var resizeInfo = ResizeInfo;
			var currentLocation = resizeInfo.CurrentLocation;
			var originLocation = resizeInfo.OriginLocation;
			var resizableHandleKind = ActualHandle.HandleKind;

			var dx = (resizableHandleKind & ~(ResizableHandleKind.Top | ResizableHandleKind.Bottom)) == 0 ? 0 : currentLocation.X - originLocation.X;
			var dy = (resizableHandleKind & ~(ResizableHandleKind.Left | ResizableHandleKind.Right)) == 0 ? 0 : currentLocation.Y - originLocation.Y;

			// Handle min/max size constraints
			dx = Math.Min(dx, _maxDx);
			dx = Math.Max(dx, _minDx);
			dy = Math.Min(dy, _maxDy);
			dy = Math.Max(dy, _minDy);

			var left = (resizableHandleKind & ResizableHandleKind.Left) != 0 ? dx : 0;
			var right = (resizableHandleKind & ResizableHandleKind.Right) != 0 ? dx : 0;
			var top = (resizableHandleKind & ResizableHandleKind.Top) != 0 ? dy : 0;
			var bottom = (resizableHandleKind & ResizableHandleKind.Bottom) != 0 ? dy : 0;

			var targetRect = new Rect(new Point(_initialRect.Left + left, _initialRect.Top + top), new Point(_initialRect.Right + right, _initialRect.Bottom + bottom));

			SetPosition(targetRect);
		}

		protected virtual void SetPosition(Rect rect)
		{
			CurrentAdvisor.SetBoundingBox(ActualElement, rect);
		}
	}
}