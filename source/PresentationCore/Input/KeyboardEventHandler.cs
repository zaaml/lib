// <copyright file="KeyboardEventHandler.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Input;

namespace Zaaml.PresentationCore.Input
{
	internal class KeyboardEventProcessor
	{
		protected virtual void OnKeyDown(KeyEventArgs e)
		{
		}

		internal void OnKeyDownInternal(KeyEventArgs e)
		{
			OnKeyDown(e);
		}

		protected virtual void OnKeyUp(KeyEventArgs e)
		{
		}

		internal void OnKeyUpInternal(KeyEventArgs e)
		{
			OnKeyUp(e);
		}

		protected virtual void OnPreviewKeyDown(KeyEventArgs e)
		{
		}

		internal void OnPreviewKeyDownInternal(KeyEventArgs e)
		{
			OnPreviewKeyDown(e);
		}

		protected virtual void OnPreviewKeyUp(KeyEventArgs e)
		{
		}

		internal void OnPreviewKeyUpInternal(KeyEventArgs e)
		{
			OnPreviewKeyUp(e);
		}
	}
}