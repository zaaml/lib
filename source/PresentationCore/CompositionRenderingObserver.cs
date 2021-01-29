// <copyright file="CompositionRenderingObserver.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Media;
using Zaaml.Core.Extensions;
using Zaaml.Core.Weak;

namespace Zaaml.PresentationCore
{
	internal sealed class CompositionRenderingObserver : IDisposable
	{
		private IDisposable _renderingDisposer;
		private WeakAction _weakOnRendering;

		public CompositionRenderingObserver(Action onRendering)
		{
			_weakOnRendering = new WeakAction(onRendering);
			_renderingDisposer = _renderingDisposer.DisposeExchange(this.CreateWeakEventListener((t, _, _) => t.OnRendering(), h => CompositionTarget.Rendering += h, h => CompositionTarget.Rendering -= h));
		}

		private void OnRendering()
		{
			_weakOnRendering?.Invoke();
		}

		public void Dispose()
		{
			_renderingDisposer = _renderingDisposer.DisposeExchange();
			_weakOnRendering = null;
		}
	}
}