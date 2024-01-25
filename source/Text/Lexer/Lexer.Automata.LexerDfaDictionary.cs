// <copyright file="Automata.DfaTransition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Runtime.CompilerServices;

namespace Zaaml.Text
{
	internal partial class Lexer<TGrammar, TToken>
	{
		private protected partial class LexerAutomata
		{
			public sealed class LexerDfaDictionary<TValue>
			{
				private int[] _buckets;
				private Entry[] _entries;

				public LexerDfaDictionary(int capacity = 16)
				{
					_buckets = new int[capacity];
					_entries = new Entry[capacity];
				}

				private int Count { get; set; }

				private ref TValue this[int key]
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get
					{
						var entries = _entries;
						var entryIndex = _buckets[key % _buckets.Length] - 1;

						while (entryIndex != -1)
						{
							if (entries[entryIndex].Key == key)
								return ref entries[entryIndex].Value;

							entryIndex = entries[entryIndex].Next;
						}

						if (Count == entries.Length)
							entries = Resize();

						entryIndex = Count++;
						entries[entryIndex].Key = key;

						var bucket = key % _buckets.Length;

						entries[entryIndex].Next = _buckets[bucket] - 1;
						_buckets[bucket] = entryIndex + 1;

						return ref entries[entryIndex].Value;
					}
				}

				public void Add(int key, TValue value)
				{
					this[key] = value;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				private Entry[] Resize()
				{
					var count = Count;
					var entries = new Entry[count * 2];

					Array.Copy(_entries, 0, entries, 0, count);

					_entries = entries;

					var newBuckets = new int[count * 2];

					_buckets = newBuckets;

					for (var i = 0; i < count;)
					{
						var bucketIndex = entries[i].Key % _buckets.Length;

						entries[i].Next = newBuckets[bucketIndex] - 1;
						newBuckets[bucketIndex] = ++i;
					}

					return entries;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public bool TryGetValue(int key, out TValue value)
				{
					var entries = _entries;
					var entryIndex = _buckets[key % _buckets.Length] - 1;

					while (entryIndex != -1)
					{
						if (entries[entryIndex].Key == key)
						{
							value = entries[entryIndex].Value;

							return true;
						}

						entryIndex = entries[entryIndex].Next;
					}

					value = default;

					return false;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public TValue GetValueOrDefault(int key)
				{
					var entries = _entries;
					var entryIndex = _buckets[key % _buckets.Length] - 1;

					while (entryIndex != -1)
					{
						if (entries[entryIndex].Key == key)
							return entries[entryIndex].Value;

						entryIndex = entries[entryIndex].Next;
					}

					return default;
				}

				private struct Entry
				{
					public int Key;
					public TValue Value;
					public int Next;
				}
			}
		}
	}
}