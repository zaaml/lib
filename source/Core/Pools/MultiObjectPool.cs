// <copyright file="MultiObjectPool.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Zaaml.Core.Extensions;

namespace Zaaml.Core.Pools
{
	internal class MultiObjectPool<TKey, TObject> where TObject : class
	{
		private readonly Action<TKey, TObject> _cleanAction;
		private readonly Func<TKey, TObject> _factory;
		private readonly Action<TKey, TObject> _initAction;
		private readonly Dictionary<TKey, LightObjectPool<TObject>> _poolMap = new Dictionary<TKey, LightObjectPool<TObject>>();

		public MultiObjectPool(Func<TKey, TObject> factory) : this(factory, null, null)
		{
			_factory = factory;
		}

		public MultiObjectPool(Func<TKey, TObject> factory, Action<TKey, TObject> initAction, Action<TKey, TObject> cleanAction)
		{
			_factory = factory;
			_initAction = initAction ?? DummyAction<TKey, TObject>.Instance;
			_cleanAction = cleanAction ?? DummyAction<TKey, TObject>.Instance;
		}

		private LightObjectPool<TObject> CreatePool(TKey key)
		{
			return new LightObjectPool<TObject>(() => _factory(key), o => _initAction(key, o), o => _cleanAction(key, o));
		}

		public TObject GetObject(TKey key)
		{
			return GetPool(key).GetObject();
		}

		private LightObjectPool<TObject> GetPool(TKey key)
		{
			return _poolMap.GetValueOrCreate(key, CreatePool);
		}

		public void Release(TKey key, TObject obj)
		{
			GetPool(key).Release(obj);
		}
	}
}