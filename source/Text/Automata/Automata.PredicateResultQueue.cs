// <copyright file="Automata.PredicateResultQueue.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		private sealed class PredicateResultQueue : PoolSharedObject<PredicateResultQueue>
		{
			#region Fields

			public readonly Queue<PredicateResult> Queue = new Queue<PredicateResult>();
			public PredicateResultQueue Next;

			#endregion

			#region Ctors

			public PredicateResultQueue(Pool<PredicateResultQueue> pool) : base(pool)
			{
			}

			#endregion

			#region Methods

			protected override void OnReleased()
			{
				while (Queue.Count != 0)
					Queue.Dequeue().Dispose();

				Next = null;

				base.OnReleased();
			}

			#endregion
		}

		#endregion
	}
}