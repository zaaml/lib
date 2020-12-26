// <copyright file="TriggerComparer.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class TriggerComparer : ITriggerValueComparer
	{
		private static readonly Dictionary<ComparerOperator, TriggerComparer> Comparers = new Dictionary<ComparerOperator, TriggerComparer>();

		private TriggerComparer(ComparerOperator comparerOperator)
		{
			Operator = comparerOperator;
		}

		public ComparerOperator Operator { get; }

		public static ITriggerValueComparer GetComparer(ComparerOperator comparerOperator)
		{
			return Comparers.GetValueOrCreate(comparerOperator, () => new TriggerComparer(comparerOperator));
		}

		public bool Compare(object triggerSourceValue, object operand)
		{
			return XamlValueComparer.EvaluateCompare(triggerSourceValue, operand, Operator);
		}
	}
}