// <copyright file="ConvertCacheStore.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore.Converters
{
	internal struct ConvertCacheStore
	{
		private object _value;

		public ConvertCacheStore(object value) : this()
		{
			_value = value;
		}

		public Type CachedType { get; set; }

		public object CachedValue { get; set; }

		public object Value
		{
			get => _value;
			set
			{
				_value = value;

				ResetCache();
			}
		}

		public void ResetCache()
		{
			CachedType = null;
			CachedValue = null;
		}
	}
}