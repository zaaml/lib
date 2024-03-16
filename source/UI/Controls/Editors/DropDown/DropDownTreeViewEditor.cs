// <copyright file="DropDownTreeViewEditor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core;
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

		private bool _suspendEditCommands;

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
			get => (DropDownEditableSelectorDisplayMode)GetValue(DisplayModeProperty);
			set => SetValue(DisplayModeProperty, value);
		}

		private DropDownTreeViewControl DropDownTreeViewControl => TemplateContract.DropDownTreeViewControl;

		public bool IsTextEditable
		{
			get => (bool)GetValue(IsTextEditableProperty);
			set => SetValue(IsTextEditableProperty, value.Box());
		}

		private DropDownTreeViewEditorTemplateContract TemplateContract => (DropDownTreeViewEditorTemplateContract)TemplateContractCore;

		public TreeViewControl TreeViewControl
		{
			get => (TreeViewControl)GetValue(TreeViewControlProperty);
			set => SetValue(TreeViewControlProperty, value);
		}

		private protected override void EnterEditState()
		{
			base.EnterEditState();

			if (_suspendEditCommands)
				return;

			try
			{
				_suspendEditCommands = true;

				DropDownTreeViewControl.BeginEdit();
			}
			finally
			{
				_suspendEditCommands = false;
			}
		}

		private void OnDropDownTreeViewControlEditingEnded(object sender, EditingEndedEventArgs e)
		{
			try
			{
				_suspendEditCommands = true;

				if (e.Result == EditingResult.Cancel)
					CancelEdit();
				else
					CommitEdit();
			}
			finally
			{
				_suspendEditCommands = false;
			}
		}

		private void OnDropDownTreeViewControlEditingStarted(object sender, EventArgs e)
		{
			try
			{
				_suspendEditCommands = true;

				BeginEdit();
			}
			finally
			{
				_suspendEditCommands = false;
			}
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			DropDownTreeViewControl.EditingStarted += OnDropDownTreeViewControlEditingStarted;
			DropDownTreeViewControl.EditingEnded += OnDropDownTreeViewControlEditingEnded;
		}

		protected override void OnTemplateContractDetaching()
		{
			DropDownTreeViewControl.EditingStarted -= OnDropDownTreeViewControlEditingStarted;
			DropDownTreeViewControl.EditingEnded -= OnDropDownTreeViewControlEditingEnded;

			base.OnTemplateContractDetaching();
		}
	}

	public class DropDownTreeViewEditorTemplateContract : DropDownEditorBaseTemplateContract
	{
		[TemplateContractPart(Required = true)]
		public DropDownTreeViewControl DropDownTreeViewControl { get; [UsedImplicitly] private set; }
	}
}