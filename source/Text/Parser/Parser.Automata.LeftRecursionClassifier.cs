// <copyright file="Parser.Automata.LeftRecursionClassifier.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private enum LeftRecursionClassifier
			{
				Primary,
				Binary,
				Ternary,
				Prefix,
				Suffix,
				Generic
			}
		}
	}
}