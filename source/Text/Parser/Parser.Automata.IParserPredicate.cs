// <copyright file="Parser.Automata.IParserPredicate.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private interface IParserPredicate
			{
				ParserPredicateKind PredicateKind { get; }

				Type ResultType { get; }
			}
		}
	}
}