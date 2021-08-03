// <copyright file="Parser.Automata.ParserState.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		#region Nested Types

		private sealed partial class ParserAutomata
		{
			#region Nested Types

			private sealed class ParserRule : Rule<ParserAutomataContext>
			{
				#region Ctors

				public ParserRule(string name, bool inline) : base(name)
				{
					Inline = inline;

					if (string.IsNullOrEmpty(name) && Inline == false)
					{
					}
				}

				#endregion

				#region Properties

				private Stack<ProcessAutomataContext> ProcessContextPool { get; } = new Stack<ProcessAutomataContext>();

				private Stack<SyntaxTreeAutomataContext> SyntaxTreeContextPool { get; } = new Stack<SyntaxTreeAutomataContext>();

				private Dictionary<Type, Stack<VisitorAutomataContext>> VisitorContextPoolDictionary { get; } = new Dictionary<Type, Stack<VisitorAutomataContext>>();

				#endregion

				#region Methods

				private Stack<VisitorAutomataContext> GetVisitorContextPool(Type visitorType)
				{
					lock (VisitorContextPoolDictionary)
					{
						if (VisitorContextPoolDictionary.TryGetValue(visitorType, out var pool))
							return pool;

						return VisitorContextPoolDictionary[visitorType] = new Stack<VisitorAutomataContext>();
					}
				}

				public ProcessAutomataContext MountProcessContext(ParserAutomata parserAutomata, LexemeSource<TToken> lexemeSource, ParserContext parserContext, Parser<TGrammar, TToken> parser)
				{
					ProcessAutomataContext context;

					lock (ProcessContextPool)
						context = ProcessContextPool.Count > 0 ? ProcessContextPool.Pop() : new ProcessAutomataContext(this, parserAutomata);

					context.Mount(lexemeSource, parserContext, parser);

					return context;
				}

				public SyntaxTreeAutomataContext MountSyntaxTreeContext(ParserAutomata parserAutomata, LexemeSource<TToken> lexemeSource, ParserContext parserContext, Parser<TGrammar, TToken> parser)
				{
					SyntaxTreeAutomataContext context;

					lock (SyntaxTreeContextPool)
						context = SyntaxTreeContextPool.Count > 0 ? SyntaxTreeContextPool.Pop() : new SyntaxTreeAutomataContext(this, parserAutomata);

					context.Mount(lexemeSource, parserContext, parser);

					return context;
				}

				public VisitorAutomataContext MountVisitorContext(Visitor visitor, ParserAutomata parserAutomata, LexemeSource<TToken> lexemeSource, ParserContext parserContext, Parser<TGrammar, TToken> parser)
				{
					var pool = GetVisitorContextPool(visitor.GetType());

					VisitorAutomataContext context;

					lock (pool)
						context = pool.Count > 0 ? pool.Pop() : new VisitorAutomataContext(visitor, this, parserAutomata);

					context.Mount(lexemeSource, parserContext, parser);

					return context;
				}

				public void ReleaseProcessContext(ProcessAutomataContext automataContext)
				{
					lock (ProcessContextPool)
						ProcessContextPool.Push(automataContext);
				}

				public void ReleaseSyntaxTreeContext(SyntaxTreeAutomataContext automataContext)
				{
					lock (SyntaxTreeContextPool)
						SyntaxTreeContextPool.Push(automataContext);
				}

				public void ReleaseVisitorContext(VisitorAutomataContext automataContext)
				{
					var pool = GetVisitorContextPool(automataContext.Visitor.GetType());

					lock (pool)
						pool.Push(automataContext);
				}

				#endregion
			}

			#endregion
		}

		#endregion
	}
}