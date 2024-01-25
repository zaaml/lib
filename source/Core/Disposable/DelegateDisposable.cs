// <copyright file="DelegateDisposable.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Threading;

namespace Zaaml.Core.Disposable
{
	public sealed class DelegateDisposable : IDisposable
	{
		private Action _dispose;

		public DelegateDisposable(Action dispose)
		{
			_dispose = dispose;
		}

		public DelegateDisposable(Action init, Action dispose)
		{
			init();

			_dispose = dispose;
		}

		public static IDisposable DummyDisposable
		{
			get { return new DelegateDisposable(() => { }); }
		}

		public bool IsDisposed => _dispose == null;

		public static IDisposable Create(Action dispose)
		{
			return new DelegateDisposable(dispose);
		}

		public static IDisposable Create(Action init, Action dispose)
		{
			return new DelegateDisposable(init, dispose);
		}

		public void Dispose()
		{
			Interlocked.Exchange(ref _dispose, null)?.Invoke();
		}
	}
}