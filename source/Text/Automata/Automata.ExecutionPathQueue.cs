// <copyright file="Automata.ExecutionPathQueue.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		private sealed class ExecutionPathQueue : PoolSharedObject<ExecutionPathQueue>
		{
			#region Fields

			public readonly List<ExecutionPath> List = new List<ExecutionPath>();
			public ExecutionPathQueue Next;

			#endregion

			#region Ctors

			public ExecutionPathQueue(Pool<ExecutionPathQueue> pool) : base(pool)
			{
			}

			#endregion

			#region Methods

			protected override void OnReleased()
			{
				// ReSharper disable once ForCanBeConvertedToForeach
				for (var index = 0; index < List.Count; index++)
				{
					var executionPath = List[index];

					executionPath.Release();
				}

				List.Clear();
				Next = null;

				base.OnReleased();
			}

			#endregion
		}

		#endregion
	}
}