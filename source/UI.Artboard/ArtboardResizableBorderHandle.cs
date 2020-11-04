// <copyright file="ArtboardResizableBorderHandle.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.PresentationCore.Behaviors.Resizable;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;

namespace Zaaml.UI.Controls.Artboard
{
	internal sealed class ArtboardResizableBorderHandle : ArtboardResizableElementHandleBase
	{
		public static readonly DependencyProperty BorderThicknessProperty = DPM.Register<Thickness, ArtboardResizableBorderHandle>
			("BorderThickness");

		private ResizableHandleKind _handleKind;

		public ArtboardResizableBorderHandle(ArtboardAdorner adorner) : base(adorner)
		{
		}

		public Thickness BorderThickness
		{
			get => (Thickness) GetValue(BorderThicknessProperty);
			set => SetValue(BorderThicknessProperty, value);
		}

		protected override ResizableHandleKind HandleKindCore
		{
			get => _handleKind;
			set
			{
				if (_handleKind == value)
					return;

				_handleKind = value;

				ResizableBehavior.UpdateCursor(Element, _handleKind);
			}
		}

		private Thickness GetActualResizeBorderThickness()
		{
			return this.GetValueSource(BorderThicknessProperty) != PropertyValueSource.Default ? BorderThickness : (Element as Control)?.BorderThickness ?? new Thickness(0);
		}

		private ResizableHandleKind GetResizableHandleKind(FrameworkElement border, Point position)
		{
			return ResizableBehaviorUtils.GetResizableHandleKind(border, position, GetActualResizeBorderThickness());
		}

		private protected override void OnElementChanged(FrameworkElement oldValue, FrameworkElement newValue)
		{
			base.OnElementChanged(oldValue, newValue);

			if (oldValue != null)
				oldValue.MouseMove -= OnElementMouseMove;

			if (newValue != null)
				newValue.MouseMove += OnElementMouseMove;
		}

		private void OnElementMouseMove(object sender, MouseEventArgs e)
		{
			if (IsDragging == false)
				HandleKindCore = CanResize ? GetResizableHandleKind(Element, e.GetPosition(Element)) : ResizableHandleKind.Undefined;
		}
	}
}