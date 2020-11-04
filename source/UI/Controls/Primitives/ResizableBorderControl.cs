// <copyright file="ResizableBorderControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Media;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Primitives
{
	public sealed class ResizableBorderControl : FixedTemplateControl<Panel>
	{
		private readonly CombinedGeometry _clipGeometry;
		private readonly RectangleGeometry _innerGeometry;
		private readonly RectangleGeometry _outerGeometry;

		public ResizableBorderControl()
		{
			_outerGeometry = new RectangleGeometry();
			_innerGeometry = new RectangleGeometry();
			_clipGeometry = new CombinedGeometry(GeometryCombineMode.Exclude, _outerGeometry, _innerGeometry);
		}

		protected override void ApplyTemplateOverride()
		{
			base.ApplyTemplateOverride();

			TemplateRoot.BindProperties(System.Windows.Controls.Panel.BackgroundProperty, this, BorderBrushProperty);
			TemplateRoot.Clip = _clipGeometry;
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			var size = base.ArrangeOverride(arrangeBounds);

			_outerGeometry.Rect = new Rect(size);
			_innerGeometry.Rect = new Rect(size).GetInflated(BorderThickness.Negate());

			return size;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == BorderThicknessProperty)
				InvalidateArrange();
		}

		protected override void UndoTemplateOverride()
		{
			TemplateRoot.Clip = null;
			TemplateRoot.ClearValue(System.Windows.Controls.Panel.BackgroundProperty);

			base.UndoTemplateOverride();
		}
	}
}