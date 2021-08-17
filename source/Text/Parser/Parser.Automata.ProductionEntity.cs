// <copyright file="Parser.Automata.ProductionEntity.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class ProductionEntity
			{
				public readonly ProductionEntityArgument[] Arguments;
				public readonly ParserProduction ParserProduction;

				public bool Busy;

				public object Result;

				public ProductionEntity(ParserProduction parserProduction)
				{
					ParserProduction = parserProduction;
					Arguments = new ProductionEntityArgument[parserProduction.Arguments.Count];

					for (var index = 0; index < Arguments.Length; index++)
						Arguments[index] = parserProduction.Arguments[index].CreateArgument(this);
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void CreateEntityInstance(ParserProcess process)
				{
					Result = ParserProduction.Binder.Bind(this, process);
				}

				public void Reset()
				{
					Result = default;

					foreach (var argument in Arguments)
						argument.Reset();

					if (Busy)
						ParserProduction.ReturnEntity(this);
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void Return()
				{
					ParserProduction.ReturnEntity(this);
				}

				public override string ToString()
				{
					return ParserProduction.ToString();
				}

				public void TransferValues(ProductionEntity targetEntity)
				{
					foreach (var sourceArgument in Arguments)
					{
						var targetProductionArgument = sourceArgument.Argument.OriginalArgument;

						if (targetProductionArgument == null)
							continue;

						var targetArgument = targetEntity.Arguments[targetProductionArgument.ArgumentIndex];

						sourceArgument.TransferValue(targetArgument);
					}
				}

				public ProductionEntity CreateSourceEntity()
				{
					var current = this;

					while (true)
					{
						var sourceEntity = current.ParserProduction.SourceProduction.RentEntity();

						current.TransferValues(sourceEntity);

						if (sourceEntity.ParserProduction.SourceProduction != null)
						{
							current = sourceEntity;

							continue;
						}

						return sourceEntity;
					}

				}
			}
		}
	}
}