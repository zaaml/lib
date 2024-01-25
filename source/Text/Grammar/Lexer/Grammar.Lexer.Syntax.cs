// <copyright file="Grammar.Lexer.Syntax.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class LexerGrammar
		{
			public abstract class Syntax : GrammarSyntax<Syntax, Production, Symbol>
			{
				protected Syntax(string name) : base(name)
				{
				}

				protected bool IsSealed { get; private set; }

				protected virtual void AcceptVisitor<TVisitor>(TVisitor visitor) where TVisitor : SyntaxVisitor
				{
					visitor.Visit(this);
				}

				public void Seal()
				{
					SealCore();
					
					IsSealed = true;
				}

				protected virtual void SealCore()
				{
				}

				protected virtual void VerifyUnsealed()
				{
					if (IsSealed)
						throw new InvalidOperationException("Syntax is sealed and can not be modified.");
				}

				public void Visit<TVisitor>(TVisitor visitor) where TVisitor : SyntaxVisitor
				{
					AcceptVisitor(visitor);
				}
			}

			protected internal abstract class SyntaxVisitor
			{
				public abstract void Visit(Syntax syntax);
			}
		}
	}
}