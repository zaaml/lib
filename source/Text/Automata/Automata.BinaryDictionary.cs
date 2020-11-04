// <copyright file="Automata.SharedObject.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private sealed class BinaryDictionary<T>
		{
			private readonly Dictionary<BinaryDictionaryKey, T> _dictionary = new Dictionary<BinaryDictionaryKey, T>();
		}

		private class BinaryDictionaryKey
		{
			//private int _count;
			//private TElement[] _elements;
			//private int _elementsHashCode;

			//public BinaryDictionaryKey()
			//{
			//}

			//public int[] ArrayKey;
		}
	}
}