// <copyright file="Grammar.ConstructorFactoryBinding.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		public sealed class ConstructorBinding : ParserProductionBinding
		{
			private ConstructorBinding(Type nodeType)
			{
				NodeType = nodeType;
			}

			public Type NodeType { get; }

			public static ParserProductionBinding Bind<TNode>()
			{
				return new ConstructorBinding(typeof(TNode));
			}
		}
	}
}
