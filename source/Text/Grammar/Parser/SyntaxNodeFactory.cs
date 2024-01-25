// <copyright file="SyntaxFactory.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract class SyntaxNodeFactory : IDisposable
	{
		protected virtual void DisposeCore()
		{
		}

		public void Dispose()
		{
			DisposeCore();
		}
	}

	internal abstract class SyntaxFactory<TNode> : SyntaxNodeFactory
	{
		protected SyntaxNode<TActualNode> SyntaxNode<TActualNode>(TActualNode node) where TActualNode : TNode
		{
			return new SyntaxNode<TActualNode>(node);
		}
	}

	internal class SyntaxNode<TNode>
	{
		public SyntaxNode(TNode node)
		{
			Node = node;
		}

		public TNode Node { get; }
	}
}