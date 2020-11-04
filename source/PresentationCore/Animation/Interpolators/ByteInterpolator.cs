namespace Zaaml.PresentationCore.Animation.Interpolators
{
  public sealed class ByteInterpolator : InterpolatorBase<byte>
  {
    #region Static Fields and Constants

    public static ByteInterpolator Instance = new ByteInterpolator();

    #endregion

    #region Ctors

    private ByteInterpolator()
    {
    }

    #endregion

    #region  Methods

    protected internal override byte EvaluateCore(byte start, byte end, double progress)
    {
      return (byte)(start + (int)((end - start + 0.5) * progress));
    }

    #endregion
  }
}