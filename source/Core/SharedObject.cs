// <copyright file="SharedObject.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core.Pools;

namespace Zaaml.Core
{
	internal abstract class SharedObject<T> : IDisposable where T : SharedObject<T>
	{
		#region Properties

		private ReferenceCounter _referenceCount = new ReferenceCounter();

		#endregion

		#region Methods

		public T AddReference()
		{
			if (_referenceCount.AddReference() == 1) 
				OnMount();

			return (T) this;
		}

		protected virtual void OnReleased()
		{
		}

		protected virtual void OnMount()
		{
		}

		public void ReleaseReference()
		{
			if (_referenceCount.ReleaseReference() == 0) 
				OnReleased();
		}

		#endregion

		#region Interface Implementations

		#region IDisposable

		public void Dispose()
		{
			ReleaseReference();
		}

		#endregion

		#endregion
	}

	internal abstract class PoolSharedObject<T> : SharedObject<T> where T : PoolSharedObject<T>
	{
		#region Ctors

		protected PoolSharedObject(IPool<T> pool)
		{
			Pool = pool;
		}

		#endregion

		#region Properties

		public IPool<T> Pool { get; }

		#endregion

		#region Methods

		protected override void OnReleased()
		{
			base.OnReleased();

			Pool?.Release((T) this);
		}

		#endregion
	}
}