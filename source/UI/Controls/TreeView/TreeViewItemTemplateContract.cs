using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TreeView
{
	public class TreeViewItemTemplateContract : IconContentControlTemplateContract
	{
		[TemplateContractPart(Required = true)]
		public TreeViewItemGlyphPresenter GlyphPresenter { get; [UsedImplicitly] private set; }
	}
}