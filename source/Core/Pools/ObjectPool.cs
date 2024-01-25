// <copyright file="ObjectPool.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Zaaml.Core.Pools
{
	internal class ObjectPool<T> where T : class
	{
		private readonly Action<T> _cleaner;
		private readonly Func<T> _creator;
		private readonly Action<T> _initializer;
		private readonly List<WeakReference> _pool = new();
		private readonly Semaphore _semaphore;

		public ObjectPool(Func<T> creator)
			: this(creator, null, null, int.MaxValue)
		{
		}

		public ObjectPool(Func<T> creator, Action<T> initializer, Action<T> cleaner, int maxInstances)
		{
			_creator = creator;
			_initializer = initializer;
			_cleaner = cleaner;
			InstanceCount = 0;
			MaxInstances = maxInstances;
			_semaphore = new Semaphore(MaxInstances);
		}

		public int InstanceCount { get; private set; }

		public int MaxInstances { get; set; }

		public int Size
		{
			get
			{
				lock (_pool)
					return _pool.Count;
			}
		}

		private T Clean(T instance)
		{
			_cleaner?.Invoke(instance);

			return instance;
		}

		protected virtual T CreateObject()
		{
			var newObject = _creator();

			InstanceCount++;

			return newObject;
		}

		public T GetObject()
		{
			lock (_pool)
			{
				var thisObject = RemoveObject();

				if (thisObject != null)
					return Initialize(thisObject);

				return InstanceCount < MaxInstances ? Initialize(CreateObject()) : null;
			}
		}

		private T Initialize(T instance)
		{
			_initializer?.Invoke(instance);

			return instance;
		}

		public void Release(T obj)
		{
			if (obj == null)
				throw new NullReferenceException();

			lock (_pool)
			{
				var refThis = new WeakReference(Clean(obj));

				_pool.Add(refThis);
				_semaphore.Release();
			}
		}

		private T RemoveObject()
		{
			while (_pool.Count > 0)
			{
				var refThis = _pool.Last();

				_pool.RemoveAt(_pool.Count - 1);

				var thisObject = (T)refThis.Target;

				if (thisObject != null)
					return thisObject;

				InstanceCount--;
			}

			return null;
		}

		public T WaitForObject()
		{
			while (true)
			{
				lock (_pool)
				{
					var thisObject = RemoveObject();
					if (thisObject != null)
						return Initialize(thisObject);

					if (InstanceCount < MaxInstances)
						return Initialize(CreateObject());
				}

				_semaphore.WaitOne();
			}
		}

		private class Semaphore
		{
			public Semaphore(int max = 1)
			{
				Mutex = new object();
				Max = max;
			}

			private int Count { get; set; }
			private int Max { get; }
			private object Mutex { get; }

			public void Release()
			{
				lock (Mutex)
				{
					if (Count >= 0)
						Count--;
				}
			}

			public void WaitOne()
			{
				while (true)
				{
					lock (Mutex)
					{
						if (Count < Max)
						{
							Count++;
							return;
						}
					}

					Thread.Sleep(50);
				}
			}
		}
	}
}