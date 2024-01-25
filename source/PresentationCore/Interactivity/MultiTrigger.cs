// <copyright file="MultiTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Zaaml.Core.Packed;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class MultiTrigger : StateTriggerBase, IConditionalTrigger
	{
		private static readonly uint DefaultPackedValue;

		private ConditionCollection _conditions;

		static MultiTrigger()
		{
			RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);

			PackedDefinition.Modifier.SetValue(ref DefaultPackedValue, ConditionLogicalOperator.And);
		}

		public MultiTrigger()
		{
			PackedValue |= DefaultPackedValue;
		}

		private IEnumerable<ConditionBase> ActualConditions => _conditions ?? Enumerable.Empty<ConditionBase>();

		public ConditionCollection Conditions => _conditions ??= new ConditionCollection(this);

		public ConditionLogicalOperator LogicalOperator
		{
			get => PackedDefinition.Modifier.GetValue(PackedValue);
			set
			{
				PackedDefinition.Modifier.SetValue(ref PackedValue, value);
				UpdateTriggerState();
			}
		}

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var triggerSource = (MultiTrigger)source;

			LogicalOperator = triggerSource.LogicalOperator;
			_conditions = triggerSource._conditions?.DeepCloneCollection<ConditionCollection, ConditionBase>(this);
		}

		protected override InteractivityObject CreateInstance()
		{
			return new MultiTrigger();
		}

		internal override void DeinitializeTrigger(IInteractivityRoot root)
		{
			foreach (var condition in ActualConditions)
				condition.Unload(root);

			base.DeinitializeTrigger(root);
		}

		internal override void InitializeTrigger(IInteractivityRoot root)
		{
			base.InitializeTrigger(root);

			foreach (var condition in ActualConditions)
				condition.Load(root);
		}

		protected override void OnIsEnabledChanged()
		{
			foreach (var condition in ActualConditions)
				condition.IsEnabled = IsEnabled;

			base.OnIsEnabledChanged();
		}

		protected override TriggerState UpdateTriggerStateCore()
		{
			if (_conditions == null || _conditions.Count == 0)
				return TriggerState.Opened;

			return LogicalOperator switch
			{
				ConditionLogicalOperator.And => _conditions.All(c => c.IsOpen) ? TriggerState.Opened : TriggerState.Closed,
				ConditionLogicalOperator.Or => _conditions.Any(c => c.IsOpen) ? TriggerState.Opened : TriggerState.Closed,
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		void IConditionalTrigger.UpdateConditionalTrigger()
		{
			UpdateTriggerState();
		}

		private static class PackedDefinition
		{
			public static readonly PackedEnumItemDefinition<ConditionLogicalOperator> Modifier;

			static PackedDefinition()
			{
				var allocator = GetAllocator<MultiTrigger>();

				Modifier = allocator.AllocateEnumItem<ConditionLogicalOperator>();
			}
		}
	}

	internal interface IConditionalTrigger
	{
		void UpdateConditionalTrigger();
	}
}