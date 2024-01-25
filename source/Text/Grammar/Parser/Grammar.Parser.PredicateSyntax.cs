// <copyright file="Grammar.Parser.ActionSymbol.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Runtime.CompilerServices;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class ParserGrammar
		{
			protected internal sealed class PredicateSyntax : Syntax
			{
				public PredicateSyntax([CallerMemberName] string name = null) : base(name)
				{
				}

				public Parser<TToken>.PredicateEntry PredicateEntry { get; private set; }

				public void Bind<TParser>(Func<TParser, Parser<TGrammar, TToken>.ParserPredicate> predicateFactory) where TParser : Parser<TGrammar, TToken>
				{
					PredicateEntry = new Parser<TToken>.PredicateEntry(p =>
					{
						var predicate = predicateFactory((TParser)p);

						return predicate();
					}, Name);
				}
			}
		}
	}
}