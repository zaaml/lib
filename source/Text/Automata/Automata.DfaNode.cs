﻿// <copyright file="Automata.DfaNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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

		private protected sealed class DfaNode : IEquatable<DfaNode>
		{
			#region Fields

			public readonly object ExecutionPathObject;
			public readonly NodeBase Node;
			public readonly DfaTransition Transition;

			#endregion

			#region Ctors

			public DfaNode(NodeBase node, DfaTransition transition)
			{
				Node = node;
				Transition = transition;
			}

			public DfaNode(NodeBase node, DfaTransition transition, object executionPathObject)
			{
				Node = node;
				Transition = transition;
				ExecutionPathObject = executionPathObject;
			}

			#endregion

			#region Methods

			public override bool Equals(object obj)
			{
				return ReferenceEquals(this, obj) || obj is DfaNode other && Equals(other);
			}

			public override int GetHashCode()
			{
#if NETCOREAPP
				return HashCode.Combine(Node, Transition, ExecutionPathObject);
#else
				unchecked
				{
					int hash = 17;

					hash = hash * 31 + Node.GetHashCode();
					hash = hash * 31 + Transition.GetHashCode();

					if (ExecutionPathObject != null)
						hash = hash * 31 + ExecutionPathObject.GetHashCode();

					return hash;
				}
#endif
			}

			public override string ToString()
			{
				return Node.ToString();
			}

			#endregion

			#region Interface Implementations

			#region IEquatable<Automata<TInstruction,TOperand>.DfaNode>

			public bool Equals(DfaNode other)
			{
				if (ReferenceEquals(null, other)) return false;
				if (ReferenceEquals(this, other)) return true;

				return Equals(Node, other.Node) && Equals(Transition, other.Transition) && Equals(ExecutionPathObject, other.ExecutionPathObject);
			}

			#endregion

			#endregion
		}

		#endregion
	}
}