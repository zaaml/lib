// <copyright file="TriggerComparer.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Interactivity
{
	internal class TriggerComparer : ITriggerValueComparer
	{
		#region Static Fields and Constants

		private static readonly Dictionary<ComparerOperator, TriggerComparer> Comparers = new Dictionary<ComparerOperator, TriggerComparer>();

		#endregion

		#region Fields

		private readonly ComparerOperator _comparerOperator;

		#endregion

		#region Ctors

		private TriggerComparer(ComparerOperator comparerOperator)
		{
			_comparerOperator = comparerOperator;
		}

		#endregion

		#region  Methods

		public static ITriggerValueComparer GetComparer(ComparerOperator comparerOperator)
		{
			return Comparers.GetValueOrCreate(comparerOperator, () => new TriggerComparer(comparerOperator));
		}

		#endregion

		#region Interface Implementations

		#region ITriggerValueComparer

		public bool Compare(object triggerSourceValue, object operand)
		{
			return XamlValueComparer.EvaluateCompare(triggerSourceValue, operand, _comparerOperator);
		}

		#endregion

		#endregion
	}
}