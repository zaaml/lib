// <copyright file="Parser.Automata.ProcessAutomataContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		#region Nested Types

		private sealed partial class ParserAutomata
		{
			#region Nested Types

			private sealed class ProcessAutomataContext : ParserAutomataContext
			{
				#region Ctors

				public ProcessAutomataContext(ParserState state, ParserAutomata parserAutomata) : base(state, parserAutomata)
				{
				}

				#endregion

				#region Properties

				private protected override Type ILBuilderType => typeof(ProcessAutomataContext);

				#endregion

				#region Methods

				protected override void BuildEnterProduction(ILBuilderContext ilBuilderContext, Production production)
				{
				}

				protected override void BuildEnterStateEntry(ILBuilderContext ilBuilderContext, StateEntry stateEntry)
				{
				}

				protected override void BuildInvoke(ILBuilderContext ilBuilderContext, MatchEntry matchEntry, bool main)
				{
				}

				protected override void BuildLeaveProduction(ILBuilderContext ilBuilderContext, Production production)
				{
				}

				protected override void BuildLeaveStateEntry(ILBuilderContext ilBuilderContext, StateEntry stateEntry)
				{
				}

				public override void Dispose()
				{
					base.Dispose();

					((ParserState) State).ReleaseProcessContext(this);
				}

				#endregion
			}

			#endregion
		}

		#endregion
	}
}