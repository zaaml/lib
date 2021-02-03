// <copyright file="DraggableBehavior.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Behaviors.Draggable
{
	internal partial class DraggableBehavior : BehaviorBase
	{
		public static readonly DependencyProperty AdvisorProperty = DPM.RegisterAttached<IDraggableAdvisor, DraggableBehavior>
			("Advisor");

		public static readonly DependencyProperty HandleProperty = DPM.Register<IDraggableHandle, DraggableBehavior>
			("Handle", d => d.OnHandlePropertyChangedPrivate);

		private readonly DraggableElementHandle _defaultHandle = new DraggableElementHandle();
		private IDraggableHandle _actualHandle;

		internal event EventHandler DragStarted;
		internal event EventHandler DragEnded;
		internal event EventHandler DragMove;

		public DraggableBehavior()
		{
			ActualHandle = _defaultHandle;
		}

		protected virtual FrameworkElement ActualElement => FrameworkElement;

		protected IDraggableHandle ActualHandle
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

		public IDraggableAdvisor Advisor
		{
			get => (IDraggableAdvisor) GetValue(AdvisorProperty);
			set => SetValue(AdvisorProperty, value);
		}

		private IDraggableAdvisor CurrentAdvisor { get; set; }

		public Vector DragDelta { get; set; }

		public virtual DragInfo DragInfo => new DragInfo(ActualHandle.OriginLocation, ActualHandle.CurrentLocation);

		private Point ElementOrigin { get; set; }

		public IDraggableHandle Handle
		{
			get => (IDraggableHandle) GetValue(HandleProperty);
			set => SetValue(HandleProperty, value);
		}

		private static bool IsDragging { get; set; }

		private protected virtual void AttachHandle(IDraggableHandle handle)
		{
			if (handle.Behavior != null)
				throw new InvalidOperationException("Handle is already attached");

			handle.Behavior = this;
		}

		internal static bool CanStartDragging(UIElement uie, MouseButtonEventArgs mouseButtonEventArgs)
		{
			return true;
		}

		private protected virtual void DetachHandle(IDraggableHandle handle)
		{
			if (ReferenceEquals(handle.Behavior, this))
				handle.Behavior = null;
		}

		protected virtual IDraggableAdvisor GetActualAdvisor()
		{
			return Advisor ?? FindAdvisorFromElement(ActualElement);
		}

		public static IDraggableAdvisor GetAdvisor(DependencyObject element)
		{
			return (IDraggableAdvisor) element.GetValue(AdvisorProperty);
		}

		protected IDraggableAdvisor FindAdvisorFromElement(FrameworkElement element)
		{
			return element.GetAncestorsAndSelf(MixedTreeEnumerationStrategy.VisualThenLogicalInstance).Select(GetElementAdvisor).FirstOrDefault(a => a != null);
		}

		private IDraggableAdvisor GetElementAdvisor(DependencyObject dependencyObject)
		{
			if (dependencyObject is IDraggableAdvisorProvider advisorProvider)
				return advisorProvider.GetAdvisor(ActualElement);

			return GetAdvisor(dependencyObject);
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

		protected virtual void OnDragEnded()
		{
			RaiseDragEnded();
		}

		protected virtual void OnDragMove()
		{
			RaiseDragMove();
		}

		protected virtual void OnDragStarted()
		{
			RaiseDragStart();
		}

		internal void OnHandleDragEnded()
		{
			if (IsDragging == false)
				return;

			RaiseDragEnding();

			CurrentAdvisor.OnDragEnd(ActualElement, this);

			OnDragEnded();

			IsDragging = false;
			CurrentAdvisor = null;
		}

		internal void OnHandleDragMove()
		{
			if (CurrentAdvisor == null)
				return;

			UpdatePosition();

			CurrentAdvisor.OnDragMove(ActualElement, this);

			OnDragMove();
		}

		internal void OnHandleDragStarted()
		{
			CurrentAdvisor = GetActualAdvisor();

			if (CurrentAdvisor == null)
				return;

			IsDragging = true;

			CurrentAdvisor.OnDragStart(ActualElement, this);

			ElementOrigin = CurrentAdvisor.GetPosition(ActualElement);

			OnDragStarted();
		}

		private void OnHandlePropertyChangedPrivate(IDraggableHandle oldValue, IDraggableHandle newValue)
		{
			ActualHandle = newValue ?? _defaultHandle;
		}

		private void RaiseDragEnded()
		{
			DragEnded?.Invoke(this, EventArgs.Empty);

			RaiseRoutedDragEnd();

			GlobalDragEnded?.Invoke(this, EventArgs.Empty);
		}

		private void RaiseDragEnding()
		{
			GlobalDragEnding?.Invoke(this, EventArgs.Empty);
		}

		private void RaiseDragMove()
		{
			DragMove?.Invoke(this, EventArgs.Empty);

			RaiseRoutedDragMove();

			GlobalDragMove?.Invoke(this, EventArgs.Empty);
		}

		private void RaiseDragStart()
		{
			DragStarted?.Invoke(this, EventArgs.Empty);

			RaiseRoutedDragStart();

			GlobalDragStarted?.Invoke(this, EventArgs.Empty);
		}

		public static void SetAdvisor(DependencyObject element, IDraggableAdvisor value)
		{
			element.SetValue(AdvisorProperty, value);
		}

		public void StopDrag()
		{
			ActualHandle.StopDrag();
		}

		private void UpdateDefaultHandle(FrameworkElement element)
		{
			_defaultHandle.Element = ReferenceEquals(ActualHandle, _defaultHandle) ? element : null;
		}

		protected void UpdatePosition()
		{
			if (IsDragging == false || CurrentAdvisor == null)
				return;

			var dragInfo = DragInfo;

			DragDelta = PointUtils.SubtractPoints(dragInfo.CurrentLocation, dragInfo.OriginLocation);

			CurrentAdvisor.SetPosition(ActualElement, PointUtils.AddVector(ElementOrigin, DragDelta));
		}

		internal static event EventHandler GlobalDragEnded;
		internal static event EventHandler GlobalDragEnding;
		internal static event EventHandler GlobalDragMove;
		internal static event EventHandler GlobalDragStarted;
	}
}