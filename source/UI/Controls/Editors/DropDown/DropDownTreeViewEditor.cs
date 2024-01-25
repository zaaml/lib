// <copyright file="DropDownTreeViewEditor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.DropDown;
using Zaaml.UI.Controls.Editors.Core;
using Zaaml.UI.Controls.TreeView;

namespace Zaaml.UI.Controls.Editors.DropDown
{
	[TemplateContractType(typeof(DropDownTreeViewEditorTemplateContract))]
	public class DropDownTreeViewEditor : DropDownEditorBase
	{
		public static readonly DependencyProperty IsTextEditableProperty = DPM.Register<bool, DropDownTreeViewEditor>
			("IsTextEditable", false);

		public static readonly DependencyProperty DisplayModeProperty = DPM.Register<DropDownEditableSelectorDisplayMode, DropDownTreeViewEditor>
			("DisplayMode", DropDownEditableSelectorDisplayMode.TextEditor);

		public static readonly DependencyProperty TreeViewControlProperty = DPM.Register<TreeViewControl, DropDownTreeViewEditor>
			("TreeViewControl");

		static DropDownTreeViewEditor()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<DropDownTreeViewEditor>();
		}

		public DropDownTreeViewEditor()
		{
			this.OverrideStyleKey<DropDownTreeViewEditor>();
		}

		public DropDownEditableSelectorDisplayMode DisplayMode
		{
			get => (DropDownEditableSelectorDisplayMode) GetValue(DisplayModeProperty);
			set => SetValue(DisplayModeProperty, value);
		}

		private DropDownTreeViewControl DropDownTreeViewControl => TemplateContract.DropDownTreeViewControl;

		public bool IsTextEditable
		{
			get => (bool) GetValue(IsTextEditableProperty);
			set => SetValue(IsTextEditableProperty, value.Box());
		}

		private DropDownTreeViewEditorTemplateContract TemplateContract => (DropDownTreeViewEditorTemplateContract) TemplateContractCore;

		public TreeViewControl TreeViewControl
		{
			get => (TreeViewControl) GetValue(TreeViewControlProperty);
			set => SetValue(TreeViewControlProperty, value);
		}

		private void DropDownTreeViewControlOnEditingEnded(object sender, EditingEndedEventArgs e)
		{
			if (e.Result == EditingResult.Cancel)
				CancelEdit();
			else
				CommitEdit();
		}

		private void DropDownTreeViewControlOnEditingStarted(object sender, EventArgs e)
		{
			BeginEdit();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			DropDownTreeViewControl.EditingStarted += DropDownTreeViewControlOnEditingStarted;
			DropDownTreeViewControl.EditingEnded += DropDownTreeViewControlOnEditingEnded;
		}

		protected override void OnTemplateContractDetaching()
		{
			DropDownTreeViewControl.EditingStarted -= DropDownTreeViewControlOnEditingStarted;
			DropDownTreeViewControl.EditingEnded -= DropDownTreeViewControlOnEditingEnded;

			base.OnTemplateContractDetaching();
		}
	}

	public class DropDownTreeViewEditorTemplateContract : DropDownEditorBaseTemplateContract
	{
		[TemplateContractPart(Required = true)]
		public DropDownTreeViewControl DropDownTreeViewControl { get; private set; }
	}
}