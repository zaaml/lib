// <copyright file="SwitchTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using Zaaml.Core.Packed;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class SwitchTrigger : SwitchTriggerBase, IPropertySubject
	{
		private static readonly InteractivityProperty SourceValueProperty = RegisterInteractivityProperty(OnValueChanged);

		private object _sourceValue;

		static SwitchTrigger()
		{
			RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);
		}

		public SwitchTrigger()
		{
			PackedDefinition.PropertyKind.SetValue(ref PackedValue, PropertyKind.Unspecified);
			PackedDefinition.SubjectKind.SetValue(ref PackedValue, SubjectKind.Unspecified);
		}

		private DependencyProperty ActualProperty => PropertyResolver.ResolveProperty(this);

		private DependencyObject ActualSource => SubjectResolver.ResolveSubject(this);

		protected override object ActualSourceValue => SourceValue;

		public string ExpandoProperty
		{
			get => PropertyResolver.GetExpandoProperty(this);
			set => PropertyResolver.SetExpandoProperty(this, value);
		}

		public object Property
		{
			get => PropertyResolver.GetProperty(this);
			set => PropertyResolver.SetProperty(this, value);
		}

		private PropertyKind PropertyKind
		{
			get => PackedDefinition.PropertyKind.GetValue(PackedValue);
			set => PackedDefinition.PropertyKind.SetValue(ref PackedValue, value);
		}

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

		private object SourceValue
		{
			get => GetValue(SourceValueProperty, ref _sourceValue);
			set => SetValue(SourceValueProperty, ref _sourceValue, value);
		}

		private SubjectKind SubjectKind
		{
			get => PackedDefinition.SubjectKind.GetValue(PackedValue);
			set => PackedDefinition.SubjectKind.SetValue(ref PackedValue, value);
		}

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var triggerSource = (SwitchTrigger)source;

			SubjectResolver.CopyFrom(this, triggerSource);
			PropertyResolver.CopyFrom(this, triggerSource);
		}

		protected override InteractivityObject CreateInstance()
		{
			return new SwitchTrigger();
		}

		internal override void LoadCore(IInteractivityRoot root)
		{
			UpdateSourceBinding();

			base.LoadCore(root);

			UpdateTrigger();
		}

		private void OnActualPropertyChanged(DependencyProperty oldProperty, DependencyProperty newProperty)
		{
		}

		private void OnActualSourceChanged(DependencyObject oldSource)
		{
			UpdateSourceBinding();
		}

		private static void OnValueChanged(InteractivityObject interactivityObject, object oldValue, object newValue)
		{
			((SwitchTrigger)interactivityObject).OnValueChanged();
		}

		private void OnValueChanged()
		{
			UpdateTrigger();
		}

		internal override void UnloadCore(IInteractivityRoot root)
		{
			Unload(SourceValueProperty, ref _sourceValue);

			SubjectResolver.UnResolveSubject(this);
			PropertyResolver.UnResolveProperty(this);

			base.UnloadCore(root);
		}

		private void UpdateSourceBinding()
		{
			var source = ActualSource;
			var property = ActualProperty;

			if (property != null && source != null)
				SourceValue = new Binding { Path = new PropertyPath(property), Source = source };
			else
				SourceValue = null;
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

		DependencyObject IPropertySubject.ActualSubject => ActualSource;

		PropertyKind IPropertySubject.PropertyKind
		{
			get => PropertyKind;
			set => PropertyKind = value;
		}

		object IPropertySubject.PropertyStore { get; set; }

		void IPropertySubject.OnPropertyChanged(DependencyProperty oldProperty, DependencyProperty newProperty)
		{
			OnActualPropertyChanged(oldProperty, newProperty);
		}

		private static class PackedDefinition
		{
			public static readonly PackedEnumItemDefinition<PropertyKind> PropertyKind;
			public static readonly PackedEnumItemDefinition<SubjectKind> SubjectKind;

			static PackedDefinition()
			{
				var allocator = GetAllocator<SwitchTrigger>();

				PropertyKind = allocator.AllocateEnumItem<PropertyKind>();
				SubjectKind = allocator.AllocateEnumItem<SubjectKind>();
			}
		}
	}
}