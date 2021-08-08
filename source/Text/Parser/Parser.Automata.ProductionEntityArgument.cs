// <copyright file="Parser.Automata.ProductionEntityArgument.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private abstract class ProductionEntityArgument
			{
				protected ProductionEntityArgument(ProductionEntity entity, ProductionArgument argument)
				{
					Entity = entity;
					Argument = argument;
				}

				public ProductionArgument Argument { get; }

				public int ArgumentIndex => Argument.ArgumentIndex;

				public Type ArgumentType => Argument.ArgumentType;

				public ProductionEntity Entity { get; }

				public abstract object Build();

				public virtual void ConsumeValue(object value)
				{
				}

				public abstract int GetCount();

				public abstract void Reset();

				public override string ToString()
				{
					return ArgumentType.Name;
				}

				public abstract void TransferValue(ProductionEntityArgument argument);
			}
		}
	}
}