namespace Zaaml.PresentationCore
{
	internal interface ISharedItem
	{
		bool IsShared { get; set; }

		SharedItemOwnerCollection Owners { get; }
	}
}