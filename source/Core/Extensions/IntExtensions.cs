using Zaaml.Core.Utils;

namespace Zaaml.Core.Extensions
{
  public static class IntExtensions
  {
    #region  Methods

    public static int Clamp(this int value, int min, int max)
    {
      return IntUtils.Clamp(value, min, max);
    }

    public static int Clamp(this int value, Range<int> range)
    {
      return IntUtils.Clamp(value, range);
    }

    #endregion
  }
}