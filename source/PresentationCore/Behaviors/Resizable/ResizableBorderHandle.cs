// <copyright file="ResizableBorderHandle.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Input;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Behaviors.Resizable
{
	internal class ResizableBorderHandle : ResizableElementHandleBase
	{
		public static readonly DependencyProperty BorderThicknessProperty = DPM.Register<Thickness, ResizableBorderHandle>
			("BorderThickness");

		private ResizableHandleKind _handleKind;

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

		private bool Filter(MouseEventArgs mouseButtonEventArgs)
		{
			return HandleKindCore != ResizableHandleKind.Undefined;
		}

		private ResizableHandleKind GetResizableHandleKind(FrameworkElement border, Point position)
		{
			return ResizableBehaviorUtils.GetResizableHandleKind(border, position, BorderThickness);
		}

		private protected override void OnElementMouseMove(object sender, MouseEventArgs e)
		{
			base.OnElementMouseMove(sender, e);

			if (DraggableMouse.IsDragging == false)
				HandleKindCore = GetResizableHandleKind(Element, e.GetPosition(Element));
		}
	}
}