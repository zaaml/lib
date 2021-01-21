// <copyright file="ListGridViewColumn.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.ListView
{
	public class ListGridViewColumn : InheritanceContextObject
	{
		public static readonly DependencyProperty HeaderProperty = DPM.Register<object, ListGridViewColumn>
			("Header", default, d => d.OnHeaderPropertyChangedPrivate);

		public static readonly DependencyProperty MemberProperty = DPM.Register<string, ListGridViewColumn>
			("Member", default, d => d.OnMemberPropertyChangedPrivate);

		public object Header
		{
			get => GetValue(HeaderProperty);
			set => SetValue(HeaderProperty, value);
		}

		public string Member
		{
			get => (string) GetValue(MemberProperty);
			set => SetValue(MemberProperty, value);
		}

		private void OnHeaderPropertyChangedPrivate(object oldValue, object newValue)
		{
		}

		private void OnMemberPropertyChangedPrivate(string oldValue, string newValue)
		{
		}
	}
}