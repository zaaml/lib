// <copyright file="MemberValueEvaluator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Data
{
	internal class MemberEvaluator
	{
		private readonly BindingValueSite _valueSite;

		public MemberEvaluator(string member)
		{
			Member = member;

			if (Member == null)
			{
				_valueSite = null;

				return;
			}

			_valueSite = new BindingValueSite(Member);
		}

		public object GetValue(object source)
		{
			return _valueSite?.Evaluate(source);
		}

		public string Member { get; }

		private class BindingValueSite : DependencyObject
		{
			private static readonly DependencyProperty ValueProperty = DPM.Register<object, BindingValueSite>
				("Value");

			public static readonly DependencyProperty SourceProperty = DPM.Register<object, BindingValueSite>
				("Source");

			public BindingValueSite(string valuePath)
			{
				this.SetBinding(ValueProperty, new Binding($"Source.{valuePath}") {Source = this});
			}

			private object Source
			{
				set => SetValue(SourceProperty, value);
			}

			private object Value => GetValue(ValueProperty);

			public object Evaluate(object source)
			{
				Source = source;

				var value = Value;

				Source = null;

				return value;
			}
		}
	}
}