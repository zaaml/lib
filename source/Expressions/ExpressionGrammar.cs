// <copyright file="ExpressionGrammar.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberHidesStaticFromOuterClass
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedType.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable ParameterHidesMember
// ReSharper disable InconsistentNaming

using Zaaml.Text;

namespace Zaaml.Expressions
{
	internal enum ExpressionToken
	{
		Undefined,
		Double,
		Identifier,
		Whitespace,
		Plus,
		Minus,
		Asterisk,
		Div,
		LeftParen,
		Comma,
		RightParen
	}

	internal sealed class ExpressionGrammar : Grammar<ExpressionToken, ExpressionNode>
	{
		private static readonly TokenFragment Int = CreateTokenFragment();
		internal static readonly TokenRule Double = CreateTokenRule(ExpressionToken.Double);
		internal static readonly TokenRule Identifier = CreateTokenRule(ExpressionToken.Identifier);
		internal static readonly TokenRule Whitespace = CreateTokenRule(ExpressionToken.Whitespace);
		internal static readonly TokenRule Plus = CreateTokenRule(ExpressionToken.Plus);
		internal static readonly TokenRule Minus = CreateTokenRule(ExpressionToken.Minus);
		internal static readonly TokenRule Asterisk = CreateTokenRule(ExpressionToken.Asterisk);
		internal static readonly TokenRule Div = CreateTokenRule(ExpressionToken.Div);
		internal static readonly TokenRule LeftParen = CreateTokenRule(ExpressionToken.LeftParen);
		internal static readonly TokenRule Comma = CreateTokenRule(ExpressionToken.Comma);
		internal static readonly TokenRule RightParen = CreateTokenRule(ExpressionToken.RightParen);
		internal static readonly ParserRule<ExprNode> Expr = CreateParserRule<ExprNode>();
		internal static readonly ParserRule<TermNode> Term = CreateParserRule<TermNode>();
		internal static readonly ParserRule<FactorNode> Factor = CreateParserRule<FactorNode>();
		internal static readonly ParserRule<NumberDoubleNode> Number = CreateParserRule<NumberDoubleNode>();

		private ExpressionGrammar() : base(ExpressionToken.Undefined)
		{
		}

		static ExpressionGrammar()
		{
			Int.Pattern = MatchRange('0', '9').OneOrMore();
			Double.Pattern = Int + Match('.') + Int | Int;
			Identifier.Pattern = MatchSet(MatchRange('a', 'z'), MatchRange('A', 'Z')).OneOrMore();
			Whitespace.Pattern = MatchSet(' ', '\r', '\t').OneOrMore();
			Whitespace.Skip = true;
			Plus.Pattern = Literal("+");
			Minus.Pattern = Literal("-");
			Asterisk.Pattern = Literal("*");
			Div.Pattern = Literal("/");
			LeftParen.Pattern = Literal("(");
			Comma.Pattern = Literal(",");
			RightParen.Pattern = Literal(")");
			Expr.Bind<ExprPlusNode>(Term + Plus + Expr);
			Expr.Bind<ExprMinusNode>(Term + Minus + Expr);
			Expr.Bind<ExprTermNode>(Term);
			Term.Bind<TermMulNode>(Factor + Asterisk + Term);
			Term.Bind<TermDivNode>(Factor + Div + Term);
			Term.Bind<TermFactorNode>(Factor);
			Factor.Bind<FactorCallArgsNode>(Identifier + LeftParen + Expr + (Comma + Expr).ZeroOrMore() + RightParen);
			Factor.Bind<FactorCallEmptyNode>(Identifier + LeftParen + RightParen);
			Factor.Bind<FactorParameterNode>(Identifier);
			Factor.Bind<FactorExprNode>(LeftParen + Expr + RightParen);
			Factor.Bind<FactorNumberNode>(Number);
			Number.Bind<NumberDoubleNode>(Double);
		}

		public static ExpressionLexer Lexer => ExpressionLexer.Instance;
		public static ExpressionParser Parser => ExpressionParser.Instance;

