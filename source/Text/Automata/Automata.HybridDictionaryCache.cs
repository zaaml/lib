// <copyright file="Automata.HybridDictionaryCache.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private sealed class HybridDictionary<T> : ICache<T> where T : class
		{
			private const int DefaultArrayLimit = 127;

			private readonly T[] _array;
			private readonly int _arrayLimit;
			private readonly Dictionary<int, T> _dictionary;

			public HybridDictionary() : this(DefaultArrayLimit)
			{
			}

			public HybridDictionary(int arrayLimit)
			{
				_arrayLimit = arrayLimit;
				_dictionary = new Dictionary<int, T>();
				_array = new T[_arrayLimit];
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool TryGetValue(int operand, out T result)
			{
				if (operand >= _arrayLimit)
					return _dictionary.TryGetValue(operand, out result);

				var value = _array[operand];

				result = value;

				return value != null;
			}

			public void SetValue(int operand, T value)
			{
				if (operand >= _arrayLimit)
					_dictionary[operand] = value;
				else
					_array[operand] = value;
			}
		}
	}
}