using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TreeView
{
	public class TreeViewControlTemplateContract : SelectorBaseTemplateContract<TreeViewItemsPresenter>
	{
		[TemplateContractPart(Required = false)]
		public TreeViewItemGridColumnHeadersPresenter ColumnHeadersPresenter { get; [UsedImplicitly] private set; }
	}
}