		public sealed class ExpressionLexer : Lexer<ExpressionGrammar, ExpressionToken>
		{
			public static readonly ExpressionLexer Instance = new ExpressionLexer();

			private ExpressionLexer()
			{
			}
		}

		public sealed class ExpressionParser : Parser<ExpressionGrammar, ExpressionToken, ExpressionNode>
		{
			public static readonly ExpressionParser Instance = new ExpressionParser();

			private ExpressionParser()
			{
			}

			public ExprNode Expr(string ExprString)
			{
				using var lexemeStream = Lexer.GetLexemeSource(ExprString);
				return ParseCore<ExprNode, ExprNode>(ExpressionGrammar.Expr, lexemeStream);
			}

			public FactorNode Factor(string FactorString)
			{
				using var lexemeStream = Lexer.GetLexemeSource(FactorString);
				return ParseCore<FactorNode, FactorNode>(ExpressionGrammar.Factor, lexemeStream);
			}

			public NumberDoubleNode Number(string NumberString)
			{
				using var lexemeStream = Lexer.GetLexemeSource(NumberString);
				return ParseCore<NumberDoubleNode, NumberDoubleNode>(ExpressionGrammar.Number, lexemeStream);
			}

			public TermNode Term(string TermString)
			{
				using var lexemeStream = Lexer.GetLexemeSource(TermString);
				return ParseCore<TermNode, TermNode>(ExpressionGrammar.Term, lexemeStream);
			}
		}
	}

	internal abstract class ExpressionNode
	{
		protected abstract TResult AcceptVisitor<TResult>(ExpressionVisitor<TResult> visitor);

		public TResult Visit<TResult>(ExpressionVisitor<TResult> visitor)
		{
			return AcceptVisitor(visitor);
		}
	}

	internal abstract class ExprNode : ExpressionNode
	{
	}

	internal sealed class ExprPlusNode : ExprNode
	{
		public ExprPlusNode(TermNode Term, ExprNode Expr)
		{
			this.Term = Term;
			this.Expr = Expr;
		}

		public ExprNode Expr { get; }

		public TermNode Term { get; }

		protected override TResult AcceptVisitor<TResult>(ExpressionVisitor<TResult> visitor)
		{
			return visitor.VisitExprPlus(this);
		}
	}

	internal sealed class ExprMinusNode : ExprNode
	{
		public ExprMinusNode(TermNode Term, ExprNode Expr)
		{
			this.Term = Term;
			this.Expr = Expr;
		}

		public ExprNode Expr { get; }

		public TermNode Term { get; }

		protected override TResult AcceptVisitor<TResult>(ExpressionVisitor<TResult> visitor)
		{
			return visitor.VisitExprMinus(this);
		}
	}

	internal sealed class ExprTermNode : ExprNode
	{
		public ExprTermNode(TermNode Term)
		{
			this.Term = Term;
		}

		public TermNode Term { get; }

		protected override TResult AcceptVisitor<TResult>(ExpressionVisitor<TResult> visitor)
		{
			return visitor.VisitExprTerm(this);
		}
	}

	internal abstract class TermNode : ExpressionNode
	{
	}

	internal sealed class TermMulNode : TermNode
	{
		public TermMulNode(FactorNode Factor, TermNode Term)
		{
			this.Factor = Factor;
			this.Term = Term;
		}

		public FactorNode Factor { get; }

		public TermNode Term { get; }

		protected override TResult AcceptVisitor<TResult>(ExpressionVisitor<TResult> visitor)
		{
			return visitor.VisitTermMul(this);
		}
	}

	internal sealed class TermDivNode : TermNode
	{
		public TermDivNode(FactorNode Factor, TermNode Term)
		{
			this.Factor = Factor;
			this.Term = Term;
		}

		public FactorNode Factor { get; }

		public TermNode Term { get; }

