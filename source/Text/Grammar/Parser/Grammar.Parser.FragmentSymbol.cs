// <copyright file="Grammar.Parser.FragmentSymbol.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class ParserGrammar
		{
			protected internal sealed class FragmentSymbol : PrimitiveSymbol
			{
				public FragmentSymbol(FragmentSyntax fragment)
				{
					Fragment = fragment;
				}

				public FragmentSyntax Fragment { get; }
			}
		}
	}
}