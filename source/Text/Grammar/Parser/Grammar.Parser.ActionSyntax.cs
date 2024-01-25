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
			protected internal sealed class ActionSyntax : Syntax
			{
				public ActionSyntax([CallerMemberName] string name = null) : base(name)
				{
				}

				public Parser<TToken>.ActionEntry ActionEntry { get; private set; }

				public void Bind<TParser>(Func<TParser, Parser<TGrammar, TToken>.ParserAction> actionFactory) where TParser : Parser<TGrammar, TToken>
				{
					ActionEntry = new Parser<TToken>.ActionEntry(p =>
					{
						var predicate = actionFactory((TParser)p);

						predicate();
					});
				}
			}
		}
	}
}