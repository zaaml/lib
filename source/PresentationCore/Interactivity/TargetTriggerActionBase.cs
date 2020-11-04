// <copyright file="TargetTriggerActionBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;
using System.Windows;
using Zaaml.Core;
using Zaaml.Core.Packed;

namespace Zaaml.PresentationCore.Interactivity
{
	public abstract class TargetTriggerActionBase : TriggerActionBase, IInteractivitySourceSubject
	{
		#region Ctors

		static TargetTriggerActionBase()
		{
			RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);
		}

		protected TargetTriggerActionBase()
		{
			PackedDefinition.SubjectKind.SetValue(ref PackedValue, SubjectKind.Unspecified);
		}

		#endregion

		#region Properties

		protected DependencyObject ActualTarget => SubjectResolver.ResolveSubject(this);

		private SubjectKind SubjectKind
		{
			get => PackedDefinition.SubjectKind.GetValue(PackedValue);
			set => PackedDefinition.SubjectKind.SetValue(ref PackedValue, value);
		}

		public object Target
		{
			get => SubjectResolver.GetExplicitSubject(this);
			set => SubjectResolver.SetExplicitSubject(this, value);
		}

		public string TargetName
		{
			get => SubjectResolver.GetSubjectName(this);
			set => SubjectResolver.SetSubjectName(this, value);
		}

		#endregion

		#region  Methods

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var triggerSource = (TargetTriggerActionBase) source;
			SubjectResolver.CopyFrom(this, triggerSource);
		}

		internal override void LoadCore(IInteractivityRoot root)
		{
			base.LoadCore(root);
			SubjectResolver.ResolveSubject(this);
		}

		protected virtual void OnActualTargetChanged(DependencyObject oldTarget)
		{
		}

		[UsedImplicitly]
		internal void SetTargetProperty(object value)
		{
			Target = value;
		}

		internal override void UnloadCore(IInteractivityRoot root)
		{
			SubjectResolver.UnresolveSubject(this);
			base.UnloadCore(root);
		}

		#endregion

		#region Interface Implementations

		#region IInteractivitySubject

		object IInteractivitySubject.SubjectStore { get; set; }

		SubjectKind IInteractivitySubject.SubjectKind
		{
			get => SubjectKind;
			set => SubjectKind = value;
		}


		void IInteractivitySubject.OnSubjectChanged(DependencyObject oldSubject, DependencyObject newSubject)
		{
			OnActualTargetChanged(oldSubject);
		}

		#endregion

		#endregion

		#region  Nested Types

		private static class PackedDefinition
		{
			#region Static Fields and Constants

			public static readonly PackedEnumItemDefinition<SubjectKind> SubjectKind;

			#endregion

			#region Ctors

			static PackedDefinition()
			{
				var allocator = GetAllocator<TargetTriggerActionBase>();

				SubjectKind = allocator.AllocateEnumItem<SubjectKind>();
			}

			#endregion
		}

		#endregion
	}
}