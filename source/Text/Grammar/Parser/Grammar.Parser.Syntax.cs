// <copyright file="Grammar.Parser.Syntax.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TGrammar, TToken>
	{
		public partial class ParserGrammar
		{
			protected internal abstract class Syntax : GrammarSyntax<Syntax, Production, Symbol>
			{
				protected Syntax(string name) : base(name)
				{
				}

				public bool CollapseBacktracking { get; set; }
			}
		}
	}
}