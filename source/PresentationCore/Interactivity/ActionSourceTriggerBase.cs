// <copyright file="ActionSourceTriggerBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;
using System.Windows;
using Zaaml.Core.Packed;

namespace Zaaml.PresentationCore.Interactivity
{
	public abstract class ActionSourceTriggerBase : ActionTriggerBase, IInteractivitySourceSubject
	{
		#region Ctors

		static ActionSourceTriggerBase()
		{
			RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);
		}

		protected ActionSourceTriggerBase()
		{
			PackedDefinition.SubjectKind.SetValue(ref PackedValue, SubjectKind.Unspecified);
		}

		#endregion

		#region Properties

		protected DependencyObject ActualSource => SubjectResolver.ResolveSubject(this);

		public object Source
		{
			get => SubjectResolver.GetExplicitSubject(this);
			set => SubjectResolver.SetExplicitSubject(this, value);
		}

		public string SourceName
		{
			get => SubjectResolver.GetSubjectName(this);
			set => SubjectResolver.SetSubjectName(this, value);
		}

		private SubjectKind SubjectKind
		{
			get => PackedDefinition.SubjectKind.GetValue(PackedValue);
			set => PackedDefinition.SubjectKind.SetValue(ref PackedValue, value);
		}

		#endregion

		#region  Methods

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var triggerSource = (ActionSourceTriggerBase) source;
			SubjectResolver.CopyFrom(this, triggerSource);
		}

		protected virtual void OnActualSourceChanged(DependencyObject oldSource)
		{
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


		void IInteractivitySubject.OnSubjectChanged(DependencyObject oldTargetSource, DependencyObject newTargetSource)
		{
			OnActualSourceChanged(oldTargetSource);
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
				var allocator = GetAllocator<ActionSourceTriggerBase>();

				SubjectKind = allocator.AllocateEnumItem<SubjectKind>();
			}

			#endregion
		}

		#endregion
	}
}