// <copyright file="Parser.Automata.ProductionEntity.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Runtime.CompilerServices;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class ProductionEntity
			{
				public readonly ProductionEntityArgument[] Arguments;
				public readonly ParserProduction ParserProduction;

				public bool Busy;
				public int ConsumeCount;

				public ProductionEntity(ParserProduction parserProduction)
				{
					ParserProduction = parserProduction;
					Arguments = new ProductionEntityArgument[parserProduction.Arguments.Count];

					for (var index = 0; index < Arguments.Length; index++)
						Arguments[index] = parserProduction.Arguments[index].CreateArgument(this);
				}

				internal ProductionEntity LeftFactor { get; set; }

				public object Result;

				public void OnAfterConsumeValue(int entryIndex)
				{
					ConsumeCount++;
				}

				public void Reset()
				{
					ConsumeCount = 0;
					Result = default;

					foreach (var argument in Arguments)
						argument.Reset();

					if (Busy)
						ParserProduction.ReturnEntity(this);
				}

				public override string ToString()
				{
					return ParserProduction.ToString();
				}

				public void BuildEntity(ValueParserAutomataContext context)
				{
					Result = ParserProduction.Binder.CreateInstance(this, context);
				}

				public void UnwindLeftFactorBuilder(ProductionEntity productionEntity)
				{
					throw new NotImplementedException();
					//foreach (var argument in EntityArguments) 
					//	LeftFactor.EntityArguments[argument.ArgumentIndex].ConsumeParserValue(argument.Build());
				}

				public void Return()
				{
					ParserProduction.ReturnEntity(this);
				}
				
				//[MethodImpl(MethodImplOptions.AggressiveInlining)]
				//public object CreateInstance(SyntaxFactory syntaxFactory)
				//{
				//	if (LeftFactorBuilder != null)
				//	{
				//		foreach (var argument in Arguments)
				//		{
				//			if (argument == null)
				//				continue;

				//			var consumeValue = argument.Build();

				//			LeftFactorBuilder.Arguments[argument.Index].ConsumeParserValue(consumeValue);
				//		}

				//		var factorInstance = LeftFactorBuilder._production.LeftFactorProduction.Binder.CreateInstanceDelegate(LeftFactorBuilder, syntaxFactory);

				//		LeftFactorBuilder = null;

				//		ConsumeCount = 0;
				//		_pool.Return(this);

				//		return factorInstance;
				//	}

				//	if (_tryReturn && ConsumeCount == 1)
				//	{
				//		var build = Arguments[0].Build();

				//		ConsumeCount = 0;
				//		_pool.Return(this);

				//		return build;
				//	}

				//	var instance = _createInstanceDelegate(this, syntaxFactory);

				//	ConsumeCount = 0;
				//	_pool.Return(this);

				//	return instance;
				//}
			}
		}
	}
}