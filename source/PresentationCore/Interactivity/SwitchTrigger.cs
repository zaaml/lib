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
		#region Static Fields and Constants

		private static readonly InteractivityProperty SourceValueProperty = RegisterInteractivityProperty(OnValueChanged);

		#endregion

		#region Fields

		private object _sourceValue;

		#endregion

		#region Ctors

		static SwitchTrigger()
		{
			RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);
		}

		public SwitchTrigger()
		{
			PackedDefinition.PropertyKind.SetValue(ref PackedValue, PropertyKind.Unspecified);
			PackedDefinition.SubjectKind.SetValue(ref PackedValue, SubjectKind.Unspecified);
		}

		#endregion

		#region Properties

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

		#endregion

		#region  Methods

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var triggerSource = (SwitchTrigger) source;

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

		private static void OnValueChanged(InteractivityObject interactivityobject, object oldvalue, object newvalue)
		{
			((SwitchTrigger) interactivityobject).OnValueChanged();
		}

		private void OnValueChanged()
		{
			UpdateTrigger();
		}

		internal override void UnloadCore(IInteractivityRoot root)
		{
			Unload(SourceValueProperty, ref _sourceValue);

			SubjectResolver.UnresolveSubject(this);
			PropertyResolver.UnresolveProperty(this);

			base.UnloadCore(root);
		}

		private void UpdateSourceBinding()
		{
			var source = ActualSource;
			var property = ActualProperty;

			if (property != null && source != null)
				SourceValue = new Binding {Path = new PropertyPath(property), Source = source};
			else
				SourceValue = null;
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
			OnActualSourceChanged(oldSubject);
		}

		#endregion

		#region IPropertySubject

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

		#endregion

		#endregion

		#region  Nested Types

		private static class PackedDefinition
		{
			#region Static Fields and Constants

			public static readonly PackedEnumItemDefinition<PropertyKind> PropertyKind;
			public static readonly PackedEnumItemDefinition<SubjectKind> SubjectKind;

			#endregion

			#region Ctors

			static PackedDefinition()
			{
				var allocator = GetAllocator<SwitchTrigger>();

				PropertyKind = allocator.AllocateEnumItem<PropertyKind>();
				SubjectKind = allocator.AllocateEnumItem<SubjectKind>();
			}

			#endregion
		}

		#endregion
	}
}