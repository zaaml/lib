// <copyright file="DisplayMemberGridCellGeneratorProperty.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Data;

namespace Zaaml.UI.Controls.Core.GridView
{
	internal sealed class GridViewCellGeneratorDisplayMemberProperty<TGridCell> : GridViewCellGeneratorBindingTargetProperty<TGridCell, object>
		where TGridCell : GridViewCell
	{
		public GridViewCellGeneratorDisplayMemberProperty(GridViewCellGenerator<TGridCell> generator) : base(generator)
		{
			MemberSourceProperty = new GridViewCellGeneratorSourceProperty<TGridCell, string>(this, StringComparer.Ordinal);
			MemberBindingSourceProperty = new GridViewCellGeneratorSourceProperty<TGridCell, Binding>(this);
		}

		private Binding ActualBinding { get; set; }

		protected override Binding BindingCore => ActualBinding;

		public string Member
		{
			get => MemberSourceProperty.Value;
			set => MemberSourceProperty.Value = value;
		}

		public Binding MemberBinding
		{
			get => MemberBindingSourceProperty.Value;
			set => MemberBindingSourceProperty.Value = value;
		}

		private GridViewCellGeneratorSourceProperty<TGridCell, Binding> MemberBindingSourceProperty { get; }

		private GridViewCellGeneratorSourceProperty<TGridCell, string> MemberSourceProperty { get; }

		protected override DependencyProperty TargetProperty => GridViewCell.ContentProperty;

		protected override void OnValueChanged(GridViewCellGeneratorSourceProperty<TGridCell> sourceProperty)
		{
			if (MemberBindingSourceProperty.Value != null)
				ActualBinding = MemberBindingSourceProperty.Value;
			else if (MemberSourceProperty.Value != null)
				ActualBinding = new Binding(MemberSourceProperty.Value);
			else
				ActualBinding = null;

			base.OnValueChanged(sourceProperty);
		}
	}
}