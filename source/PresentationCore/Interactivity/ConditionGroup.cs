// <copyright file="ConditionGroup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;

namespace Zaaml.PresentationCore.Interactivity
{
	[ContentProperty("Conditions")]
	public abstract class ConditionGroup : ConditionBase
	{
		#region Fields

		private ConditionCollection _conditions;

		#endregion

		#region Properties

		private IEnumerable<ConditionBase> ActualConditions => _conditions ?? Enumerable.Empty<ConditionBase>();

		public ConditionCollection Conditions => _conditions ?? (_conditions = new ConditionCollection(this));

		protected abstract ConditionLogicalOperator LogicalOperator { get; }

		#endregion

		#region  Methods

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);
			var conditionGroupSource = (ConditionGroup) source;
			_conditions = conditionGroupSource._conditions?.DeepCloneCollection<ConditionCollection, ConditionBase>(conditionGroupSource);
		}

		internal override void Deinitialize(IInteractivityRoot root)
		{
			foreach (var condition in ActualConditions)
				condition.Unload(root);

			base.Deinitialize(root);
		}

		internal override void Initialize(IInteractivityRoot root)
		{
			base.Initialize(root);

			foreach (var condition in ActualConditions)
				condition.Load(root);
		}

		protected override void OnIsEnabledIntChanged()
		{
			foreach (var condition in ActualConditions)
				condition.IsEnabled = IsEnabled;

			base.OnIsEnabledIntChanged();
		}

		protected override void UpdateConditionalTrigger()
		{
			UpdateConditionState();
		}

		protected override bool UpdateConditionStateCore()
		{
			if (_conditions == null || _conditions.Count == 0)
				return true;

			switch (LogicalOperator)
			{
				case ConditionLogicalOperator.And:
					return _conditions.All(c => c.IsOpen);
				case ConditionLogicalOperator.Or:
					return _conditions.Any(c => c.IsOpen);
      }

			return true;
		}

		#endregion
	}
}