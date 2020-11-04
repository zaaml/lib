// <copyright file="PixelSnapper.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Core
{
	public abstract class PixelSnapperBase : FixedTemplateControl<PixelSnapperPanel>
	{
		private readonly TranslateTransform _renderTransform = new TranslateTransform();

		protected override void ApplyTemplateOverride()
		{
			base.ApplyTemplateOverride();

			TemplateRoot.RenderTransform = _renderTransform;
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);

			UpdateRenderTransform();
		}

		protected override void UndoTemplateOverride()
		{
			TemplateRoot.RenderTransform = null;

			base.UndoTemplateOverride();
		}

		private void UpdateRenderTransform()
		{
			var rootBox = TransformToVisual((FrameworkElement) this.GetVisualRoot()).TransformBounds(this.GetClientBox());

			_renderTransform.X = rootBox.Left - Math.Floor(rootBox.Left);
			_renderTransform.Y = rootBox.Top - Math.Floor(rootBox.Top);
		}
	}

	[ContentProperty(nameof(Child))]
	public sealed class PixelSnapper : PixelSnapperBase, IFixedTemplateContentControl<PixelSnapperPanel>
	{
		public static readonly DependencyProperty ChildProperty = DPM.Register<FrameworkElement, PixelSnapper>
			("Child", default, d => d.OnChildPropertyChangedPrivate);

		public PixelSnapper()
		{
			Controller = new FixedTemplateContentControlController<PixelSnapperPanel, FrameworkElement>(this);
		}

		public FrameworkElement Child
		{
			get => (FrameworkElement) GetValue(ChildProperty);
			set => SetValue(ChildProperty, value);
		}

		private FixedTemplateContentControlController<PixelSnapperPanel, FrameworkElement> Controller { get; }

		protected override IEnumerator LogicalChildren => Controller.LogicalChildren;

		protected override void ApplyTemplateOverride()
		{
			base.ApplyTemplateOverride();

			Controller.AttachChild();
		}

		private void OnChildPropertyChangedPrivate()
		{
			Controller.Child = Child;
		}

		protected override void UndoTemplateOverride()
		{
			Controller.DetachChild();

			base.UndoTemplateOverride();
		}

		public bool IsLogicalParent => true;

		public PixelSnapperPanel Panel => TemplateRoot;
	}

	public sealed class PixelSnapperPanel : Panel
	{
		//private double _arrangeDelta = 0.001;

		//protected override Size ArrangeOverrideCore(Size finalSize)
		//{
		//	var size = new Size(Math.Floor(finalSize.Width), Math.Floor(finalSize.Height));
		//	var arrangeOverrideCore = base.ArrangeOverrideCore(size);

		//	arrangeOverrideCore.Width += _arrangeDelta;

		//	_arrangeDelta = _arrangeDelta.Equals(0.0001) ? 0.0002 : 0.0001;

		//	return arrangeOverrideCore;
		//}
	}
}