		protected override TResult AcceptVisitor<TResult>(ExpressionVisitor<TResult> visitor)
		{
			return visitor.VisitTermDiv(this);
		}
	}

	internal sealed class TermFactorNode : TermNode
	{
		public TermFactorNode(FactorNode Factor)
		{
			this.Factor = Factor;
		}

		public FactorNode Factor { get; }

		protected override TResult AcceptVisitor<TResult>(ExpressionVisitor<TResult> visitor)
		{
			return visitor.VisitTermFactor(this);
		}
	}

	internal abstract class FactorNode : ExpressionNode
	{
	}

	internal sealed class FactorCallArgsNode : FactorNode
	{
		public FactorCallArgsNode(string Identifier, ExprNode[] Expr)
		{
			this.Identifier = Identifier;
			this.Expr = Expr;
		}

		public ExprNode[] Expr { get; }

		public string Identifier { get; }

		protected override TResult AcceptVisitor<TResult>(ExpressionVisitor<TResult> visitor)
		{
			return visitor.VisitFactorCallArgs(this);
		}
	}

	internal sealed class FactorCallEmptyNode : FactorNode
	{
		public FactorCallEmptyNode(string Identifier)
		{
			this.Identifier = Identifier;
		}

		public string Identifier { get; }

		protected override TResult AcceptVisitor<TResult>(ExpressionVisitor<TResult> visitor)
		{
			return visitor.VisitFactorCallEmpty(this);
		}
	}

	internal sealed class FactorParameterNode : FactorNode
	{
		public FactorParameterNode(string Identifier)
		{
			this.Identifier = Identifier;
		}

		public string Identifier { get; }

		protected override TResult AcceptVisitor<TResult>(ExpressionVisitor<TResult> visitor)
		{
			return visitor.VisitFactorParameter(this);
		}
	}

	internal sealed class FactorExprNode : FactorNode
	{
		public FactorExprNode(ExprNode Expr)
		{
			this.Expr = Expr;
		}

		public ExprNode Expr { get; }

		protected override TResult AcceptVisitor<TResult>(ExpressionVisitor<TResult> visitor)
		{
			return visitor.VisitFactorExpr(this);
		}
	}

	internal sealed class FactorNumberNode : FactorNode
	{
		public FactorNumberNode(NumberDoubleNode Number)
		{
			this.Number = Number;
		}

		public NumberDoubleNode Number { get; }

		protected override TResult AcceptVisitor<TResult>(ExpressionVisitor<TResult> visitor)
		{
			return visitor.VisitFactorNumber(this);
		}
	}

	internal sealed class NumberDoubleNode : ExpressionNode
	{
		public NumberDoubleNode(string Double)
		{
			this.Double = Double;
		}

		public string Double { get; }

		protected override TResult AcceptVisitor<TResult>(ExpressionVisitor<TResult> visitor)
		{
			return visitor.VisitNumberDouble(this);
		}
	}

	internal abstract class ExpressionVisitor<TResult>
	{
		public abstract TResult VisitExprMinus(ExprMinusNode ExprMinus);
		public abstract TResult VisitExprPlus(ExprPlusNode ExprPlus);
		public abstract TResult VisitExprTerm(ExprTermNode ExprTerm);
		public abstract TResult VisitFactorCallArgs(FactorCallArgsNode FactorCallArgs);
		public abstract TResult VisitFactorCallEmpty(FactorCallEmptyNode FactorCallEmpty);
		public abstract TResult VisitFactorExpr(FactorExprNode FactorExpr);
		public abstract TResult VisitFactorNumber(FactorNumberNode FactorNumber);
		public abstract TResult VisitFactorParameter(FactorParameterNode FactorParameter);
		public abstract TResult VisitNumberDouble(NumberDoubleNode NumberDouble);
		public abstract TResult VisitTermDiv(TermDivNode TermDiv);
		public abstract TResult VisitTermFactor(TermFactorNode TermFactor);
		public abstract TResult VisitTermMul(TermMulNode TermMul);
	}
}