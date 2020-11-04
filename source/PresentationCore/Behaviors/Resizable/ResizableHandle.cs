// <copyright file="ResizableHandle.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Behaviors.Resizable
{
	internal class ResizableHandle : ResizableElementHandleBase
	{
		public static readonly DependencyProperty HandleKindProperty = DPM.Register<ResizableHandleKind, ResizableHandle>
			("HandleKind", ResizableHandleKind.Undefined, h => h.OnHandleKindChanged);

		public ResizableHandleKind HandleKind
		{
			get => (ResizableHandleKind) GetValue(HandleKindProperty);
			set => SetValue(HandleKindProperty, value);
		}

		protected override ResizableHandleKind HandleKindCore
		{
			get => HandleKind;
			set { }
		}

		private void OnHandleKindChanged()
		{
			UpdateCursor();
		}
	}
}