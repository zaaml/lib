// <copyright file="Parser.Automata.VisitorAutomataContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		#region Nested Types

		private sealed partial class ParserAutomata
		{
			#region Nested Types

			private sealed class VisitorAutomataContext : ValueParserAutomataContext
			{
				#region Ctors

				public VisitorAutomataContext(Visitor visitor, ParserState state, ParserAutomata parserAutomata) : base(state, parserAutomata)
				{
					Visitor = visitor;
				}

				#endregion

				#region Properties

				private protected override Type ILBuilderType => typeof(VisitorAutomataContext);

				public Visitor Visitor { get; }

				#endregion

				#region Methods

				public override void Dispose()
				{
					base.Dispose();

					((ParserState) State).ReleaseVisitorContext(this);
				}

				#endregion
			}

			#endregion
		}

		#endregion
	}
}