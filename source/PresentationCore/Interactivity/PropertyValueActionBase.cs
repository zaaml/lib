// <copyright file="PropertyValueActionBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Interactivity
{
	public abstract class PropertyValueActionBase : PropertyActionBase
	{
		#region Static Fields and Constants

		private static readonly InteractivityProperty ValueProperty = RegisterInteractivityProperty(null);

		#endregion

		#region Fields

		private object _value;

		#endregion

		#region Ctors

		internal PropertyValueActionBase()
		{
		}

		#endregion

		#region Properties

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

		#endregion

		#region  Methods

		[UsedImplicitly]
		internal void SetValueProperty(object value)
		{
			SetValue(ValueProperty, ref _value, value);
		}

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var setPropertyValueSource = (PropertyValueActionBase) source;

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

		internal override void UnloadCore(IInteractivityRoot root)
		{
			Unload(ValueProperty, ref _value);
			base.UnloadCore(root);
		}

		#endregion
	}
}