// <copyright file="Or.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class Or : ConditionGroup
	{
		#region Properties

		protected override ConditionLogicalOperator LogicalOperator => ConditionLogicalOperator.Or;

		#endregion

		#region  Methods

		protected override InteractivityObject CreateInstance()
		{
			return new Or();
		}

		#endregion
	}
}