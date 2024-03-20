// <copyright file="PropertyActionBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;
using System.Windows;
using Zaaml.Core.Packed;

namespace Zaaml.PresentationCore.Interactivity
{
	public abstract class PropertyActionBase : TargetTriggerActionBase, IPropertySubject
	{
		static PropertyActionBase()
		{
			RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);
		}

		protected PropertyActionBase()
		{
			PackedDefinition.PropertyKind.SetValue(ref PackedValue, PropertyKind.Unspecified);
		}

		internal DependencyProperty ActualProperty => PropertyResolver.ResolveProperty(this);

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

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var propertyActionSource = (PropertyActionBase)source;

			PropertyResolver.CopyFrom(this, propertyActionSource);
		}

		protected virtual void OnActualPropertyChanged(DependencyProperty oldProperty, DependencyProperty newProperty)
		{
		}

		internal override void UnloadCore(IInteractivityRoot root)
		{
			PropertyResolver.UnResolveProperty(this);

			base.UnloadCore(root);
		}

		DependencyObject IPropertySubject.ActualSubject => ActualTarget;

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
				var allocator = GetAllocator<PropertyActionBase>();

				PropertyKind = allocator.AllocateEnumItem<PropertyKind>();
			}
		}
	}
}