// <copyright file="PropertyValueActionBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Interactivity
{
	public abstract class PropertyValueActionBase : PropertyActionBase
	{
		private static readonly InteractivityProperty ValueProperty = RegisterInteractivityProperty(null);

		private object _value;

		internal PropertyValueActionBase()
		{
		}

		protected object ActualValue
		{
			get
			{
				var actualProperty = ActualProperty;

				return actualProperty == null ? Value : CacheConvert(ValueProperty, actualProperty.GetPropertyType(), ref _value);
			}
		}


		public object Value
		{
			get => GetOriginalValue(ValueProperty, _value);
			set => SetValue(ValueProperty, ref _value, value);
		}

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var setPropertyValueSource = (PropertyValueActionBase)source;

			Value = setPropertyValueSource.Value;
		}

		protected sealed override void InvokeCore()
		{
			var actualProperty = ActualProperty;
			var actualTarget = ActualTarget;

			if (actualTarget == null || actualProperty == null)
				return;

			InvokeOverride();
		}


		protected abstract void InvokeOverride();

		internal override void LoadCore(IInteractivityRoot root)
		{
			base.LoadCore(root);

			Load(ValueProperty, ref _value);
		}

		[UsedImplicitly]
		internal void SetValueProperty(object value)
		{
			SetValue(ValueProperty, ref _value, value);
		}

		internal override void UnloadCore(IInteractivityRoot root)
		{
			Unload(ValueProperty, ref _value);

			base.UnloadCore(root);
		}
	}
}