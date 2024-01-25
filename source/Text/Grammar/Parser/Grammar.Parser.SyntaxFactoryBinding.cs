// <copyright file="Grammar.Parser.SyntaxFactoryBinding.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Reflection;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class ParserGrammar
		{
			public sealed class SyntaxNodeFactoryBinding : ProductionNodeBinding
			{
				private SyntaxNodeFactoryBinding(Type nodeType, Type syntaxFactoryType, Delegate expression) : base(nodeType)
				{
					SyntaxFactoryType = syntaxFactoryType;

					MethodTarget = expression.Target;
					Method = expression.Method;
				}

				public MethodInfo Method { get; }

				public object MethodTarget { get; }

				public Type SyntaxFactoryType { get; }

				public static ProductionNodeBinding Bind<TNode, TSyntaxFactory>(Delegate expression) where TSyntaxFactory : SyntaxFactory<TNode>
				{
					return new SyntaxNodeFactoryBinding(typeof(TNode), typeof(TSyntaxFactory), expression);
				}
			}
		}
	}
}