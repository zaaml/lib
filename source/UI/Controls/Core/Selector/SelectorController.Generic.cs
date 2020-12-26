// <copyright file="SelectorController.Generic.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using Zaaml.Core;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.Core
{
	internal class SelectorController<TSelector, TItem> : SelectorController<TItem>
		where TSelector : FrameworkElement, ISelector<TItem>
		where TItem : FrameworkElement
	{
		public SelectorController(TSelector selector, ISelectorAdvisor<TItem> advisor) : base(selector, advisor)
		{
		}

		private TSelector SelectorInt => (TSelector) Selector;

		protected override bool SupportsIndex => SelectorInt.SelectedIndexProperty != null;

		protected override bool SupportsItem => SelectorInt.SelectedItemProperty != null;

		protected override bool SupportsSource => SelectorInt.SelectedSourceProperty != null;

		protected override bool SupportsValue => SelectorInt.SelectedValueProperty != null;

		private void PushBoundValue(DependencyProperty property, [UsedImplicitly] object coercedValue)
		{
			if (SelectorInt.ReadLocalValue(property) is BindingExpression localBindingExpression)
				SelectorInt.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, () => localBindingExpression.UpdateSource());
		}

		protected override void PushSelectedIndexBoundValueCore(object coerceSelectedIndex)
		{
			PushBoundValue(SelectorInt.SelectedIndexProperty, coerceSelectedIndex);
		}

		protected override void PushSelectedItemBoundValueCore(TItem coerceSelectedItem)
		{
			PushBoundValue(SelectorInt.SelectedItemProperty, coerceSelectedItem);
		}

		protected override void PushSelectedSourceBoundValueCore(object coerceSelectedSource)
		{
			PushBoundValue(SelectorInt.SelectedSourceProperty, coerceSelectedSource);
		}

		protected override void PushSelectedValueBoundValueCore(object coerceSelectedValue)
		{
			PushBoundValue(SelectorInt.SelectedValueProperty, coerceSelectedValue);
		}

		private void PushValue(DependencyProperty property, object value)
		{
			SelectorInt.SetCurrentValueInternal(property, value);
		}

		protected override int ReadSelectedIndex()
		{
			return (int) SelectorInt.GetValue(SelectorInt.SelectedIndexProperty);
		}

		protected override TItem ReadSelectedItem()
		{
			return (TItem) SelectorInt.GetValue(SelectorInt.SelectedItemProperty);
		}

		protected override object ReadSelectedSource()
		{
			return SelectorInt.GetValue(SelectorInt.SelectedSourceProperty);
		}

		protected override object ReadSelectedValue()
		{
			return SelectorInt.GetValue(SelectorInt.SelectedValueProperty);
		}

		protected override void WriteSelectedIndex(int index)
		{
			PushValue(SelectorInt.SelectedIndexProperty, index);
		}

		protected override void WriteSelectedItem(TItem item)
		{
			PushValue(SelectorInt.SelectedItemProperty, item);
		}

		protected override void WriteSelectedSource(object source)
		{
			PushValue(SelectorInt.SelectedSourceProperty, source);
		}

		protected override void WriteSelectedValue(object value)
		{
			PushValue(SelectorInt.SelectedValueProperty, value);
		}

	}
}