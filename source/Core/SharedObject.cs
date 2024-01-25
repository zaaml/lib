// <copyright file="SharedObject.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Runtime.CompilerServices;
using Zaaml.Core.Pools;

namespace Zaaml.Core
{
	internal abstract class SharedObject : IDisposable
	{
		#region Properties

		private ReferenceCounter _referenceCount;

		#endregion

		#region Interface Implementations

		#region IDisposable

		public void Dispose()
		{
			ReleaseReference();
		}

		#endregion

		#endregion

		#region Methods

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddReference()
		{
			if (_referenceCount.AddReference() == 1)
				OnMount();
		}

		protected virtual void OnReleased()
		{
		}

		protected virtual void OnMount()
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReleaseReference()
		{
			if (_referenceCount.ReleaseReference() == 0)
				OnReleased();
		}

		#endregion
	}

	internal abstract class PoolSharedObject<T> : SharedObject where T : PoolSharedObject<T>
	{
		#region Ctors

		protected PoolSharedObject(IPool<T> pool)
		{
			Pool = pool ?? DummyPool<T>.Instance;
		}

		#endregion

		#region Properties

		public IPool<T> Pool { get; }

		#endregion

		#region Methods

		protected override void OnReleased()
		{
			Pool.Return((T)this);
		}

		#endregion
	}
}