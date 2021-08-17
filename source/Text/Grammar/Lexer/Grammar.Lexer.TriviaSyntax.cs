// <copyright file="Grammar.Lexer.TriviaSyntax.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class LexerGrammar
		{
			protected internal sealed class TriviaSyntax : TokenBaseSyntax
			{
				public TriviaSyntax([CallerMemberName] string name = null) : base(name)
				{
					RegisterTriviaSyntax(this);
				}

				public TriviaSyntax AddProduction(TToken token, Production production)
				{
					AddProductionCore(token, production);

					return this;
				}
			}
		}
	}
}