// <copyright file="Condition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using Zaaml.Core.Packed;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class Condition : ConditionBase, IInteractivitySourceSubject, IPropertySubject
	{
		private static readonly InteractivityProperty TargetValueProperty = RegisterInteractivityProperty(UpdateConditionState);
		private static readonly InteractivityProperty SourceValueProperty = RegisterInteractivityProperty(UpdateConditionState);

		private ITriggerValueComparer _comparer;
		private object _sourceValue;
		private object _targetValue;

		static Condition()
		{
			RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);
		}

		public Condition()
		{
			PackedDefinition.SubjectKind.SetValue(ref PackedValue, SubjectKind.Unspecified);
			PackedDefinition.PropertyKind.SetValue(ref PackedValue, PropertyKind.Unspecified);
		}

		private DependencyProperty ActualProperty => PropertyResolver.ResolveProperty(this);

		private DependencyObject ActualSource => SubjectResolver.ResolveSubject(this);

		public ITriggerValueComparer Comparer
		{
			get => _comparer;
			set
			{
				if (ReferenceEquals(_comparer, value))
					return;

				_comparer = value;

				UpdateConditionState();
			}
		}

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
			set => SetValue(SourceValueProperty, ref _sourceValue, value);
		}

		private SubjectKind SubjectKind
		{
			get => PackedDefinition.SubjectKind.GetValue(PackedValue);
			set => PackedDefinition.SubjectKind.SetValue(ref PackedValue, value);
		}

		public object Value
		{
			get => GetOriginalValue(TargetValueProperty, _targetValue);
			set => SetValue(TargetValueProperty, ref _targetValue, value);
		}

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var conditionSource = (Condition)source;

			SubjectResolver.CopyFrom(this, conditionSource);
			PropertyResolver.CopyFrom(this, conditionSource);

			Value = conditionSource.Value;
			Comparer = conditionSource.Comparer;
		}

		protected override InteractivityObject CreateInstance()
		{
			return new Condition();
		}

		internal override void Deinitialize(IInteractivityRoot root)
		{
			Unload(TargetValueProperty, ref _targetValue);
			Unload(SourceValueProperty, ref _sourceValue);

			PropertyResolver.UnResolveProperty(this);

			base.Deinitialize(root);
		}

		internal override void Initialize(IInteractivityRoot root)
		{
			Load(TargetValueProperty, ref _targetValue);
			UpdateSourceBinding();

			base.Initialize(root);
		}

		private void OnActualPropertyChanged(DependencyProperty oldProperty, DependencyProperty newProperty)
		{
			UpdateSourceBinding();
		}

		private void OnActualSourceChanged(DependencyObject oldSource)
		{
			UpdateSourceBinding();
		}

		private static void UpdateConditionState(InteractivityObject interactivityObject, object oldValue, object newValue)
		{
			((Condition)interactivityObject).UpdateConditionState();
		}

		protected override bool UpdateConditionStateCore()
		{
			return TriggerCompareUtil.UpdateState(this, SourceValueProperty, ref _sourceValue, TargetValueProperty, ref _targetValue, Comparer) == TriggerState.Opened;
		}

		private void UpdateSourceBinding()
		{
			var source = ActualSource;
			var property = ActualProperty;

			SourceValue = property != null && source != null ? new Binding { Path = new PropertyPath(property), Source = source } : null;
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

		PropertyKind IPropertySubject.PropertyKind
		{
			get => PropertyKind;
			set => PropertyKind = value;
		}

		DependencyObject IPropertySubject.ActualSubject => ActualSource;

		void IPropertySubject.OnPropertyChanged(DependencyProperty oldProperty, DependencyProperty newProperty)
		{
			OnActualPropertyChanged(oldProperty, newProperty);
		}

		object IPropertySubject.PropertyStore { get; set; }

		private static class PackedDefinition
		{
			public static readonly PackedEnumItemDefinition<PropertyKind> PropertyKind;
			public static readonly PackedEnumItemDefinition<SubjectKind> SubjectKind;

			static PackedDefinition()
			{
				var allocator = GetAllocator<Condition>();

				PropertyKind = allocator.AllocateEnumItem<PropertyKind>();
				SubjectKind = allocator.AllocateEnumItem<SubjectKind>();
			}
		}
	}
}