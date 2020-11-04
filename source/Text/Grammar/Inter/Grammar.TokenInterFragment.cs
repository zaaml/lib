// <copyright file="Grammar.TokenInterFragment.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		#region Nested Types

		protected internal sealed class TokenInterFragment : TokenInterPrimitiveEntry
		{
			#region Properties

			public TokenInterProductionCollection Productions { get; } = new TokenInterProductionCollection();

			#endregion

			#region Methods

			public override ParserEntry CreateParserEntry()
			{
				var fragment = new ParserFragment(true);

				foreach (var interProduction in Productions)
					fragment.Productions.Add(new ParserProduction(interProduction.Entries.Select(e => e.CreateParserEntry()).ToArray()));

				return fragment;
			}

			public static implicit operator TokenInterProduction(TokenInterFragment entry)
			{
				return new TokenInterProduction(new TokenInterEntry[] {entry});
			}

			#endregion
		}

		#endregion
	}
}