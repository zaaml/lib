// <copyright file="MemberValueEvaluator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Data
{
	internal struct MemberValueEvaluator
	{
		private readonly BindingValueSite _valueSite;

		public MemberValueEvaluator(string valuePath)
		{
			ValuePath = valuePath;

			if (ValuePath == null)
			{
				_valueSite = null;
				return;
			}

			_valueSite = new BindingValueSite(ValuePath);
		}

		public object GetValue(object source)
		{
			return _valueSite?.Evaluate(source);
		}

		public string ValuePath { get; }

		private class BindingValueSite : DependencyObject
		{
			#region Static Fields and Constants

			private static readonly DependencyProperty ValueProperty = DPM.Register<object, BindingValueSite>
				("Value");

			public static readonly DependencyProperty SourceProperty = DPM.Register<object, BindingValueSite>
				("Source");

			#endregion

			#region Ctors

			public BindingValueSite(string valuePath)
			{
				this.SetBinding(ValueProperty, new Binding($"Source.{valuePath}") { Source = this });
			}

			#endregion

			#region Properties

			private object Source
			{
				set => SetValue(SourceProperty, value);
			}

			private object Value => GetValue(ValueProperty);

			#endregion

			#region  Methods

			public object Evaluate(object source)
			{
				Source = source;

				var value = Value;

				Source = null;

				return value;
			}

			#endregion
		}
	}
}