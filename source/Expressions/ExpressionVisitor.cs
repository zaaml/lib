// <copyright file="ExpressionVisitor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;
using System.Linq.Expressions;

namespace Zaaml.Expressions
{
	internal sealed class ExpressionVisitor : ExpressionVisitor<Expression>
	{
		public ExpressionVisitor(ExpressionCompiler compiler)
		{
			Compiler = compiler;
		}

		private ExpressionCompiler Compiler { get; }

		public override Expression VisitExprMinus(ExprMinusNode exprMinus)
		{
			return Compiler.CompileBinaryExpression(ExpressionType.Subtract, exprMinus.Term.Visit(this), exprMinus.Expr.Visit(this));
		}

		public override Expression VisitExprPlus(ExprPlusNode exprPlus)
		{
			return Compiler.CompileBinaryExpression(ExpressionType.Add, exprPlus.Term.Visit(this), exprPlus.Expr.Visit(this));
		}

		public override Expression VisitExprTerm(ExprTermNode exprTerm)
		{
			return exprTerm.Term.Visit(this);
		}

		public override Expression VisitFactorCallArgs(FactorCallArgsNode factorCallArgs)
		{
			return Compiler.CompileInvokeMethod(factorCallArgs.Identifier, factorCallArgs.Expr.Select(e => e.Visit(this)));
		}

		public override Expression VisitFactorCallEmpty(FactorCallEmptyNode factorCallEmpty)
		{
			return Compiler.CompileInvokeMethod(factorCallEmpty.Identifier, Enumerable.Empty<Expression>());
		}

		public override Expression VisitFactorExpr(FactorExprNode factorExpr)
		{
			return factorExpr.Expr.Visit(this);
		}

		public override Expression VisitFactorNumber(FactorNumberNode factorNumber)
		{
			return factorNumber.Number.Visit(this);
		}

		public override Expression VisitFactorParameter(FactorParameterNode factorParameter)
		{
			Compiler.AddParameter(factorParameter);

			return Compiler.CompileParameter(factorParameter.Identifier);
		}

		public override Expression VisitNumberDouble(NumberDoubleNode numberDouble)
		{
			return Compiler.CompileConstant(numberDouble.Double);
		}

		public override Expression VisitTermDiv(TermDivNode termDiv)
		{
			return Compiler.CompileBinaryExpression(ExpressionType.Divide, termDiv.Factor.Visit(this), termDiv.Term.Visit(this));
		}

		public override Expression VisitTermFactor(TermFactorNode termFactor)
		{
			return termFactor.Factor.Visit(this);
		}

		public override Expression VisitTermMul(TermMulNode termMul)
		{
			return Compiler.CompileBinaryExpression(ExpressionType.Multiply, termMul.Factor.Visit(this), termMul.Term.Visit(this));
		}
	}
}