// <copyright file="Automata.Process.ThreadParent.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		partial class Process
		{
			#region Nested Types

			private sealed class ThreadParent : PoolSharedObject<ThreadParent>
			{
				#region Fields

				public ExecutionPathQueue ExecutionQueue;
				public ThreadParent Parent;
				public PredicateResultQueue PredicateResultQueue;
				public AutomataStack Stack;

				#endregion

				#region Ctors

				public ThreadParent(Pool<ThreadParent> pool) : base(pool)
				{
				}

				#endregion

				#region Methods

				protected override void OnReleased()
				{
					Stack?.ReleaseReference();
					ExecutionQueue?.ReleaseReference();
					PredicateResultQueue?.ReleaseReference();
					Parent?.ReleaseReference();

					Stack = null;
					Parent = null;
					ExecutionQueue = null;

					base.OnReleased();
				}

				public void Mount(ref Thread thread)
				{
					Stack = thread.Stack.AddReference();
					ExecutionQueue = thread.ExecutionQueue.AddReference();
					PredicateResultQueue = thread.PredicateResultQueue?.AddReference() ?? thread.Parent?.PredicateResultQueue?.AddReference();
					Parent = thread.Parent?.AddReference();
				}

				#endregion
			}

			#endregion
		}

		#endregion
	}
}