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
		private protected sealed class DfaTransition : IEquatable<DfaTransition>
		{
			public readonly int LazyIndex;
			public readonly SubGraph SubGraph;

			public DfaTransition(SubGraph subGraph, int lazyIndex)
			{
				SubGraph = subGraph;
				LazyIndex = lazyIndex;
			}

			public override string ToString()
			{
				return SubGraph.ToString();
			}

			public bool Equals(DfaTransition transition)
			{
				return ReferenceEquals(SubGraph, transition.SubGraph);
			}
		}
	}
}