// <copyright file="Automata.ICache.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private interface ICache<T>
		{
			void SetValue(int operand, T value);

			bool TryGetValue(int operand, out T result);
		}
	}
}