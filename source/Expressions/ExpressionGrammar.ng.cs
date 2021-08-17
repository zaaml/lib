// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberHidesStaticFromOuterClass
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedType.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable ParameterHidesMember
// ReSharper disable InconsistentNaming
using System;
using Zaaml.Text;

namespace Zaaml.Expressions
{
    enum ExpressionToken
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

    internal sealed class ExpressionGrammar : Grammar<ExpressionGrammar, ExpressionToken>
    {
        private ExpressionGrammar(): base(ExpressionToken.Undefined, new ExpressionLexerGrammar(), new ExpressionParserGrammar())
        {
        }

        private static Lexer<ExpressionGrammar, ExpressionToken> Lexer { get; } = new();
        private static Parser<ExpressionGrammar, ExpressionToken> Parser { get; } = new();
        internal sealed class ExpressionLexerGrammar : LexerGrammar
        {
            private static FragmentSyntax Int { get; } = new();
            public static TriviaSyntax Whitespace { get; } = new();
            public static TokenSyntax Double { get; } = new();
            public static TokenSyntax Identifier { get; } = new();
            public static TokenSyntax Plus { get; } = new();
            public static TokenSyntax Minus { get; } = new();
            public static TokenSyntax Asterisk { get; } = new();
            public static TokenSyntax Div { get; } = new();
            public static TokenSyntax LeftParen { get; } = new();
            public static TokenSyntax Comma { get; } = new();
            public static TokenSyntax RightParen { get; } = new();
            static ExpressionLexerGrammar()
            {
                Int.AddProduction(SyntaxFactory.Production(SyntaxFactory.CharRange('0', '9').OneOrMore()));
                Whitespace.AddProduction(ExpressionToken.Whitespace, SyntaxFactory.Production(SyntaxFactory.CharSet(' ', '\r', '\t').OneOrMore()));
                Double.AddProduction(ExpressionToken.Double, SyntaxFactory.Production(SyntaxFactory.Fragment(SyntaxFactory.Production(Int, SyntaxFactory.Char('.'), Int), SyntaxFactory.Production(Int))));
                Identifier.AddProduction(ExpressionToken.Identifier, SyntaxFactory.Production(SyntaxFactory.CharSet(SyntaxFactory.CharRange('a', 'z'), SyntaxFactory.CharRange('A', 'Z')).OneOrMore()));
                Plus.AddProduction(ExpressionToken.Plus, SyntaxFactory.Production(SyntaxFactory.String("+")));
                Minus.AddProduction(ExpressionToken.Minus, SyntaxFactory.Production(SyntaxFactory.String("-")));
                Asterisk.AddProduction(ExpressionToken.Asterisk, SyntaxFactory.Production(SyntaxFactory.String("*")));
                Div.AddProduction(ExpressionToken.Div, SyntaxFactory.Production(SyntaxFactory.String("/")));
                LeftParen.AddProduction(ExpressionToken.LeftParen, SyntaxFactory.Production(SyntaxFactory.String("(")));
                Comma.AddProduction(ExpressionToken.Comma, SyntaxFactory.Production(SyntaxFactory.String(",")));
                RightParen.AddProduction(ExpressionToken.RightParen, SyntaxFactory.Production(SyntaxFactory.String(")")));
            }
        }

        internal sealed class ExpressionParserGrammar : ParserGrammar
        {
            public static NodeSyntax<ExprNode> Expr { get; } = new();
            public static NodeSyntax<TermNode> Term { get; } = new();
            public static NodeSyntax<FactorNode> Factor { get; } = new();
            public static NodeSyntax<NumberNode> Number { get; } = new();
            static ExpressionParserGrammar()
            {
                Expr.AddProduction<ExprPlusNode>(SyntaxFactory.Production(Term, SyntaxFactory.Token(ExpressionLexerGrammar.Plus), Expr));
                Expr.AddProduction<ExprMinusNode>(SyntaxFactory.Production(Term, SyntaxFactory.Token(ExpressionLexerGrammar.Minus), Expr));
                Expr.AddProduction<ExprTermNode>(SyntaxFactory.Production(Term));
                Term.AddProduction<TermMulNode>(SyntaxFactory.Production(Factor, SyntaxFactory.Token(ExpressionLexerGrammar.Asterisk), Term));
                Term.AddProduction<TermDivNode>(SyntaxFactory.Production(Factor, SyntaxFactory.Token(ExpressionLexerGrammar.Div), Term));
                Term.AddProduction<TermFactorNode>(SyntaxFactory.Production(Factor));
                Factor.AddProduction<FactorCallArgsNode>(SyntaxFactory.Production(SyntaxFactory.Token(ExpressionLexerGrammar.Identifier), SyntaxFactory.Token(ExpressionLexerGrammar.LeftParen), Expr, SyntaxFactory.Fragment(SyntaxFactory.Production(SyntaxFactory.Token(ExpressionLexerGrammar.Comma), Expr)).ZeroOrMore(), SyntaxFactory.Token(ExpressionLexerGrammar.RightParen)));
                Factor.AddProduction<FactorCallEmptyNode>(SyntaxFactory.Production(SyntaxFactory.Token(ExpressionLexerGrammar.Identifier), SyntaxFactory.Token(ExpressionLexerGrammar.LeftParen), SyntaxFactory.Token(ExpressionLexerGrammar.RightParen)));
                Factor.AddProduction<FactorParameterNode>(SyntaxFactory.Production(SyntaxFactory.Token(ExpressionLexerGrammar.Identifier)));
                Factor.AddProduction<FactorExprNode>(SyntaxFactory.Production(SyntaxFactory.Token(ExpressionLexerGrammar.LeftParen), Expr, SyntaxFactory.Token(ExpressionLexerGrammar.RightParen)));
                Factor.AddProduction<FactorNumberNode>(SyntaxFactory.Production(Number));
                Number.AddProduction<NumberNode>(SyntaxFactory.Production(SyntaxFactory.Token(ExpressionLexerGrammar.Double)));
            }
        }

