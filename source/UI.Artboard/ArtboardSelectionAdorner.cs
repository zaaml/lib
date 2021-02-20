// <copyright file="ArtboardSelectionAdorner.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Primitives;

namespace Zaaml.UI.Controls.Artboard
{
	[TemplateContractType(typeof(ArtboardSelectionAdornerTemplateContract))]
	public class ArtboardSelectionAdorner : ArtboardAdorner
	{
		private static readonly DependencyPropertyKey IsDraggingPropertyKey = DPM.RegisterReadOnly<bool, ArtboardSelectionAdorner>
			("IsDragging");

		private static readonly DependencyPropertyKey IsResizingPropertyKey = DPM.RegisterReadOnly<bool, ArtboardSelectionAdorner>
			("IsResizing");

		public static readonly DependencyProperty IsResizingProperty = IsResizingPropertyKey.DependencyProperty;

		public static readonly DependencyProperty IsDraggingProperty = IsDraggingPropertyKey.DependencyProperty;

		private readonly ArtboardDraggableBehavior _draggableBehavior;
		private readonly ArtboardResizableBehavior _resizableBehavior;

		static ArtboardSelectionAdorner()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ArtboardSelectionAdorner>();
		}

		public ArtboardSelectionAdorner()
		{
			this.OverrideStyleKey<ArtboardSelectionAdorner>();

			_draggableBehavior = new ArtboardDraggableBehavior();
			_resizableBehavior = new ArtboardResizableBehavior();

			var behaviors = Extension.GetBehaviors(this);

			behaviors.Add(_draggableBehavior);
			behaviors.Add(_resizableBehavior);

			_draggableBehavior.DragStarting += OnDraggableBehaviorDragStarting;
			_draggableBehavior.DragStarted += OnDraggableBehaviorDragStarted;
			_draggableBehavior.DragEnded += OnDraggableBehaviorDragEnded;

			_resizableBehavior.ResizeStarting += OnResizableBehaviorResizeStarting;
			_resizableBehavior.ResizeStarted += OnResizableBehaviorResizeStarted;
			_resizableBehavior.ResizeEnded += OnResizableBehaviorResizeEnded;
		}

		protected virtual bool CanDragStart => true;

		protected virtual bool CanResizeStart => true;

		public bool IsDragging
		{
			get => (bool) GetValue(IsDraggingProperty);
			private set => this.SetReadOnlyValue(IsDraggingPropertyKey, value);
		}

		public bool IsResizing
		{
			get => (bool) GetValue(IsResizingProperty);
			private set => this.SetReadOnlyValue(IsResizingPropertyKey, value);
		}

		private ResizableBorderControl ResizableBorderHandle => TemplateContract.ResizableBorderHandle;

		private ArtboardSelectionAdornerTemplateContract TemplateContract => (ArtboardSelectionAdornerTemplateContract) TemplateContractInternal;

		protected override void AttachElement(FrameworkElement adornedElement)
		{
			base.AttachElement(adornedElement);

			adornedElement.MouseDown += OnAdornedElementMouseDown;
			adornedElement.MouseUp += OnAdornedElementMouseUp;
			adornedElement.PreviewMouseDown += OnAdornedElementPreviewMouseDown;
			adornedElement.PreviewMouseUp += OnAdornedElementPreviewMouseUp;

			_draggableBehavior.Handle = new ArtboardDraggableElementHandle(this)
			{
				Element = adornedElement
			};
		}

		protected override void DetachElement(FrameworkElement adornedElement)
		{
			_draggableBehavior.Handle = null;
			_resizableBehavior.Handle = null;

			adornedElement.MouseDown -= OnAdornedElementMouseDown;
			adornedElement.MouseUp -= OnAdornedElementMouseUp;
			adornedElement.PreviewMouseDown -= OnAdornedElementPreviewMouseDown;
			adornedElement.PreviewMouseUp -= OnAdornedElementPreviewMouseUp;

			base.DetachElement(adornedElement);
		}

		private void OnAdornedElementMouseDown(object sender, MouseButtonEventArgs e)
		{
		}

		private void OnAdornedElementMouseUp(object sender, MouseButtonEventArgs e)
		{
		}

		private void OnAdornedElementPreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
		}

		private void OnAdornedElementPreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
		}

		protected override void OnMatrixChanged()
		{
			base.OnMatrixChanged();

			_draggableBehavior.OnMatrixChanged();
			_resizableBehavior.OnMatrixChanged();
		}

		private void OnDraggableBehaviorDragEnded(object sender, EventArgs e)
		{
			IsDragging = false;
		}

		private void OnDraggableBehaviorDragStarted(object sender, EventArgs e)
		{
			IsDragging = true;
		}

		private void OnDraggableBehaviorDragStarting(object sender, CancelEventArgs e)
		{
			if (CanDragStart == false)
				e.Cancel = true;
		}

		private void OnResizableBehaviorResizeEnded(object sender, EventArgs e)
		{
			IsResizing = false;
		}

		private void OnResizableBehaviorResizeStarted(object sender, EventArgs e)
		{
			IsResizing = true;
		}

		private void OnResizableBehaviorResizeStarting(object sender, CancelEventArgs e)
		{
			if (CanResizeStart == false)
				e.Cancel = true;
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			_resizableBehavior.Handle = new ArtboardResizableBorderHandle(this)
			{
				Element = ResizableBorderHandle
			};
		}

		protected override void OnTemplateContractDetaching()
		{
			_resizableBehavior.Handle = null;

			base.OnTemplateContractDetaching();
		}
	}

	public class ArtboardSelectionAdornerTemplateContract : ArtboardAdornerTemplateContract
	{
		[TemplateContractPart]
		public ResizableBorderControl ResizableBorderHandle { get; [UsedImplicitly] private set; }
	}
}