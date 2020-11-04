namespace Zaaml.UI.Controls.Artboard
{
	public abstract class ArtboardSnapTargetLine : ArtboardSnapTargetPrimitive
	{
		protected ArtboardSnapTargetLine(ArtboardSnapTarget target) : base(target)
		{
		}

		public abstract ArtboardAxis Axis { get; }

		public override bool CanSnap(ArtboardSnapSourcePrimitive sourcePrimitive)
		{
			return (sourcePrimitive is ArtboardSnapSourceLine sourceLine && Axis == sourceLine.Axis || sourcePrimitive is ArtboardSnapSourcePoint) && CanSnapCore(sourcePrimitive);
		}

		protected virtual bool CanSnapCore(ArtboardSnapSourcePrimitive sourcePrimitive)
		{
			return true;
		}

		public abstract double GetAxisValue(double sourceAxisValue, ArtboardSnapEngineContext context);
	}
}