// <copyright file="GarbageCollectorObserver.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore
{
	internal sealed class GarbageCollectorObserver : IDisposable
	{
		private int _gcCount;
		private CompositionRenderingObserver _renderingObserver;
		public event EventHandler GarbageCollected;

		public GarbageCollectorObserver()
		{
			_renderingObserver = new CompositionRenderingObserver(Update);
		}

		private void Update()
		{
			var gcCount = GC.CollectionCount(0);
			var isAlive = _gcCount == gcCount;

			try
			{
				if (isAlive == false)
					GarbageCollected?.Invoke(this, EventArgs.Empty);
			}
			finally
			{
				_gcCount = gcCount;
			}
		}

		public void Dispose()
		{
			_renderingObserver = _renderingObserver.DisposeExchange();
		}
	}
}