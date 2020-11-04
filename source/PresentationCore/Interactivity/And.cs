// <copyright file="And.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class And : ConditionGroup
	{
		#region Properties

		protected override ConditionLogicalOperator LogicalOperator => ConditionLogicalOperator.And;

		#endregion

		#region  Methods

		protected override InteractivityObject CreateInstance()
		{
			return new And();
		}

		#endregion
	}
}