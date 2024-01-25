// <copyright file="Or.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class Or : ConditionGroup
	{
		protected override ConditionLogicalOperator LogicalOperator => ConditionLogicalOperator.Or;

		protected override InteractivityObject CreateInstance()
		{
			return new Or();
		}
	}
}