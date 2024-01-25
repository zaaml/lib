// <copyright file="PropertyChangeTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using Zaaml.Core.Packed;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class PropertyChangeTrigger : ActionSourceTriggerBase, IPropertySubject
	{
		private static readonly InteractivityProperty SourceValueProperty = RegisterInteractivityProperty(OnValueChanged);
		private static readonly uint DefaultPackedValue;

		private object _sourceValue;

		static PropertyChangeTrigger()
		{
			RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);

			PackedDefinition.PropertyKind.SetValue(ref DefaultPackedValue, PropertyKind.Unspecified);
		}

		private DependencyProperty ActualProperty => PropertyResolver.ResolveProperty(this);

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

		private object SourceValue
		{
			set => SetValue(SourceValueProperty, ref _sourceValue, value);
		}

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var triggerSource = (PropertyChangeTrigger)source;

			PropertyResolver.CopyFrom(this, triggerSource);
		}

		protected override InteractivityObject CreateInstance()
		{
			return new PropertyChangeTrigger();
		}

		internal override void LoadCore(IInteractivityRoot root)
		{
			base.LoadCore(root);

			UpdateSourceBinding();
		}

		internal override void UnloadCore(IInteractivityRoot root)
		{
			SourceValue = null;

			base.UnloadCore(root);
		}

		private void OnActualPropertyChanged(DependencyProperty oldProperty, DependencyProperty newProperty)
		{
			UpdateSourceBinding();
		}

		private void OnPropertyValueChanged()
		{
			Invoke();
		}

		protected override void OnActualSourceChanged(DependencyObject oldSource)
		{
			base.OnActualSourceChanged(oldSource);

			UpdateSourceBinding();
		}

		private static void OnValueChanged(InteractivityObject interactivityObject, object oldValue, object newValue)
		{
			((PropertyChangeTrigger)interactivityObject).OnPropertyValueChanged();
		}

		private void UpdateSourceBinding()
		{
			var source = ActualSource;
			var property = ActualProperty;

			SourceValue = property != null && source != null ? new Binding { Path = new PropertyPath(property), Source = source } : null;
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

			static PackedDefinition()
			{
				var allocator = GetAllocator<Trigger>();

				PropertyKind = allocator.AllocateEnumItem<PropertyKind>();
			}
		}
	}
}