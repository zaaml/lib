// <copyright file="Grammar.Parser.ConstructorBinding.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class ParserGrammar
		{
			public sealed class ConstructorNodeBinding : ProductionNodeBinding
			{
				private ConstructorNodeBinding(Type nodeType) : base(nodeType)
				{
				}

				public static ProductionNodeBinding Bind<TNode>()
				{
					return new ConstructorNodeBinding(typeof(TNode));
				}
			}

			public sealed class ReturnNodeBinding : ProductionNodeBinding
			{
				private ReturnNodeBinding(Type nodeType) : base(nodeType)
				{
				}

				public static ReturnNodeBinding Bind<TNode>()
				{
					return new ReturnNodeBinding(typeof(TNode));
				}
			}
		}
	}
}