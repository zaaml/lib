// <copyright file="Parser.Automata.ProductionEntity.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		protected readonly ref struct NodeContext
		{
			public NodeContext(object node, TextSpan textSpan)
			{
				Node = node;
				TextSpan = textSpan;
			}

			public readonly object Node;
			public readonly TextSpan TextSpan;
		}

		private sealed partial class ParserAutomata
		{
			private sealed class SourceEntityFactory
			{
				private readonly ParserProduction _sourceProduction;
				private readonly ParserProduction _targetProduction;
				private readonly (int, int)[] _argumentMappings;

				public SourceEntityFactory(ParserProduction targetProduction)
				{
					_targetProduction = targetProduction;

					var argumentMapping = new List<(ProductionArgument, ProductionArgument)>();
					var sourceProduction = targetProduction.SourceProduction;

					while (true)
					{
						foreach (var targetArgument in targetProduction.Arguments)
						{
							var sourceArgument = targetArgument.OriginalArgument;
							var currentMappingIndex = argumentMapping.FindIndex(t => ReferenceEquals(targetArgument, t.Item1));

							if (sourceArgument == null)
							{
								if (currentMappingIndex != -1)
									argumentMapping.RemoveAt(currentMappingIndex);
							}
							else
							{
								if (currentMappingIndex == -1)
								{
									if (ReferenceEquals(targetProduction, _targetProduction))
										argumentMapping.Add((sourceArgument, targetArgument));
								}
								else
								{
									var currentMapping = argumentMapping[currentMappingIndex];

									argumentMapping[currentMappingIndex] = (sourceArgument, currentMapping.Item2);
								}
							}
						}

						if (sourceProduction.SourceProduction == null)
							break;

						targetProduction = sourceProduction;
						sourceProduction = sourceProduction.SourceProduction;
					}

					_sourceProduction = sourceProduction;
					_argumentMappings = argumentMapping.Select(m => (m.Item1.ArgumentIndex, m.Item2.ArgumentIndex)).ToArray();
				}

				public ProductionEntity CreateSourceEntity(ProductionEntity entity)
				{
					var sourceEntity = _sourceProduction.RentEntity();

					foreach (var argumentMapping in _argumentMappings)
						entity.Arguments[argumentMapping.Item2].TransferValue(sourceEntity.Arguments[argumentMapping.Item1]);

					entity.Return();

					return sourceEntity;
				}
			}

			private sealed class ProductionEntity
			{
				public readonly ProductionEntityArgument[] Arguments;
				public readonly ParserProduction ParserProduction;

				public bool Busy;
				public ProductionEntity PoolNext;
				public int SpanStart;
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
					var end = process.InstructionStreamPosition;

					Result = ParserProduction.Binder.Bind(this, process);

					process.Parser.BuildNodeInternal(Result, process.GetSpan(SpanStart, end));
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
					for (var index = 0; index < Arguments.Length; index++)
					{
						var sourceArgument = Arguments[index];
						var targetProductionArgument = sourceArgument.Argument.OriginalArgument;

						if (targetProductionArgument == null)
							continue;

						var targetArgument = targetEntity.Arguments[targetProductionArgument.ArgumentIndex];

						sourceArgument.TransferValue(targetArgument);
					}
				}

				public ProductionEntity CreateSourceEntity()
				{
					var productionEntity = ParserProduction.SourceEntityFactory.CreateSourceEntity(this);

					productionEntity.SpanStart = SpanStart;

					return productionEntity;

					//var current = this;

					//while (true)
					//{
					//	var sourceEntity = current.ParserProduction.SourceProduction.RentEntity();

					//	current.TransferValues(sourceEntity);
					//	current.Return();

					//	if (sourceEntity.ParserProduction.SourceProduction != null)
					//	{
					//		current = sourceEntity;

					//		continue;
					//	}

					//	return sourceEntity;
					//}
				}

				

				public ProductionEntity EnterPosition(int instructionStreamPosition)
				{
					SpanStart =instructionStreamPosition;

					return this;
				}
			}
		}
	}
}