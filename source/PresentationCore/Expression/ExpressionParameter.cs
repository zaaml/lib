// <copyright file="ExpressionParameter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.Core;
using Zaaml.Expressions;
using Zaaml.PresentationCore.Converters;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore
{
	public sealed class ExpressionParameter : InheritanceContextObject, IExpressionParameter
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty ValueProperty = DPM.Register<object, ExpressionParameter>
			(nameof(Value), e => e.OnValueChanged);

		public static readonly DependencyProperty NameProperty = DPM.Register<string, ExpressionParameter>
			(nameof(Name), e => e.OnNameChanged);

		#endregion

		#region Type: Fields

		private XamlConvertCacheStruct _valueCache;

		#endregion

		#region Ctors

		public ExpressionParameter()
		{
		}

		public ExpressionParameter(string name, object value)
		{
			Name = name;
			Value = value;
		}

		#endregion

		#region Properties

		internal ExpressionParameterCollection Collection { get; set; }

		public string Name
		{
			get => (string) GetValue(NameProperty);
			set => SetValue(NameProperty, value);
		}

		public object Value
		{
			get => GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		#endregion

		#region  Methods

		internal object ConvertValue(Type targetType)
		{
			return _valueCache.XamlConvert(targetType);
		}

		private void OnNameChanged(string oldName, string newName)
		{
			Collection?.OnParameterNameChanged(this, oldName, newName);
		}

		private void OnValueChanged(object oldValue, object newValue)
		{
			_valueCache.Value = newValue;
			Collection?.OnParameterValueChanged(this, oldValue, newValue);
		}

		#endregion

		#region Interface Implementations

		#region IExpressionParameter

		object IExpressionParameter.Value => Value;

		#endregion

		#endregion

		protected override bool FreezeCore(bool isChecking)
		{
			return false;
		}
	}
}