// <copyright file="Grammar.Lexer.Fragment.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class LexerGrammar
		{
			protected internal sealed class FragmentSyntax : Syntax
			{
				public FragmentSyntax([CallerMemberName] string name = null) : base(name)
				{
					RegisterFragmentSyntax(this);
				}

				internal FragmentSyntax(string name, bool internalFragment) : base(name)
				{
					Debug.Assert(internalFragment);

					IsInternal = true;
				}

				internal bool IsInternal { get; }

				public FragmentSyntax AddProduction(Production lexerProduction)
				{
					AddProductionCore(lexerProduction);

					return this;
				}

				public QuantifierSymbol AtLeast(int count, QuantifierMode mode = QuantifierMode.Greedy)
				{
					return new QuantifierSymbol(new FragmentSymbol(this), QuantifierHelper.AtLeast(count), mode);
				}

				public QuantifierSymbol Between(int from, int to, QuantifierMode mode = QuantifierMode.Greedy)
				{
					return new QuantifierSymbol(new FragmentSymbol(this), QuantifierHelper.Between(from, to), mode);
				}

				public QuantifierSymbol Exact(int count, QuantifierMode mode = QuantifierMode.Greedy)
				{
					return new QuantifierSymbol(new FragmentSymbol(this), QuantifierHelper.Exact(count), mode);
				}

				public QuantifierSymbol OneOrMore(QuantifierMode mode = QuantifierMode.Greedy)
				{
					return new QuantifierSymbol(new FragmentSymbol(this), QuantifierKind.OneOrMore, mode);
				}

				public QuantifierSymbol ZeroOrMore(QuantifierMode mode = QuantifierMode.Greedy)
				{
					return new QuantifierSymbol(new FragmentSymbol(this), QuantifierKind.ZeroOrMore, mode);
				}

				public QuantifierSymbol ZeroOrOne(QuantifierMode mode = QuantifierMode.Greedy)
				{
					return new QuantifierSymbol(new FragmentSymbol(this), QuantifierKind.ZeroOrOne, mode);
				}
			}
		}
	}
}