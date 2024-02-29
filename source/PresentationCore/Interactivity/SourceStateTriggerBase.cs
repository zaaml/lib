// <copyright file="SourceStateTriggerBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;
using System.Windows;
using Zaaml.Core.Packed;

namespace Zaaml.PresentationCore.Interactivity
{
	public abstract class SourceStateTriggerBase : StateTriggerBase, IInteractivitySourceSubject
	{
		private static readonly uint DefaultPackedValue;

		static SourceStateTriggerBase()
		{
			RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);

			PackedDefinition.SubjectKind.SetValue(ref DefaultPackedValue, SubjectKind.Unspecified);
		}

		internal SourceStateTriggerBase()
		{
			PackedValue |= DefaultPackedValue;
		}

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

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var triggerSource = (SourceStateTriggerBase)source;

			SubjectResolver.CopyFrom(this, triggerSource);
		}

		internal override void DeinitializeTrigger(IInteractivityRoot root)
		{
			SubjectResolver.UnResolveSubject(this);

			base.DeinitializeTrigger(root);
		}

		protected virtual void OnActualSourceChanged(DependencyObject oldSource)
		{
		}

		object IInteractivitySubject.SubjectStore { get; set; }

		SubjectKind IInteractivitySubject.SubjectKind
		{
			get => SubjectKind;
			set => SubjectKind = value;
		}

		void IInteractivitySubject.OnSubjectChanged(DependencyObject oldSubject, DependencyObject newSubject)
		{
			OnActualSourceChanged(oldSubject);
		}

		private static class PackedDefinition
		{
			public static readonly PackedEnumItemDefinition<SubjectKind> SubjectKind;

			static PackedDefinition()
			{
				var allocator = GetAllocator<SourceStateTriggerBase>();

				SubjectKind = allocator.AllocateEnumItem<SubjectKind>();
			}
		}
	}
}