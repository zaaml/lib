// <copyright file="DebugId.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Core
{
	internal struct DebugId<T>
	{
#if DEBUG
		private DebugId(int id, string name)
		{
			Id = id;
			Name = name;
		}

		public static DebugId<T> Create(string name = null)
		{
			var type = typeof(T);

			if (IdCounterDictionary.TryGetValue(type, out var id))
				IdCounterDictionary[type] = id + 1;
			else
				IdCounterDictionary[type] = 1;

			return new DebugId<T>(id, name ?? typeof(T).ToString());
		}

		private static readonly Dictionary<Type, int> IdCounterDictionary = new Dictionary<Type, int>();

		public int Id { get; }

		public string Name { get; }

		public override string ToString()
		{
			return $"{Name}:{Id}";
		}
#endif
	}

	internal class DebugCounter
	{
#if DEBUG
		private DebugCounter(string id)
		{
			Id = id;
		}

		public int Count { get; private set; }

		public static string Invoke(string id)
		{
			if (IdCounterDictionary.TryGetValue(id, out var counter) == false)
				IdCounterDictionary[id] = counter = new DebugCounter(id);

			counter.Count++;

			return $"{counter.Id}: {counter.Count}";
		}

		private static readonly Dictionary<string, DebugCounter> IdCounterDictionary = new Dictionary<string, DebugCounter>();

		public string Id { get; }
#endif
	}
}