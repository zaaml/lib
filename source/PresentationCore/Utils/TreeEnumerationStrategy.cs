using System.Windows;

namespace Zaaml.PresentationCore.Utils
{
  public abstract class TreeEnumerationStrategy
  {
    #region  Methods

    public abstract DependencyObject GetAncestor(DependencyObject dependencyObject);

    #endregion
  }
}