        public static Parser<ExpressionGrammar, ExpressionToken>.SyntaxNodeParser<ExprNode> Expr => new(ExpressionParserGrammar.Expr, Lexer, Parser);
        public static Parser<ExpressionGrammar, ExpressionToken>.SyntaxNodeParser<TermNode> Term => new(ExpressionParserGrammar.Term, Lexer, Parser);
        public static Parser<ExpressionGrammar, ExpressionToken>.SyntaxNodeParser<FactorNode> Factor => new(ExpressionParserGrammar.Factor, Lexer, Parser);
        public static Parser<ExpressionGrammar, ExpressionToken>.SyntaxNodeParser<NumberNode> Number => new(ExpressionParserGrammar.Number, Lexer, Parser);
    }

    abstract class ExpressionNode
    {
        protected abstract TResult AcceptVisitor<TResult>(ExpressionVisitor<TResult> visitor);
        public TResult Visit<TResult>(ExpressionVisitor<TResult> visitor)
        {
            return AcceptVisitor(visitor);
        }
    }

    abstract class ExprNode : ExpressionNode
    {
    }

    sealed class ExprPlusNode : ExprNode
    {
        public ExprPlusNode(TermNode Term, ExprNode Expr)
        {
            this.Term = Term;
            this.Expr = Expr;
        }

        public TermNode Term { get; }

        public ExprNode Expr { get; }

        protected override TResult AcceptVisitor<TResult>(ExpressionVisitor<TResult> visitor)
        {
            return visitor.VisitExprPlus(this);
        }
    }

    sealed class ExprMinusNode : ExprNode
    {
        public ExprMinusNode(TermNode Term, ExprNode Expr)
        {
            this.Term = Term;
            this.Expr = Expr;
        }

        public TermNode Term { get; }

        public ExprNode Expr { get; }

        protected override TResult AcceptVisitor<TResult>(ExpressionVisitor<TResult> visitor)
        {
            return visitor.VisitExprMinus(this);
        }
    }

    sealed class ExprTermNode : ExprNode
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

    abstract class TermNode : ExpressionNode
    {
    }

    sealed class TermMulNode : TermNode
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

    sealed class TermDivNode : TermNode
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

    sealed class TermFactorNode : TermNode
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

    abstract class FactorNode : ExpressionNode
    {
    }

    sealed class FactorCallArgsNode : FactorNode
    {
        public FactorCallArgsNode(string Identifier, ExprNode[] Expr)
        {
            this.Identifier = Identifier;
            this.Expr = Expr;
        }

        public string Identifier { get; }

        public ExprNode[] Expr { get; }

        protected override TResult AcceptVisitor<TResult>(ExpressionVisitor<TResult> visitor)
        {
            return visitor.VisitFactorCallArgs(this);
        }
    }

    sealed class FactorCallEmptyNode : FactorNode
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

    sealed class FactorParameterNode : FactorNode
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

    sealed class FactorExprNode : FactorNode
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

    sealed class FactorNumberNode : FactorNode
    {
        public FactorNumberNode(NumberNode Number)
        {
            this.Number = Number;
        }

        public NumberNode Number { get; }

        protected override TResult AcceptVisitor<TResult>(ExpressionVisitor<TResult> visitor)
        {
            return visitor.VisitFactorNumber(this);
        }
    }

    sealed class NumberNode : ExpressionNode
    {
        public NumberNode(string Double)
        {
            this.Double = Double;
        }

        public string Double { get; }

        protected override TResult AcceptVisitor<TResult>(ExpressionVisitor<TResult> visitor)
        {
            return visitor.VisitNumber(this);
        }
    }

    abstract class ExpressionVisitor<TResult>
    {
        public abstract TResult VisitExprPlus(ExprPlusNode ExprPlus);
        public abstract TResult VisitExprMinus(ExprMinusNode ExprMinus);
        public abstract TResult VisitExprTerm(ExprTermNode ExprTerm);
        public abstract TResult VisitTermMul(TermMulNode TermMul);
        public abstract TResult VisitTermDiv(TermDivNode TermDiv);
        public abstract TResult VisitTermFactor(TermFactorNode TermFactor);
        public abstract TResult VisitFactorCallArgs(FactorCallArgsNode FactorCallArgs);
        public abstract TResult VisitFactorCallEmpty(FactorCallEmptyNode FactorCallEmpty);
        public abstract TResult VisitFactorParameter(FactorParameterNode FactorParameter);
        public abstract TResult VisitFactorExpr(FactorExprNode FactorExpr);
        public abstract TResult VisitFactorNumber(FactorNumberNode FactorNumber);
        public abstract TResult VisitNumber(NumberNode Number);
    }
}
