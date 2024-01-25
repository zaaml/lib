// <copyright file="Grammar.Parser.ProductionBinding.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core.Reflection;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class ParserGrammar
		{
			public abstract class ProductionBinding
			{
			}

			public abstract class ProductionNodeBinding
			{
				protected ProductionNodeBinding(Type nodeType)
				{
					NodeType = nodeType;
					ConstValue = GetConstValue(nodeType);
				}

				internal object ConstValue { get; }

				public Type NodeType { get; }

				private static object GetConstValue(Type nodeType)
				{
					var constructors = nodeType.GetConstructors(BF.IPNP);

					if (constructors.Length != 1 || constructors[0].IsPrivate == false)
						return null;

					var instanceProperty = nodeType.GetProperty("Instance", BF.SPNP);
					var instanceField = nodeType.GetField("Instance", BF.SPNP);

					if (instanceProperty != null && instanceProperty.PropertyType == nodeType)
						return instanceProperty.GetValue(null);

					if (instanceField != null && instanceField.FieldType == nodeType)
						return instanceField.GetValue(null);

					return null;
				}
			}
		}
	}
}