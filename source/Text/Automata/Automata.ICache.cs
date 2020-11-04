// <copyright file="Automata.ICache.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		private interface ICache<T>
		{
			#region Methods

			void SetValue(int operand, T value);

			bool TryGetValue(int operand, out T result);

			#endregion
		}

		#endregion
	}
}