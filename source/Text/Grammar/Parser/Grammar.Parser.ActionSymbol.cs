// <copyright file="Grammar.Parser.ActionSymbol.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class ParserGrammar
		{
			protected internal sealed class ActionSymbol : Symbol
			{
				public ActionSymbol(Parser<TToken>.ActionEntry action)
				{
					Action = action;
				}

				public Parser<TToken>.ActionEntry Action { get; }
			}
		}
	}
}