// <copyright file="Grammar.Parser.Fragment.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Zaaml.Text
{
	internal partial class Grammar<TGrammar, TToken>
	{
		public partial class ParserGrammar
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

				public Production AddProduction(Production parserSyntaxProduction)
				{
					AddProductionCore(parserSyntaxProduction);

					return parserSyntaxProduction;
				}

				public QuantifierSymbol AtLeast(int count)
				{
					return new QuantifierSymbol(new FragmentSymbol(this), QuantifierHelper.AtLeast(count), QuantifierMode.Greedy);
				}

				public QuantifierSymbol Between(int from, int to)
				{
					return new QuantifierSymbol(new FragmentSymbol(this), QuantifierHelper.Between(from, to), QuantifierMode.Greedy);
				}

				public QuantifierSymbol Exact(int count)
				{
					return new QuantifierSymbol(new FragmentSymbol(this), QuantifierHelper.Exact(count), QuantifierMode.Greedy);
				}

				public QuantifierSymbol OneOrMore(QuantifierMode quantifierMode = QuantifierMode.Greedy)
				{
					return new QuantifierSymbol(new FragmentSymbol(this), QuantifierKind.OneOrMore, quantifierMode);
				}

				public QuantifierSymbol ZeroOrMore(QuantifierMode quantifierMode = QuantifierMode.Greedy)
				{
					return new QuantifierSymbol(new FragmentSymbol(this), QuantifierKind.ZeroOrMore, quantifierMode);
				}

				public QuantifierSymbol ZeroOrOne(QuantifierMode quantifierMode = QuantifierMode.Greedy)
				{
					return new QuantifierSymbol(new FragmentSymbol(this), QuantifierKind.ZeroOrOne, quantifierMode);
				}
			}
		}
	}
}