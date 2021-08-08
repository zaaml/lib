// <copyright file="Grammar.SyntaxFactoryBinding.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Reflection;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		public sealed class SyntaxFactoryBinding : ParserProductionBinding
		{
			private SyntaxFactoryBinding(Type nodeType, Type syntaxFactoryType, Delegate expression)
			{
				NodeType = nodeType;
				SyntaxFactoryType = syntaxFactoryType;

				MethodTarget = expression.Target;
				Method = expression.Method;
			}

			public MethodInfo Method { get; }

			public object MethodTarget { get; }

			public Type NodeType { get; }

			public Type SyntaxFactoryType { get; }

			public static ParserProductionBinding Bind<TNode, TSyntaxFactory>(Delegate expression) where TSyntaxFactory : SyntaxFactory<TNode>
			{
				return new SyntaxFactoryBinding(typeof(TNode), typeof(TSyntaxFactory), expression);
			}
		}
	}
}