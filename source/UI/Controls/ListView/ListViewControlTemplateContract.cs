using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	public class ListViewControlTemplateContract : IndexedSelectorBaseTemplateContract<ListViewItemsPresenter>
	{
		[TemplateContractPart(Required = false)]
		public ListGridViewHeadersPresenter GridViewHeadersPresenter { get; [UsedImplicitly] private set; }
	}
}