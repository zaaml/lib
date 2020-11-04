// <copyright file="Grammar.ParserEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		#region Nested Types

		protected internal abstract class ParserEntry
		{
			#region Properties

			public string Name { get; internal set; }



			#endregion

			#region Methods

			public static implicit operator ParserEntry(ParserRule rule)
			{
				return new ParserRuleEntry(rule);
			}

			public static implicit operator ParserEntry(Parser<TToken>.PredicateEntry parserPredicateEntry)
			{
				return new ParserPredicate(parserPredicateEntry);
			}

			public static implicit operator ParserEntry(Parser<TToken>.ActionEntry parserActionEntry)
			{
				return new ParserAction(parserActionEntry);
			}

			public override string ToString()
			{
				return Name;
			}

			#endregion
		}

		#endregion
	}
}