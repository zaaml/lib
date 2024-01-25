// <copyright file="Grammar.Parser.ExternalSymbol.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal partial class Grammar<TGrammar, TToken>
	{
		public partial class ParserGrammar
		{
			protected internal abstract class ExternalSymbol : PrimitiveSymbol
			{
				public abstract Type ExternalGrammarType { get; }

				public abstract Type ExternalTokenType { get; }
			}
		}
	}
}