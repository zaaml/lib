using System.Windows;

namespace Zaaml.UI.Controls.Artboard
{
	public abstract class ArtboardSnapTargetPoint : ArtboardSnapTargetPrimitive
	{
		protected ArtboardSnapTargetPoint(ArtboardSnapTarget target) : base(target)
		{
		}

		public sealed override bool CanSnap(ArtboardSnapSourcePrimitive sourcePrimitive)
		{
			return CanSnapCore(sourcePrimitive);
		}

		protected virtual bool CanSnapCore(ArtboardSnapSourcePrimitive sourcePrimitive)
		{
			return true;
		}

		public abstract Point GetLocation(Point sourceLocation, ArtboardSnapEngineContext context);
	}
}