// <copyright file="SpyMultiTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Spy
{
	public class SpyMultiTrigger : SpyTrigger
	{
		public static readonly DependencyProperty OperatorProperty = DPM.Register<SpyMultiTriggerOperator, SpyMultiTrigger>
			("Operator", SpyMultiTriggerOperator.And, d => d.OnOperatorPropertyChangedPrivate);

		private static readonly DependencyPropertyKey TriggersPropertyKey = DPM.RegisterReadOnly<SpyTriggerCollection, SpyMultiTrigger>
			("TriggersPrivate");

		public static readonly DependencyProperty TriggersProperty = TriggersPropertyKey.DependencyProperty;

		public SpyMultiTriggerOperator Operator
		{
			get => (SpyMultiTriggerOperator) GetValue(OperatorProperty);
			set => SetValue(OperatorProperty, value);
		}

		public SpyTriggerCollection Triggers => this.GetValueOrCreate(TriggersPropertyKey, () => new SpyTriggerCollection(this));

		private void OnOperatorPropertyChangedPrivate(SpyMultiTriggerOperator oldValue, SpyMultiTriggerOperator newValue)
		{
			UpdateState();
		}

		private void UpdateState()
		{
			if (Triggers.Count == 0)
			{
				IsOpen = true;

				return;
			}

			IsOpen = Operator switch
			{
				SpyMultiTriggerOperator.And => Triggers.All(t => t.IsOpen),
				SpyMultiTriggerOperator.Or => Triggers.Any(t => t.IsOpen),
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		internal void UpdateStateInternal()
		{
			UpdateState();
		}
	}
}