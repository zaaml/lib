// <copyright file="Grammar.Parser.Node.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Zaaml.Text
{
	internal partial class Grammar<TGrammar, TToken>
	{
		public partial class ParserGrammar
		{
			protected internal class NodeSyntax : Syntax
			{
				public NodeSyntax([CallerMemberName] string name = null) : base(name)
				{
					RegisterNodeSyntax(this);
				}

				internal NodeSyntax(string name, bool internalNode) : base(name)
				{
					Debug.Assert(internalNode);
				}

				public Grammar<TGrammar, TToken> Grammar => Get<TGrammar, TToken>();

				public virtual Type NodeType => null;

				public Production AddProduction(Production parserSyntaxProduction)
				{
					AddProductionCore(parserSyntaxProduction);

					return parserSyntaxProduction;
				}

				public QuantifierSymbol AtLeast(int count)
				{
					return new QuantifierSymbol(new NodeSymbol(this), QuantifierHelper.AtLeast(count), QuantifierMode.Greedy);
				}

				public QuantifierSymbol Between(int from, int to)
				{
					return new QuantifierSymbol(new NodeSymbol(this), QuantifierHelper.Between(from, to), QuantifierMode.Greedy);
				}

				public NodeSymbol Bind(string name)
				{
					return new NodeSymbol(this)
					{
						ArgumentName = name
					};
				}

				internal string EnsureName()
				{
					return Name;
				}

				public QuantifierSymbol Exact(int count)
				{
					return new QuantifierSymbol(new NodeSymbol(this), QuantifierHelper.Exact(count), QuantifierMode.Greedy);
				}

				public QuantifierSymbol OneOrMore(QuantifierMode quantifierMode = QuantifierMode.Greedy)
				{
					return new QuantifierSymbol(new NodeSymbol(this), QuantifierKind.OneOrMore, quantifierMode);
				}

				public override string ToString()
				{
					return Name ?? base.ToString();
				}

				public QuantifierSymbol ZeroOrMore(QuantifierMode quantifierMode = QuantifierMode.Greedy)
				{
					return new QuantifierSymbol(new NodeSymbol(this), QuantifierKind.ZeroOrMore, quantifierMode);
				}

				public QuantifierSymbol ZeroOrOne(QuantifierMode quantifierMode = QuantifierMode.Greedy)
				{
					return new QuantifierSymbol(new NodeSymbol(this), QuantifierKind.ZeroOrOne, quantifierMode);
				}
			}

			protected internal sealed class NodeSyntax<TNode> : NodeSyntax where TNode : class
			{
				internal NodeSyntax([CallerMemberName] string name = null) : base(name)
				{
				}

				public override Type NodeType => typeof(TNode);

				public Production AddProduction<TProductionNode>(Production parserSyntaxProduction) where TProductionNode : TNode
				{
					parserSyntaxProduction.Name = typeof(TProductionNode).Name;
					parserSyntaxProduction.ProductionBinding = ConstructorNodeBinding.Bind<TProductionNode>();

					AddProductionCore(parserSyntaxProduction);

					return parserSyntaxProduction;
				}

				public Production AddReturnProduction<TProductionNode>(NodeSyntax<TProductionNode> nodeSyntax) where TProductionNode : class, TNode
				{
					var parserSyntaxProduction = new Production(new Symbol[] { new NodeSymbol(nodeSyntax) })
					{
						Name = typeof(TProductionNode).Name,
						ProductionBinding = ReturnNodeBinding.Bind<TProductionNode>()
					};

					AddProductionCore(parserSyntaxProduction);

					return parserSyntaxProduction;
				}
			}
		}
	}
}