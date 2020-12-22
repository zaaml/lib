using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TreeView
{
	public sealed class TreeViewSelectionCollection : SelectionCollectionBase<TreeViewItem>
	{
		internal TreeViewSelectionCollection(TreeViewSelectorController selectorController) : base(selectorController)
		{
		}
	}
}