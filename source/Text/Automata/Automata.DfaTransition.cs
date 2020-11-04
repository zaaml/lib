// <copyright file="Automata.DfaTransition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;

namespace Zaaml.Text
{
	[SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		private protected sealed class DfaTransition : IEquatable<DfaTransition>
		{
			#region Fields

			public readonly int LazyIndex;
			public readonly SubGraphBase SubGraph;

			#endregion

			#region Ctors

			public DfaTransition(SubGraphBase subGraph, int lazyIndex)
			{
				SubGraph = subGraph;
				LazyIndex = lazyIndex;
			}

			#endregion

			#region Methods

			public override string ToString()
			{
				return SubGraph.ToString();
			}

			#endregion

			#region Interface Implementations

			#region IEquatable<Automata<TInstruction,TOperand>.DfaTransition>

			public bool Equals(DfaTransition transition)
			{
				return ReferenceEquals(SubGraph, transition.SubGraph);
			}

			#endregion

			#endregion
		}

		#endregion
	}
}