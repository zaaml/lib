namespace Zaaml.UI.Controls.Artboard
{
	public abstract class ArtboardSnapTargetPrimitive
	{
		protected ArtboardSnapTargetPrimitive(ArtboardSnapTarget target)
		{
			Target = target;
		}

		public abstract bool IsFixed { get; }

		public ArtboardSnapTarget Target { get; }

		public abstract bool CanSnap(ArtboardSnapSourcePrimitive sourcePrimitive);
	}
}