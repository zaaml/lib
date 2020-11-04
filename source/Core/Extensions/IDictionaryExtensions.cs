// <copyright file="IDictionaryExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Zaaml.Core.Extensions
{
// ReSharper disable once InconsistentNaming
	internal static class IDictionaryExtensions
  {
    #region Methods

		[DebuggerStepThrough]
    public static TValue GetValueOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key,
      Func<TValue> factoryMethod)
    {
	    return dict.TryGetValue(key, out var value) ? value : dict[key] = factoryMethod();
    }

		[DebuggerStepThrough]
    public static TValue GetValueOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key,
      Func<TKey, TValue> factoryMethod)
    {
	    return dict.TryGetValue(key, out var value) ? value : dict[key] = factoryMethod(key);
    }

		[DebuggerStepThrough]
		public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default)
		{
			return dict.TryGetValue(key, out var value) ? value : defaultValue;
		}

		[DebuggerStepThrough]
    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> valueFactory)
    {
	    return dict.TryGetValue(key, out var value) ? value : valueFactory();
    }

    [DebuggerStepThrough]
    public static TValue GetValueOrCreateOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, bool create, Func<TValue> factoryMethod)
    {
      return create ? GetValueOrCreate(dict, key, factoryMethod) : GetValueOrDefault(dict, key);
    }

    [DebuggerStepThrough]
    public static TValue GetValueOrCreateOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, bool create, Func<TKey, TValue> factoryMethod)
    {
      return create ? GetValueOrCreate(dict, key, factoryMethod) : GetValueOrDefault(dict, key);
    }

    [DebuggerStepThrough]
    public static TValue GetValueOrCreateOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, bool create, Func<TValue> factoryMethod, TValue defaultValue)
    {
      return create ? GetValueOrCreate(dict, key, factoryMethod) : GetValueOrDefault(dict, key, defaultValue);
    }

    [DebuggerStepThrough]
    public static TValue GetValueOrCreateOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, bool create, Func<TKey, TValue> factoryMethod, TValue defaultValue)
    {
      return create ? GetValueOrCreate(dict, key, factoryMethod) : GetValueOrDefault(dict, key, defaultValue);
    }

    #endregion
  }
}