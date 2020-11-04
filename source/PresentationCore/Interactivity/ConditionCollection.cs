// <copyright file="ConditionCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class ConditionCollection : InteractivityCollection<ConditionBase>
	{
		#region Ctors

		internal ConditionCollection(IInteractivityObject parent) : base(parent)
		{
		}

		#endregion

		#region  Methods

		internal override InteractivityCollection<ConditionBase> CreateInstance(IInteractivityObject parent)
		{
			return new ConditionCollection(parent);
		}

		#endregion
	}
}