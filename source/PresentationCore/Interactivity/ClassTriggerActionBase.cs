using System.Windows;
using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Interactivity
{
	public abstract class ClassTriggerActionBase : TargetTriggerActionBase
	{
		public string Class { get; set; }

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var classTriggerActionBase = (ClassTriggerActionBase)source;

			Class  = classTriggerActionBase.Class;
		}

		protected abstract void InvokeClassAction(DependencyObject dependencyObject, string className);

		protected override void InvokeCore()
		{
			if (Class.IsNullOrEmpty())
				return;

			var target = ActualTarget;

			if (target == null)
				return;

			InvokeClassAction(target, Class);
		}
	}
}